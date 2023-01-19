using RCI.ACC.SharedMemory.Core.Interfaces;
using System.Runtime.InteropServices;

namespace RCI.ACC.SharedMemory.Core;

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
