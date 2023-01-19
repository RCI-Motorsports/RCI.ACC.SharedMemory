using System.Runtime.InteropServices;

namespace RCI.ACC.SharedMemory.Core.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct Coordinates
{
  public float X;
  public float Y;
  public float Z;
}