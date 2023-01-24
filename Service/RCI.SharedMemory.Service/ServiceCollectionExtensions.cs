using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service;

public static class ServiceCollectionExtensions
{
  public static SharedMemoryServiceBuilder AddSharedMemoryService<T>(this IServiceCollection services,
    IConfiguration configuration)
    where T : class, ISharedMemoryService
  {
    services.AddSingleton<ISharedMemoryService, T>();
    services.AddSingleton<T>(s => (T)s.GetRequiredService<ISharedMemoryService>());

    return new SharedMemoryServiceBuilder(services, configuration);
  }
}

public class SharedMemoryServiceBuilder
{
  public IConfiguration Configuration { get; }
  public IServiceCollection Services { get; }

  internal SharedMemoryServiceBuilder(IServiceCollection services, IConfiguration configuration)
  {
    Configuration = configuration;
    Services = services;
  }
}