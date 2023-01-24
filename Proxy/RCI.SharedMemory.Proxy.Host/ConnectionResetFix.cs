using System.Net.Sockets;

namespace RCI.SharedMemory.Proxy.Client;

/// <summary>
/// Fix for windows connection reset issue
/// <a href="http://stackoverflow.com/questions/74327225/why-does-sending-via-a-udpclient-cause-subsequent-receiving-to-fail">Stackoverflow</a>
/// </summary>
public static class ConnectionResetFix
{
  const int SioUdpConnreset = unchecked((int)(IocIn | IocVendor | 12));
  const uint IocIn = 0x80000000U;
  const uint IocVendor = 0x18000000U;
  
  public static void FixConnectionReset(this UdpClient udpClient)
  
  { udpClient.Client.IOControl(SioUdpConnreset, new byte[] { 0x00 }, null);
  }
}