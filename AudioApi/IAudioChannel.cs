namespace AudioApi;

public interface IAudioChannel {

  /// <summary>
  /// The id of the channel.
  /// </summary>
  string Id { get; }

  /// <summary>
  /// Sets the source of the channel.
  /// </summary>
  /// <param name="source">The source to set for the channel. </param>
  void SetSource(IAudioSource source);

  /// <summary>
  /// Gets the volume of the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to get the volume for. </param>
  /// <returns>The volume of the channel for the given player.</returns>
  float GetVolume(int playerId);

  /// <summary>
  /// Sets the volume of the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to set the volume for. </param>
  /// <param name="volume">The volume to set for the given player. </param>
  void SetVolume(int playerId, float volume);

  /// <summary>
  /// Plays the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to play the track for. </param>
  void Play(int playerId);

  /// <summary>
  /// Pauses the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to pause the track for. </param>
  void Pause(int playerId);


  /// <summary>
  /// Stop and resets the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to stop and reset the channel for. </param>
  void Reset(int playerId);

  /// <summary>
  /// Sets the volume of the channel to the given volume for all players.
  /// </summary>
  /// <param name="volume">The volume to set for all players. </param>
  void SetVolumeToAll(float volume);

  /// <summary>
  /// Plays the channel for all players.
  /// </summary>
  void PlayToAll();

  /// <summary>
  /// Pauses the channel for all players.
  /// </summary>
  void PauseAll();

  /// <summary>
  /// Stop and resets the channel for all players.
  /// </summary>
  void ResetAll();

}