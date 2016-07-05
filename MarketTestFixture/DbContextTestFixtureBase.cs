using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Market.TestFixture
{
    public class DbContextTestFixtureBase
    {
        private TransactionScope _trans;

        [TestInitialize()]
        public void Init()
        {
            _trans = new TransactionScope();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            _trans.Dispose();
        }

    }
}