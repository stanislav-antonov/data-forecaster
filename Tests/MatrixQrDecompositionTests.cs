using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataForecaster;

namespace Tests
{
    [TestClass]
    public class MatrixQrDecompositionTests
    {
        [TestMethod]
        public void Common()
        {
            var matrix = new Matrix<double>(new double[,] {
                { -1, -1,  1 },
                {  1,  3,  3 },
                { -1, -1,  5 },
                {  1,  3,  7 },
            });

            var result = MatrixQrDecomposition.GramSchmidtProcess(matrix);
        }
    }
}
