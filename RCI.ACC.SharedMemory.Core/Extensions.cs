using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCI.ACC.SharedMemory.Core; 
public static class Extensions
{
  public void Stop(this Timer timer) => timer.Change(Timeout.Infinite, Timeout.Infinite);
  public void Start(this Timer timer, TimeSpan interval) => timer.Change(TimeSpan.Zero, interval);
}
