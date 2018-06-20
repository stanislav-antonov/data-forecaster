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
            var betas = linearRegression.ComputeBetas(
                new Matrix<double>(new double[,] { 
                    { 12, -51, 4 },
                    { 6, 167, -68 },
                    { -4, 24, -41 },
                    { 15, 90, -4 },
                    { -44, 11, 13 }
                }),
                new Vector<double>(new double[] { 18, 4, 30, -19, -4 })
            );
        }
    }
}
