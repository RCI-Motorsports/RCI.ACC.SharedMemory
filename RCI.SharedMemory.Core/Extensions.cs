namespace RCI.SharedMemory.Core; 

public static class Extensions
{
  public static void Stop(this Timer timer) => timer.Change(Timeout.Infinite, Timeout.Infinite);
  public static void Start(this Timer timer, TimeSpan interval) => timer.Change(TimeSpan.Zero, interval);
}
