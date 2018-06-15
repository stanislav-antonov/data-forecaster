using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataForecaster.Approach
{
    // https://en.wikipedia.org/wiki/Linear_regression
    // https://en.wikipedia.org/wiki/Linear_least_squares_(mathematics)
    public class LinearRegression
    {
        // https://rstudio-pubs-static.s3.amazonaws.com/251311_c8970d1f1a8541aaa5884d86b1487ea6.html
        public Vector<double> ComputeOlsCoefficients(Matrix<double> x, Vector<double> y)
        {
            var qr = x.GramSchmidtProcess();

            var q = qr.Item1;
            var r = qr.Item2;

            q.Transpose();
            var qb = q * y;

            return null;
        }
    }
}
