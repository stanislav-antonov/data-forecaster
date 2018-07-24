using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataForecaster;

namespace Tests
{
    [TestClass]
    public class MatrixTests
    {
        [TestMethod]
        public void RemoveColumnTest()
        {
            var matrix = new Matrix<double>(new double[,] {
                { -1, -1,  1 },
                {  1,  3,  3 },
                { -1, -1,  5 },
                {  1,  3,  7 },
            });

            var colsNumber = matrix.ColsNumber;

            matrix.RemoveColumn(2);
            Assert.IsTrue(matrix.ColsNumber == colsNumber - 1);
        }
    }
}
