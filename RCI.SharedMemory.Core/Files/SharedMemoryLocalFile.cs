using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace RCI.SharedMemory.Core.Files;

[SupportedOSPlatform("windows")]
public class SharedMemoryLocalFile<T> : ISharedMemoryFile<T> where T : struct
{
  private readonly string _memoryFileName;
  private MemoryMappedFile? _memoryMappedFile;
  private readonly int _size;

  public SharedMemoryLocalFile(string memoryFileName, int? overrideSize = null)
  {
    _memoryFileName = memoryFileName;
    _size = overrideSize ?? Marshal.SizeOf<T>();
  }

  public void Connect()
  {
    _memoryMappedFile = MemoryMappedFile.OpenExisting(_memoryFileName);
  }

  public bool TryConnect()
  {
    try
    {
      Connect();
      return true;
    }
    catch
    {
      // ignored
      return false;
    }
  }

  public bool IsConnected() => _memoryMappedFile is { };

  public byte[] ReadRaw()
  {
    if (_memoryMappedFile is null)
      throw new Exception($"Not connected to mapped memory file ({_memoryFileName})");

    using var viewStream = _memoryMappedFile.CreateViewStream();
    using var binaryReader = new BinaryReader(viewStream);

    return binaryReader.ReadBytes(_size);
  }

  public void Dispose()
  {
    _memoryMappedFile?.Dispose();
  }
}