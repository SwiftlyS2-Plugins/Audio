using AudioApi;

namespace Audio;

public class AudioApi : IAudioApi, IDisposable {

  private AudioManager AudioManager { get; set; }
  private bool _disposed = false;

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
    return AudioSource.Decode(data);
  }

  public async Task<IAudioSource> DecodeAsync(byte[] data)
  {
    ThrowIfDisposed();
    return await AudioSource.DecodeAsync(data);
  }

  public IAudioSource DecodeFromFile(string path)
  {
    ThrowIfDisposed();
    return AudioSource.DecodeFromFile(path);
  }

  public async Task<IAudioSource> DecodeFromFileAsync(string path)
  {
    ThrowIfDisposed();
    return await AudioSource.DecodeFromFileAsync(path);
  }

  public IAudioSource DecodeFromUrl(Uri url)
  {
    ThrowIfDisposed();
    return AudioSource.DecodeFromUrl(url);
  }

  public async Task<IAudioSource> DecodeFromUrlAsync(Uri url)
  {
    ThrowIfDisposed();
    return await AudioSource.DecodeFromUrlAsync(url);
  }
}