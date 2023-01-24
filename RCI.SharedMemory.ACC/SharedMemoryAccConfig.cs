namespace RCI.SharedMemory.ACC;

public class SharedMemoryAccConfig
{
  public SharedMemoryAccConfig()
  {
    Graphics = new()
    {
      Enabled = true,
      Interval = 10
    };
    Physics = new()
    {
      Enabled = true,
      Interval = 1000
    };
    StaticInfo = new()
    {
      Enabled = true,
      Interval = 5000
    };
  }

  public int RetryTimerInterval { get; init; } = 2000;

  public AccSharedMemoryFileStaticConfig Graphics { get; init; }
  public AccSharedMemoryFileStaticConfig Physics { get; init; }
  public AccSharedMemoryFileStaticConfig StaticInfo { get; init; }
}

public class AccSharedMemoryFileStaticConfig
{
  public bool Enabled { get; init; }

  /// <summary>
  /// Update interval in ms
  /// </summary>
  public int Interval { get; init;}
}