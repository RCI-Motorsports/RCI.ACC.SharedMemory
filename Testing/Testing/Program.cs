// See https://aka.ms/new-console-template for more information

namespace Testing;

public class Program
{
  static async Task Main(string[] args)
  {
    await ProxyHost.ProxyHost.RunAsync(args);
    // Benchmarks.Benchmarks.Run();
  }
}