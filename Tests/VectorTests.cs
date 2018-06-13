using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataForecaster;

namespace Tests
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void Common()
        {
            var vector = new Vector<double>(new double[] {0, 1, 2, 3});
            var norm = vector.Norm();
            Assert.IsTrue((int)norm == 3);
        }
    }
}
