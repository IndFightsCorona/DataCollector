using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WebScrapingService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteErrorLog("Web Scraping Service Started");
            timer.Elapsed += new ElapsedEventHandler(WebScrapingMethod);
            int timerFromConfig = int.Parse(System.Configuration.ConfigurationManager.AppSettings["timerInMinutes"]);
            timer.Interval = 1000 * 60  * timerFromConfig; //number in milisecinds  
            timer.Enabled = true;
        }

        protected override void OnStop()
        {
            timer.Enabled = false;
            WriteErrorLog("Web Scraping Service Stopped");
        }

        private void WebScrapingMethod(object source, ElapsedEventArgs e)
        {
            StatisticsInformation.GetStatisticsInformation();
            WriteErrorLog("Web Scraping Service completed successfully");
        }

        private static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;

            sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + @"\\WebScrapingServiceLog"+ DateTime.Now.ToString("yyyyMMddHH").ToString() +".txt", true);
            sw.WriteLine(DateTime.Now.ToString() + " :" + Message);
            sw.Flush();
            sw.Close();
            
        }
    }
}
