using RCI.SharedMemory.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace RCI.SharedMemory.Service.Remote;

public static class SharedMemoryBuilderExtensions
{
  public static SharedMemoryServiceBuilder AddRemoteProvider(this SharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<ISharedMemoryProvider, RemoteSharedMemoryProvider>();
    return builder;
  }
}

