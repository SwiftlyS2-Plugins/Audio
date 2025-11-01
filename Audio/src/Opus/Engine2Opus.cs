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

using System.Runtime.InteropServices;

namespace Audio.Opus;

public static class Engine2Opus {

  [DllImport("engine2", EntryPoint = "opus_encoder_create")]
  private static extern OpusSafeHandle opus_encoder_create(int sampleRate, int channels, int application, out int error);

  [DllImport("engine2", EntryPoint = "opus_encoder_destroy")]
  private static extern void opus_encoder_destroy(IntPtr encoder);

  [DllImport("engine2", EntryPoint = "opus_encode")]
  private static extern int opus_encode(IntPtr encoder, IntPtr input, int inputLength, IntPtr output, int outputLength);

  [DllImport("engine2", EntryPoint = "opus_encoder_ctl")]
  private static extern int opus_encoder_ctl(IntPtr encoder, int type, nint data);

  public static OpusSafeHandle Create(int sampleRate, int channels, OpusApplication application) {
    int error;
    var handle = opus_encoder_create(sampleRate, channels, (int)application, out error);
    OpusException.ThrowIfError("opus_encoder_create", error);
    return handle;
  }

  public static void SetEncoderCTL(OpusSafeHandle encoder, int request, nint data) {
    var result = opus_encoder_ctl(encoder.DangerousGetHandle(), request, data);
    OpusException.ThrowIfError("opus_encoder_ctl", result);
  }

  public static void Destroy(OpusSafeHandle encoder) {
    opus_encoder_destroy(encoder.DangerousGetHandle());
  }

  public static int Encode(OpusSafeHandle encoder, ReadOnlySpan<short> input, int inputLength, Span<byte> output, int outputLength) {
    unsafe {
      fixed (short* inputPtr = input) {
        fixed (byte* outputPtr = output) {
          var result = opus_encode(encoder.DangerousGetHandle(), (nint)inputPtr, inputLength, (nint)outputPtr, outputLength);
          OpusException.ThrowIfError("opus_encode", result);
          return result;
        }
      }
    }
  }


}