using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataForecaster.Approach;
using DataForecaster;

namespace Tests
{
    [TestClass]
    public class LinearRegressionTests
    {
        [TestMethod]
        public void Common()
        {
            var linearRegression = new LinearRegression();
            var olsCoefficients = linearRegression.ComputeOlsCoefficients(
                new Matrix<double>(new double[,] { }),
                new Vector<double>(new double[] { })
            );
        }
    }
}
