using System.Reactive.Subjects;

namespace RCI.SharedMemory.Service.Interfaces;

public interface ISharedMemoryConnection<out T> : IDisposable where T : struct
{
  public IConnectableObservable<T> Observable { get; }
}