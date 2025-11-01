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

using AudioApi;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpusSharp.Core;
using OpusSharp.Core.Extensions;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Audio;

public class AudioManager : IDisposable {
  private List<AudioChannel> Channels { get; set; } = new();

  private List<IAudioChannel> CustomChannels { get; set; } = new();

  private OpusEncoder[] Encoders { get; set; } = new OpusEncoder[AudioConstants.MaxPlayers];
  private short[] CurrentFrame { get; set; } = new short[AudioConstants.FrameSize];
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
    Logger.LogInformation("Configuring Opus encoder with complexity = {Complexity}.", config.OpusComplexity);
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      if (Encoders[i] == null) {
        Encoders[i] = new OpusEncoder(AudioConstants.SampleRate, AudioConstants.Channels, OpusPredefinedValues.OPUS_APPLICATION_AUDIO);
      }
      Encoders[i].SetComplexity(config.OpusComplexity);
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
    foreach (var encoder in Encoders) {
      encoder.Dispose();
    }
    foreach (var channel in Channels) {
      channel.Dispose();
    }
    foreach (var channel in CustomChannels) {
      channel.Dispose();
    }
    CustomChannels.Clear();
    Channels.Clear();
  }

  public AudioChannel UseChannel(string id) {
    if (Channels.Any(channel => channel.Id == id)) {
      return Channels.First(channel => channel.Id == id);
    }
    var newChannel = new AudioChannel(id);
    Channels.Add(newChannel);
    newChannel.OnOpusResetRequested += OpusReset;
    return newChannel;
  }

  public void AddCustomChannel(IAudioChannel channel) {
    CustomChannels.Add(channel);
    channel.OnOpusResetRequested += OpusReset;
  }

  public void RemoveCustomChannel(IAudioChannel channel) {
    channel.OnOpusResetRequested -= OpusReset;
    CustomChannels.Remove(channel);
  }

  public bool HasFrame(int slot) {
    return Channels.Any(channel => channel.HasFrame(slot)) || CustomChannels.Any(channel => channel.HasFrame(slot));
  }

  public void NextFrame() {
    foreach (var channel in Channels) {
      channel.NextFrame();
    }
    foreach (var channel in CustomChannels) {
      channel.NextFrame();
    }
  }

  public ReadOnlySpan<short> GetFrame(int slot) {
    ResetCurrentFrame();
    foreach (var channel in Channels)
    {
      if (channel.HasFrame(slot)) {
        MixFrames(CurrentFrame.AsSpan(), channel.GetFrame(slot), channel.GetVolume(slot));
      }
    }
    foreach (var channel in CustomChannels) {
      if (channel.HasFrame(slot)) {
        MixFrames(CurrentFrame.AsSpan(), channel.GetFrame(slot), 1.0f);
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


  private void MixFrames(Span<short> target, ReadOnlySpan<short> source, float volume = 1.0f) {
    for (int i = 0; i < source.Length; i++) {
      int mixed = target[i] + (int)(source[i] * volume);
      target[i] = (short)Math.Clamp(mixed, short.MinValue, short.MaxValue);
    }
  }
}