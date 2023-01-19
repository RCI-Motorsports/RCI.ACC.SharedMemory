using Microsoft.Extensions.DependencyInjection;
using RCI.ACC.SharedMemory.Service.Interfaces;

namespace RCI.ACC.SharedMemory.Service;

public static class ServiceCollectionExtensions
{
  public static AccSharedMemoryServiceBuilder AddAccSharedMemoryService(this IServiceCollection services)
  {
    services.AddSingleton<IAccSharedMemoryService>();


    return new AccSharedMemoryServiceBuilder(services);
  }
}

public class AccSharedMemoryServiceBuilder
{
  public IServiceCollection Services { get; }
  internal AccSharedMemoryServiceBuilder(IServiceCollection services)
  {
    Services = services;
  }
}

