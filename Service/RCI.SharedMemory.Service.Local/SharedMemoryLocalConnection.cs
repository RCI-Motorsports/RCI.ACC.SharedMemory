using System.Reactive.Linq;
using System.Reactive.Subjects;
using RCI.SharedMemory.Core.Files;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.Local;

public class SharedMemoryLocalConnection<T> : ISharedMemoryConnection<T> where T : struct
{
  private readonly ISharedMemoryFile<T> _file;
  private readonly IDisposable _observableConnection;

  public IConnectableObservable<T> Observable { get; }

  public SharedMemoryLocalConnection(ISharedMemoryFile<T> file, int interval)
  {
    _file = file;

    Observable = System.Reactive.Linq.Observable.Interval(TimeSpan.FromMilliseconds(interval))
      .StartWith(0)
      .Select(_ =>
      {
        if (!file.IsConnected() && !file.TryConnect())
          return null;
        return file.Read();
      }).Where(data => data is not null).Select(data => (T)data!)
      .Publish();

    _observableConnection = Observable.Connect();
    
  }
  
  public void Dispose()
  {
    _file.Dispose();
    _observableConnection.Dispose();
  }
}