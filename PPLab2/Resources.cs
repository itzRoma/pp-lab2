using System;

namespace PPLab2
{
    /// <summary>
    /// W = max(C * MD) * E * (MA * MB) * d
    /// </summary>
    public class Resources
    {
        public int N { get; } // size of vectors and matrices
        public int P { get; } // amount of processors (threads)
        public int H { get; } // size of subvectors and submatrices

        public int[] VectorW { get; }
        public int[,] MatrixMD { get; }
        public int[,] MatrixMB { get; }

        // shared resources
        public int[] VectorC { get; }
        public int ScalarM { get; set; }
        public int[,] MatrixMA { get; }
        public int[] VectorE { get; }
        public int ScalarD { get; set; }

        // intermediate resources
        public int[] VectorR { get; }
        public int[,] MatrixMR { get; }
        public int[] VectorX { get; }

        public Resources(int n, int p)
        {
            if (n <= 0) throw new ArgumentException("Invalid size of vectors and matrices");
            if (p <= 0) throw new ArgumentException("Invalid amount of processors (threads)");

            if (n % p != 0) throw new InvalidOperationException("N should be exactly divisible by P");

            N = n;
            P = p;
            H = n / p;

            VectorW = new int[N];
            MatrixMD = new int[N, N];
            MatrixMB = new int[N, N];

            VectorC = new int[N];
            ScalarM = int.MinValue;
            MatrixMA = new int[N, N];
            VectorE = new int[N];

            VectorR = new int[N];
            MatrixMR = new int[N, N];
            VectorX = new int[N];
        }
    }
}