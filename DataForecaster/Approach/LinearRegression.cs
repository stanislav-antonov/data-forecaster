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
        // x - design matrix (independent input parameters aka predictor variables)
        // y - vector of observations according to input parameters
        public Vector<double> ComputeBetas(Matrix<double> x, Vector<double> y)
        {
            // should I add a column of ones at the first position to compute beta0?

            var qr = x.GramSchmidtProcess();

            var q = qr.Item1;
            var r = qr.Item2;

            var qt = q.Transpose();
            var qb = qt * y;
            var ri = r.Inverse();
            var betas = ri * qb;

            return betas;
        }

        public Vector<double> Fit(Matrix<double> x, Vector<double> betas)
        {
            int m = x.RowsNumber;
            int n = x.ColsNumber;
            var yy = new Vector<double>(m);

            for (var i = 0; i < m; i++)
            {
                double y = 0; 
                for (var j = 0; j < n; j++)
                {
                    y += betas[j] * x[i, j];
                }

                yy[i] = y;
            }

            return yy;
        }

        // http://reliawiki.org/index.php/Multiple_Linear_Regression_Analysis
        // https://math.stackexchange.com/questions/80848/calculate-p-value
        // http://www.statsoft.com/Textbook/Distribution-Tables
        // http://users.stat.ufl.edu/~athienit/Tables/tables
        public Matrix<double> SignificanceTest(Matrix<double> x, Vector<double> y, Vector<double> betas)
        {
            var X = x.Clone() as Matrix<double>;
            var Xt = X.Transpose();
            var XtXi = (Xt * X).Inverse();

            // the hat matrix
            var H = X * XtXi * Xt;
            
            // total number of observations
            var n = X.RowsNumber;

            // number of predictor variables (x)
            // - 1 is needed because a first column of X matrix is for b0 and should not be counted
            var k = X.ColsNumber - 1;

            // matrix of ones
            // var J = new Matrix<double>(n, n);
            // J.Fill(1);

            // regression of sum squares
            // var SSr = y * ((H - (J * 1 / n)) * y);

            // identity matrix
            var I = new Matrix<double>(n, n);
            I.FillAsIdentity();

            // error sum of squares
            var SSe = y * ((I - H) * y);

            // degrees of freedom
            var df = (n - (k + 1));

            // error mean square 
            var MSe = SSe / df;

            // the variance-covariance matrix of the estimated regression 
            var C = XtXi * MSe;

            // boundaries of acceptance region
            // -t < tBeta < t
            var t = StudentTDistribution.Value(df, StudentTDistribution.Alpha._005);

            // Compute test statistics for betas
            // Start from [1, 1] element in C matrix skipping [0, 0] element since it contains the value for b0
            var tBetas = new Vector<double>(C.ColsNumber - 1);
            for (var i = 1; i < C.ColsNumber; i++)
            {
                tBetas[i - 1] = betas[i] / Math.Sqrt(C[i, i]);
            }

            // Null hypothesis H0 for each beta is: b = 0
            // Alternative hyposethis H1 for each beta is: b != 0

            // If the sample statistic lies within the acceptance region, 
            // the null hypothesis (a hypothesis usually based on conventional theory) is provisionally accepted. 
            // If the sample statistic does not lies in the acceptance region,
            // the alternative hypothesis that contradicts the null hypothesis is accepted.

            return H;
        }
    }
}
