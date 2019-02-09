using ArrayMathNETFramework;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathNETFramework472
{
    class Program
    {
        static void Main(string[] args)
        {

            //ArrayBenchmark x = new ArrayBenchmark();
            //x.Setup();
            //x.AddWithSpan();

            BenchmarkRunner.Run<AddSIMD>();
            Console.ReadKey();
        }
    }
}
