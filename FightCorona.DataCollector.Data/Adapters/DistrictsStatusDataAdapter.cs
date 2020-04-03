using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;

namespace FightCorona.DataCollector.Data.Adapters
{
    public class DistrictsStatusDataAdapter
    {
        private static string loggerName = "DistrictsStatusDataAdapter";

        public DateTime? GetLastInsertedDate()
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    return context.DistrictsStatus?.Max(x => x.Date);
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
                return null;
            }
        }

        public void AddOrUpdateRange(List<DistrictStatus> districtsStatus)
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    foreach (var ds in districtsStatus)
                    {
                        var existingStatus = context.DistrictsStatus.FirstOrDefault(x => x.Date == ds.Date && x.Location == ds.Location);
                        if (existingStatus == null)
                        {
                            context.DistrictsStatus.Add(ds);
                        }
                        else
                        {
                            existingStatus.Confirmed = ds.Confirmed;
                            existingStatus.Active = ds.Active;
                            existingStatus.Recovered = ds.Recovered;
                            existingStatus.Deaths = ds.Deaths;
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
                throw;
            }
        }
    }
}
