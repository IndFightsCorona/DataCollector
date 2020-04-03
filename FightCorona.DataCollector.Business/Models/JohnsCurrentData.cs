using System.Collections.Generic;

namespace FightCorona.DataCollector.Business.Models
{
    public class CountriesCurrentData
    {
        public List<Datum> data { get; set; }
        public string dt { get; set; }
        public double ts { get; set; }
    }

    public class Datum
    {
        public string location { get; set; }
        public int confirmed { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }
        public int active { get; set; }
    }
}
