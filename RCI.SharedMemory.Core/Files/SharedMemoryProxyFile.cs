namespace RCI.SharedMemory.Core.Files;

public class SharedMemoryProxyFile<T> : ISharedMemoryFile<T> where T : struct
{
  private byte[]? _data;

  public void Fill(byte[] data)
  {
    _data = data;
  }

  public bool IsConnected() => throw new NotImplementedException();

  public void Connect() => throw new NotImplementedException();

  public bool TryConnect() => throw new NotImplementedException();

  public byte[] ReadRaw() => _data!;
  public void Dispose()
  {
    _data = null;
  }
}