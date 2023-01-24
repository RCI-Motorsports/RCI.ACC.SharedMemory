using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

namespace Testing.Benchmarks;

public class Benchmarks
{
  public static void Run()
  {
    BenchmarkRunner.Run<BinaryRead>();
    // var x = new BinaryRead();
    // x.Raw();
  }
}