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