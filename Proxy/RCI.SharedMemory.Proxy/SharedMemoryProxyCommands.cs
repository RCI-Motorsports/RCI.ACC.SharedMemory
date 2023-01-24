namespace RCI.SharedMemory.Proxy;

public static class SharedMemoryProxyCommands
{
  public static byte AddFile => 0x00;
  public static byte RemoveFile => 0x01;
  public static byte Disconnect => 0x02;
}