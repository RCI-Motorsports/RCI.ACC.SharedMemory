using RCI.ACC.SharedMemory.Core.Interfaces;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace RCI.ACC.SharedMemory.Core;


public class LocalSharedMemoryFile<T> : ISharedMemoryFile<T> where T : struct
{
  private readonly string _memoryFileName;
  private MemoryMappedFile? _memoryMappedFile;

  public LocalSharedMemoryFile(string memoryFileName)
  {
    _memoryFileName = memoryFileName;
  }

  public void Connect()
  {
    _memoryMappedFile = MemoryMappedFile.OpenExisting(_memoryFileName);
  }

  public T Read()
  {
    var bytes = ReadRaw();
    var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    var obj = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
    handle.Free();

    if (obj is T and { } data)
      return data;

    throw new Exception();
  }

  public byte[] ReadRaw()
  {
    if (_memoryMappedFile is null)
      throw new Exception($"Not connected to mapped memory file ({_memoryFileName})");

    using var viewStream = _memoryMappedFile.CreateViewStream();
    using var binaryReader = new BinaryReader(viewStream);

    return binaryReader.ReadBytes(Marshal.SizeOf(typeof(T)));

  }
}
