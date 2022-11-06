using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace TechSupportTechnoBot
{
	internal class GoogleClass
	{
		public static SheetsService ConnectToGoogle()
		{
			// При изменении этих областей удалите ранее сохраненные учетные данные.
			// в ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
			string[] Scopes = { SheetsService.Scope.Spreadsheets };
			string ApplicationName = "Excel to Google Sheet";

			UserCredential credential;

			using (var stream =
				new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
			{
				// Файл token.json хранит токены доступа и обновления пользователя и создается
				// автоматически, когда поток авторизации завершается в первый раз.
				string credPath = "token.json";
				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			// Создать службу API Google Таблиц
			var service = new SheetsService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			return service;
		}
	}
}

