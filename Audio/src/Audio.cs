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

using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared;
using Microsoft.Extensions.DependencyInjection;
using AudioApi;
using Microsoft.Extensions.Configuration;
using SwiftlyS2.Shared.Commands;

namespace Audio;

[PluginMetadata(
  Id = "Audio", 
  Version = "0.4.0", 
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
      .AddSingleton<AudioMainloop>();
    
    collection
      .AddOptions<AudioConfig>()
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


    // [Command("test4")]
    // public void Test3(ICommandContext context) {
    //   var api = ServiceProvider!.GetRequiredService<AudioApi>();
    //   var channel = api.UseChannel("test");

    // Console.WriteLine("Source decoded");
    // Task.Run(async () =>
    //   {
    //     var task = api.DecodeFromFileAsync("E:/p.mp3");
    //     var source = await task;
    //     Console.WriteLine("Source decoded");
    //     channel.SetSource(source);
    //     channel.PlayToAll();
    //   });
    // }

} 