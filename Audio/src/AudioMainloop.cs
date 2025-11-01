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

using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.ProtobufDefinitions;
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
  private int[] Offsets { get; set; } = new int[AudioConstants.MaxPlayers];
  private CSVCMsg_VoiceData[] Protobufs { get; set; } = new CSVCMsg_VoiceData[AudioConstants.MaxPlayers];

  public bool IsRunning { get; set; }

  public AudioMainloop(ISwiftlyCore Core, ILogger<AudioMainloop> logger, AudioManager audioManager) {
    this.logger = logger;
    this.audioManager = audioManager;
    this.Core = Core;
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      var msg = Core.NetMessage.Create<CSVCMsg_VoiceData>();

      msg.Client = -1;
      msg.Audio.SequenceBytes = 0;
      msg.Audio.SampleRate = AudioConstants.SampleRate;
      msg.Audio.Format = VoiceDataFormat_t.VOICEDATA_FORMAT_OPUS;
      msg.Audio.SectionNumber = sectionNumber;
      msg.Audio.NumPackets = 0;
      msg.Audio.PacketOffsets.Clear();
      msg.Recipients.AddRecipient(i);

      Protobufs[i] = msg;

      Buffer[i] = new byte[AudioConstants.MainloopBufferSize];
      Offsets[i] = 0;
    }
    cancellationTokenSource = new CancellationTokenSource();
    audioTask = Task.Run(() => StartAudio(cancellationTokenSource.Token));
    timer = new PeriodicTimer(TimeSpan.FromMilliseconds(AudioConstants.PacketIntervalMilliseconds));
  }

  public void Dispose() {
    cancellationTokenSource.Dispose();
    timer.Dispose();
    audioTask.Dispose();
  }

  public void Reset(int playerId) {
    var msg = Protobufs[playerId];
    msg.Audio.NumPackets = 0;
    msg.Audio.PacketOffsets.Clear();
  }

  public async void StartAudio(CancellationToken cancellationToken)
  {
    try {
      while (await timer.WaitForNextTickAsync())
      {
        if (cancellationToken.IsCancellationRequested)
        {
          return;
        }
        bool[] hasFrame = new bool[AudioConstants.MaxPlayers];
        if (!IsRunning) continue;
        Core.Profiler.StartRecording("AudioMainloop");
        var allPlayers = Core.PlayerManager.GetAllPlayers();
        for (int j = 0; j < AudioConstants.MaxPacketCount; j++)
        {
          foreach (var player in allPlayers)
          {
            if (player is not { IsValid: true }) continue;
            var i = player.PlayerID;
            if (!audioManager.HasFrame(i)) continue;
            var length = audioManager.GetFrameAsOpus(i, Buffer[i].AsSpan(Offsets[i]), AudioConstants.MainloopBufferSize);
            Offsets[i] += length;
            Protobufs[i].Audio.NumPackets += 1;
            Protobufs[i].Audio.PacketOffsets.Add((uint)Offsets[i]);
            hasFrame[i] = true;
          }
          audioManager.NextFrame();
        } 

        sectionNumber++;

        foreach (var player in allPlayers)
        {
          if (player is not { IsValid: true} || !hasFrame[player.PlayerID]) continue; 
          var i = player.PlayerID;
          Protobufs[i].Tick = (uint)Core.Engine.TickCount;
          Protobufs[i].Audio.SectionNumber = sectionNumber;
          Protobufs[i].Audio.VoiceData = Buffer[i].AsSpan(0, Offsets[i]).ToArray();
          Offsets[i] = 0;
          Buffer[i].AsSpan().Clear();

          // Core.Scheduler.NextTick(() =>
          // {
            Protobufs[i].Send();
            Reset(i);

          // });
        }

        Core.Profiler.StopRecording("AudioMainloop");
        // Console.WriteLine($"Time taken: {sw.ElapsedMilliseconds}ms");
      }
    }
    catch (Exception e)
    {
      logger.LogError(e, "Error in AudioMainloop");
    }
  }
}