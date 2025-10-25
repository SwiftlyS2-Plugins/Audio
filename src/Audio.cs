using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared;
using OpusSharp.Core;
using SwiftlyS2.Shared.Commands;
using FFMpegCore;
using FFMpegCore.Pipes;
using FFMpegCore.Enums;
using FFMpegCore.Arguments;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared.ProtobufDefinitions;
using System.Linq;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Sounds;
using SwiftlyS2.Shared.Misc;
using System.Diagnostics;
using System.Buffers;
using Microsoft.Extensions.DependencyInjection;
using AudioApi;

namespace Audio;

[PluginMetadata(Id = "Audio", Version = "1.0.0", Name = "Audio", Author = "samyyc", Description = "No description.")]
public partial class Audio(ISwiftlyCore core) : BasePlugin(core) {

  private ServiceProvider? ServiceProvider { get; set; }

  public override void Load(bool hotReload) {


    var collection = new ServiceCollection();
    collection
      .AddSwiftly(Core)
      .AddSingleton<AudioManager>()
      .AddSingleton<AudioApi>()
      .AddSingleton<AudioMainloop>();

    ServiceProvider = collection.BuildServiceProvider();

    ServiceProvider.GetRequiredService<AudioMainloop>();
  }

  public override void Unload()
  {
    ServiceProvider!.Dispose();
  }

  public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
  {
    interfaceManager.AddSharedInterface<IAudioApi, AudioApi>("audio", ServiceProvider!.GetRequiredService<AudioApi>());
  }

} 