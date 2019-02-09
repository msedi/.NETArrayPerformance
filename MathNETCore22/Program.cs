using ArrayMathNETFramework;
using BenchmarkDotNet.Running;
using System;

namespace MathNETCore21
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ArrayBenchmark>();
            Console.ReadKey();
        }
    }
}
