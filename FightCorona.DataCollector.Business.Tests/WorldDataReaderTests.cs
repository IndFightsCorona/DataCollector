using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FightCorona.DataCollector.Business.Tests
{
    [TestClass]
    public class WorldDataReaderTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await WorldDataReader.UpdateCountriesCurrentData();
        }
    }
}
