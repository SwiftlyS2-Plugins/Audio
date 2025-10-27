using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared;
using Microsoft.Extensions.DependencyInjection;
using AudioApi;
using Microsoft.Extensions.Configuration;
using SwiftlyS2.Shared.Commands;

namespace Audio;

[PluginMetadata(
  Id = "Audio", 
  Version = "0.2.0", 
  Name = "Audio", 
  Author = "samyyc", 
  Description = "A high performance VoIP audio lib for swiftlys2."
)]
public partial class Audio(ISwiftlyCore core) : BasePlugin(core) {

  private ServiceProvider? ServiceProvider { get; set; }

  public override void Load(bool hotReload) {

    Core.Configuration
      .InitializeJsonWithModel<AudioConfig>("config.jsonc", "Main")
      .Configure(builder =>
      {
        builder.AddJsonFile("config.jsonc", false, true);
      });

    var collection = new ServiceCollection();
    collection
      .AddSwiftly(Core)
      .AddSingleton<AudioManager>()
      .AddSingleton<AudioApi>()
      .AddSingleton<AudioMainloop>()
      .AddOptionsWithValidateOnStart<AudioConfig>()
      .BindConfiguration("Main");

    ServiceProvider = collection.BuildServiceProvider();

    var mainloop = ServiceProvider.GetRequiredService<AudioMainloop>();

    if (hotReload) {
      mainloop.IsRunning = true;
    }

    Core.Event.OnMapLoad += (map) => {
      mainloop.IsRunning = true;
    };

    Core.Event.OnMapUnload += (map) => {
      mainloop.IsRunning = false;
    };
  }

  public override void Unload()
  {
    ServiceProvider!.Dispose();
  }

  public override void ConfigureSharedInterface(IInterfaceManager interfaceManager)
  {
    interfaceManager.AddSharedInterface<IAudioApi, AudioApi>("audio", ServiceProvider!.GetRequiredService<AudioApi>());
  }

    // public override void UseSharedInterface(IInterfaceManager interfaceManager)
    // {
    //     var api = interfaceManager.GetSharedInterface<IAudioApi>("audio");
    //     var channel = api.UseChannel("test");
    //     channel.SetSource(api.DecodeFromFile("E:/p.mp3"));
    //     channel.SetVolumeToAll(0.5f);
    //     channel.PlayToAll();
    // }


    // [Command("test3")]
    // public void Test3(ICommandContext context) {
    //   var api = ServiceProvider!.GetRequiredService<AudioApi>();
    //   var channel = api.UseChannel("test");
    //   channel.SetSource(api.DecodeFromFile("E:/p.mp3"));

    //   channel.SetVolumeToAll(1.2f);
    //   channel.PlayToAll();
    // }

} 