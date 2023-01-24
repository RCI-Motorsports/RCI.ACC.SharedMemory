using RCI.SharedMemory.Core.Files;

namespace RCI.SharedMemory.Proxy.Client.FileProvider;

public interface ISharedMemoryProxyHostFileProvider
{
  ISharedMemoryFile<T> GetFile<T>(string fileName, int? filesize = null) where T : struct;
}