using System.Runtime.InteropServices;

namespace RCI.ACC.SharedMemory.Core.Interfaces;
public interface ISharedMemoryFile<T> where T: struct
{
  void Connect();
  byte[] ReadRaw();
  public T Read()
  {
    var bytes = ReadRaw();
    var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    var data = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
    handle.Free();
    return data;
  }
}
