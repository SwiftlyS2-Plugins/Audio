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
 
namespace Audio.Opus
{
  /// <summary>
  /// Encoder CTL (control) request IDs.
  /// These correspond to the internal Opus encoder control numbers.
  /// In general, SET requests are even and GET requests are odd.
  /// </summary>
  public enum OpusCtlRequest
  {
    /// <summary>Set the encoder's application mode.</summary>
    OPUS_SET_APPLICATION_REQUEST = 4000,

    /// <summary>Get the encoder's application mode.</summary>
    OPUS_GET_APPLICATION_REQUEST = 4001,

    /// <summary>Set the target bitrate.</summary>
    OPUS_SET_BITRATE_REQUEST = 4002,

    /// <summary>Get the current bitrate.</summary>
    OPUS_GET_BITRATE_REQUEST = 4003,

    /// <summary>Set maximum bandwidth allowed.</summary>
    OPUS_SET_MAX_BANDWIDTH_REQUEST = 4004,

    /// <summary>Get maximum bandwidth setting.</summary>
    OPUS_GET_MAX_BANDWIDTH_REQUEST = 4005,

    /// <summary>Enable or disable variable bitrate (VBR).</summary>
    OPUS_SET_VBR_REQUEST = 4006,

    /// <summary>Get current VBR setting.</summary>
    OPUS_GET_VBR_REQUEST = 4007,

    /// <summary>Set encoder output bandwidth.</summary>
    OPUS_SET_BANDWIDTH_REQUEST = 4008,

    /// <summary>Get encoder output bandwidth.</summary>
    OPUS_GET_BANDWIDTH_REQUEST = 4009,

    /// <summary>Set encoder complexity (0–10).</summary>
    OPUS_SET_COMPLEXITY_REQUEST = 4010,

    /// <summary>Get encoder complexity.</summary>
    OPUS_GET_COMPLEXITY_REQUEST = 4011,

    /// <summary>Enable or disable in-band Forward Error Correction (FEC).</summary>
    OPUS_SET_INBAND_FEC_REQUEST = 4012,

    /// <summary>Get FEC setting.</summary>
    OPUS_GET_INBAND_FEC_REQUEST = 4013,

    /// <summary>Set expected packet loss percentage (0–100).</summary>
    OPUS_SET_PACKET_LOSS_PERC_REQUEST = 4014,

    /// <summary>Get expected packet loss percentage.</summary>
    OPUS_GET_PACKET_LOSS_PERC_REQUEST = 4015,

    /// <summary>Enable or disable discontinuous transmission (DTX).</summary>
    OPUS_SET_DTX_REQUEST = 4016,

    /// <summary>Get DTX setting.</summary>
    OPUS_GET_DTX_REQUEST = 4017,

    /// <summary>Set constrained VBR mode.</summary>
    OPUS_SET_VBR_CONSTRAINT_REQUEST = 4020,

    /// <summary>Get constrained VBR setting.</summary>
    OPUS_GET_VBR_CONSTRAINT_REQUEST = 4021,

    /// <summary>Force encoder output channels.</summary>
    OPUS_SET_FORCE_CHANNELS_REQUEST = 4022,

    /// <summary>Get forced channel setting.</summary>
    OPUS_GET_FORCE_CHANNELS_REQUEST = 4023,

    /// <summary>Set the signal type (voice/music).</summary>
    OPUS_SET_SIGNAL_REQUEST = 4024,

    /// <summary>Get the signal type.</summary>
    OPUS_GET_SIGNAL_REQUEST = 4025,

    /// <summary>Get encoder lookahead (samples of pre-skip).</summary>
    OPUS_GET_LOOKAHEAD_REQUEST = 4027,

    /// <summary>Reset the encoder.</summary>
    OPUS_RESET_STATE = 4028,

    /// <summary>Get encoder sample rate.</summary>
    OPUS_GET_SAMPLE_RATE_REQUEST = 4029,

    /// <summary>Get encoder's final RNG range value.</summary>
    OPUS_GET_FINAL_RANGE_REQUEST = 4031,

    /// <summary>Get pitch (number of samples per pitch period).</summary>
    OPUS_GET_PITCH_REQUEST = 4033,

    /// <summary>Set output gain (in Q8 dB units).</summary>
    OPUS_SET_GAIN_REQUEST = 4034,

    /// <summary>Get output gain (note: should have been 4035, but actually 4045).</summary>
    OPUS_GET_GAIN_REQUEST = 4045,

    /// <summary>Set input signal precision (LSB depth).</summary>
    OPUS_SET_LSB_DEPTH_REQUEST = 4036,

    /// <summary>Get input signal precision (LSB depth).</summary>
    OPUS_GET_LSB_DEPTH_REQUEST = 4037,

    /// <summary>Get duration (in samples) of last packet.</summary>
    OPUS_GET_LAST_PACKET_DURATION_REQUEST = 4039,

