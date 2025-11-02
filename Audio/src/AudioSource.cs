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
using AudioApi;

namespace Audio;

public class AudioSource : IAudioSource {

  private byte[] PcmData { get; set; }

  public AudioSource(byte[] pcmData) {
    PcmData = pcmData;
  }

  public bool HasFrame(int cursor) {
    return cursor * AudioConstants.FrameSizeInBytes < PcmData.Length;
  }
  public ReadOnlySpan<float> GetFrame(int cursor) {
    var min = cursor * AudioConstants.FrameSizeInBytes;
    var max = Math.Min((cursor + 1) * AudioConstants.FrameSizeInBytes, PcmData.Length);
    return MemoryMarshal.Cast<byte, float>(PcmData.AsSpan(min, max - min));
  }

}