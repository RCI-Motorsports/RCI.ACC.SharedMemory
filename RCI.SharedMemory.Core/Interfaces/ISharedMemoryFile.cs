using System.Runtime.InteropServices;

namespace RCI.SharedMemory.Core.Interfaces;
public interface ISharedMemoryFile<T> : IDisposable where T: struct
{
  void Connect();
  byte[] ReadRaw();
  public T? Read()
  {
    var bytes = ReadRaw();
    var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    
    var marshalledObject = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
    if (marshalledObject is null)
      return null;
    
    var data = (T)marshalledObject;
    handle.Free();
    return data;
  }
}
