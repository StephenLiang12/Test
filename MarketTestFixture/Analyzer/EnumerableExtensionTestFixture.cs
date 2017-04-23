using Market.Analyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture.Analyzer
{
    [TestClass]
    public class EnumerableExtensionTestFixture
    {
        [TestMethod]
        public void AbleToGetMedian()
        {
            double[] numbers1 = { 4, 4, 4, 4, 3, 2, 2, 2, 1 }; 
            Assert.AreEqual(3, numbers1.GetMedian());
            double[] numbers2 = { 4, 4, 4, 4, 3, 2, 2, 2, 2, 1 }; 
            Assert.AreEqual(2.5, numbers2.GetMedian());
        }
    }
}