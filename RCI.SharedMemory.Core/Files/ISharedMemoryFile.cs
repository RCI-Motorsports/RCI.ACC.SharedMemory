using System.Runtime.InteropServices;

namespace RCI.SharedMemory.Core.Files;

public interface ISharedMemoryFile<T> : IDisposable where T : struct
{
  bool IsConnected();
  void Connect();
  bool TryConnect();
  byte[] ReadRaw();

  public T? Read()
  {
    var bytes = ReadRaw();
    var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    var marshalledObject = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
    handle.Free();

    if (marshalledObject is T and { } data)
      return data;

    throw new Exception();
  }
}