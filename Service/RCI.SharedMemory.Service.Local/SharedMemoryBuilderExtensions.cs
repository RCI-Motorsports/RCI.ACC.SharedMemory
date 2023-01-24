using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.Local;

[SupportedOSPlatform("windows")]
public static class SharedMemoryBuilderExtensions
{
  public static SharedMemoryServiceBuilder AddLocalProvider(this SharedMemoryServiceBuilder builder)
  {
    builder.Services.AddSingleton<ISharedMemoryProvider, SharedMemoryLocalProvider>();
    return builder;
  }
}