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
 
namespace Audio;

public static class AudioConstants {
  public const int SampleRate = 48000;
  public const int Channels = 1;
  public const int FrameSize = 480;
  public const int FrameSizeInBytes = FrameSize * 4;
  public const int MaxPacketCount = 3;
  public const int PacketIntervalMilliseconds = MaxPacketCount * 10;
  public const int OpusBufferSize = 1024;
  public const int MainloopBufferSize = 2048;
  public const int MaxPlayers = 64;
}