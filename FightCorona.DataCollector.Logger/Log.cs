using System;
using System.IO;

namespace FightCorona.DataCollector.Logger
{
    public static class Log
    {
        public static void WriteEntityLog(string loggerName, string message, LogType logType = LogType.Info)
        {
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\\WebScrapingEntityExceptionLog" + DateTime.Now.ToString("yyyyMMddHH").ToString() + ".txt", true);
            sw.WriteLine( loggerName + "-" + logType.ToString() + ":"+ DateTime.Now.ToString() + " :" + message);
            sw.Flush();
            sw.Close();
        }
    }
}
