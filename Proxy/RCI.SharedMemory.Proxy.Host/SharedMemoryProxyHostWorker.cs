using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using MemoryPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RCI.SharedMemory.Proxy.Client.FileProvider;

namespace RCI.SharedMemory.Proxy.Client;

[SupportedOSPlatform("windows")]
public class SharedMemoryProxyHostWorker : BackgroundService
{
  private readonly ILogger<SharedMemoryProxyHostWorker> _logger;
  private readonly ILoggerFactory _loggerFactory;
  private readonly ISharedMemoryProxyHostFileProvider _fileProvider;
  private readonly IHostApplicationLifetime _lifetime;
  private readonly SharedMemoryProxyHostConfig _config;
  private readonly UdpClient _udpClient;
  private readonly Dictionary<IPEndPoint, SharedMemoryProxyClientSender> _clients = new();

  public SharedMemoryProxyHostWorker(ILogger<SharedMemoryProxyHostWorker> logger,
    ILoggerFactory loggerFactory,
    IOptions<SharedMemoryProxyHostConfig> config,
    ISharedMemoryProxyHostFileProvider fileProvider,
    IHostApplicationLifetime lifetime)
  {
    _logger = logger;
    _loggerFactory = loggerFactory;
    _fileProvider = fileProvider;
    _lifetime = lifetime;
    _config = config.Value;
    var ip = new IPEndPoint(IPAddress.Any, _config.Port);
    _udpClient = new UdpClient(ip);
    _udpClient.FixConnectionReset();
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    await Task.Factory.StartNew(() => _lifetime.ApplicationStarted.WaitHandle.WaitOne(), cancellationToken);
    await Task.Delay(1000, cancellationToken);
    _logger.LogInformation("Listening on port {port}", _config.Port.ToString());
    try
    {
      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          var result = await _udpClient.ReceiveAsync(cancellationToken);
          if (result.Buffer[0] == SharedMemoryProxyCommands.AddFile)
          {
            using var memoryStream = new MemoryStream(result.Buffer[1..]);
            var config = await MemoryPackSerializer.DeserializeAsync<SharedMemoryFileProxyConfig>(memoryStream,
              cancellationToken: cancellationToken);

            if (!_clients.TryGetValue(result.RemoteEndPoint, out var clientSender))
            {
              clientSender = new(_loggerFactory, _fileProvider, _udpClient, result.RemoteEndPoint, cancellationToken);
              _clients.Add(result.RemoteEndPoint, clientSender);
            }

            await clientSender.AddFileAsync(config!);
          }
          else if (result.Buffer[0] == SharedMemoryProxyCommands.RemoveFile)
          {
            if (!_clients.TryGetValue(result.RemoteEndPoint, out var client))
              continue;

            var fileName = Encoding.UTF8.GetString(result.Buffer[1..]);
            await client.RemoveFileAsync(fileName);
          }
          else if (result.Buffer[0] == SharedMemoryProxyCommands.Disconnect)
          {
            var remoteEndpoint = result.RemoteEndPoint.ToString();
            _logger.LogDebug("Trying to remove client listener with ip {remoteEndpoint}", remoteEndpoint);
            if (!_clients.TryGetValue(result.RemoteEndPoint, out var client))
              continue;

            _clients.Remove(result.RemoteEndPoint);
            await client.StopAsync();
            _logger.LogDebug("Removed client listener with ip {remoteEndpoint}", remoteEndpoint);
          }
        }
        catch (SocketException ex)
        {
          // Handle the error. 10060 is a timeout error, which is expected.
          if (ex.ErrorCode != 10060) 
          {
            _logger.LogWarning(ex, "Timeout");
          }
          else
          {
            throw;
          }
        }
        catch (OperationCanceledException)
        {
          _logger.LogDebug("Stopped UdpClient");
        }
      }

      _logger.LogDebug("Stopping");
      await Task.WhenAll(
        _clients.Select(c => c.Value.StopAsync().WaitAsync(TimeSpan.FromSeconds(5), cancellationToken)));
      _logger.LogDebug("Stopped");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled error occured");
    }
  }
}