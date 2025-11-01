use std::{
    io::{Cursor, ErrorKind},
    ptr,
    slice,
    sync::Mutex,
};

use anyhow::{anyhow, Context, Result};
use rubato::{FastFixedIn, PolynomialDegree, Resampler};
use once_cell::sync::Lazy;
use symphonia::core::{
    audio::SampleBuffer,
    codecs::DecoderOptions,
    errors::Error,
    formats::FormatOptions,
    io::MediaSourceStream,
    meta::MetadataOptions,
    probe::Hint,
};

const TARGET_SAMPLE_RATE: u32 = 48_000;

static DECODER: Lazy<Mutex<PCMDecoder>> = Lazy::new(|| Mutex::new(PCMDecoder::new()));

pub struct PCMDecoder {
    buffer: Option<Vec<i16>>,
}

impl PCMDecoder {
    pub fn new() -> Self {
        Self { buffer: None }
    }

    pub fn decode(&mut self, audio_data: &[u8]) -> Result<()> {
        let cursor = Cursor::new(audio_data.to_vec());
        let mss = MediaSourceStream::new(Box::new(cursor), Default::default());

        let hint = Hint::new();
        let probed = symphonia::default::get_probe()
            .format(&hint, mss, &FormatOptions::default(), &MetadataOptions::default())
            .context("failed to probe audio format")?;

        let mut format = probed.format;
        let (track_id, codec_params) = {
            let track = format
                .default_track()
                .ok_or_else(|| anyhow!("no default audio track found"))?;
            (track.id, track.codec_params.clone())
        };

        let mut decoder = symphonia::default::get_codecs()
            .make(&codec_params, &DecoderOptions::default())
            .context("failed to create decoder")?;

        let mut mono_samples = Vec::<f32>::new();
        let mut source_rate = codec_params.sample_rate.unwrap_or(TARGET_SAMPLE_RATE);

        loop {
            let packet = match format.next_packet() {
                Ok(packet) => packet,
                Err(Error::ResetRequired) => {
                    decoder.reset();
                    continue;
                }
                Err(Error::IoError(err)) if err.kind() == ErrorKind::UnexpectedEof => break,
                Err(Error::DecodeError(_)) => continue,
                Err(err) => return Err(err.into()),
            };

            if packet.track_id() != track_id {
                continue;
            }

            let decoded = match decoder.decode(&packet) {
                Ok(decoded) => decoded,
                Err(Error::ResetRequired) => {
                    decoder.reset();
                    continue;
                }
                Err(Error::IoError(err)) if err.kind() == ErrorKind::UnexpectedEof => break,
                Err(Error::DecodeError(_)) => continue,
                Err(err) => return Err(err.into()),
            };

            source_rate = decoded.spec().rate;
            let channels = decoded.spec().channels.count();
            if channels == 0 {
                continue;
            }

            let mut sample_buffer = SampleBuffer::<i16>::new(decoded.capacity() as u64, *decoded.spec());
            sample_buffer.copy_interleaved_ref(decoded);

            let interleaved = sample_buffer.samples();
            if interleaved.is_empty() {
                continue;
            }

            let frames = interleaved.len() / channels;
            mono_samples.reserve(frames);

            for frame in 0..frames {
                let base = frame * channels;
                let mut acc = 0f32;
                for ch in 0..channels {
                    acc += interleaved[base + ch] as f32;
                }
                mono_samples.push(acc / channels as f32);
            }
        }

        let pcm = resample_and_quantize_to_s16le(&mono_samples, source_rate, TARGET_SAMPLE_RATE)?;
        self.buffer = Some(pcm);
        Ok(())
    }

    pub fn buffer(&self) -> Option<&[i16]> {
        self.buffer.as_deref()
    }
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn pcmdecoder_decode(data_ptr: *const u8, len: usize) -> bool {
    if data_ptr.is_null() {
        println!("pcmdecoder_decode: received null data pointer");
        return false;
    }

    let data = unsafe { slice::from_raw_parts(data_ptr, len) };

    match DECODER.lock() {
        Ok(mut decoder) => match decoder.decode(data) {
            Ok(()) => true,
            Err(err) => {
                println!("pcmdecoder_decode error: {err}");
                false
            }
        },
        Err(err) => {
            println!("pcmdecoder_decode mutex poisoned: {err}");
            false
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn pcmdecoder_get_size() -> usize {
    DECODER
        .lock()
        .ok()
        .and_then(|decoder| decoder.buffer().map(|buf| buf.len()))
        .unwrap_or(0)
}

#[unsafe(no_mangle)]
pub unsafe extern "C" fn pcmdecoder_copy(dest_ptr: *mut i16) -> bool {
    if dest_ptr.is_null() {
        println!("pcmdecoder_copy: received null destination pointer");
        return false;
    }

    match DECODER.lock() {
        Ok(decoder) => {
            if let Some(buf) = decoder.buffer() {
                unsafe { ptr::copy_nonoverlapping(buf.as_ptr(), dest_ptr, buf.len()) };
                true
            } else {
                println!("pcmdecoder_copy: buffer is empty");
                false
            }
        }
        Err(err) => {
            println!("pcmdecoder_copy mutex poisoned: {err}");
            false
        }
    }
}

#[unsafe(no_mangle)]
pub extern "C" fn pcmdecoder_free() {
    if let Ok(mut decoder) = DECODER.lock() {
        decoder.buffer = None;
    }
}

fn resample_and_quantize_to_s16le(samples: &[f32], source_rate: u32, target_rate: u32) -> Result<Vec<i16>> {
    if samples.is_empty() || source_rate == 0 || target_rate == 0 {
        return Ok(Vec::new());
    }

    if source_rate == target_rate {
        return Ok(samples.iter().map(|&sample| float_to_i16(sample)).collect());
    }

    let ratio = target_rate as f64 / source_rate as f64;

    let chunk_size = samples.len();

    let mut resampler = FastFixedIn::<f32>::new(
        ratio,
        2.0,
        PolynomialDegree::Cubic,
        chunk_size,
        1,
    )?;

    let input = vec![samples];

    let output = resampler.process(&input, None)?;

    let estimated_len = (samples.len() as f64 * ratio).round() as usize;

    let mut result = Vec::with_capacity(estimated_len);
    result.extend(output.into_iter().flatten().map(float_to_i16));
    Ok(result)
}


fn float_to_i16(value: f32) -> i16 {
    let clamped = value.clamp(i16::MIN as f32, i16::MAX as f32);
    clamped.round() as i16
}