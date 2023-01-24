namespace RCI.SharedMemory.Service.Interfaces;

public interface ISharedMemoryProvider
{
  ISharedMemoryConnection<T> GetConnection<T>(int interval) where T : struct;
}