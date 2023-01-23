using RCI.SharedMemory.Core.Interfaces;

namespace RCI.SharedMemory.Core;

public class RemoteSharedMemoryFile<T> where T : struct, ISharedMemoryFile<T>
{
  private readonly byte[] _data;
  public RemoteSharedMemoryFile(byte[] data)
  {
    _data = data;
  }

  public void Connect() => throw new NotImplementedException();

  public byte[] ReadRaw() => _data;
}
