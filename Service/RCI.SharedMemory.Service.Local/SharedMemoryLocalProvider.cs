using System.Reflection;
using System.Runtime.Versioning;
using RCI.SharedMemory.Core;
using RCI.SharedMemory.Core.Attributes;
using RCI.SharedMemory.Core.Files;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.Local;

[SupportedOSPlatform("windows")]
public class SharedMemoryLocalProvider : ISharedMemoryProvider
{
  public ISharedMemoryConnection<T> GetConnection<T>(int interval) where T : struct
  {
    var nameAttribute = typeof(T).GetCustomAttribute<SharedMemoryFileAttribute>();
    if (nameAttribute is null)
      throw new Exception();

    var file = new SharedMemoryLocalFile<T>(nameAttribute.Name);
    return new SharedMemoryLocalConnection<T>(file, interval);
  }
}