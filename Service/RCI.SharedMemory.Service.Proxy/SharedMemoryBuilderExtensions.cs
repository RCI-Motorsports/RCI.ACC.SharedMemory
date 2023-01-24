using RCI.SharedMemory.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace RCI.SharedMemory.Service.Remote;

public static class SharedMemoryBuilderExtensions
{
  public static SharedMemoryServiceBuilder AddProxyProvider(this SharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<ISharedMemoryProvider, SharedMemoryProxyProvider>();
    builder.Services.AddHostedService(s => (SharedMemoryProxyProvider)s.GetRequiredService<ISharedMemoryProvider>());
    builder.Services.Configure<SharedMemoryProxyConfig>(builder.Configuration.GetSection("SharedMemory")
      .GetSection("Proxy"));
    return builder;
  }
}

