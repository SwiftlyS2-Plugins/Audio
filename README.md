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

## Get Started

### Download Plugin

Download the plugin from the latest release and install it on your server.

### Include API In Your Project

Add following lines to your `.csproj` file:

```xml
  <PackageReference Include="SwiftlyS2.Plugin.Audio.API" Version="1.0.0" />
```

## Configuration

You can find the configuration file in `addons/swiftlys2/configs/plugins/Audio/config.jsonc` after the plugin has properly started up once.

The configuration keys are as follows.

### `OpusComplexity`

Control the complexity of opus encoder, ranging from 0 to 10 ( 10 by default ).

The higher the complexity is, the better the encoding quality but the slower the encoding speed.

You can try to lower this if you experience audio lag.

## API Example

```csharp
public override void UseSharedInterface(IInterfaceManager interfaceManager)
{
  // get api, you can store it in other place, but remember to reset it every time this method is called
  IAudioApi api = interfaceManager.GetSharedInterface<IAudioApi>("audio");

  // get a channel for your plugin
  IAudioChannelController controller = api.UseChannel("test");

  /// decode source from mp3 file
  IAudioSource source = api.DecodeFromFile("E:/p.mp3");

  // assign source to the channel
  controller.SetSource(source);

  // adjust volume
  controller.SetVolumeToAll(0.5f);

  // play
  controller.PlayToAll();
}
```

To see more api, please check [AudioApi](https://github.com/SwiftlyS2-Plugins/Audio/tree/main/AudioApi)
