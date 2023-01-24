using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Text;
using Microsoft.Extensions.Logging;
using RCI.SharedMemory.Proxy.Client.FileProvider;

namespace RCI.SharedMemory.Proxy.Client;

[SupportedOSPlatform("windows")]
public class SharedMemoryProxyClientSender
{
  private readonly ISharedMemoryProxyHostFileProvider _fileProvider;
  private readonly UdpClient _udpClient;
  private readonly IPEndPoint _clientIp;
  private readonly CancellationTokenSource _cancellationTokenSource;
  private readonly ILogger<SharedMemoryProxyClientSender> _logger;

  private Dictionary<string, (Task Task, CancellationTokenSource TokenSource)> Tasks { get; } = new();

  public SharedMemoryProxyClientSender(ILoggerFactory loggerFactory, ISharedMemoryProxyHostFileProvider fileProvider,
    UdpClient udpClient, IPEndPoint clientIp, CancellationToken parentToken)
  {
    _logger = loggerFactory.CreateLogger<SharedMemoryProxyClientSender>();
    _fileProvider = fileProvider;
    _udpClient = udpClient;
    _clientIp = clientIp;
    _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(parentToken);
  }

  public async Task AddFileAsync(SharedMemoryFileProxyConfig config)
  {
    if (Tasks.ContainsKey(config.Filename))
      await RemoveFileAsync(config.Filename);

    var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token);
    var task = Task.Factory.StartNew(() => StartSendingAsync(config, cancellationTokenSource.Token),
      cancellationTokenSource.Token);

    Tasks.Add(config.Filename, (task, cancellationTokenSource));
    _logger.LogDebug("New shared memory file listener added for file {fileName}", config.Filename);
  }

  public async Task RemoveFileAsync(string fileName)
  {
    _logger.LogDebug("Listener removal requested for file {fileName}", fileName);
    if (Tasks.TryGetValue(fileName, out var task))
    {
      task.TokenSource.Cancel();
      await task.Task.WaitAsync(TimeSpan.FromSeconds(5));
      Tasks.Remove(fileName);
      _logger.LogDebug("Listener for file {fileName} removed", fileName);
      return;
    }

    _logger.LogDebug("{fileName} not found. Request ignored", fileName);
  }

  private async Task StartSendingAsync(SharedMemoryFileProxyConfig config, CancellationToken cancellationToken)
  {
    var fileNameBytes = Encoding.UTF8.GetBytes(config.Filename);
    var baseBytes = BitConverter.GetBytes((short)fileNameBytes.Length)
      .Concat(fileNameBytes)
      .ToArray();

    using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(config.Interval));
    using var loggerScope = _logger.BeginScope(config.Filename);

    var file = _fileProvider.GetFile<MappedMemoryFilePlaceholder>(config.Filename, config.Filesize);
    while (await timer.WaitForNextTickAsync(cancellationToken))
    {
      if (!file.IsConnected() && !file.TryConnect())
      {
        _logger.LogDebug("File is not connected and failed to reconnect");
        continue;
      }

      var fileBytes = file.ReadRaw();
      await _udpClient.SendAsync(baseBytes.Concat(fileBytes).ToArray(), _clientIp, cancellationToken);
    }
  }

  public async Task StopAsync()
  {
    _logger.LogDebug("Stop requested");
    _cancellationTokenSource.Cancel();
    await Task.WhenAll(Tasks.Select(t => t.Value.Task.WaitAsync(TimeSpan.FromSeconds(5))));
    _logger.LogDebug("Stopped");
  }
}