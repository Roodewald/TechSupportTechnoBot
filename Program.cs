using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

//V1.0 Release date 11.11.22
namespace TechSupportTechnoBot
{
	internal class Program
	{
		public long[] sysAdmins = {
			1517971517, // Рязанов Станислав Павлович
			970587775,  // Савченко Павел Петрович
			825199836,  // Маша
			461152825,  // Кирилл Балабушко
		};
		TelegramBotClient botClient = new TelegramBotClient("5522766988:AAEK8OThHKWoDc7A7ZBMFson5XjhMy__XDM");

		static async Task Main(string[] args)
		{
			GoogleHelper googleHelper = new GoogleHelper();
			var program = new Program();
			var botClient = program.botClient;



			string[,] dbase = new string[200, 5];

			using var cts = new CancellationTokenSource();
			googleHelper.Run().Wait();



			// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
			var receiverOptions = new ReceiverOptions
			{
				AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
			};
			botClient.StartReceiving(
			updateHandler: HandleUpdateAsync,
			pollingErrorHandler: HandlePollingErrorAsync,
			receiverOptions: receiverOptions,
			cancellationToken: cts.Token
			);

			var me = await botClient.GetMeAsync();


			Console.WriteLine($"Start listening for @{me.Username}");
			Console.ReadLine();

			// Send cancellation request to stop bot
			cts.Cancel();



			async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
			{
				var housingKeyboard = new InlineKeyboardMarkup(new[]
				{
					// first row
					new []
					{
						InlineKeyboardButton.WithCallbackData("Мелик-Карамова 4/1", "Мелик-Карамова 4/1"),
						InlineKeyboardButton.WithCallbackData("Рабочая 43", "Рабочей 43"),
					},
					// second row
					new []
					{
						InlineKeyboardButton.WithCallbackData("Крылова.д 41/1", "\"Крылова.д 41/1"),
						InlineKeyboardButton.WithCallbackData("50 ЛетВЛКСМ", "50 ЛетВЛКСМ"),
					},
					// third row
					new []
					{
						InlineKeyboardButton.WithCallbackData("Кукуевицкого 2", "\"Кукуевицкого 2"),
						InlineKeyboardButton.WithCallbackData("Дзержинского 6/1", "Дзержинского 6/1"),
					}
				});


				if (update.Type == UpdateType.Message)
				{

					Message message = update.Message;

					if (message.Text != null)
					{
						Console.WriteLine(message.Text + " ID: " + message.From.Id);
						if (message.Text.ToLower().StartsWith("/start"))
						{
							await botClient.SendTextMessageAsync(message.Chat.Id, "Бот создан для формирования запросов в техническую службу. \nДля формирования запроса используйте предложенные ботом команды.", replyMarkup: housingKeyboard);
						}
						if (message.Text.ToLower().StartsWith("/cab"))
						{
							dbase[ChekID(message.From.Id), 2] = message.Text.Remove(0, 4).TrimStart(' ');
							await botClient.SendTextMessageAsync(message.Chat.Id, "Чем мы можем вам помочь?\nДля отправки используйте команду /task\nПример: /task На компьютере №5 не работает сетевая папка.");
						}
						if (message.Text.ToLower().StartsWith("/task"))
						{
							dbase[ChekID(message.From.Id), 3] = message.Text.Remove(0, 5).TrimStart(' ');
							await botClient.SendTextMessageAsync(message.Chat.Id, "Как к Вам можно обращаться? \nДля отправки используйте команду /name\nПример: /name Павел Петрович ");
						}
						if (message.Text.ToLower().StartsWith("/name"))
						{
							dbase[ChekID(message.From.Id), 4] = message.Text.Remove(0, 5).TrimStart(' ');
							await botClient.SendTextMessageAsync(message.Chat.Id, UserData(message.Chat.Id));
							Console.WriteLine("Отправлен запрос");
							VipeUserData(message.Chat.Id);
						}
					}
					return;
				}
				//Оброботка запроса при нажатиии на клавиатуру
				if (update.Type == UpdateType.CallbackQuery)
				{
					CallbackQuery callbackQuery = update.CallbackQuery;

					dbase[ChekID(callbackQuery.From.Id), 1] = callbackQuery.Data;
					await botClient.SendTextMessageAsync(callbackQuery.From.Id, "В каком кабинете требуется обслуживание?\nДля отправки используйте команду /cab\nПример: /cab 7");
				}
			}

			//Проверяет кореектность введеных данных
			string UserData(long senderId)
			{
				long ID = ChekID(senderId);
				for (int i = 0; i < dbase.GetLength(1); i++)
				{
					if (dbase[ID, i] == null || dbase[ID, i].Length < 1)
					{
						return $"Бот получил от вас пустую строчку в оном из сообщений. Начните заново командой /start";
						Console.WriteLine("Запрос отклонен");
					}
				}
				string createdUserData = $"В корпусе на {dbase[ID, 1]}\nВ кабинете: {dbase[ID, 2]} \nСообщение: {dbase[ID, 3]}";
				string ueserMassage = $"Вы отправили запрос в ТехСлужбу!\n\n{createdUserData}\nМы сделаем все возможное чтобы помочь вам!\nДля отправки еще одного запроса напишите команду /start";
				string sysAdminMassage = $"Пользователь {dbase[ID, 4]} \n{createdUserData}";
				program.SendToSysAdmins(sysAdminMassage);
				googleHelper.CreateEntries(dbase[ID, 4], dbase[ID, 1], dbase[ID, 2], dbase[ID, 3], dbase[ID, 0]);

				return ueserMassage;
			}
			//Удаляет пользоваетля из базы данных
			void VipeUserData(long senderId)
			{
				var id = ChekID(senderId);
				for (int i = 0; i < dbase.GetLength(1); i++)
				{
					dbase[id, i] = null;
				}
			}
			//Выделяет ячейку памяти под ID пользователя. Или ищит ее
			int ChekID(long senderId)
			{
				for (int i = 0; i < dbase.GetLength(0); i++)
				{
					if (dbase[i, 0] == senderId.ToString())
					{
						return i;
					}
				}
				for (int i = 0; i < dbase.GetLength(0); i++)
				{
					if (dbase[i, 0] == null)
					{
						dbase[i, 0] = senderId.ToString();
						return i;
					}
				}
				return dbase.GetLength(0)-1;
			}

			Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
			{
				var ErrorMessage = exception switch
				{
					ApiRequestException apiRequestException
						=> $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
					_ => exception.ToString()
				};

				Console.WriteLine(ErrorMessage);
				return Task.CompletedTask;
			}

		}

		async void SendToSysAdmins(string message)
		{
			var program = new Program();
			var sysAdmins = program.sysAdmins;
			var botClient = program.botClient;
			foreach (var admin in sysAdmins)
			{
				await botClient.SendTextMessageAsync(admin, message);
			}
		}
	}
}