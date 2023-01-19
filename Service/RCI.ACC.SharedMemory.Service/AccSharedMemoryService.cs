using RCI.ACC.SharedMemory.Core.Structs;
using RCI.ACC.SharedMemory.Service.Interfaces;
using System.Reactive.Subjects;

namespace RCI.ACC.SharedMemory.Service;
internal class AccSharedMemoryService : IAccSharedMemoryService
{
  private IAccSharedMemoryProvider _provider;

  public IObservable<Physics> Physics => throw new NotImplementedException();

  public AccSharedMemoryService(IAccSharedMemoryProvider provider)
  {
    _provider = provider;
    Physics = new Subject<Physics>();
  }
}
