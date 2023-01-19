// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Testing;

public class Program
{
  static void Main(string[] args)
  {
    var host = Host.CreateDefaultBuilder(args)
      .ConfigureHostConfiguration(config =>
      {
        config.AddJsonFile("config.json", false, true);
      })
      .ConfigureLogging(logging =>
      {
        logging.ClearProviders();
        logging.AddConsole();
      })
      .ConfigureServices((context,services) =>
      {
        services.AddHostedService<Worker>();
        services.Configure<MySettings>(context.Configuration);
        // services.AddScoped(sp => sp.GetService<IOptionsSnapshot<MySettings>>().Value);
      })
      .Build();

    host.Run();
  }
}

public class MySettings
{
  public string Test { get; set; }
}

public class Worker : IHostedService
{
  private readonly Timer _timer;
  private readonly IConfiguration _configuration;
  private readonly IServiceProvider _sp;
  private readonly IOptionsMonitor<MySettings> _settings;

  public Worker(IConfiguration configuration,  IServiceProvider sp)
  {
    _configuration = configuration;
    _timer = new Timer(Handle, null, Timeout.Infinite, Timeout.Infinite);
    _sp = sp;
    _settings = sp.GetRequiredService<IOptionsMonitor<MySettings>>();
    _settings.OnChange(a => Console.WriteLine("Changed: " + a.Test));
  }
  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(2));
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    _timer.Change(Timeout.Infinite, Timeout.Infinite);
  }

  public void Handle(object? state)
  {
    //using var scope = _sp.CreateScope();
    //var settings = scope.ServiceProvider.GetService<MySettings>();
    Console.WriteLine(_configuration.GetValue<string>("test") + "|" + _settings.CurrentValue.Test);
  }
}


