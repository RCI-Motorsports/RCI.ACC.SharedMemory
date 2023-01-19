using RCI.ACC.SharedMemory.Core.Structs;

namespace RCI.ACC.SharedMemory.Service.Interfaces;

public interface IAccSharedMemoryService
{
  public IObservable<Physics> Physics { get; }
}
