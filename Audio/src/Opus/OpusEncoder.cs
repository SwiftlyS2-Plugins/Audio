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

namespace Audio.Opus;

/// <summary>
/// A wrapper for the Opus encoder.
/// </summary>
public class OpusEncoder : IDisposable {

  private OpusSafeHandle Handle { get; set; }

  private bool _disposed = false;

  private void ThrowIfDisposed() {
    if (_disposed) {
      throw new ObjectDisposedException(nameof(OpusEncoder));
    }
  }

  /// <summary>
  /// Creates a new Opus encoder.
  /// </summary>
  /// <param name="sampleRate"> The sample rate of the audio to encode. </param>
  /// <param name="channels"> The number of channels of the audio to encode. </param>
  /// <param name="application"> The application of the audio to encode. </param>
  public OpusEncoder(int sampleRate, int channels, OpusApplication application) {
    ThrowIfDisposed();
    Handle = Engine2Opus.Create(sampleRate, channels, application);
  }

  /// <summary>
  /// Encodes a frame of audio.
  /// </summary>
  /// <param name="input"> The input audio data. </param>
  /// <param name="inputLength"> The length of the input audio data. </param>
  /// <param name="output"> The output audio data. </param>
  /// <param name="outputLength"> The length of the output audio data. </param>
  /// <returns> The number of bytes encoded. </returns>
  public int Encode(ReadOnlySpan<float> input, int inputLength, Span<byte> output, int outputLength) {
    ThrowIfDisposed();
    return Engine2Opus.EncodeFloat(Handle, input, inputLength, output, outputLength);
  }


  /// <summary>
  /// Sets the complexity of the encoder.
  /// </summary>
  /// <param name="complexity"> The complexity of the encoder. </param>
  public void SetComplexity(int complexity) {
    ThrowIfDisposed();
    if (complexity < 0 || complexity > 10) {
      throw new ArgumentException("OpusComplexity must be between 0 and 10.");
    }
    Engine2Opus.SetEncoderCTL(Handle, (int)OpusCtlRequest.OPUS_SET_COMPLEXITY_REQUEST, complexity);
  }

  /// <summary>
  /// Resets the encoder.
  /// </summary>
  public void Reset() {
    ThrowIfDisposed();
    Engine2Opus.SetEncoderCTL(Handle, (int)OpusCtlRequest.OPUS_RESET_STATE, 0);
  }

  public void Dispose() {
    if (_disposed) return;
    Engine2Opus.Destroy(Handle);
    _disposed = true;
  }
}