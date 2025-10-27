namespace AudioApi;

/// <summary>
/// A generic interface for an audio channel.
/// You can implement this interface to create your own audio channel.
/// </summary>
public interface IAudioChannel : IDisposable {

  /// <summary>
  /// Call this to request a reset of the opus state of the channel.
  /// 
  /// The parameter is the player id, pass -1 to reset all players opus state.
  /// </summary>
  event Action<int> OnOpusResetRequested;


  /// <summary>
  /// Will be called when the channel needs to update the next frame for the audio mainloop.
  /// </summary>
  void NextFrame();

  /// <summary>
  /// Checks if the channel has a frame for the given player.
  /// </summary>
  /// <param name="playerId">Player slot.</param>
  /// <returns>Whether the channel has a frame for the given player.</returns>
  bool HasFrame(int playerId);

  /// <summary>
  /// Gets the next frame for the given player.
  /// 
  /// The frame should be in PCM S16LE format and have exact 480 shorts.
  /// </summary>
  /// <param name="playerId">Player slot.</param>
  /// <returns>The next frame for the given player.</returns>
  ReadOnlySpan<short> GetFrame(int playerId);
}