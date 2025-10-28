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
      audioManager.GetFrameAsOpus(i, new byte[AudioConstants.MainloopBufferSize]);
      audioManager.GetFrameAsOpus(i, new byte[AudioConstants.MainloopBufferSize]);

    }
  }
}
