using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using MemoryPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RCI.SharedMemory.Core.Attributes;
using RCI.SharedMemory.Proxy;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.Remote;

public class SharedMemoryProxyProvider : ISharedMemoryProvider, IHostedService
{
  private readonly ILogger<SharedMemoryProxyProvider> _logger;
  private readonly UdpClient _udpClient = new(0, AddressFamily.InterNetwork);
  private readonly CancellationTokenSource _cancellationTokenSource = new();
  private readonly Subject<(string FileName, byte[] data)> _receiveSubject = new();
  private readonly IPEndPoint _proxyEndpoint;

  private Task? _listenTask;

  public SharedMemoryProxyProvider(ILogger<SharedMemoryProxyProvider> logger,
    IOptions<SharedMemoryProxyConfig> config)
  {
    _logger = logger;
    _proxyEndpoint = new IPEndPoint(IPAddress.Parse(config.Value.Ip), config.Value.Port);
  }

  public ISharedMemoryConnection<T> GetConnection<T>(int interval) where T : struct
  {
    var attribute = typeof(T).GetCustomAttribute<SharedMemoryFileAttribute>();
    if (attribute is null)
      throw new Exception();

    _udpClient.Send(GetAddFileCommand(interval, attribute.Name, Marshal.SizeOf<T>()), _proxyEndpoint);

    return new SharedMemoryProxyConnection<T>(_receiveSubject.Where(v => v.FileName == attribute.Name)
      .Select(v => v.data)
      .AsObservable());
  }

  public Task StartAsync(CancellationToken cancellationToken)
  {
    _logger.LogDebug("Starting listen task");
    _listenTask = Listen();
    return Task.CompletedTask;
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _logger.LogDebug("Stopping listen task");
    _cancellationTokenSource.Cancel();
    try
    {
      if (_listenTask is not null && !_listenTask.IsCanceled)
        await _listenTask.WaitAsync(cancellationToken);
    }
    catch (OperationCanceledException) { }

    await _udpClient.SendAsync(new[] { SharedMemoryProxyCommands.Disconnect }, _proxyEndpoint, cancellationToken);
    _logger.LogDebug("Listen task stopped");
  }

  private async Task Listen()
  {
    while (!_cancellationTokenSource.IsCancellationRequested)
    {
      try
      {
        var result = await _udpClient.ReceiveAsync(_cancellationTokenSource.Token);
        var fileNameLength = BitConverter.ToInt16(result.Buffer[..^2]);
        var dataIndex = fileNameLength + 2;
        var x = result.Buffer[2..dataIndex];
        var fileName = Encoding.UTF8.GetString(x);
        _receiveSubject.OnNext((fileName, result.Buffer[dataIndex..]));
      }
      catch (SocketException ex)
      {
        _logger.LogError(ex, "error");
      }
    }
  }

  private byte[] GetAddFileCommand(int interval, string filename, int filesize)
  {
    return new[] { SharedMemoryProxyCommands.AddFile }
      .Concat(MemoryPackSerializer.Serialize(new SharedMemoryFileProxyConfig()
      { Interval = interval, Filename = filename, Filesize = filesize}))
      .ToArray();
  }

  // private byte[] GetRemoveFileCommand(int interval, string filename)
  // {
  //   return new[] { SharedMemoryProxyCommands.RemoveFile }
  //     .Concat(MemoryPackSerializer.Serialize(new SharedMemoryFileProxyConfig()
  //       { Interval = interval, Filename = filename }))
  //     .ToArray();
  // }
}