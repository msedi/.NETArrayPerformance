using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ArrayMathNETFramework
{
    public class AddSIMD
    {
        const int N = 100_000_000;
        float[] A;
        float[] B;

        [GlobalSetup]
        public void Setup()
        {
            A = new float[N];
            B = new float[N];

            for (int i = 0; i < N; i++)
            {
                A[i] = i;
                B[i] = i;
            }
        }

        /// <summary>
        /// Span was used.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithSIMD()
        {
            int N = A.Length;
            float[] C = new float[N];

            var simdLength = Vector<int>.Count;
            int i;
            for (i = 0; i <= A.Length - simdLength; i += simdLength)
            {
                var va = new Vector<float>(A, i);
                var vb = new Vector<float>(B, i);
                (va + vb).CopyTo(C, i);
            }

            for (; i < A.Length; ++i)
            {
                C[i] = A[i] + B[i];
            }
        }

        /// <summary>
        /// Span was used.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithSIMDOptimized()
        {
            int N = A.Length;
            float[] C = new float[N];

            var simdLength = Vector<int>.Count;
            int len = A.Length - simdLength;
            int i;
            for (i = 0; i <= len; i += simdLength)
            {
                var va = new Vector<float>(A, i);
                var vb = new Vector<float>(B, i);
                (va + vb).CopyTo(C, i);
            }

            for (; i < A.Length; ++i)
            {
                C[i] = A[i] + B[i];
            }
        }

        // Add scalar to each element of dst
        //private static void Add(float scalar, Span<float> dst)
        //{
        //    Sha256
        //    if (Avx.IsSupported)
        //    {
        //        AvxIntrinsics.AddScalarU(scalar, dst);
        //    }
        //    else if (Sse.IsSupported)
        //    {
        //        SseIntrinsics.AddScalarU(scalar, dst);
        //    }
        //    else
        //    {
        //        for (int i = 0; i < dst.Length; i++)
        //        {
        //            dst[i] += scalar;
        //        }
        //    }
        //}
    }
}
