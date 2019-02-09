using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

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
        public void Add()
        {
            float[] C = new float[A.Length];

            if (Avx.IsSupported)
            {
                for (int i=0; i<A.Length; i++)
                {
                    var v1 = System.Runtime.Intrinsics.Vector256.Create(A[i]);
                    var v2 = System.Runtime.Intrinsics.Vector256.Create(B[i]);
                    C[i] = Avx2.Add(v1, v2).

                }
              //  Avx.Add(scalar, dst);
            }
            else if (Sse.IsSupported)
            {
               // SseIntrinsics.AddScalarU(scalar, dst);
            }
            else
            {
           //     for (int i = 0; i < dst.Length; i++)
                {
          //          dst[i] += scalar;
                }
            }
        }
    }
}
