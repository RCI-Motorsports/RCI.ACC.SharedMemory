namespace RCI.SharedMemory.Core;



public class SharedMemoryFileConfig<T> where T: struct
{
  public Type StructType { get; }

  /// <summary>
  /// Update interval in ms
  /// </summary>
  public int UpdateInterval { get; }

  public SharedMemoryFileConfig(int updateInterval)
  {
    UpdateInterval = updateInterval;
    StructType = typeof(T);
  }
}
