
using System.Diagnostics;
using Audio;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Benchmarker;
[MemoryDiagnoser]
public class EncoderBenchmark
{
  private AudioManager? audioManager;

  [GlobalSetup]
  public void Setup()
  {
    audioManager = new(null, LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AudioManager>());
    var source = AudioSource.DecodeFromFile("E:/p.mp3");
    var source2 = AudioSource.DecodeFromFile("E:/p.mp3");
    var track1 = audioManager.UseChannel("track1");
    var track2 = audioManager.UseChannel("track2");
    track1.ResetAll();
    track1.SetSource(source);
    track2.ResetAll();
    track2.SetSource(source2);
    track1.PlayToAll();
    track2.PlayToAll();
  }
  
  [Benchmark]
  public void Encode()
  {
    for (int i = 0; i < 64; i++)
    {
      audioManager.GetNextFrameAsOpus(i);
      audioManager.GetNextFrameAsOpus(i);
      audioManager.GetNextFrameAsOpus(i);

    }
  }
}
