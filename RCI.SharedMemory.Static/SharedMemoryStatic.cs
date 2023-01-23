using System.Reflection;
using System.Runtime.Versioning;
using RCI.SharedMemory.ACC.Structs;
using RCI.SharedMemory.Core;
using RCI.SharedMemory.Core.Attributes;
using RCI.SharedMemory.Core.Interfaces;

namespace RCI.SharedMemory.Static;

[SupportedOSPlatform("windows")]
public static class SharedMemoryStatic
{
  public static SharedMemoryStaticConnection<T> Connect<T>(int intervalMs) where T : struct
  {
    var nameAttribute = typeof(T).GetCustomAttribute<SharedMemoryFileAttribute>();
    if (nameAttribute is null)
      throw new Exception();

    var file = (ISharedMemoryFile<T>)new LocalSharedMemoryFile<Physics>(nameAttribute.Name);
    return new SharedMemoryStaticConnection<T>(file, intervalMs);
  }
}