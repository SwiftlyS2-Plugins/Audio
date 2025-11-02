use std::{
    io::{Cursor, ErrorKind},
    ptr,
    slice,
    sync::Mutex,
};

use anyhow::{anyhow, Context, Result};
use rubato::{FastFixedIn, FftFixedIn, PolynomialDegree, Resampler};
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
    buffer: Option<Vec<f32>>,
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

        let mut channel_samples: Vec<Vec<f32>> = Vec::new();
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

            let mut sample_buffer = SampleBuffer::<f32>::new(decoded.capacity() as u64, *decoded.spec());
            sample_buffer.copy_interleaved_ref(decoded);

            let interleaved = sample_buffer.samples();
            if interleaved.is_empty() {
                continue;
            }

            if channel_samples.is_empty() {
                channel_samples = (0..channels).map(|_| Vec::new()).collect();
            } else if channel_samples.len() != channels {
                return Err(anyhow!(
                    "channel count changed within stream: expected {}, got {}",
                    channel_samples.len(),
                    channels
                ));
            }

            let frames = interleaved.len() / channels;

            for ch in 0..channels {
                if let Some(buffer) = channel_samples.get_mut(ch) {
                    buffer.reserve(frames);
                }
            }

            for frame in 0..frames {
                let base = frame * channels;
                for ch in 0..channels {
                    if let Some(buffer) = channel_samples.get_mut(ch) {
                        buffer.push(interleaved[base + ch] as f32);
                    }
                }
            }
        }

        let pcm = resample(&channel_samples, source_rate, TARGET_SAMPLE_RATE)?;
        self.buffer = Some(pcm);
        Ok(())
    }

    pub fn buffer(&self) -> Option<&[f32]> {
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
pub unsafe extern "C" fn pcmdecoder_copy(dest_ptr: *mut f32) -> bool {
    if dest_ptr.is_null() {
        println!("pcmdecoder_copy: received null destination pointer");
        return false;
    }

    match DECODER.lock() {
        Ok(decoder) => {
            if let Some(buf) = decoder.buffer() {
                unsafe { ptr::copy_nonoverlapping(buf.as_ptr() as *mut f32, dest_ptr, buf.len()) };
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

fn resample(samples: &[Vec<f32>], source_rate: u32, target_rate: u32) -> Result<Vec<f32>> {
    if samples.is_empty() || source_rate == 0 || target_rate == 0 {
        return Ok(Vec::new());
    }
    let channels = samples.len();
    let in_frames = samples[0].len();
    if samples.iter().any(|c| c.len() != in_frames) {
        return Err(anyhow!("channel buffers must be the same length"));
    }

    if source_rate == target_rate {
        let mut out = Vec::with_capacity(in_frames);
        for f in 0..in_frames {
            let mut acc = 0f32;
            for ch in 0..channels {
                acc += samples[ch][f];
            }
            out.push(acc / channels as f32);
        }
        return Ok(out);
    }

    let chunk_in = 4096;
    let mut resampler = rubato::FftFixedIn::<f32>::new(
        source_rate as usize,
        target_rate as usize,
        chunk_in,
        4,
        channels
    )?;

    let mut out_ch = vec![Vec::<f32>::new(); channels];

    let mut pos = 0;
    while pos < in_frames {
        let take = (in_frames - pos).min(chunk_in);
        let mut block = Vec::with_capacity(channels);
        for ch in 0..channels {
            block.push(samples[ch][pos..pos + take].to_vec());
        }
        pos += take;

        if take < chunk_in {
            for ch in 0..channels {
                block[ch].resize(chunk_in, 0.0);
            }
        }

        let y = resampler.process(&block, None)?; 
        for ch in 0..channels {
            out_ch[ch].extend_from_slice(&y[ch]);
        }
    }

    for _ in 0..3 {
        let zeros = vec![vec![0f32; chunk_in]; channels];
        let y = resampler.process(&zeros, None)?;
        for ch in 0..channels {
            out_ch[ch].extend_from_slice(&y[ch]);
        }
    }

    let out_frames = out_ch[0].len();
    let mut out = Vec::with_capacity(out_frames);
    for f in 0..out_frames {
        let mut acc = 0f32;
        for ch in 0..channels {
            acc += out_ch[ch][f];
        }
        out.push(acc / channels as f32);
    }
    Ok(out)
}