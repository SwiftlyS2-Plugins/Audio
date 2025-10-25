namespace AudioApi;

/// <summary>
/// Represents an audio API that can be used to decode audio sources and use audio channels.
/// The API is used to interact with the audio system.
/// </summary>
public interface IAudioApi {

  /// <summary>
  /// Get a track with the given id.
  /// If the track does not exist, it will be created.
  /// </summary>
  /// <param name="id">The id of the track to create. </param>
  /// <returns>The created track. </returns>
  IAudioChannel UseChannel(string id);

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