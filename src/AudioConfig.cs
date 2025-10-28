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

public class AudioConfig {
  /// <summary>
  /// The complexity of the Opus encoder.
  /// 0 is the lowest complexity and 10 is the highest.
  /// The higher the complexity, the higher the quality of the audio, and the slower the encoding speed.
  /// The lower the complexity, the lower the quality of the audio, and the faster the encoding speed.
  /// The default is 10.
  /// </summary>
  public int OpusComplexity { get; set; } = 10;

}