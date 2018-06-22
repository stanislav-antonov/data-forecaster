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
            var x = new Matrix<double>(new double[,] {
                { 1, 41.9, 29.1 },
                { 1, 43.4, 29.3 },
                { 1, 43.9, 29.5 },
                { 1, 44.5, 29.7 },
                { 1, 47.3, 29.9 },
                { 1, 47.5, 30.3 },
                { 1, 47.9, 30.5 },
                { 1, 50.2, 30.7 },
                { 1, 52.8, 30.8 },
                { 1, 53.4, 30.9 },
                { 1, 56.7, 31.5 },
                { 1, 57.0, 31.7 },
                { 1, 57.0, 31.9 },
                { 1, 57.0, 32.0 },
                { 1, 57.0, 32.1 },
                { 1, 57.0, 32.5 },
                { 1, 57.0, 32.9 },
            });

            var y = new Vector<double>(new double[] {
                251.3,
                251.3,
                248.3,
                267.5,
                273.0,
                276.5,
                270.3,
                274.9,
                285.0,
                290.0,
                297.0,
                302.5,
                304.5,
                309.3,
                321.7,
                330.7,
                349.0,
            });

            var linearRegression = new LinearRegression();
            var betas = linearRegression.ComputeBetas(x, y);

            var yy = linearRegression.Fit(x, betas);
        }
    }
}
