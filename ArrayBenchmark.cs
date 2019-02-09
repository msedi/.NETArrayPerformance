using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ArrayMathNETFramework
{
    public class ArrayBenchmark
    {
        const int N = 100_000_000;
        float[] A;
        float[] B;
        float[] R;

        Func<float, float, float> ExpressionLambda;

        private float Add(float a, float b) => a + b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float AddInlined(float a, float b) => a + b;

        [GlobalSetup]
        public void Setup()
        {
            A = new float[N];
            B = new float[N];
            R = new float[N];

            for (int i = 0; i < N; i++)
            {
                A[i] = i;
                B[i] = i;
                R[i] = i + i;
            }

            var par1 = Expression.Parameter(typeof(float));
            var par2 = Expression.Parameter(typeof(float));
            var add = Expression.Add(par1, par2);
            ExpressionLambda = (Func<float, float, float>)Expression.Lambda(add, par1, par2).Compile();
        }

        /// <summary>
        /// Method that has no optimizations.
        /// </summary>
        [Benchmark(Baseline =true, OperationsPerInvoke =100)]
        public void AddWithoutAnyOptimization()
        {
            float[] C = new float[A.Length];

            for (int i = 0; i < A.Length; i++)
                C[i] = A[i] + B[i];
        }

        /// <summary>
        /// Same as <see cref="AddWithoutAnyOptimization"/> but with the length of the 
        /// array from a global constant N.
        /// </summary>
        [Benchmark(OperationsPerInvoke =100)]
        public void AddWithNTakenFromGlobalConst()
        {
            float[] C = new float[N];

            for (int i = 0; i < N; i++)
                C[i] = A[i] + B[i];
        }

        /// <summary>
        /// Same as <see cref="AddWithNTakenFromGlobalConst"/> but with the array length
        /// taken from the array that shall be added.
        [Benchmark(OperationsPerInvoke = 100)]
        public void AddWithArrayLengthNotUsedInLoop()
        {
            int N = A.Length;
            float[] C = new float[N];

            for (int i = 0; i < N; i++)
                C[i] = A[i] + B[i];
        }

        /// <summary>
        /// Add with pointer arithmetics, the array length is taken again from A.Length.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithPointers()
        {
            float[] C = new float[A.Length];

            fixed (float* Apinned = A)
            fixed (float* Bpinned = B)
            fixed (float* Cpinned = C)
            {
                float* aptr = Apinned;
                float* bptr = Bpinned;
                float* cptr = Cpinned;

                for (int i = 0; i < A.Length; i++, aptr++, bptr++, cptr++)
                    *cptr = *aptr + *bptr;
            }
        }

        /// <summary>
        /// Same as <see cref="ArrayWithPointers"/> but now taken the N not in the loop.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithPointersArrayLengthNotUsedInLoop()
        {
            int N = A.Length;
            float[] C = new float[N];

            fixed (float* Apinned = A)
            fixed (float* Bpinned = B)
            fixed (float* Cpinned = C)
            {
                float* aptr = Apinned;
                float* bptr = Bpinned;
                float* cptr = Cpinned;

                for (int i = 0; i < N; i++, aptr++, bptr++, cptr++)
                    *cptr = *aptr + *bptr;
            }
        }

        /// <summary>
        /// Same as <see cref="ArrayWithPointers"/> but now taken the N not in the loop.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithExpressionWithPointers()
        {
            int N = A.Length;
            float[] C = new float[N];

            fixed (float* Apinned = A)
            fixed (float* Bpinned = B)
            fixed (float* Cpinned = C)
            {
                float* aptr = Apinned;
                float* bptr = Bpinned;
                float* cptr = Cpinned;

                for (int i = 0; i < N; i++, aptr++, bptr++, cptr++)
                    *cptr = ExpressionLambda(*aptr, *bptr);
            }
        }

        /// <summary>
        /// Same as <see cref="ArrayWithPointers"/> but now taken the N not in the loop.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithFunction()
        {
            int N = A.Length;
            float[] C = new float[N];

            fixed (float* Apinned = A)
            fixed (float* Bpinned = B)
            fixed (float* Cpinned = C)
            {
                float* aptr = Apinned;
                float* bptr = Bpinned;
                float* cptr = Cpinned;

                for (int i = 0; i < N; i++, aptr++, bptr++, cptr++)
                    *cptr = Add(*aptr, *bptr);
            }
        }

        /// <summary>
        /// Same as <see cref="ArrayWithPointers"/> but now taken the N not in the loop.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithFunctionInlined()
        {
            int N = A.Length;
            float[] C = new float[N];

            fixed (float* Apinned = A)
            fixed (float* Bpinned = B)
            fixed (float* Cpinned = C)
            {
                float* aptr = Apinned;
                float* bptr = Bpinned;
                float* cptr = Cpinned;

                for (int i = 0; i < N; i++, aptr++, bptr++, cptr++)
                    *cptr = AddInlined(*aptr, *bptr);
            }
        }

        /// <summary>
        /// Span was used.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithSpan()
        {
            int N = A.Length;
            float[] C = new float[N];

            Span<float> a = new Span<float>(A);
            Span<float> b = new Span<float>(B);
            Span<float> c = new Span<float>(C);

            for (int i = 0; i < N; i++)
                c[i] = a[i] + b[i];
        }

        /// <summary>
        /// Span was used.
        /// </summary>
        [Benchmark(OperationsPerInvoke = 100)]
        public unsafe void AddWithSIMD()
        {
            int N = A.Length;
            float[] C = new float[N];

            int SIMDLength = Vector<float>.Count;

            var i = 0;
            for (i = 0; i <= A.Length - SIMDLength; i += SIMDLength)
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

            int SIMDLength = Vector<float>.Count;

            var i = 0;
            int len = A.Length - SIMDLength;
            for (i = 0; i <= len; i += SIMDLength)
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
    }
}
