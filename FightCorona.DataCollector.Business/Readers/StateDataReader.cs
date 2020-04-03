using System;
using System.Collections.Generic;
using System.Linq;
using FightCorona.DataCollector.Business.Helpers;
using FightCorona.DataCollector.Data.Adapters;
using FightCorona.DataCollector.Data.Models;
using FightCorona.DataCollector.Logger;

namespace FightCorona.DataCollector.Business.Readers
{
    public class StateDataReader
    {
        #region Private Members

        private static string spreadsheetId = "1u40dztecfGHJhFXrvvnV0fUWSSnLRZLhCpXHi_kfO0M";
        private static string readerName = "TNDataReader";
        private static string loggerName = "StateDataReader";
        private static DateTime startDate = new DateTime(2020, 03, 31);
        private static TimeZoneInfo indianTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        private GoogleSheetReader googleSheetReader { get; set; }
        private ReaderStatusDataAdapter readerStatusDataAdapter { get; set; }
        private DistrictsStatusDataAdapter districtsStatusDataAdapter { get; set; }

        #endregion Private Members

        #region Constructor

        public StateDataReader()
        {
            googleSheetReader = new GoogleSheetReader(spreadsheetId);
            readerStatusDataAdapter = new ReaderStatusDataAdapter();
            districtsStatusDataAdapter = new DistrictsStatusDataAdapter();
        }

        #endregion Constructor

        #region Public Methods

        public void Read()
        {
            try
            {
                var currentSheetVersion = GetCurrentSheetVersion();
                var readerVersion = GetReaderVersion();
                Log.WriteEntityLog(loggerName, string.Format("Current Sheet Version: {0}, Reader Version: {1}", currentSheetVersion, readerVersion));

                if (currentSheetVersion == readerVersion)
                {
                    Log.WriteEntityLog(loggerName, "Current Sheet Version and Reader Version are same, completed the task");
                    return;
                }

                var datesToBeRead = GetDatesToBeRead();
                foreach (var date in datesToBeRead)
                {
                    UpdateDistrictsData(date);
                }

                readerStatusDataAdapter.Update(readerName, currentSheetVersion.ToString());

                Log.WriteEntityLog(loggerName, string.Format("Data updated to version {0}", currentSheetVersion), LogType.Error);
            }
            catch (Exception ex)
            {
                Log.WriteEntityLog(loggerName, String.Format("Stopped with error, error details: {0}", ex.Message), LogType.Error);
            }

        }

        #endregion Public Methods

        #region Private Methods

        private int GetCurrentSheetVersion()
        {
            int version = 0;
            var versionData = googleSheetReader.GetSheetData("Version", "A1:A1");
            if (versionData?.Count == 1)
                int.TryParse(versionData[0][0].ToString(), out version);
            return version;
        }

        private int GetReaderVersion()
        {
            var readerStatus = readerStatusDataAdapter.Get(readerName);
            if (readerStatus == null)
                readerStatus = readerStatusDataAdapter.Add(new ReaderStatus { ReaderName = readerName, Version = "0" });
            int.TryParse(readerStatus.Version, out int version);
            return version;
        }

        private List<DateTime> GetDatesToBeRead()
        {
            var datesToBeRead = new List<DateTime>();
            var currentDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indianTimeZone).Date;
            var lastInsertedDate = districtsStatusDataAdapter.GetLastInsertedDate();

            var firstDateToBeRead = lastInsertedDate ?? startDate;

            for (var dt = firstDateToBeRead; dt <= currentDate; dt = dt.AddDays(1))
            {
                datesToBeRead.Add(dt);
            }

            return datesToBeRead;
        }

        private void UpdateDistrictsData(DateTime date)
        {
            //Read
            var sheetName = date.ToString("dd-MM");
            var result = googleSheetReader.GetSheetData(sheetName, "A1:E");

            //Transform
            var districtsData = TransformDistrictData(result, date);

            //Save
            districtsStatusDataAdapter.AddOrUpdateRange(districtsData);
        }

        private List<DistrictStatus> TransformDistrictData(IList<IList<object>> data, DateTime date)
        {
            var districtsStatus = new List<DistrictStatus>();
            foreach (var item in data.Skip(1))
            {
                var districtStatusObj = item.ToArray();
                districtsStatus.Add(
                   new DistrictStatus
                   {
                       Location = (string)districtStatusObj[0],
                       Confirmed = Convert.ToInt32(districtStatusObj[1]),
                       Active = Convert.ToInt32(districtStatusObj[2]),
                       Recovered = Convert.ToInt32(districtStatusObj[3]),
                       Deaths = Convert.ToInt32(districtStatusObj[4]),
                       Date = date
                   }
                );
            }
            return districtsStatus;
        }

        #endregion Private Methods
    }
}
