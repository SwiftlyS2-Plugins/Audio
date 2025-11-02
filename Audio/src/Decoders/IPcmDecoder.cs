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

namespace Audio.Decoders;

public interface IPcmDecoder {
  /// <summary>
  /// Decode the given data to PCM S16LE format.
  /// </summary>
  /// <param name="data">The data to decode.</param>
  /// <returns>The decoded data.</returns>
  byte[] Decode(byte[] data);

  /// <summary>
  /// Decode the given data to PCM S16LE format asynchronously.
  /// </summary>
  /// <param name="data">The data to decode.</param>
  /// <returns>The decoded data.</returns>
  Task<byte[]> DecodeAsync(byte[] data);

  /// <summary>
  /// Decode the given file to PCM S16LE format.
  /// </summary>
  /// <param name="path">The path of the file to decode.</param>
  /// <returns>The decoded data.</returns>
  byte[] DecodeFromFile(string path);
  
  /// <summary>
  /// Decode the given file to PCM S16LE format asynchronously.
  /// </summary>
  /// <param name="path">The path of the file to decode.</param>
  /// <returns>The decoded data.</returns>
  Task<byte[]> DecodeFromFileAsync(string path);

  /// <summary>
  /// Decode the given URL to PCM S16LE format.
  /// </summary>
  /// <param name="url">The URL of the file to decode.</param>
  /// <returns>The decoded data.</returns>
  byte[] DecodeFromUrl(Uri url);
  
  /// <summary>
  /// Decode the given URL to PCM S16LE format asynchronously.
  /// </summary>
  /// <param name="url">The URL of the file to decode.</param>
  /// <returns>The decoded data.</returns>
  Task<byte[]> DecodeFromUrlAsync(Uri url);
}