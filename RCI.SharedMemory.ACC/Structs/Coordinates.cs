using System.Runtime.InteropServices;

// ReSharper disable MemberCanBePrivate.Global

namespace RCI.SharedMemory.ACC.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct Coordinates
{
  public float X;
  public float Y;
  public float Z;
}