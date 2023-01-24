using RCI.SharedMemory.Core;
using RCI.SharedMemory.Core.Files;

namespace RCI.SharedMemory.Static;

public class SharedMemoryStaticConnection<T> : IDisposable where T : struct
{
  private readonly ISharedMemoryFile<T> _file;
  private readonly int _intervalMs;
  private readonly Timer _timer;

  public delegate void UpdatedEventHandler(T value);

  public event UpdatedEventHandler? Updated;

  public SharedMemoryStaticConnection(ISharedMemoryFile<T> file, int intervalMs)
  {
    _file = file;
    _intervalMs = intervalMs;
    _timer = new Timer(_ =>
    {
      var value = _file.Read();
      if (value is null)
        return;

      Updated?.Invoke(value.Value);
    }, null, Timeout.Infinite, Timeout.Infinite);
    file.Connect();
  }

  public void Restart()
  {
    _timer.Start(TimeSpan.FromMilliseconds(_intervalMs));
  }

  public void Stop()
  {
    _timer.Stop();
  }

  public void Dispose()
  {
    _timer.Dispose();
    _file.Dispose();
  }
}