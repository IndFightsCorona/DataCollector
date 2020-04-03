using System;
using System.IO;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Timers;
using FightCorona.DataCollector.Business.Readers;

namespace FightCorona.DataCollector.Service
{
    public partial class DataCollectorService : ServiceBase
    {
        object readLock = new object();
        Timer timer = new Timer();
        public DataCollectorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteLog("Data Collector Service Started");
            timer.Elapsed += new ElapsedEventHandler(DataCollectorEvent);
            int timerFromConfig = int.Parse(System.Configuration.ConfigurationManager.AppSettings["timerInMinutes"]);
            timer.Interval = 1000 * 60  * timerFromConfig; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            WriteLog("Data Collector Service Stopped");
        }

        private void DataCollectorEvent(object source, ElapsedEventArgs e)
        {
            WriteLog(String.Format("Data Collector Service - {0} started", e.SignalTime));
            Read(e.SignalTime);
            WriteLog(String.Format("Data Collector Service - {0} completed", e.SignalTime));
        }

        private void Read(DateTime signalTime)
        {
            lock(readLock)
            {
                try
                {
                    Parallel.Invoke(() => new MohfwDataReader().Read(), () => new StateDataReader().Read());
                }
                catch (Exception ex)
                {
                    WriteLog(String.Format("Data Collector Service - {0} did not run successfully; Error details: {1}", signalTime, ex.Message));
                }
            }
        }

        private static void WriteLog(string Message)
        {
            StreamWriter sw = null;

            sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\\DataCollectorServiceLog"+ DateTime.Now.ToString("yyyyMMddHH").ToString() +".txt", true);
            sw.WriteLine(DateTime.Now.ToString() + " :" + Message);
            sw.Flush();
            sw.Close();
            
        }
    }
}
