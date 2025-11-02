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

using FFMpegCore;
using FFMpegCore.Pipes;

namespace Audio.Decoders;

public class FFMpegDecoder : IPcmDecoder {

  private static void WithOpusPcmOption(FFMpegArgumentOptions options)
  {
    options
      .WithAudioCodec("pcm_f32le")
      .WithAudioSamplingRate(AudioConstants.SampleRate)
      .WithCustomArgument($"-ac {AudioConstants.Channels}")
      .ForceFormat("f32le");
  }

  public byte[] Decode(byte[] data)
  {
    using var inputStream = new MemoryStream(data);
    using var outputStream = new MemoryStream();

    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromPipeInput(new StreamPipeSource(inputStream))
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return outputStream.ToArray();
  }

  public async Task<byte[]> DecodeAsync(byte[] data)
  {

    using var inputStream = new MemoryStream(data);
    using var outputStream = new MemoryStream();

    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromPipeInput(new StreamPipeSource(inputStream))
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return outputStream.ToArray();

  }

  public byte[] DecodeFromFile(string path)
  {

    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromFileInput(path)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return outputStream.ToArray();
  }

  public async Task<byte[]> DecodeFromFileAsync(string path)
  {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromFileInput(path)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return outputStream.ToArray();
  }

  public byte[] DecodeFromUrl(Uri url)
  {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    FFMpegArguments
      .FromUrlInput(url)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessSynchronously();

    return outputStream.ToArray();
  }

  public async Task<byte[]> DecodeFromUrlAsync(Uri url)
  {
    using var outputStream = new MemoryStream();
    var outputSink = new StreamPipeSink(outputStream);

    await FFMpegArguments
      .FromUrlInput(url)
      .OutputToPipe(outputSink, WithOpusPcmOption)
      .ProcessAsynchronously();

    return outputStream.ToArray();
  }
}