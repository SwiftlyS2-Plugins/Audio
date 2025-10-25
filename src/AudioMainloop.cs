using System.Diagnostics;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Audio;

public class AudioMainloop : IDisposable {
  private ISwiftlyCore Core;
  private AudioManager audioManager;
  private CancellationTokenSource cancellationTokenSource;
  private PeriodicTimer timer;
  private Task audioTask;
  private uint sectionNumber;
  private byte[] buffer = new byte[2048];

  public AudioMainloop(ISwiftlyCore Core, AudioManager audioManager) {
    this.audioManager = audioManager;
    this.Core = Core;
    cancellationTokenSource = new CancellationTokenSource();
    audioTask = Task.Run(() => StartAudio(cancellationTokenSource.Token));
    timer = new PeriodicTimer(TimeSpan.FromMilliseconds(30));
  }

  public void Dispose() {
    cancellationTokenSource.Dispose();
    timer.Dispose();
    audioTask.Dispose();
  }

  public async void StartAudio(CancellationToken cancellationToken)
  {
    while (await timer.WaitForNextTickAsync())
    {
      if (cancellationToken.IsCancellationRequested)
      {
        return;
      }
      for (int i = 0; i < 64; i++)
      {
        if (Core.PlayerManager.GetPlayer(i) is not { IsValid: true }) continue;
        if (audioManager.HasNextFrame(i))
        {
          buffer.AsSpan().Clear();
          Core.NetMessage.Send<CSVCMsg_VoiceData>(msg =>
          {
            msg.Client = -1;
            msg.Tick = (uint)Core.Engine.TickCount;
            int offset = 0;
            msg.Audio.SequenceBytes = 0;
            msg.Audio.SampleRate = 48000;
            msg.Audio.Format = VoiceDataFormat_t.VOICEDATA_FORMAT_OPUS;
            msg.Audio.SectionNumber = sectionNumber;
            for (int j = 0; j < 3; j++)
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
    }
  }
}