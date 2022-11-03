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
	// Only process Message updates: https://core.telegram.org/bots/api#message
	if (update != null)
	{
		if (update.Message is not { } message)
			return;
		// Only process text messages
		if (message.Text is not { } messageText)
			return;


		var chatId = message.Chat.Id;

		Console.WriteLine($"Received a '{messageText}' message in chat {chatId} {message.ReplyMarkup}");



		InlineKeyboardMarkup inlineKeyboard = new(new[]
			{
        // first row
        new []
		{
			InlineKeyboardButton.WithCallbackData(text: "1.1", callbackData: "11"),
			InlineKeyboardButton.WithCallbackData(text: "1.2", callbackData: "12"),
		},
        // second row
        new []
		{
			InlineKeyboardButton.WithCallbackData(text: "2.1", callbackData: "21"),
			InlineKeyboardButton.WithCallbackData(text: "2.2", callbackData: "22"),
		},
	});

		Message sentMessage = await botClient.SendTextMessageAsync(
			chatId: chatId,
			text: "You said:\n" + messageText,
			 replyMarkup: inlineKeyboard,
			cancellationToken: cancellationToken);
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