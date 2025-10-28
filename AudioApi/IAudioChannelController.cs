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

namespace AudioApi;

/// <summary>
/// The controller of an internal audio channel.
/// </summary>
public interface IAudioChannelController {

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
  /// <param name="playerId">The player id to play the channel for. </param>
  void Play(int playerId);

  /// <summary>
  /// Stops the channel for the given player.
  /// </summary>
  /// <param name="playerId">The player id to stop the channel for. </param>
  void Stop(int playerId);

  /// <summary>
  /// Pauses the channel for the given player.
  /// 
  /// Pausing the channel will stop the audio from playing, which means the audio will not be synchronized with the other players.
  /// </summary>
  /// <param name="playerId">The player id to pause the channel for. </param>
  void Pause(int playerId);


  /// <summary>
  /// Resume the channel for the given player.
  /// 
  /// Unpausing the channel will resume the audio from the last paused state of this player.
  /// </summary>
  /// <param name="playerId">The player id to resume the channel for. </param>
  void Resume(int playerId);

  /// <summary>
  /// Mutes the channel for the given player.
  /// 
  /// Muting the channel will stop the audio from playing,
  /// but acts like setting the volume to 0 and still progressing and synchronizing with other players.
  /// </summary>
  /// <param name="playerId"></param>
  void Mute(int playerId);

  /// <summary>
  /// Unmutes the channel for the given player.
  /// 
  /// Unmuting the channel will recover the playing state.
  /// </summary>
  /// <param name="playerId">The player id to unmute the channel for. </param>
  void Unmute(int playerId);


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
  /// Stops the channel for all players.
  /// </summary>
  void StopAll();

  /// <summary>
  /// Pauses the channel for all players.
  /// </summary>
  void PauseAll();

  /// <summary>
  /// Resumes the channel for all players.
  /// </summary>
  void ResumeAll();


  /// <summary>
  /// Unmutes the channel for all players.
  /// </summary>
  void UnmuteAll();

  /// <summary>
  /// Stop and resets the channel for all players.
  /// </summary>
  void ResetAll();

  /// <summary>
  /// Mutes the channel for all players.
  /// </summary>
  void MuteAll();

}