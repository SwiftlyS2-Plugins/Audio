using AudioApi;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Audio;

public class AudioChannel : IAudioChannel, IDisposable {
  public string Id { get; set; }
  private IAudioSource? Source { get; set; }
  private int[] Cursors { get; set; } = new int[AudioConstants.MaxPlayers];
  public float[] Volume { get; set;} = new float[AudioConstants.MaxPlayers];
  public bool[] IsPlaying { get; private set; } = new bool[AudioConstants.MaxPlayers];

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
      IsPlaying[i] = false;
    }
  }

  public void Dispose() {
    Source = null;  
    _disposed = true;
  }

  public AudioChannel(string id, IAudioSource source) {
    Id = id;
    Source = source;
  }
  public void SetSource(IAudioSource source) {
    ThrowIfDisposed();
    Source = source;
  }

  public bool HasNextFrame(int slot) {
    return Source != null && IsPlaying[slot] && Source.HasFrame(Cursors[slot]+1);
  }

  public ReadOnlySpan<short> GetNextFrame(int slot) {
    ThrowIfDisposed();
    return Source!.GetFrame(Cursors[slot]++);
  }

  public void Play(int slot) {
    ThrowIfDisposed();
    IsPlaying[slot] = true;
  }

  public void PlayToAll() {
    ThrowIfDisposed();
    Array.Fill(IsPlaying, true);
  }

  public void Pause(int slot) {
    ThrowIfDisposed();
    IsPlaying[slot] = false;
  }

  public void PauseAll() {
    ThrowIfDisposed();
    Array.Fill(IsPlaying, false);
  }

  public void Reset(int slot) {
    ThrowIfDisposed();
    Pause(slot);
    Cursors[slot] = 0;
  }

  public void ResetAll() {
    ThrowIfDisposed();
    PauseAll();
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
}
