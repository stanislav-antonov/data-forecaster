using System;

namespace DataForecaster
{
    //
    // https://en.wikipedia.org/wiki/QR_decomposition
    //
    internal static class QRDecomposition
    {
        // https://www.math.ucla.edu/~yanovsky/Teaching/Math151B/handouts/GramSchmidt.pdf
        // https://en.wikipedia.org/wiki/Gram%E2%80%93Schmidt_process
        public static Tuple<Matrix<double>, Matrix<double>> GramSchmidtProcess(Matrix<double> matrix)
        {
            var a0 = matrix.GetColumnVector(0);
            var u0 = a0;
            var e0 = u0 / u0.Norm();

            var ee = new Vector<double>[matrix.ColsNumber];
            ee[0] = e0;

            for (int j = 1; j < matrix.ColsNumber; j++)
            {
                var a = matrix.GetColumnVector(j);
                var u = a.Clone() as Vector<double>;

                for (int i = 0; i < j; i++)
                {
                    double dot = a * ee[i];
                    u = u - ee[i] * dot;
                }

                var e = u / u.Norm();
            }

            return null;
        }
    }
}

