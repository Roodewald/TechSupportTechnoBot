using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
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
		static readonly string SpreadsheetId = "12WcDQ3_Gtxl18dK3Vfg83yEh2MleKjg8Jqq3facPNpo";
		static readonly string sheet = "Tasks";
		private SheetsService service;
		[STAThread]
			

		public async Task Run()
		{
			UserCredential credential;
			using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
			{
				credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					new[] { SheetsService.Scope.Spreadsheets },
					"user", CancellationToken.None);
			}

			// Create the service.
			service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = "TechSupportTechnoBot",
			});		
		}
		static void ReadEntries(SheetsService service)
		{

			var range = $"{sheet}!A1:F10";
			var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

			var response = request.Execute();
			var values = response.Values;
			if (values != null && values.Count > 0)
			{
				foreach (var row in values)
				{
					Console.WriteLine("{0} {1} | {2} | {3}", row[5], row[4], row[3], row[1]);
				}
			}
			else Console.WriteLine("No data.");

		}
		public void CreateEntries(string name, string building, string cab, string message)
		{
			
			var range = $"{sheet}!A:D";
			var valueRange = new ValueRange();

			var objectList = new List<object>() { name,building,cab,message };
			valueRange.Values = new List<IList<object>> { objectList };

			var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
			appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
			var appendResponse = appendRequest.Execute();
			
		}
	}
}