    /// <summary>Set expert frame duration control.</summary>
    OPUS_SET_EXPERT_FRAME_DURATION_REQUEST = 4040,

    /// <summary>Get expert frame duration control.</summary>
    OPUS_GET_EXPERT_FRAME_DURATION_REQUEST = 4041,

    /// <summary>Disable or enable prediction.</summary>
    OPUS_SET_PREDICTION_DISABLED_REQUEST = 4042,

    /// <summary>Get prediction disable state.</summary>
    OPUS_GET_PREDICTION_DISABLED_REQUEST = 4043,

    /// <summary>Set phase inversion disable state.</summary>
    OPUS_SET_PHASE_INVERSION_DISABLED_REQUEST = 4046,

    /// <summary>Get phase inversion disable state.</summary>
    OPUS_GET_PHASE_INVERSION_DISABLED_REQUEST = 4047,

    /// <summary>Get whether encoder is in DTX mode.</summary>
    OPUS_GET_IN_DTX_REQUEST = 4049,

    /// <summary>Set DRED (Deep Redundancy) duration.</summary>
    OPUS_SET_DRED_DURATION_REQUEST = 4050,

    /// <summary>Get DRED (Deep Redundancy) duration.</summary>
    OPUS_GET_DRED_DURATION_REQUEST = 4051,

    /// <summary>Set DNN blob (for neural network assisted mode).</summary>
    OPUS_SET_DNN_BLOB_REQUEST = 4052
  }

  /// <summary>
  /// Application modes for the Opus encoder.
  /// </summary>
  public enum OpusApplication
  {
    /// <summary>Auto/default setting.</summary>
    OPUS_AUTO = -1000,

    /// <summary>Maximum possible bitrate.</summary>
    OPUS_BITRATE_MAX = -1,

    /// <summary>
    /// Best for most VoIP/videoconference use where intelligibility matters most.
    /// </summary>
    OPUS_APPLICATION_VOIP = 2048,

    /// <summary>
    /// Best for broadcast/high-fidelity applications where audio quality is top priority.
    /// </summary>
    OPUS_APPLICATION_AUDIO = 2049,

    /// <summary>
    /// Use when lowest achievable latency is required (voice-optimized modes disabled).
    /// </summary>
    OPUS_APPLICATION_RESTRICTED_LOWDELAY = 2051
  }

  /// <summary>
  /// Signal types for Opus encoder.
  /// </summary>
  public enum OpusSignal
  {
    /// <summary>Signal being encoded is voice.</summary>
    OPUS_SIGNAL_VOICE = 3001,

    /// <summary>Signal being encoded is music.</summary>
    OPUS_SIGNAL_MUSIC = 3002
  }

  /// <summary>
  /// Bandwidth modes used by Opus encoder.
  /// </summary>
  public enum OpusBandwidth
  {
    /// <summary>4 kHz bandpass (narrowband).</summary>
    OPUS_BANDWIDTH_NARROWBAND = 1101,

    /// <summary>6 kHz bandpass (mediumband).</summary>
    OPUS_BANDWIDTH_MEDIUMBAND = 1102,

    /// <summary>8 kHz bandpass (wideband).</summary>
    OPUS_BANDWIDTH_WIDEBAND = 1103,

    /// <summary>12 kHz bandpass (superwideband).</summary>
    OPUS_BANDWIDTH_SUPERWIDEBAND = 1104,

    /// <summary>20 kHz bandpass (fullband).</summary>
    OPUS_BANDWIDTH_FULLBAND = 1105
  }

  /// <summary>
  /// Frame size options for Opus encoder.
  /// </summary>
  public enum OpusFrameSize
  {
    /// <summary>Select frame size from argument (default).</summary>
    OPUS_FRAMESIZE_ARG = 5000,

    /// <summary>Use 2.5 ms frames.</summary>
    OPUS_FRAMESIZE_2_5_MS = 5001,

    /// <summary>Use 5 ms frames.</summary>
    OPUS_FRAMESIZE_5_MS = 5002,

    /// <summary>Use 10 ms frames.</summary>
    OPUS_FRAMESIZE_10_MS = 5003,

    /// <summary>Use 20 ms frames.</summary>
    OPUS_FRAMESIZE_20_MS = 5004,

    /// <summary>Use 40 ms frames.</summary>
    OPUS_FRAMESIZE_40_MS = 5005,

    /// <summary>Use 60 ms frames.</summary>
    OPUS_FRAMESIZE_60_MS = 5006,

    /// <summary>Use 80 ms frames.</summary>
    OPUS_FRAMESIZE_80_MS = 5007,

    /// <summary>Use 100 ms frames.</summary>
    OPUS_FRAMESIZE_100_MS = 5008,

    /// <summary>Use 120 ms frames.</summary>
    OPUS_FRAMESIZE_120_MS = 5009
  }
}
