<div align="center">
  <img src="https://pan.samyyc.dev/s/VYmMXE" />
  <h2><strong>Audio</strong></h2>
  <h3>A high performance VoIP audio lib for swiftlys2.</h3>
</div>

<p align="center">
  <img src="https://img.shields.io/badge/build-passing-brightgreen" alt="Build Status">
  <img src="https://img.shields.io/github/downloads/SwiftlyS2-Plugins/Audio/total" alt="Downloads">
  <img src="https://img.shields.io/github/stars/SwiftlyS2-Plugins/Audio?style=flat&logo=github" alt="Stars">
  <img src="https://img.shields.io/github/license/SwiftlyS2-Plugins/Audio" alt="License">
</p>

## API Example

```csharp
public override void UseSharedInterface(IInterfaceManager interfaceManager)
{
  // get api, you can store it in other place, but remember to reset it every time this method is called
  IAudioApi api = interfaceManager.GetSharedInterface<IAudioApi>("audio");

  // get a channel for your plugin
  IAudioChannel channel = api.UseChannel("test");

  /// decode source from mp3 file
  IAudioSource source = api.DecodeFromFile("E:/p.mp3");

  // assign source to the channel
  channel.SetSource(source);

  // adjust volume
  channel.SetVolumeToAll(0.5f);

  // play
  channel.PlayToAll();
}
```

To see more api, please check [AudioApi](https://github.com/SwiftlyS2-Plugins/Audio/tree/main/AudioApi)
