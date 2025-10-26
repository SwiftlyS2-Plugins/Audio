using AudioApi;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.ProtobufDefinitions;

namespace Audio;

public class AudioChannel : IAudioChannel, IDisposable {
  public string Id { get; set; }
  private IAudioSource? Source { get; set; }
  private int[] Cursors { get; set; } = new int[AudioConstants.MaxPlayers];
  public float[] Volume { get; set;} = new float[AudioConstants.MaxPlayers];
  public bool[] IsPaused { get; private set; } = new bool[AudioConstants.MaxPlayers];
  public bool[] IsMuted { get; private set; } = new bool[AudioConstants.MaxPlayers];

  [SwiftlyInject]
  private static ISwiftlyCore Core { get; set; } = null!;

  private AudioManager AudioManager { get; set; }

  private bool _disposed = false;

  private void ThrowIfDisposed() {
    if (_disposed) {
      throw new ObjectDisposedException(nameof(AudioChannel));
    }
  }

  public AudioChannel(string id, AudioManager audioManager) {
    Id = id;
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      Cursors[i] = 0;
      Volume[i] = 1.0f;
      IsPaused[i] = true;
      IsMuted[i] = false;
    }
    AudioManager = audioManager;
  }

  public void Dispose() {
    Source = null;  
    _disposed = true;
  }

  public void SetSource(IAudioSource source) {
    ThrowIfDisposed();
    Source = source;
    AudioManager.NotifyOpusResetAll();
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

  public void DoLoop() {
    ThrowIfDisposed();
    for (int i = 0; i < AudioConstants.MaxPlayers; i++) {
      // the frame will not be get for these players
      if (!Core.PlayerManager.IsPlayerOnline(i) || (!IsPaused[i] && IsMuted[i])) {
        Cursors[i] += AudioConstants.MaxPacketCount;
      }
    }
  }

  public void Play(int slot) {
    ThrowIfDisposed();
    Reset(slot);
    Resume(slot);
    Unmute(slot);
  }

  public void PlayToAll() {
    ThrowIfDisposed();
    ResetAll();
    ResumeAll();
    UnmuteAll();
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
    AudioManager.NotifyOpusReset(slot);
  }

  public void ResetAll() {
    ThrowIfDisposed();
    Array.Fill(Cursors, 0);
    AudioManager.NotifyOpusResetAll();
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
