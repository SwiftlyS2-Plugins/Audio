/*
 * Audio - A swiftlys2 plugin to control counter-strike 2 in-game VoIP audio stream.
 * Copyright (C) 2025  samyyc
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Runtime.InteropServices;
using AudioApi;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace Audio;

public class AudioSource : IAudioSource {

  private byte[] PcmData { get; set; }

  public AudioSource(byte[] pcmData) {
    PcmData = pcmData;
  }

  private static void WithOpusPcmOption(FFMpegArgumentOptions options) {
    options
      .WithAudioCodec("pcm_s16le")
      .WithAudioSamplingRate(AudioConstants.SampleRate)
      .WithCustomArgument($"-ac {AudioConstants.Channels}")
      .ForceFormat("s16le");
  }

  public static AudioSource Decode(byte[] data) {
    using var inputStream = new MemoryStream(data);
    using var outputStream = new MemoryStream();
    
    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromPipeInput(new StreamPipeSource(inputStream))
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return new AudioSource(outputStream.ToArray());
  }

  public static async Task<AudioSource> DecodeAsync(byte[] data) { 

    using var inputStream = new MemoryStream(data);
    using var outputStream = new MemoryStream();

    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromPipeInput(new StreamPipeSource(inputStream))
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return new AudioSource(outputStream.ToArray());

  }

  public static AudioSource DecodeFromFile(string path) {
    
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromFileInput(path)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return new AudioSource(outputStream.ToArray());
  }

  public static async Task<AudioSource> DecodeFromFileAsync(string path) {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromFileInput(path)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return new AudioSource(outputStream.ToArray());
  }

  public static AudioSource DecodeFromUrl(Uri url) {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromUrlInput(url)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return new AudioSource(outputStream.ToArray());
  }

  public static async Task<AudioSource> DecodeFromUrlAsync(Uri url) {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromUrlInput(url)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return new AudioSource(outputStream.ToArray());
  }
  public bool HasFrame(int cursor) {
    return cursor * AudioConstants.FrameSizeInBytes < PcmData.Length;
  }
  public ReadOnlySpan<short> GetFrame(int cursor) {
    var min = cursor * AudioConstants.FrameSizeInBytes;
    var max = Math.Min((cursor + 1) * AudioConstants.FrameSizeInBytes, PcmData.Length);
    return MemoryMarshal.Cast<byte, short>(PcmData.AsSpan(min, max - min));
  }

}