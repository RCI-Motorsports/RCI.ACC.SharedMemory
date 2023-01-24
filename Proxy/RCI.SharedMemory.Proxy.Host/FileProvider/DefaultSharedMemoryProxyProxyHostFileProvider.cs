using System.Reflection;
using System.Runtime.Versioning;
using RCI.SharedMemory.Core.Attributes;
using RCI.SharedMemory.Core.Files;

namespace RCI.SharedMemory.Proxy.Client.FileProvider;

[SupportedOSPlatform("windows")]
public class DefaultSharedMemoryProxyProxyHostFileProvider : ISharedMemoryProxyHostFileProvider
{
  public ISharedMemoryFile<T> GetFile<T>(string fileName, int? filesize = null) where T : struct
  {
    var x = AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(a => a.GetTypes())
      .Where(t => t.GetCustomAttribute<SharedMemoryFileAttribute>() != null)
      .ToArray();
    
    return new SharedMemoryLocalFile<T>(fileName, filesize);
  }
}