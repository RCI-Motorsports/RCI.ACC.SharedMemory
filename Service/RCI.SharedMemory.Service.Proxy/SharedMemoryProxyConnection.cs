using System.Reactive.Linq;
using System.Reactive.Subjects;
using RCI.SharedMemory.Core.Files;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.Remote;

public class SharedMemoryProxyConnection<T> : ISharedMemoryConnection<T> where T : struct
{
  private readonly ISharedMemoryFile<T> _file;
  private readonly IDisposable _observableConnection;
  public IConnectableObservable<T> Observable { get; }
  
  public SharedMemoryProxyConnection(IObservable<byte[]> bytes)
  {
    var remoteFile = new SharedMemoryProxyFile<T>();
    _file = remoteFile;

    Observable = bytes.Select(data =>
    {
      remoteFile.Fill(data);
      return _file.Read();
    }).Where(v => v is not null).Select(v => (T)v!).Publish()!;
    _observableConnection = Observable.Connect();
  }

  public void Dispose()
  {
    _file.Dispose();
    _observableConnection.Dispose();
  }
}