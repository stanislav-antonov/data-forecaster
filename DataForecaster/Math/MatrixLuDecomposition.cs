using System;

namespace DataForecaster
{
    public static class MatrixLuDecomposition
    {
        public static int CroutProcess(this Matrix<double> matrix, out Matrix<double> lum, out int[] perm)
        {
            // Crout's LU decomposition for matrix determinant and inverse
            // stores combined lower & upper in lum
            // stores row permuations into perm[]
            // returns +1 or -1 according to even or odd number of row permutations
            // lower gets dummy 1.0s on diagonal (0.0s above)
            // upper gets lum values on diagonal (0.0s below)

            int toggle = +1; // even (+1) or odd (-1) row permutatuions
            int n = matrix.ColsNumber;

            // make a copy of m into result lum
            lum = matrix.Clone() as Matrix<double>;

            // make perm[]
            perm = new int[n];
            for (int i = 0; i < n; ++i)
            {
                perm[i] = i;
            }
                
            for (int j = 0; j < n - 1; ++j) // process by column. note n-1 
            {
                double max = Math.Abs(lum[j, j]);
                int piv = j;

                for (int i = j + 1; i < n; ++i) // find pivot index
                {
                    double xij = Math.Abs(lum[i, j]);
                    if (xij > max)
                    {
                        max = xij;
                        piv = i;
                    }
                }

                if (piv != j)
                {
                    // swap rows j, piv
                    for (int r = 0; r < n; ++r)
                    {
                        var temp = lum[piv, r];
                        lum[piv, r] = lum[j, r];
                        lum[j, r] = temp;
                    }

                    // swap perm elements
                    int t = perm[piv]; 
                    perm[piv] = perm[j];
                    perm[j] = t;

                    toggle = -toggle;
                }

                double xjj = lum[j, j];
                if (xjj != 0.0)
                {
                    for (int i = j + 1; i < n; ++i)
                    {
                        double xij = lum[i, j] / xjj;
                        lum[i, j] = xij;

                        for (int k = j + 1; k < n; ++k)
                        {
                            lum[i, k] -= xij * lum[j, k];
                        }
                    }
                }
            }

            return toggle;
        }

        public static double[] CourtProcessHelper(this Matrix<double> lum, double[] b)
        {
            int n = lum.ColsNumber;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                {
                    sum -= lum[i, j] * x[j];
                }

                x[i] = sum;
            }

            x[n - 1] /= lum[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                {
                    sum -= lum[i, j] * x[j];
                }

                x[i] = sum / lum[i, i];
            }

            return x;
        }
    }
}
