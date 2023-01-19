using Microsoft.Extensions.DependencyInjection;
using RCI.ACC.SharedMemory.Service.Interfaces;
using RCI.ACC.SharedMemory.Service.Local;

namespace RCI.ACC.SharedMemory.Service;

public static class AccSharedMemoryBuilderExtensions
{
  public static AccSharedMemoryServiceBuilder AddLocalProvider(this AccSharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<IAccSharedMemoryProvider, LocalAccSharedMemoryProvider>();
    return builder;
  }
}

