using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var botClient = new TelegramBotClient("5522766988:AAEK8OThHKWoDc7A7ZBMFson5XjhMy__XDM");

using var cts = new CancellationTokenSource();

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
							InlineKeyboardButton.WithCallbackData("Мелик-Карамова", "MK"),
							InlineKeyboardButton.WithCallbackData("Рабочая 43", "R43"),
						},
                        // second row
                        new []
						{
							InlineKeyboardButton.WithCallbackData("Крылова.д 41/1", "Krilova"),
							InlineKeyboardButton.WithCallbackData("50 ЛетВЛКСМ", "50let"),
						}
					});


	if (update.Type == UpdateType.Message)
	{
		Message message = update.Message;
		if (message.Text.ToLower().StartsWith("/start"))
		{
			await botClient.SendTextMessageAsync(message.Chat.Id, "Этот бот создан для создания запросов в Техническую службу. \nВыбирете ваш корпус", replyMarkup: housingKeyboard);
		}
	}

	if (update.Type == UpdateType.CallbackQuery)
	{
		CallbackQuery callbackQuery = update.CallbackQuery;
		Console.WriteLine($"Received a  {callbackQuery.Data}");
	}
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