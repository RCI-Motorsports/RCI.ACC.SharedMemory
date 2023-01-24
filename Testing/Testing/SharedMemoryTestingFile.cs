using RCI.SharedMemory.Core.Files;

namespace Testing;

public class SharedMemoryTestingFile<T> : ISharedMemoryFile<T> where T : struct
{
  private readonly string _memoryFilePath;
  private byte[]? _data;

  public string MemoryFilePath => _memoryFilePath;

  public SharedMemoryTestingFile(string folder, string fileName)
  {
    var memoryFileName = GetMappedFileName(Path.GetFileName(fileName));
    _memoryFilePath = Path.Combine(folder, memoryFileName);
  }

  private static string GetMappedFileName(string fileName) => fileName.Replace(@"\", "_");

  public bool IsConnected() => _data is not null;

  public void Connect()
  {
    _data = File.ReadAllBytes(_memoryFilePath);
  }

  public bool TryConnect()
  {
    if (!File.Exists(_memoryFilePath))
      return false;

    Connect();
    return true;
  }

  public byte[] ReadRaw()
  {
    if (_data is null)
      throw new FileNotFoundException(_memoryFilePath);

    return _data;
  }

  public void Dispose()
  {
  }
}