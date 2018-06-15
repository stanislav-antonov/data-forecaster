using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataForecaster;

namespace Tests
{
    [TestClass]
    public class QRDecompositionTests
    {
        [TestMethod]
        public void Common()
        {
            var matrix = new Matrix<double>(new double[,] {
                { 12, -51, 4 },
                { 6, 167, -68 },
                { -4, 24, -41 },
               //  { 15, 90, -4 },
               // { -44, 11, 13 }
            });

            var result = QRDecomposition.GramSchmidtProcess(matrix);

        }
    }
}
