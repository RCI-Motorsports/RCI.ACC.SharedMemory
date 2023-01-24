using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RCI.SharedMemory.Proxy.Client.FileProvider;

namespace RCI.SharedMemory.Proxy.Client;

[SupportedOSPlatform("windows")]
public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSharedMemoryHostWorker(this IServiceCollection services,
    IConfiguration configuration)
  {
    services.Configure<SharedMemoryProxyHostConfig>(configuration.GetSection("SharedMemory").GetSection("ProxyHost"));
    services.AddSingleton<ISharedMemoryProxyHostFileProvider, DefaultSharedMemoryProxyProxyHostFileProvider>();
    return services.AddHostedService<SharedMemoryProxyHostWorker>();
  }
}