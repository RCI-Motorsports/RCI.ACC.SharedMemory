using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RCI.SharedMemory.ACC;
using RCI.SharedMemory.ACC.Structs;
using RCI.SharedMemory.Core.Files;
using RCI.SharedMemory.Proxy.Client;
using RCI.SharedMemory.Proxy.Client.FileProvider;

namespace Testing.ProxyHost;

public static class ProxyHost
{
  public static async Task RunAsync(string[] args)
  {
    var host = Host.CreateDefaultBuilder(args)
      .ConfigureHostConfiguration(config => { config.AddCommandLine(args); })
      .ConfigureLogging((context, logging) =>
      {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(args.Contains("--debug") || context.HostingEnvironment.IsDevelopment()
          ? LogLevel.Debug
          : LogLevel.Information);
      })
      .ConfigureServices((context, services) =>
      {
        services.AddSharedMemoryHostWorker(context.Configuration);
        
        if (context.Configuration.GetSection("SharedMemory").GetSection("ProxyHost").GetValue<bool>("UseCachedFiles"))
        {
          services.RemoveAll<ISharedMemoryProxyHostFileProvider>();
          services.AddSingleton<ISharedMemoryProxyHostFileProvider, SharedMemoryTestingProxyHostFileProvider>();
        }
      })
      .Build();

    var config = host.Services.GetService<IOptions<SharedMemoryProxyHostConfig>>();
    if (config?.Value.UseSharedFiles == true)
    {
      CreateTestFiles();
    }

    await host.RunAsync();
  }
  
  private static void CreateTestFiles()
  {
    CreateTestFile<StaticInfo>(SharedMemoryAccFileNames.StaticInfo);
    CreateTestFile<Physics>(SharedMemoryAccFileNames.Physics);
    CreateTestFile<Graphics>(SharedMemoryAccFileNames.Graphics);
  }

  private static void CreateTestFile<T>(string fileName) where T : struct
  {
    var file = new SharedMemoryLocalFile<T>(fileName);
    if (!file.TryConnect())
      return;

    var data = file.ReadRaw();
    var testFile =
      new SharedMemoryTestingFile<StaticInfo>(Path.Combine(Directory.GetCurrentDirectory(), "TestMappedFiles"),
        fileName);
    File.WriteAllBytes(testFile.MemoryFilePath, data);
    Console.WriteLine($"{fileName} has been written to \"{testFile.MemoryFilePath}\"");
  }
}