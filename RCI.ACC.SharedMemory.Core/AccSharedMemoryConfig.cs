namespace RCI.ACC.SharedMemory.Core;

public class AccSharedMemoryConfig
{
  public AccSharedMemoryConfig()
  {
    Graphics = new()
    {
      Enabled = true,
      UpdateInterval = 10
    };
    Physics = new()
    {
      Enabled = true,
      UpdateInterval = 1000
    };
    StaticInfo = new()
    {
      Enabled = true,
      UpdateInterval = 5000
    };
  }

  public AccSharedMemoryFileConfig Graphics { get; set; }
  public AccSharedMemoryFileConfig Physics { get; set; }
  public AccSharedMemoryFileConfig StaticInfo { get; set; }
}

public class AccSharedMemoryFileConfig
{
  public bool Enabled { get; set; }

  /// <summary>
  /// Update interval in ms
  /// </summary>
  public int UpdateInterval { get; set; }
}
