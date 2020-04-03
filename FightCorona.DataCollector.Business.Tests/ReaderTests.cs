using System.Threading.Tasks;
using FightCorona.DataCollector.Business.Readers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FightCorona.DataCollector.Business.Tests
{
    [TestClass]
    public class ReaderTests
    {
        [TestMethod]
        public void MohfwDataReaderTest()
        {
           new MohfwDataReader().Read();
        }

        [TestMethod]
        public async Task WorldDataReaderTest()
        {
            await WorldDataReader.UpdateCountriesCurrentData();
        }

        [TestMethod]
        public void StateDataReaderTest()
        {
             new StateDataReader().Read();
        }
    }
}
