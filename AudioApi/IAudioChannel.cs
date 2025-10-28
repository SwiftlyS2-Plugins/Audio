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