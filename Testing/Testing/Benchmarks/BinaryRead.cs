using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Testing.Benchmarks;

public class BinaryRead
{
  private byte[] _bytes;

  public BinaryRead()
  {
    using var ms = new MemoryStream();
    using var bw = new BinaryWriter(ms);
    bw.Write(1234);
    bw.Write(true);
    bw.Write((long)1235123);
    const string str = "Asdasfljshdgfkljhsdg";
    var strBytes = Encoding.UTF8.GetBytes(str);
    bw.Write((short)strBytes.Length);
    bw.Write(str);
    _bytes = ms.ToArray();
  }

  [Benchmark]
  public void BinaryReader()
  {
    using var ms = new MemoryStream(_bytes);
    using var br = new BinaryReader(ms);

    _ = br.ReadInt32();
    _ = br.ReadBoolean();
    _ = br.ReadInt64();
    var length = br.ReadInt16();
    _ = br.ReadBytes(length);
  }

  [Benchmark]
  public void Raw()
  {
    byte[] buffer = new byte[64];
    using var ms = new MemoryStream(_bytes);
    _ = ms.ReadInt(buffer);
    _ = ms.ReadBool(buffer);
    _ = ms.ReadLong(buffer);
    _ = ms.ReadString(buffer);
  }
}


public static class Ext {
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int ReadInt(this MemoryStream ms, byte[] buffer)
  {
    _ = ms.Read(buffer, 0, 4);
    return BitConverter.ToInt32(buffer);
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static long ReadLong(this MemoryStream ms, byte[] buffer)
  {
    _ = ms.Read(buffer, 0, 8);
    return BitConverter.ToInt64(buffer);
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static int ReadShort(this MemoryStream ms, byte[] buffer)
  {
    _ = ms.Read(buffer, 0, 2);
    return BitConverter.ToInt16(buffer);
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static bool ReadBool(this MemoryStream ms, byte[] buffer)
  {
    _ = ms.Read(buffer, 0, 1);
    return BitConverter.ToBoolean(buffer);
  }
  
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static byte[] ReadString(this MemoryStream ms, byte[] buffer)
  {
    _ = ms.Read(buffer, 0, 2); // Length 
    var length = BitConverter.ToInt16(buffer);
    _ = ms.Read(buffer, 0, length);
    return buffer;
  }
}