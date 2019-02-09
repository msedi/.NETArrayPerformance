using ArrayMathNETFramework;
using BenchmarkDotNet.Running;
using System;

namespace MathNETCore21
{
    class Program
    {
        static void Main(string[] args)
        {
            AddSIMD add = new AddSIMD();
            add.Setup();
            add.Add();
            //BenchmarkRunner.Run<ArrayBenchmark>();
            //Console.ReadKey();
        }
    }
}
