using System;
using System.Data.Entity;
using System.IO;
using System.Reflection;
using FightCorona.DataCollector.Logger;

namespace FightCorona.DataCollector.Data
{
    public class StatisticsDbInitializer : CreateDatabaseIfNotExists<StatisticsContext>
    {
        private static string loggerName = "StatisticsDbInitializer";

        protected override void Seed(StatisticsContext context)
        {
            try
            {
                Log.WriteEntityLog(loggerName, "Data Seeding Started");
                base.Seed(context);
                
                Assembly asy = Assembly.GetExecutingAssembly();
                Stream stm = asy.GetManifestResourceStream("FightCorona.DataCollector.Data.SeedQuery.sql");
                if (stm != null)
                {
                    string sql = new StreamReader(stm).ReadToEnd();
                    // now you have the SQL statements which you can execute 
                    context.Database.ExecuteSqlCommand(sql);
                }

                Log.WriteEntityLog(loggerName, "Data Seeding Completed");
            }
            catch(Exception ex)
            {
                Log.WriteEntityLog(loggerName, ex.Message, LogType.Error);
            }
        }
    }
}
