using AudioApi;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Audio;

public class AudioChannel : IAudioChannel, IDisposable {
  public string Id { get; set; }
  private IAudioSource? Source { get; set; }
  private int[] Cursors { get; set; } = new int[AudioConstants.MaxPlayers];
  public float[] Volume { get; set;} = new float[AudioConstants.MaxPlayers];
  public bool[] IsPaused { get; private set; } = new bool[AudioConstants.MaxPlayers];
  public bool[] IsMuted { get; private set; } = new bool[AudioConstants.MaxPlayers];

  private bool _disposed = false;

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
  }

  public bool HasNextFrame(int slot)
  {
    ThrowIfDisposed();
    return Source != null && !IsPaused[slot] && !IsMuted[slot] && Source.HasFrame(Cursors[slot]+1);
  }

  public ReadOnlySpan<short> GetNextFrame(int slot) {
    ThrowIfDisposed();
    return Source!.GetFrame(Cursors[slot]++);
  }

  public void ProgressIfMuted(int slot) {
    ThrowIfDisposed();
    if (IsMuted[slot]) {
      Cursors[slot]++;
    }
  }

  public void Play(int slot) {
    ThrowIfDisposed();
    Resume(slot);
    Unmute(slot);
    Reset(slot);
  }

  public void PlayToAll() {
    ThrowIfDisposed();
    ResumeAll();
    UnmuteAll();
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
  }

  public void ResetAll() {
    ThrowIfDisposed();
    Array.Fill(Cursors, 0);
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
