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

using Audio.Opus;
using AudioApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ZLinq.Simd;

namespace Audio;

public class AudioManager : IDisposable {
  private readonly object _channelsLock = new();
  private readonly List<AudioChannel> _channelsList = new();
  private readonly List<IAudioChannel> _customChannelsList = new();
  private volatile AudioChannel[] _channels = [];
  private volatile IAudioChannel[] _customChannels = [];

  private OpusEncoder[] Encoders { get; set; } = new OpusEncoder[AudioConstants.MaxPlayers];
  private float[] CurrentFrame { get; set; } = new float[AudioConstants.FrameSize];
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
    try {
      Logger.LogInformation("Configuring Opus encoder with complexity = {Complexity}.", config.OpusComplexity);
      for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
        if (Encoders[i] == null) {
          Encoders[i] = new OpusEncoder(AudioConstants.SampleRate, AudioConstants.Channels, OpusApplication.OPUS_APPLICATION_AUDIO);
        }
        Encoders[i].SetComplexity(config.OpusComplexity);
      }
    }
    catch (Exception e) {
      Logger.LogError(e, "Error configuring Opus encoder.");
    }
  }

  public void OpusReset(int slot) {
    if (slot == -1) {
      foreach (var encoder in Encoders) {
        encoder?.Reset();
      }
      return;
    }
    Encoders[slot].Reset();
  }


  public void Dispose() {
    AudioChannel[] channels;
    IAudioChannel[] customChannels;
    lock (_channelsLock) {
      channels = _channels;
      customChannels = _customChannels;
      _channelsList.Clear();
      _customChannelsList.Clear();
      _channels = [];
      _customChannels = [];
    }
    foreach (var encoder in Encoders) {
      encoder.Dispose();
    }
    foreach (var channel in channels) {
      channel.Dispose();
    }
    foreach (var channel in customChannels) {
      channel.Dispose();
    }
  }

  public AudioChannel UseChannel(string id) {
    lock (_channelsLock) {
      var existing = _channelsList.FirstOrDefault(channel => channel.Id == id);
      if (existing != null) return existing;
      var newChannel = new AudioChannel(id);
      _channelsList.Add(newChannel);
      _channels = [.. _channelsList];
      newChannel.OnOpusResetRequested += OpusReset;
      return newChannel;
    }
  }

  public void AddCustomChannel(IAudioChannel channel) {
    lock (_channelsLock) {
      _customChannelsList.Add(channel);
      _customChannels = [.. _customChannelsList];
    }
    channel.OnOpusResetRequested += OpusReset;
  }

  public void RemoveCustomChannel(IAudioChannel channel) {
    channel.OnOpusResetRequested -= OpusReset;
    lock (_channelsLock) {
      _customChannelsList.Remove(channel);
      _customChannels = [.. _customChannelsList];
    }
  }

  public bool HasFrame(int slot) {
    var channels = _channels;
    var customChannels = _customChannels;
    return channels.Any(channel => channel.HasFrame(slot)) || customChannels.Any(channel => channel.HasFrame(slot));
  }

  public void NextFrame() {
    var channels = _channels;
    var customChannels = _customChannels;
    foreach (var channel in channels) {
      channel.NextFrame();
    }
    foreach (var channel in customChannels) {
      channel.NextFrame();
    }
  }

  public ReadOnlySpan<float> GetFrame(int slot) {
    ResetCurrentFrame();
    var channels = _channels;
    var customChannels = _customChannels;
    foreach (var channel in channels)
    {
      if (channel.HasFrame(slot)) {
        var frame = channel.GetFrame(slot);
        if (!frame.IsEmpty)
          MixFrames(CurrentFrame.AsSpan(), frame, channel.GetVolume(slot));
      }
    }
    foreach (var channel in customChannels) {
      if (channel.HasFrame(slot)) {
        var frame = channel.GetFrame(slot);
        if (!frame.IsEmpty)
          MixFrames(CurrentFrame.AsSpan(), frame, 1.0f);
      }
    }
    return CurrentFrame.AsSpan();
  }

  public int GetFrameAsOpus(int slot, Span<byte> outBuffer) {
    GetFrame(slot);
    var encoded = Encoders[slot].Encode(CurrentFrame.AsSpan(), AudioConstants.FrameSize, outBuffer, outBuffer.Length);
    return encoded;
  } 


  private void ResetCurrentFrame() {
    CurrentFrame.AsSpan().Clear();
  }


  private void MixFrames(Span<float> target, ReadOnlySpan<float> source, float volume = 1.0f) {
    target.AsVectorizable().Zip(
      source,
      (a, b) => a + b * volume,
      (a, b) => a + b * volume
    ).CopyTo(target);
  }
}
