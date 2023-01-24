using MemoryPack;

namespace RCI.SharedMemory.Proxy;

[MemoryPackable]
public partial class SharedMemoryFileProxyConfig
{
  public int Interval { get; init; }
  public string Filename { get; init; }
  public int Filesize { get; set; }
}