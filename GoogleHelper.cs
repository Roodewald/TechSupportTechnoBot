using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.IO;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace TechSupportTechnoBot
{
	/// <summary>
	/// Sample which demonstrates how to use the Books API.
	/// https://developers.google.com/books/docs/v1/getting_started
	/// <summary>
	internal class GoogleHelper
	{
		static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
		static readonly string SpreadsheetId = "12WcDQ3_Gtxl18dK3Vfg83yEh2MleKjg8Jqq3facPNpo";
		static readonly string sheet = "Tasks";
		private SheetsService service;
		[STAThread]
			

		public async Task Run()
		{
			GoogleCredential credential;
			using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				credential = GoogleCredential.FromStream(stream)
					.CreateScoped(Scopes);
			}

			// Create the service.
			service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "TechSupportTechnoBot",
			});
			Console.WriteLine($"GoogleHelper запущен.");
		}
		public void CreateEntries(string name, string building, string cab, string message,string ID)
		{
			
			var range = $"{sheet}!A:G";
			var valueRange = new ValueRange();

			var objectList = new List<object>() { name,building,cab,message, ID, DateTime.Now.ToString("dd/MM/yy HH:mm"), "Заявка принята" };
			Console.WriteLine($"Заявка {name} отправлена !");
			valueRange.Values = new List<IList<object>> { objectList };

			var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
			var appendResponse = appendRequest.Execute();
			
		}
	}
}
