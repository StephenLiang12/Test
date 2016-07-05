using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture
{
    [TestClass]
    public class ListExtensionTestFixture
    {
        [TestMethod]
        public void AbleToGetFrontPartialList()
        {
            List<int> list = new List<int>(10);
            for (int i = 1; i <= 10; i++)
            {
                list.Add(i);
            }
            var partial = list.GetFrontPartial(3);
            Assert.AreEqual(3, partial.Count);
            Assert.AreEqual(1, partial[0]);
            Assert.AreEqual(2, partial[1]);
            Assert.AreEqual(3, partial[2]);
        }

        [TestMethod]
        public void AbleToGetRearPartialList()
        {
            List<int> list = new List<int>(10);
            for (int i = 1; i <= 10; i++)
            {
                list.Add(i);
            }
            var partial = list.GetRearPartial(3);
            Assert.AreEqual(3, partial.Count);
            Assert.AreEqual(8, partial[0]);
            Assert.AreEqual(9, partial[1]);
            Assert.AreEqual(10, partial[2]);
        }
    }
}