using Microsoft.Extensions.DependencyInjection;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service;

public static class ServiceCollectionExtensions
{
  public static SharedMemoryServiceBuilder AddAccSharedMemoryService(this IServiceCollection services)
  {
    services.AddSingleton<ISharedMemoryService>();


    return new SharedMemoryServiceBuilder(services);
  }
}

public class SharedMemoryServiceBuilder
{
  public IServiceCollection Services { get; }
  internal SharedMemoryServiceBuilder(IServiceCollection services)
  {
    Services = services;
  }
}

