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
/// Represents an audio API that can be used to decode audio sources and use audio channels.
/// The API is used to interact with the audio system.
/// </summary>
public interface IAudioApi {

  /// <summary>
  /// Get a channel with the given id.
  /// If the channel does not exist, it will be created.
  /// </summary>
  /// <param name="id">The id of the channel to create. </param>
  /// <returns>The controller of the channel. </returns>
  IAudioChannelController UseChannel(string id);


  /// <summary>
  /// Add a custom channel to the API.
  /// The channel will be used to send audio data to the players.
  /// </summary>
  /// <param name="channel">The channel to add. </param>
  void AddCustomChannel(IAudioChannel channel);

  /// <summary>
  /// Remove a custom channel from the API.
  /// The channel will no longer be used to send audio data to the players.
  /// </summary>
  /// <param name="channel">The channel to remove. </param>
  void RemoveCustomChannel(IAudioChannel channel);

  /// <summary>
  /// Get a source with the given data synchronously.
  /// The data can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="data">The data of the source.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the data cannot be decoded.</exception>
  IAudioSource Decode(byte[] data);

  /// <summary>
  /// Get a source with the given data asynchronously.
  /// The data can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="data">The data of the source.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the data cannot be decoded.</exception>
  Task<IAudioSource> DecodeAsync(byte[] data);

  /// <summary>
  /// Get a source with the given file path synchronously.
  /// The file can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="path">The path of the file.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the file cannot be decoded.</exception>
  IAudioSource DecodeFromFile(string path);

  /// <summary>
  /// Get a source with the given file path asynchronously.
  /// The file can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="path">The path of the file.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the file cannot be decoded.</exception>
  Task<IAudioSource> DecodeFromFileAsync(string path);

  /// <summary>
  /// Get a source with the given URL synchronously.
  /// The URL can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="url">The URL of the source.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the URL cannot be decoded.</exception>
  IAudioSource DecodeFromUrl(Uri url);

  /// <summary>
  /// Get a source with the given URL asynchronously.
  /// The URL can be given as any format as long as it can be recognized by FFMpeg.
  /// </summary>
  /// <param name="url">The URL of the source.</param>
  /// <returns>The created source. </returns>
  /// <exception cref="Exception">Thrown when the URL cannot be decoded.</exception>
  Task<IAudioSource> DecodeFromUrlAsync(Uri url);

}