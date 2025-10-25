namespace Audio;

public static class AudioConstants {
  public const int SampleRate = 48000;
  public const int Channels = 1;
  public const int FrameSize = 480;
  public const int FrameSizeInBytes = FrameSize * 2;
  public const int MaxPacketCount = 3;
  public const int PacketIntervalMilliseconds = MaxPacketCount * 10;
  public const int OpusBufferSize = 1024;
  public const int MainloopBufferSize = 2048;
  public const int MaxPlayers = 64;
}