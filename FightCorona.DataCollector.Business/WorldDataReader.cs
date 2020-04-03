using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FightCorona.DataCollector.Business.Models;
using FightCorona.DataCollector.Data.Adapters;
using FightCorona.DataCollector.Data.Models;
using Newtonsoft.Json;

namespace FightCorona.DataCollector.Business
{
    public class WorldDataReader
    {
        public async Task UpdateCountriesCurrentData()
        {
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
        }

        public async Task<CountriesCurrentData> GetCurrentData()
        {
            var client = new HttpClient();
            var response = await client.GetAsync("https://covid2019-api.herokuapp.com/v2/current");
            string data = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CountriesCurrentData>(data);
        }


        //public List<string> GetTimeSeriesData()
        //{
        //    List<string> splitted = new List<string>();
        //    string fileList = ReadWebData("https://raw.githubusercontent.com/CSSEGISandData/COVID-19/master/csse_covid_19_data/csse_covid_19_time_series/time_series_covid19_confirmed_global.csv");
        //    string[] tempStr;

        //    tempStr = fileList.Split(',');

        //    foreach (string item in tempStr)
        //    {
        //        if (!string.IsNullOrWhiteSpace(item))
        //        {
        //            splitted.Add(item);
        //        }
        //    }
        //    return splitted;
        //}

        //public string ReadWebData(string url)
        //{
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
        //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        //    StreamReader sr = new StreamReader(resp.GetResponseStream());
        //    string results = sr.ReadToEnd();
        //    sr.Close();

        //    return results;
        //}
    }
}
