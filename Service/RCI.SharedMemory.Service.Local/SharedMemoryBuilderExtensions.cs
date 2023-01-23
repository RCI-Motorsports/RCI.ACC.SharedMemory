using RCI.SharedMemory.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace RCI.SharedMemory.Service.Local;

public static class SharedMemoryBuilderExtensions
{
  public static SharedMemoryServiceBuilder AddLocalProvider(this SharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<ISharedMemoryProvider, LocalSharedMemoryProvider>();
    return builder;
  }
}

