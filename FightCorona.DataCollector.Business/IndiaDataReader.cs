using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using FightCorona.DataCollector.Data;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using LogType = FightCorona.DataCollector.Logger.LogType;

namespace FightCorona.DataCollector.Business
{
    public class IndiaDataReader

    {
        private static string loggerName = "IndiaDataRetriever";

        public static void UpdateData()
        {
            try
            {
                using (var context = new StatisticsContext())
                {
                    try
                    {

                        var lastUpdateInDatabse = context.LastUpdate.Find(1);
                        Log.WriteEntityLog(loggerName, "lastUpdateInDatabse passed.");

                        var maxVersionOfOverallStatistics = context.OverallStatistics.Any() ? context.OverallStatistics.Max(s => s.Version) : 0;
                        Log.WriteEntityLog(loggerName, "maxVersionOfOverallStatistics passed. " + maxVersionOfOverallStatistics);

                        var overallStatistics = context.OverallStatistics.Where(x => x.Version == maxVersionOfOverallStatistics).FirstOrDefault<OverallStatistics>();
                        Log.WriteEntityLog(loggerName, "maxVersionOfOverallStatistics passed. " + maxVersionOfOverallStatistics);

                        bool isLatestData = false;

                        var maxVersionOfStatistics = context.Statistics.Any() ? context.Statistics.Max(s => s.Version) : 0;
                        Log.WriteEntityLog(loggerName, "maxVersionOfStatistics passed. " + maxVersionOfStatistics);

                        var latestStatisticsList = context.Statistics.Where(x => x.Version == maxVersionOfStatistics).ToList<Statistics>();

                        Log.WriteEntityLog(loggerName, "latestStatisticsList passed. ");
                        List<Statistics> statisticsList = null;
                        using (var driver = new ChromeDriver())
                        {
                            // Go to the home page
                            driver.Navigate().GoToUrl("https://www.mohfw.gov.in/");

                            DateTime lastUpdatedDate = GetLastUpdatedDate(driver);
                            Log.WriteEntityLog(loggerName, "lastUpdatedDate passed. " + lastUpdatedDate.ToString());

                            isLatestData = lastUpdatedDate == lastUpdateInDatabse.LastUpdated ? true : false;
                            Log.WriteEntityLog(loggerName, "isLatestData passed. " + isLatestData.ToString());

                            if (!isLatestData)
                            {

                                DateTime previousDate = lastUpdatedDate.AddDays(-1);
                                var totalCasesPreviousDay = context.OverallStatistics.Where(x => System.Data.Entity.DbFunctions.TruncateTime(x.LastUpdatedTime) == previousDate.Date).FirstOrDefault<OverallStatistics>();
                                Log.WriteEntityLog(loggerName, "totalCasesPreviousDay passed.");

                                List<String> overallStatisticsData = GetOverAllStatistics(maxVersionOfOverallStatistics, driver);
                                Log.WriteEntityLog(loggerName, "overallStatisticsData passed.");

                                OverallStatistics overallStatisticsObject = InsertOrUpdateOveAllStatisticsData(maxVersionOfStatistics, overallStatisticsData, lastUpdatedDate, overallStatistics, totalCasesPreviousDay);
                                Log.WriteEntityLog(loggerName, "overallStatisticsObject passed.");

                                List<string> tableHeaderValues = GetTableHeaderValues(driver);
                                List<dynamic> tableRowData = GetTableRowData(driver, tableHeaderValues);
                                statisticsList = InsertOrUpdateStatisticsData(maxVersionOfStatistics, tableHeaderValues, tableRowData, lastUpdatedDate, latestStatisticsList);
                                Log.WriteEntityLog(loggerName, "InsertOrUpdateStatisticsData passed.");

                                if (statisticsList.Any())
                                {
                                    context.Statistics.AddRange(statisticsList);
                                }

                                InsertOrUpdateLastUpdateData(lastUpdatedDate, lastUpdateInDatabse);
                                Log.WriteEntityLog(loggerName, "InsertOrUpdateLastUpdateData passed.");

                                //context.LastUpdate.Add(lastUpdate);
                                if (overallStatisticsObject != null)
                                {
                                    context.OverallStatistics.Add(overallStatisticsObject);
                                }
                                context.SaveChanges();


                                Log.WriteEntityLog(loggerName, "SaveChanges() passed.");

                                ResetCache();
                            }

                            Log.WriteEntityLog(loggerName, "end");
                            string jsonLog = JsonConvert.SerializeObject(statisticsList);
                            Log.WriteEntityLog(loggerName, jsonLog);
                            Thread.Sleep(8000);


                            //driver.Close();

                        }
                    }
                    catch (EntityException exception)
                    {

                        if (exception.Message != null)
                        {
                            // Output unexpected InnerExceptions.
                            Log.WriteEntityLog(loggerName, exception.Message.ToString());
                        }
                        exception.InnerException.ToString();

                    }
                }
            }
            catch (Exception exception)
            {
                if (exception.Message != null)
                    Log.WriteEntityLog(loggerName, exception.Message.ToString() + exception.InnerException.ToString(), LogType.Error);
            }


        }

