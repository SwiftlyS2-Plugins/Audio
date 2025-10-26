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