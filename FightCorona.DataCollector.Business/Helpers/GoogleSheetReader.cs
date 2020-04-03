using System;
using System.Collections.Generic;
using System.Reflection;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace FightCorona.DataCollector.Business.Helpers
{
    public class GoogleSheetReader
    {
        private static string applicationName = "Data Collector";
        private static string credentialsFileResourceName = "FightCorona.DataCollector.Business.credentials.json";
        private string _spreadSheetId { get; set; }

        public GoogleSheetReader(string spreadSheetId)
        {
            _spreadSheetId = spreadSheetId;
        }

        public IList<IList<Object>> GetSheetData(string sheetName, string cellRange)
        {
            GoogleCredential credential;
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(credentialsFileResourceName))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName,
            });

            var range = $"{sheetName}!{cellRange}";
            var request = service.Spreadsheets.Values.Get(_spreadSheetId, range);

            var response = request.Execute();
            return response.Values;
        }
    }
}
