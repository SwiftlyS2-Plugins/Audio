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

namespace Audio;

public class AudioChannel : IAudioChannelController, IAudioChannel, IDisposable {
  public string Id { get; set; }
  private IAudioSource? Source { get; set; }
  private int[] Cursors { get; set; } = new int[AudioConstants.MaxPlayers];
  public float[] Volume { get; set;} = new float[AudioConstants.MaxPlayers];
  public bool[] IsPaused { get; private set; } = new bool[AudioConstants.MaxPlayers];
  public bool[] IsMuted { get; private set; } = new bool[AudioConstants.MaxPlayers];

  private bool _disposed = false;

  public event Action<int>? OnOpusResetRequested;

  private void ThrowIfDisposed() {
    if (_disposed) {
      throw new ObjectDisposedException(nameof(AudioChannel));
    }
  }

  public AudioChannel(string id) {
    Id = id;
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      Cursors[i] = 0;
      Volume[i] = 1.0f;
      IsPaused[i] = true;
      IsMuted[i] = false;
    }
  }

  public void Dispose() {
    Source = null;  
    _disposed = true;
  }

  public void SetSource(IAudioSource source) {
    ThrowIfDisposed();
    Source = source;
    OnOpusResetRequested?.Invoke(-1);
  }

  public bool HasFrame(int slot)
  {
    ThrowIfDisposed();
    return Source != null && !IsPaused[slot] && !IsMuted[slot] && Source.HasFrame(Cursors[slot]);
  }

  public ReadOnlySpan<float> GetFrame(int slot) {
    ThrowIfDisposed();
    return Source!.GetFrame(Cursors[slot]);
  }

  public void NextFrame() {
    ThrowIfDisposed();
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      if (!IsPaused[i] && Source != null && Source.HasFrame(i+1)) {
        Cursors[i] += 1;
      }
    }
  }

  public void Play(int slot) {
    ThrowIfDisposed();
    Reset(slot);
    Resume(slot);
  }

  public void PlayToAll() {
    ThrowIfDisposed();
    ResetAll();
    ResumeAll();
  }

  public void Stop(int slot) {
    ThrowIfDisposed();
    Pause(slot);
    Reset(slot);
  }

  public void StopAll() {
    ThrowIfDisposed();
    PauseAll();
    ResetAll();
  }

  public void Pause(int slot) {
    ThrowIfDisposed();
    IsPaused[slot] = true;
  }

  public void Resume(int slot) {
    ThrowIfDisposed();
    IsPaused[slot] = false;
  }

  public void ResumeAll() {
    ThrowIfDisposed();
    Array.Fill(IsPaused, false);
  }

  public void PauseAll() {
    ThrowIfDisposed();
    Array.Fill(IsPaused, true);
  }

  public void Reset(int slot) {
    ThrowIfDisposed();
    Cursors[slot] = 0;
    OnOpusResetRequested?.Invoke(slot);
  }

  public void ResetAll() {
    ThrowIfDisposed();
    Array.Fill(Cursors, 0);
    OnOpusResetRequested?.Invoke(-1);
  }

  public float GetVolume(int playerId)
  {
    ThrowIfDisposed();
    return Volume[playerId];
  }

  public void SetVolume(int playerId, float volume)
  {
    ThrowIfDisposed();
    Volume[playerId] = volume;
  }

  public void SetVolumeToAll(float volume)
  {
    ThrowIfDisposed();
    Array.Fill(Volume, volume);
  }

  public void Mute(int playerId)
  {
    ThrowIfDisposed();
    IsMuted[playerId] = true;
  }

  public void Unmute(int playerId)
  {
    ThrowIfDisposed();
    IsMuted[playerId] = false;
  }

  public void MuteAll()
  {
    ThrowIfDisposed();
    Array.Fill(IsMuted, true);
  }

  public void UnmuteAll()
  {
    ThrowIfDisposed();
    Array.Fill(IsMuted, false);
  }
}
