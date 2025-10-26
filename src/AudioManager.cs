using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpusSharp.Core;
using OpusSharp.Core.Extensions;
using SwiftlyS2.Shared.SchemaDefinitions;

namespace Audio;

public class AudioManager : IDisposable {
  private List<AudioChannel> Channels { get; set; } = new();

  private OpusEncoder[] Encoders { get; set; } = new OpusEncoder[AudioConstants.MaxPlayers];
  private short[] CurrentFrame { get; set; } = new short[AudioConstants.FrameSize];
  private byte[] OpusBuffer { get; set; } = new byte[AudioConstants.OpusBufferSize];
  private IOptionsMonitor<AudioConfig>? Config { get; set; }
  private ILogger<AudioManager> Logger { get; set; }
  public AudioManager(IOptionsMonitor<AudioConfig>? config, ILogger<AudioManager> logger) {
    Logger = logger;
    if (config == null) {
      ConfigureOpusEncoder(new AudioConfig());
      return;
    }
    Config = config;
    ConfigureOpusEncoder(Config.CurrentValue);
    Config.OnChange(ConfigureOpusEncoder);
  }



  public void ConfigureOpusEncoder(AudioConfig config) {
    Logger.LogInformation("Configuring Opus encoder with complexity = {Complexity}", config.OpusComplexity);
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      if (Encoders[i] == null) {
        Encoders[i] = new OpusEncoder(AudioConstants.SampleRate, AudioConstants.Channels, OpusPredefinedValues.OPUS_APPLICATION_AUDIO);
      }
      Encoders[i].SetComplexity(config.OpusComplexity);
    }
  }

  public void NotifyOpusReset(int slot) {
    Encoders[slot].Reset();
  }

  public void NotifyOpusResetAll()
  {
    foreach (var encoder in Encoders) {
      encoder.Reset();
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
    var newChannel = new AudioChannel(id, this);
    Channels.Add(newChannel);
    return newChannel;
  }

  public bool HasNextFrame(int slot) {
    return Channels.Any(channel => channel.HasNextFrame(slot));
  }

  public void DoLoop() {
    foreach (var channel in Channels) {
      channel.DoLoop();
    }
  }

  public ReadOnlySpan<short> GetNextFrame(int slot) {
    ResetCurrentFrame();
    foreach (var channel in Channels)
    {
      if (channel.HasNextFrame(slot)) {
        MixFrames(CurrentFrame.AsSpan(), channel.GetNextFrame(slot), channel.Volume[slot]);
      }
    }
    return CurrentFrame.AsSpan();
  }

  public ReadOnlySpan<byte> GetNextFrameAsOpus(int slot) {
    ClearOpusBuffer();
    GetNextFrame(slot);
    var encoded = Encoders[slot].Encode(CurrentFrame.AsSpan(), AudioConstants.FrameSize, OpusBuffer.AsSpan(), OpusBuffer.Length);
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