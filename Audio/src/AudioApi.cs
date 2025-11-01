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

using Audio.Decoders;
using AudioApi;

namespace Audio;

public class AudioApi : IAudioApi, IDisposable {

  private AudioManager AudioManager { get; set; }
  private bool _disposed = false;

  private IPcmDecoder Decoder { get => new NativeDecoder(); }

  private void ThrowIfDisposed() {
    if (_disposed) {
      throw new ObjectDisposedException(nameof(AudioApi));
    }
  }

  public AudioApi(AudioManager audioManager) {
    AudioManager = audioManager;
  }

  public void Dispose() {
    _disposed = true;
  }

  public IAudioChannelController UseChannel(string id) {
    ThrowIfDisposed();
    return AudioManager.UseChannel(id);
  }

  public void AddCustomChannel(IAudioChannel channel) {
    ThrowIfDisposed();
    AudioManager.AddCustomChannel(channel);
  }

  public void RemoveCustomChannel(IAudioChannel channel) {
    ThrowIfDisposed();
    AudioManager.RemoveCustomChannel(channel);
  }

  public IAudioSource Decode(byte[] data)
  {
    ThrowIfDisposed();
    return new AudioSource(Decoder.Decode(data));
  }

  public async Task<IAudioSource> DecodeAsync(byte[] data)
  {
    ThrowIfDisposed();
    return new AudioSource(await Decoder.DecodeAsync(data));
  }

  public IAudioSource DecodeFromFile(string path)
  {
    ThrowIfDisposed();
    return new AudioSource(Decoder.DecodeFromFile(path));
  }

  public async Task<IAudioSource> DecodeFromFileAsync(string path)
  {
    ThrowIfDisposed();
    return new AudioSource(await Decoder.DecodeFromFileAsync(path));
  }

  public IAudioSource DecodeFromUrl(Uri url)
  {
    ThrowIfDisposed();
    return new AudioSource(Decoder.DecodeFromUrl(url));
  }

  public async Task<IAudioSource> DecodeFromUrlAsync(Uri url)
  {
    ThrowIfDisposed();
    return new AudioSource(await Decoder.DecodeFromUrlAsync(url));
  }
}