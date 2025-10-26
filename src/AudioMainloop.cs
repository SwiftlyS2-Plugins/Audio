using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.ProtobufDefinitions;
using SwiftlyS2.Shared.Sounds;

namespace Audio;

public class AudioMainloop : IDisposable {
  private ILogger<AudioMainloop> logger;
  private ISwiftlyCore Core;
  private AudioManager audioManager;
  private CancellationTokenSource cancellationTokenSource;
  private PeriodicTimer timer;
  private Task audioTask;
  private uint sectionNumber;
  private byte[] buffer = new byte[AudioConstants.MainloopBufferSize];

  public AudioMainloop(ISwiftlyCore Core, ILogger<AudioMainloop> logger, AudioManager audioManager) {
    this.logger = logger;
    this.audioManager = audioManager;
    this.Core = Core;
    cancellationTokenSource = new CancellationTokenSource();
    audioTask = Task.Run(() => StartAudio(cancellationTokenSource.Token));
    timer = new PeriodicTimer(TimeSpan.FromMilliseconds(AudioConstants.PacketIntervalMilliseconds));
  }

  public void Dispose() {
    cancellationTokenSource.Dispose();
    timer.Dispose();
    audioTask.Dispose();
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
        Core.Profiler.StartRecording("AudioMainloop");
        audioManager.DoLoop();
        foreach (var player in Core.PlayerManager.GetAllPlayers())
        // var sw = Stopwatch.StartNew();
        // for (int i = 0; i < AudioConstants.MaxPlayers; i++)
        {
          var i = player.PlayerID;
          if (audioManager.HasNextFrame(i))
          {
            buffer.AsSpan().Clear();
            Core.NetMessage.Send<CSVCMsg_VoiceData>(msg =>
            {
              msg.Client = -1;
              msg.Tick = (uint)Core.Engine.TickCount;
              int offset = 0;
              msg.Audio.SequenceBytes = 0;
              msg.Audio.SampleRate = AudioConstants.SampleRate;
              msg.Audio.Format = VoiceDataFormat_t.VOICEDATA_FORMAT_OPUS;
              msg.Audio.SectionNumber = sectionNumber;
              for (int j = 0; j < AudioConstants.MaxPacketCount; j++)
              {
                if (!audioManager.HasNextFrame(i)) break;
                var data = audioManager.GetNextFrameAsOpus(i);
                data.CopyTo(buffer.AsSpan(offset));
                msg.Audio.NumPackets += 1;
                offset += data.Length;
                msg.Audio.PacketOffsets.Add((uint)offset);
              }
              msg.Audio.VoiceData = buffer.AsSpan(0, offset).ToArray();
              msg.Recipients.AddRecipient(i);
              sectionNumber++;
            });
          }
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