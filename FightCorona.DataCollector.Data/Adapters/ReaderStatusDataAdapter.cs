using System;
using System.Linq;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;

namespace FightCorona.DataCollector.Data.Adapters
{
    public class ReaderStatusDataAdapter
    {
        private static string loggerName = "ReaderStatusDataAdapter";

        public ReaderStatus Get(string readerName)
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    return context.ReaderStatus.FirstOrDefault(x => x.ReaderName == readerName);
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
                return null;
            }
        }

        public ReaderStatus Add(ReaderStatus readerStatus)
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    context.ReaderStatus.Add(readerStatus);
                    context.SaveChanges();
                    return readerStatus;
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
                return null;
            }
        }

        public void Update(string readerName, string version)
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    var result = context.ReaderStatus.FirstOrDefault(x => x.ReaderName == readerName);
                    result.Version = version;
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
            }
        }
    }
}
