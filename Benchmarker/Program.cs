// See https://aka.ms/new-console-template for more information
using Audio;
using BenchmarkDotNet.Running;

namespace Benchmarker;

public class Program
{
  public static void Main(string[] args)
  {
    var summary = BenchmarkRunner.Run<EncoderBenchmark>();
    Console.WriteLine(summary.ToString());
  }
}