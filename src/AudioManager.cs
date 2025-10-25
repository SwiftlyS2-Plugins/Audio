using System.Collections.Concurrent;
using OpusSharp.Core;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Audio;

public class AudioManager : IDisposable {
  private List<AudioChannel> Channels { get; set; } = new();

  private OpusEncoder[] Encoders { get; set; } = new OpusEncoder[64];
  private short[] CurrentFrame { get; set; } = new short[480];
  private byte[] OpusBuffer { get; set; } = new byte[1024];
  private OpusEncoder Encoder { get; set; } = new(48000, 1, OpusPredefinedValues.OPUS_APPLICATION_AUDIO);
  public AudioManager() {
    for (int i = 0; i < 64; i++) {
      Encoders[i] = new OpusEncoder(48000, 1, OpusPredefinedValues.OPUS_APPLICATION_AUDIO);
    }
  }

  public void Dispose() {
    foreach (var encoder in Encoders) {
      encoder.Dispose();
    }
    foreach (var channel in Channels) {
      channel.Dispose();
    }
    Channels.Clear();
  }

  public AudioChannel UseChannel(string id) {
    if (Channels.Any(channel => channel.Id == id)) {
      return Channels.First(channel => channel.Id == id);
    }
    var newChannel = new AudioChannel(id);
    Channels.Add(newChannel);
    return newChannel;
  }

  public bool HasNextFrame(int slot) {
    return Channels.Any(channel => channel.HasNextFrame(slot));
  }

  public ReadOnlySpan<short> GetNextFrame(int slot) {
    ResetCurrentFrame();
    foreach (var channel in Channels) {
      if (channel.HasNextFrame(slot)) {
        MixFrames(CurrentFrame.AsSpan(), channel.GetNextFrame(slot), channel.Volume[slot]);
      }
    }
    return CurrentFrame.AsSpan();
  }

  public ReadOnlySpan<byte> GetNextFrameAsOpus(int slot) {
    ClearOpusBuffer();
    GetNextFrame(slot);
    var encoded = Encoders[slot].Encode(CurrentFrame.AsSpan(), 480, OpusBuffer.AsSpan(), OpusBuffer.Length);
    return OpusBuffer.AsSpan(0, encoded);
  } 


  private void ResetCurrentFrame() {
    CurrentFrame.AsSpan().Clear();
  }

  private void ClearOpusBuffer() {
    OpusBuffer.AsSpan().Clear();
  }

  private void MixFrames(Span<short> target, ReadOnlySpan<short> source, float volume) {
    for (int i = 0; i < source.Length; i++) {
      int mixed = target[i] + (int)(source[i] * volume);
      target[i] = (short)Math.Clamp(mixed, short.MinValue, short.MaxValue);
    }
  }
}