namespace AudioApi;

/// <summary>
/// Represents an audio source that can be used to play audio.
/// The source should be in PCM S16LE format.
/// </summary>
public interface IAudioSource {

  /// <summary>
  /// Checks if the source has a frame at the given cursor.
  /// 
  /// If your source acts like a live stream and does not have a fixed length, you can ignore the given cursor.
  /// </summary>
  /// <param name="cursor">The cursor to get the frame at. </param>
  /// <returns>Whether the source has a frame at the given cursor.</returns>
  bool HasFrame(int cursor);

  /// <summary>
  /// Gets the frame at the given cursor.
  /// The frame should have exact 960 bytes (480 shorts).
  /// 
  /// If your source acts like a live stream and does not have a fixed length, you can ignore the given cursor.
  /// Do notice that this function can be called multiple times with the same cursor to be played for multiple players.
  /// </summary>
  /// <param name="cursor">The cursor to get the frame at. </param>
  /// <returns>The frame at the given cursor.</returns>
  ReadOnlySpan<short> GetFrame(int cursor);
}