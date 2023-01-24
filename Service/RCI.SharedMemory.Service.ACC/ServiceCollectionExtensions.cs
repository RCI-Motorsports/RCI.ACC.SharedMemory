using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RCI.SharedMemory.ACC;

namespace RCI.SharedMemory.Service.ACC;

public static class ServiceCollectionExtensions
{
  public static SharedMemoryServiceBuilder AddSharedMemoryAccService(this IServiceCollection services,
    IConfiguration configuration)
  {
    services.Configure<SharedMemoryAccConfig>(configuration.GetSection("SharedMemory").GetSection("Acc"));
    return services.AddSharedMemoryService<SharedMemoryAccService>(configuration);
  }
}