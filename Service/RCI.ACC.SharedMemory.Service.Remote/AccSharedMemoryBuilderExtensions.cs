using Microsoft.Extensions.DependencyInjection;
using RCI.ACC.SharedMemory.Service.Interfaces;
using RCI.ACC.SharedMemory.Service.Remote;

namespace RCI.ACC.SharedMemory.Service;

public static class AccSharedMemoryBuilderExtensions
{
  public static AccSharedMemoryServiceBuilder AddRemoteProvider(this AccSharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<IAccSharedMemoryProvider, RemoteAccSharedMemoryProvider>();
    return builder;
  }
}

