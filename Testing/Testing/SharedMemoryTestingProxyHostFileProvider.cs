using RCI.SharedMemory.Core.Files;
using RCI.SharedMemory.Proxy.Client.FileProvider;

namespace Testing;

public class SharedMemoryTestingProxyHostFileProvider : ISharedMemoryProxyHostFileProvider
{
  public ISharedMemoryFile<T> GetFile<T>(string fileName, int? filesize) where T : struct
    => new SharedMemoryTestingFile<T>(Path.Combine(Directory.GetCurrentDirectory(), "TestMappedFiles"), fileName);
}