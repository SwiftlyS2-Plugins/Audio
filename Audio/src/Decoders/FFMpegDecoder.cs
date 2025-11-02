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