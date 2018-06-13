using System;

namespace DataForecaster
{
    //
    // https://en.wikipedia.org/wiki/QR_decomposition
    //
    internal static class QRDecomposition
    {
        // https://en.wikipedia.org/wiki/QR_decomposition
        // https://www.math.ucla.edu/~yanovsky/Teaching/Math151B/handouts/GramSchmidt.pdf
        // https://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process
        // http://www.seas.ucla.edu/~vandenbe/133A/lectures/qr.pdf
        // We can factor a complex m × n matrix A, with m ≥ n, as the product of an m × m unitary matrix Q 
        // and an m × n upper triangular matrix R. 
        // As the bottom(m − n) rows of an m × n upper triangular matrix consist entirely of zeroes, 
        // it is often useful to partition R, or both R and Q
        public static Tuple<Matrix<double>, Matrix<double>> GramSchmidtProcess(Matrix<double> matrix)
        {
            var a0 = matrix.GetColumnVector(0);
            var u0 = a0;
            var e0 = u0 / u0.Norm();

            var n = matrix.ColsNumber;
            var m = matrix.RowsNumber;

            var ee = new Vector<double>[n];
            ee[0] = e0;
            
            // m x n
            var q = new Matrix<double>(m, n);
            q.InsertColumn(e0, 0);

            // n x n
            var r = new Matrix<double>(n, n);

            for (int j = 1; j < n; j++)
            {
                var a = matrix.GetColumnVector(j);
                var u = a.Clone() as Vector<double>;

                for (int i = 0; i < j; i++)
                {
                    double dot = a * ee[i];
                    u = u - ee[i] * dot;
                }

                var e = u / u.Norm();

                ee[j] = e;
                q.InsertColumn(e, j);
            }

            return Tuple.Create(q, r);
        }
    }
}

