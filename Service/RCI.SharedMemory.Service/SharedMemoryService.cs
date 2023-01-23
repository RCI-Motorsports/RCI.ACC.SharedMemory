using System.Reactive.Subjects;
using RCI.SharedMemory.ACC.Structs;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service;
internal class SharedMemoryService : ISharedMemoryService
{
  private ISharedMemoryProvider _provider;

  public IObservable<Physics> Physics => throw new NotImplementedException();

  public SharedMemoryService(ISharedMemoryProvider provider)
  {
    _provider = provider;
    // Physics = new Subject<Physics>();
  }
}
