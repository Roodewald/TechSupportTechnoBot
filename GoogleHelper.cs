using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechSupportTechnoBot
{
	internal class GoogleHelper
	{
		private readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

		static readonly string ApplicationName = "TechSupportTechnoBot";
		static readonly string SpreadsheetId = "12WcDQ3_Gtxl18dK3Vfg83yEh2MleKjg8Jqq3facPNpo";
		static readonly string sheet = "Tasks";
		static SheetsService service;
		public GoogleHelper(string token)
		{
			
		}
	}
}