        private static void ResetCache()
        {

            // Create an HttpClient instance
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.fightcorona.co.in/");
            // Usage
            HttpResponseMessage response = client.GetAsync("cache/reset").Result;

            if (response.IsSuccessStatusCode)
            {
                var dto = response.Content.ReadAsStringAsync().Result;
                Log.WriteEntityLog(loggerName, "resetcache passed." + dto);
            }
            else
            {
                Log.WriteEntityLog(loggerName, "resetcache failed" + (int)response.StatusCode + response.ReasonPhrase);
            }
        }

        private static void InsertOrUpdateLastUpdateData(DateTime lastUpdatedDate, LastUpdate lastUpdate)
        {
            lastUpdate.LastUpdated = lastUpdatedDate;
        }

        private static List<Statistics> InsertOrUpdateStatisticsData(int version, List<string> tableHeaderValues, List<dynamic> tableRowData, DateTime lastUpdatedDate, List<Statistics> latestStatisticsList)
        {

            List<Statistics> statisticsDataList = new List<Statistics>();

            DateTime createdDate = latestStatisticsList.Any() ? latestStatisticsList.FirstOrDefault().CreatedDate.Date : new DateTime();

            bool update = createdDate == lastUpdatedDate.Date ? true : false;
            var totalTableRowData = int.Parse(ConfigurationManager.AppSettings["noOfStates"]);
            tableRowData = tableRowData.Take(totalTableRowData).ToList();
            foreach (var tr in tableRowData)
            {

                var latestStatistics = latestStatisticsList.Where(x => x.Name_of_State_UT == tr[tableHeaderValues[1]]).FirstOrDefault();
                if (!update)
                {
                    statisticsDataList.Add(new Statistics()
                    {
                        SNo = int.Parse(tr[tableHeaderValues[0]]),
                        Name_of_State_UT = tr[tableHeaderValues[1]],
                        State_UT_Code = tr[tableHeaderValues[1]].Replace(" ", "").ToUpper(),
                        Total_Confirmed_cases_Indian_National = int.Parse(tr[tableHeaderValues[2]]),
                        Total_Confirmed_cases_Foreign_National = latestStatistics.Total_Confirmed_cases_Foreign_National,// int.Parse(tr[tableHeaderValues[3]]),
                        Cured_Discharged_Migrated = int.Parse(tr[tableHeaderValues[3]]),
                        Death = int.Parse(tr[tableHeaderValues[4]].Replace("#", "")),
                        CreatedDate = lastUpdatedDate,
                        Version = version + 1,
                        Id = 0

                    });
                }
                else if (latestStatistics != null)
                {
                    latestStatistics.SNo = int.Parse(tr[tableHeaderValues[0]]);
                    latestStatistics.Name_of_State_UT = tr[tableHeaderValues[1]];
                    latestStatistics.State_UT_Code = tr[tableHeaderValues[1]].Replace(" ", "").ToUpper();
                    latestStatistics.Total_Confirmed_cases_Indian_National = int.Parse(tr[tableHeaderValues[2]]);
                    latestStatistics.Total_Confirmed_cases_Foreign_National = latestStatistics.Total_Confirmed_cases_Foreign_National;//int.Parse(tr[tableHeaderValues[3]]);
                    latestStatistics.Cured_Discharged_Migrated = int.Parse(tr[tableHeaderValues[3]]);
                    latestStatistics.Death = int.Parse(tr[tableHeaderValues[4]].Replace("#", ""));
                    latestStatistics.CreatedDate = lastUpdatedDate;

                }

            }
            return statisticsDataList.ToList();
        }

