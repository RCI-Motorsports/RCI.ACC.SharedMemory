using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RCI.SharedMemory.Service.ACC;
using RCI.SharedMemory.Service.Remote;
using System.Reactive.Linq;

namespace Testing.Client;

public class Client
{
  public static async Task RunAsync(string[] args)
  {
    var host = Host.CreateDefaultBuilder(args)
      .ConfigureHostConfiguration(config => { })
      .ConfigureLogging((context, logging) =>
      {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(args.Contains("--debug") || context.HostingEnvironment.IsDevelopment()
          ? LogLevel.Debug
          : LogLevel.Information);
      })
      .ConfigureServices((context, services) =>
      {
        services.AddHostedService<Worker>();
        services
          .AddSharedMemoryAccService(context.Configuration)
          .AddProxyProvider();
        // .AddLocalProvider();
      })
      .Build();

    await host.RunAsync();
  }

  class Worker : IHostedService
  {
    private readonly SharedMemoryAccService _service;
    private readonly ILogger<Worker> _logger;

    public Worker(SharedMemoryAccService service, ILogger<Worker> logger)
    {
      _service = service;
      _logger = logger;
    }


    public Task StartAsync(CancellationToken cancellationToken)
    {
      _service.Physics!
        .Select(p => Math.Round(p.SpeedKmh))
        .DistinctUntilChanged()
        .Subscribe(s => { _logger.LogInformation($"{s} km/h"); });

      //_service.Physics!
      //  .Select(p => Math.Round(p.Brake, 2))
      //  .DistinctUntilChanged()
      //  .Subscribe(s => { _logger.LogInformation($"{s} Brake"); });

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}