using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SwiftlyS2.Shared;

namespace Audio.Decoders;

public class NativeDecoder : IPcmDecoder {

  private static readonly HttpClient Http = new();

  public byte[] Decode(byte[] data) {
    ArgumentNullException.ThrowIfNull(data);
    return NativeBindings.Decode(data);
  }

  public Task<byte[]> DecodeAsync(byte[] data) {
    ArgumentNullException.ThrowIfNull(data);
    return Task.Run(() => NativeBindings.Decode(data));
  }

  public byte[] DecodeFromFile(string path) {
    ArgumentException.ThrowIfNullOrEmpty(path);
    var bytes = File.ReadAllBytes(path);
    return NativeBindings.Decode(bytes);
  }

  public async Task<byte[]> DecodeFromFileAsync(string path) {
    ArgumentException.ThrowIfNullOrEmpty(path);
    var bytes = await File.ReadAllBytesAsync(path).ConfigureAwait(false);
    return await Task.Run(() => NativeBindings.Decode(bytes)).ConfigureAwait(false);
  }

  public byte[] DecodeFromUrl(Uri url) {
    ArgumentNullException.ThrowIfNull(url);
    var data = Http.GetByteArrayAsync(url).GetAwaiter().GetResult();
    return NativeBindings.Decode(data);
  }

  public async Task<byte[]> DecodeFromUrlAsync(Uri url) {
    ArgumentNullException.ThrowIfNull(url);
    var data = await Http.GetByteArrayAsync(url).ConfigureAwait(false);
    return await Task.Run(() => NativeBindings.Decode(data)).ConfigureAwait(false);
  }

  private static class NativeBindings {
    private const string LibraryName = "pcmdecoder";
    private static readonly object SyncRoot = new();

    [SwiftlyInject]
    private static ISwiftlyCore Core = null!;

    static NativeBindings() {
      NativeLibrary.SetDllImportResolver(typeof(NativeBindings).Assembly, DllImportResolver);
    }
    private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
      if (libraryName == "pcmdecoder")
      {
        if (OperatingSystem.IsWindows()) {
          var runtimePath = Path.Combine(
              Core.PluginPath,
              "resources",
              "natives",
              "pcmdecoder.dll"
          );

          if (NativeLibrary.TryLoad(runtimePath, out var handle))
            return handle;
        } else if (OperatingSystem.IsLinux()) {
          var runtimePath = Path.Combine(
              Core.PluginPath,
              "resources",
              "natives",
              "libpcmdecoder.so"
          );

          if (NativeLibrary.TryLoad(runtimePath, out var handle))
            return handle;
        }

      }

      return IntPtr.Zero;
    }


    [DllImport(LibraryName, EntryPoint = "pcmdecoder_decode", ExactSpelling = true)]
    private static extern bool PcmDecoderDecode(IntPtr dataPtr, UIntPtr len);

    [DllImport(LibraryName, EntryPoint = "pcmdecoder_get_size", ExactSpelling = true)]
    private static extern UIntPtr PcmDecoderGetSize();

    [DllImport(LibraryName, EntryPoint = "pcmdecoder_copy", ExactSpelling = true)]
    private static extern bool PcmDecoderCopy(IntPtr destPtr);

    [DllImport(LibraryName, EntryPoint = "pcmdecoder_free", ExactSpelling = true)]
    private static extern void PcmDecoderFree();

    public static unsafe byte[] Decode(byte[] data) {

      lock(SyncRoot) {

        if (data.Length == 0) {
          return Array.Empty<byte>();
        }

        bool decoded;
        fixed (byte* dataPtr = data) {
          decoded = PcmDecoderDecode((IntPtr)dataPtr, (UIntPtr)data.Length);
        }

        if (!decoded) {
          PcmDecoderFree();
          throw new InvalidOperationException("Native PCM decoder failed to decode the provided data. Check native logs for details.");
        }

        var sampleCount = PcmDecoderGetSize();
        if (sampleCount == 0) {
          PcmDecoderFree();
          return Array.Empty<byte>();
        }


        var samples = new byte[sampleCount * 4];

        fixed (byte* destPtr = samples) {
          var success = PcmDecoderCopy((IntPtr)destPtr);
          if (!success) {
            PcmDecoderFree();
            throw new InvalidOperationException("Failed to copy PCM data from native decoder buffer.");
          }
          PcmDecoderFree();
        }

        return samples;
      }
    }

  }
}