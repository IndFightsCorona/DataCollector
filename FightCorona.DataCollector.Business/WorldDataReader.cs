using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FightCorona.DataCollector.Business.Models;
using FightCorona.DataCollector.Data.Adapters;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;
using Newtonsoft.Json;

namespace FightCorona.DataCollector.Business
{
    public static class WorldDataReader
    {
        private static string loggerName = "WorldDataRetriever";

        public static async Task UpdateCountriesCurrentData()
        {
            try
            {
                Log.WriteEntityLog(loggerName, "UpdateCountriesCurrentData started");
                var currentData = await GetCurrentData();
                var lastUpdateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(currentData.ts);
                var dataToAdd = currentData.data.Select(
                                        x => new CountryStatus
                                        {
                                            Active = x.active,
                                            Confirmed = x.confirmed,
                                            Deaths = x.deaths,
                                            LastUpdated = lastUpdateTime,
                                            Location = x.location,
                                            Recovered = x.recovered
                                        }).ToList();
                new CountriesDataAdapter().Add(dataToAdd);
                Log.WriteEntityLog(loggerName, "UpdateCountriesCurrentData completed");
            }
            catch(Exception ex)
            {
                Log.WriteEntityLog(loggerName, String.Format("UpdateCountriesCurrentData stopped with error, error details: {0}", ex.Message), LogType.Error);
            }
        }

        public static async Task<CountriesCurrentData> GetCurrentData()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://covid2019-api.herokuapp.com/v2/current");
            string data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CountriesCurrentData>(data);
        }
    }
}
