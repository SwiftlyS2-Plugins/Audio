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

using System.Collections;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.SchemaDefinitions;
namespace Audio;

public class AudioMainloop : IDisposable {
  private ILogger<AudioMainloop> logger;
  private ISwiftlyCore Core;
  private AudioManager audioManager;
  private CancellationTokenSource cancellationTokenSource;
  private PeriodicTimer timer;
  private Task audioTask;
  private uint sectionNumber;
  private byte[][] Buffer { get; set; } = new byte[AudioConstants.MaxPlayers][];
  private ConcurrentQueue<CSVCMsg_VoiceData> VoiceDataQueue { get; set; } = new();
  private ConcurrentQueue<CSVCMsg_VoiceData> DisposedVoiceDataQueue { get; set; } = new();

  public bool IsRunning { get; set; }

  public AudioMainloop(ISwiftlyCore Core, ILogger<AudioMainloop> logger, AudioManager audioManager) {
    this.logger = logger;
    this.audioManager = audioManager;
    this.Core = Core;

    Core.Event.OnTick += SendingLoop;

    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {

      Buffer[i] = new byte[AudioConstants.MainloopBufferSize];
    }
    cancellationTokenSource = new CancellationTokenSource();
    audioTask = Task.Run(async () => {
      try {
        await StartAudio(cancellationTokenSource.Token);
      } catch (Exception e) {
        logger.LogError(e, "Error in AudioMainloop");
      }
    });
    timer = new PeriodicTimer(TimeSpan.FromMilliseconds(AudioConstants.PacketIntervalMilliseconds));
  }

  public void Dispose() {
    cancellationTokenSource.Dispose();
    timer.Dispose();
    audioTask.Dispose();
  }

  public async Task StartAudio(CancellationToken cancellationToken)
  {
    while (await timer.WaitForNextTickAsync())
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return;
      }
      if (!IsRunning) continue;
      Core.Profiler.StartRecording("AudioMainloop");
      var allPlayers = Core.PlayerManager.GetAllPlayers();

      var offsets = new List<int>[AudioConstants.MaxPlayers]; 
      for (int j = 0; j < AudioConstants.MaxPacketCount; j++)
      {
        foreach (var player in allPlayers)
        {
          if (player is not { IsValid: true }) continue;
          var i = player.PlayerID;
          if (!audioManager.HasFrame(i)) continue;
          if (offsets[player.PlayerID] is null) offsets[player.PlayerID] = new List<int>();
          var lastOffset = offsets[i].Count == 0 ? 0 : offsets[i].Last();

          var length = audioManager.GetFrameAsOpus(i, Buffer[i].AsSpan(lastOffset), AudioConstants.MainloopBufferSize);
          offsets[i].Add(lastOffset + length);
        }
        audioManager.NextFrame();
      } 

      sectionNumber++;

      foreach (var player in allPlayers)
      {
        if (player is not { IsValid: true} || offsets[player.PlayerID] is null) continue; 
        var i = player.PlayerID;

        var msg = Core.NetMessage.Create<CSVCMsg_VoiceData>();

        msg.Client = -1;
        msg.Audio.SequenceBytes = 0;
        msg.Audio.SampleRate = AudioConstants.SampleRate;
        msg.Audio.Format = VoiceDataFormat_t.VOICEDATA_FORMAT_OPUS;
        msg.Audio.SectionNumber = sectionNumber;
        msg.Audio.NumPackets = (uint)offsets[i].Count;
        foreach (var offset in offsets[i]) {
          msg.Audio.PacketOffsets.Add((uint)offset);
        }
        msg.Audio.VoiceData = Buffer[i].AsSpan(0, offsets[i].Last()).ToArray();
        msg.Recipients.AddRecipient(i);
        VoiceDataQueue.Enqueue(msg);
      }

      Core.Profiler.StopRecording("AudioMainloop");
      // Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");
    }
  }

  public void SendingLoop() {
    while (VoiceDataQueue.TryDequeue(out var msg)) {
      msg.Send();
      DisposedVoiceDataQueue.Enqueue(msg);
    }
    while (DisposedVoiceDataQueue.TryDequeue(out var msg)) {
      msg.Dispose();
    }
  }
}