        private static OverallStatistics InsertOrUpdateOveAllStatisticsData(int maxVersionOfStatistics, List<string> overallStatisticsData, DateTime lastUpdatedDate, OverallStatistics overallStatistics, OverallStatistics totalCasesPreviousDay)
        {

            OverallStatistics overallStatisticsObject = null;

            DateTime createdDate = overallStatistics != null ? overallStatistics.LastUpdatedTime.Date : new DateTime();

            bool update = createdDate == lastUpdatedDate.Date ? true : false;
            int overallStatisticsData0 = int.Parse(overallStatisticsData[0].Replace(",", ""));
            int overallStatisticsData1 = int.Parse(overallStatisticsData[1]);
            int overallStatisticsData2 = int.Parse(overallStatisticsData[2]);
            int overallStatisticsData3 = int.Parse(overallStatisticsData[3].Replace("#", ""));
            int overallStatisticsData4 = int.Parse(overallStatisticsData[4]);


            int totalCasesToday = overallStatisticsData1 + overallStatisticsData2 + overallStatisticsData3 + overallStatisticsData4;

            int totalCasesPrevDay = totalCasesPreviousDay != null ? totalCasesPreviousDay.TotalCasesOverDays : 0;

            if (!update)
            {

                overallStatisticsObject = new OverallStatistics()
                {
                    Passengers_screened_at_airport = overallStatisticsData0,
                    Active_COVID_2019_cases = overallStatisticsData1,
                    Cured_discharged_cases = overallStatisticsData2,
                    Death_cases = overallStatisticsData3,
                    Migrated_COVID19_Patient = overallStatisticsData4,
                    LastUpdatedTime = lastUpdatedDate,
                    DeathsOverDays = overallStatisticsData3,
                    NewCasesOverDays = totalCasesToday - totalCasesPrevDay,
                    TotalCasesOverDays = totalCasesToday,
                    Version = maxVersionOfStatistics + 1
                };
            }
            else
            {

                overallStatistics.Passengers_screened_at_airport = overallStatisticsData0;
                overallStatistics.Active_COVID_2019_cases = overallStatisticsData1;
                overallStatistics.Cured_discharged_cases = overallStatisticsData2;
                overallStatistics.Death_cases = overallStatisticsData3;
                overallStatistics.Migrated_COVID19_Patient = overallStatisticsData4;
                overallStatistics.LastUpdatedTime = lastUpdatedDate;
                overallStatistics.DeathsOverDays = overallStatisticsData3;
                overallStatistics.NewCasesOverDays = totalCasesToday - totalCasesPrevDay;
                overallStatistics.TotalCasesOverDays = totalCasesToday;
            }


            return overallStatisticsObject;
        }

        private static List<dynamic> GetTableRowData(ChromeDriver driver, List<string> tableHeaderValues)
        {

            #region tableRows
            var tableRows = driver.FindElements(By.XPath("//div[@class='content newtab']/div/table/tbody/tr"));

            var tableRowData = new List<dynamic>();

            foreach (IWebElement tr in tableRows)
            {
                var rowIndex = 0;
                var row = new Dictionary<object, object>();

                foreach (IWebElement td in tr.FindElements(By.XPath(".//td")))
                {

                    var rowValue = td.GetAttribute("innerText").ToUpper() == "TELENGANA" ? "Telangana" : td.GetAttribute("innerText");
                    row.Add(tableHeaderValues[rowIndex], rowValue);
                    //Console.WriteLine("" + td.GetAttribute("innerText"));
                    rowIndex++;
                }
                tableRowData.Add(row);

            }

            //if (tableRowData != null)
            //{
            //    tableRowData.RemoveAt(tableRowData.Count - 1);
            //}

            #endregion
            return tableRowData;
        }

        private static List<string> GetTableHeaderValues(ChromeDriver driver)
        {

            #region TableHeader
            var tableHeaders = driver.FindElements(By.XPath("//div[@class='content newtab']/div/table/thead/tr/th/strong"));

            Thread.Sleep(8000);
            List<String> tableHeaderValues = new List<String>();

            foreach (IWebElement th in tableHeaders)
            {
                tableHeaderValues.Add(th.GetAttribute("innerHTML"));
                //Console.WriteLine("" + th.GetAttribute("innerHTML"));
            }

            #endregion
            return tableHeaderValues;
        }

        private static List<String> GetOverAllStatistics(int maxVersionOfOverallStatistics, ChromeDriver driver)
        {
            #region Overall data

            var overAllData = driver.FindElements(By.XPath("//div[@class='contribution col-sm-9']/div[@class='information_block']//div[@class='information_row']/div[@class='iblock']/div[@class='iblock_text']/span"));

            // var overAllData = overAllData_iblocks.
            Thread.Sleep(8000);

            List<String> overAllDataValues = new List<String>();

            foreach (IWebElement data in overAllData)
            {
                overAllDataValues.Add(data.GetAttribute("innerHTML"));
                //Console.WriteLine("" + data.GetAttribute("innerHTML"));
            }

            return overAllDataValues;

            #endregion
        }

        private static DateTime GetLastUpdatedDate(ChromeDriver driver)
        {
            #region Lastupdated

            var lastUpdatedInfo = driver.FindElements(By.XPath("//div[@class='content newtab']/p"));

            string stringWithDate = lastUpdatedInfo[0].GetAttribute("innerText");//"(*including foreign nationals, as on 22.03.2020 at 09:45 AM)";
            Match matchDate = Regex.Match(stringWithDate, @"\d{2}.\d{2}.\d{4}");
            Match matchtime = Regex.Match(stringWithDate, @"\d{2}:\d{2} ([AaPp][Mm])");
            var dateString = matchDate.Value.Replace('.', '/') + " " + matchtime.Value;
            DateTime lastUpdatedDate = new DateTime();



            if (!string.IsNullOrEmpty(dateString))
            {
                lastUpdatedDate = DateTime.Parse(dateString, new CultureInfo("en-GB"));

            }

            #endregion
            return lastUpdatedDate;
        }
    }
}



