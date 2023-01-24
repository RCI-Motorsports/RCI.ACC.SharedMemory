// See https://aka.ms/new-console-template for more information

namespace Testing.Client;

public class Program
{
  static async Task Main(string[] args)
  {
    await Client.RunAsync(args);
    // Benchmarks.Benchmarks.Run();
  }
}