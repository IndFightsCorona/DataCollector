using System;
using System.Collections.Generic;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;

namespace FightCorona.DataCollector.Data.Adapters
{
    public class CountriesDataAdapter
    {
        public void Add(List<CountryStatus> countriesStatus)
        {
            using (var context = new StatisticsContext())
            {
                try
                {
                    context.CountriesStatus.AddRange(countriesStatus);
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    Log.WriteEntityLog("CountriesDataAdapter", ex.Message, LogType.Error);
                }
            }
        }
    }
}
