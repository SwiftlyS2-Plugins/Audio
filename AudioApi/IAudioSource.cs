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
/// Represents an audio source that can be used to play audio.
/// The source should be in PCM F32LE format.
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
  /// The frame should have exact 1920 bytes (480 floats) in PCM F32LE format.
  /// 
  /// If your source acts like a live stream and does not have a fixed length, you can ignore the given cursor.
  /// Do notice that this function can be called multiple times with the same cursor to be played for multiple players.
  /// </summary>
  /// <param name="cursor">The cursor to get the frame at. </param>
  /// <returns>The frame at the given cursor.</returns>
  ReadOnlySpan<float> GetFrame(int cursor);
}