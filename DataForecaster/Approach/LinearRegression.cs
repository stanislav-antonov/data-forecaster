﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DataForecaster.Approach
{
    // https://en.wikipedia.org/wiki/Linear_regression
    // https://en.wikipedia.org/wiki/Linear_least_squares_(mathematics)
    public class LinearRegression
    {
        public struct SignificanceResult
        {
            public int Index { get; private set; }
            public double Beta { get; private set; }
            public double TBeta { get; private set; }
            public double PValue { get; private set; }
            public bool IsSignificant { get; private set; }

            public SignificanceResult(int index, double beta, double tBeta, double pValue, bool isSignificant)
            {
                Index = index;
                Beta = beta;
                TBeta = tBeta;
                PValue = pValue;
                IsSignificant = isSignificant;
            }
        }

        private Vector<double> Coefficients { get; set; }
        private Vector<double> ResponseVariables { get; set; }
        private Matrix<double> PredictorVariables { get; set; }
        private ISet<int> UnsignificantPredictorIndexes { get; set; }


        // https://rstudio-pubs-static.s3.amazonaws.com/251311_c8970d1f1a8541aaa5884d86b1487ea6.html
        // x - design matrix (independent input parameters aka predictor variables)
        // y - vector of observations according to input parameters
        public void Fit(Matrix<double> x, Vector<double> y)
        {
            // should I add a column of ones at the first position to compute beta0?

            var qr = x.GramSchmidtProcess();

            var q = qr.Item1;
            var r = qr.Item2;

            var qt = q.Transpose();
            var qb = qt * y;
            var ri = r.Inverse();

            Coefficients = ri * qb;
            PredictorVariables = x;
            ResponseVariables = y;
        }

        public Vector<double> Predict(Matrix<double> x)
        {
            int m = x.RowsNumber;
            int n = x.ColsNumber;
            var yy = new Vector<double>(m);

            for (var i = 0; i < m; i++)
            {
                double y = 0;
                for (var j = 0; j < n; j++)
                {
                    y += Coefficients[j] * x[i, j];
                }

                yy[i] = y;
            }

            return yy;
        }

        // Step-wise building process
        //
        public void BuildModel(Matrix<double> x, Vector<double> y)
        {
            bool finalModelFound = false;
            var predictors = x.Clone() as Matrix<double>;
            var coefficients = Coefficients.Clone() as Vector<double>;

            // newIndex -> originalIndex
            var indexMap = new Dictionary<int, int>();
            for (int i = 0; i < predictors.ColsNumber; i++)
            {
                indexMap.Add(i, i);
            }

            var indexMapKeys = indexMap.Keys.OrderBy(_ => _);

            while (!finalModelFound && predictors.ColsNumber > 0)
            {
                Fit(predictors, y);
                finalModelFound = true;

                var significance = SignificanceTest(predictors, y);

                foreach (var s in significance)
                {
                    if (s.IsSignificant)
                    {
                        // Nothing to do with significant predictor
                        continue;
                    }

                    var index = s.Index;

                    // remove the respective unsignificant predictor and coefficient
                    predictors.RemoveColumn(index);
                    coefficients.Remove(index);

                    ShiftIndexes(indexMap, indexMapKeys, index);

                    finalModelFound = false;
                }
            }
        }

        private void ShiftIndexes(IDictionary<int, int> indexMap, IOrderedEnumerable<int> indexMapKeys, int index)
        {
            foreach (var key in indexMapKeys.Where(_ => _ >= index).ToList())
            {
                if (indexMap.ContainsKey(key + 1))
                {
                    indexMap[key] = indexMap[key + 1];
                }
            }

            var maxKey = indexMapKeys.Last();
            indexMap.Remove(maxKey);
        }

        // http://reliawiki.org/index.php/Multiple_Linear_Regression_Analysis
        // https://math.stackexchange.com/questions/80848/calculate-p-value
        // http://www.statsoft.com/Textbook/Distribution-Tables
        // http://users.stat.ufl.edu/~athienit/Tables/tables
        public List<SignificanceResult> SignificanceTest(Matrix<double> x, Vector<double> y)
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
            var t = StudentTDistribution.Value(df, StudentTDistribution.Alpha._05);
            var alpha = StudentTDistribution.AlphaValue(StudentTDistribution.Alpha._05);

            // Null hypothesis H0 for each beta is: b = 0
            // Alternative hyposethis H1 for each beta is: b != 0

            // If the sample statistic lies within the acceptance region, 
            // the null hypothesis (a hypothesis usually based on conventional theory) is provisionally accepted. 
            // If the sample statistic does not lies in the acceptance region,
            // the alternative hypothesis that contradicts the null hypothesis is accepted.

            var result = new List<SignificanceResult>();

            // Go throuth the main diagonal of C matrix.
            // Note that [0, 0] element contains the b0 value (means very first beat with no predictor)
            for (var i = 0; i < C.ColsNumber; i++)
            {
                double beta = Coefficients[i];

                // Compute test statistic for beta
                double tBeta = beta / Math.Sqrt(C[i, i]);

                // Get p-value
                double pValue = PValues.Value(df, tBeta);

                // test p-value for significance
                bool isSignificant = pValue < alpha;

                result.Add(new SignificanceResult(i, beta, tBeta, pValue, isSignificant));
            }

            return result;
        }
    }
}
