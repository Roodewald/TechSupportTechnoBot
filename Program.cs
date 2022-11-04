﻿using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

string[,] dbase = new string[200,5];
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
		InlineKeyboardButton.WithCallbackData("Мелик-Карамова 4/1", "Мелик-Карамова 4/1"),
		InlineKeyboardButton.WithCallbackData("Рабочая 43", "Рабочей 43"),
	},
    // second row
    new []
	{
		InlineKeyboardButton.WithCallbackData("Крылова.д 41/1", "\"Крылова.д 41/1"),
		InlineKeyboardButton.WithCallbackData("50 ЛетВЛКСМ", "50 ЛетВЛКСМ"),
	}
	});
	

	if (update.Type == UpdateType.Message)
	{

		Message message = update.Message;
		if (message.Text.ToLower().StartsWith("/start"))
		{
			await botClient.SendTextMessageAsync(message.Chat.Id, "Этот бот создан для создания запросов в Техническую службу. \nВыберете ваш корпус", replyMarkup: housingKeyboard);
		}
		if (message.Text.ToLower().StartsWith("/cab"))
		{
			dbase[ChekID(message.From.Id), 2] = message.Text;
			await botClient.SendTextMessageAsync(message.Chat.Id, "Чем может помочь вам тех служба?\nДля отправки используйте команду/task\nПример:/task 5 компьютер не работает сеть");
		}
		if (message.Text.ToLower().StartsWith("/task"))
		{
			dbase[ChekID(message.From.Id), 3] = message.Text;
			await botClient.SendTextMessageAsync(message.Chat.Id, "Как к Вам можно обращаться? \nДля отправки используйте команду/name\nПример:/name Павел Петрович ");
		}
		if (message.Text.ToLower().StartsWith("/name"))
		{
			dbase[ChekID(message.From.Id), 4] = message.Text;
			await botClient.SendTextMessageAsync(message.Chat.Id, UserData(message.Chat.Id));
			Console.WriteLine(UserData(message.Chat.Id));
		}
		return;
	}

	if (update.Type == UpdateType.CallbackQuery)
	{
		CallbackQuery callbackQuery = update.CallbackQuery;

		dbase[ChekID(callbackQuery.From.Id), 1] = callbackQuery.Data;
		await botClient.SendTextMessageAsync(callbackQuery.From.Id, "В каком кабинете требуется обслуживание?\nДля отправки используйте команду/cab\nПример:/cab 21");
	}
}

string UserData(long senderId)
{
	long ID = ChekID(senderId);
	return $"Пользователь {dbase[ID,4]} в корпусе на {dbase[ID,1]}, в кабинете: {dbase[ID, 2]}, \nОставил сообщение:\n{dbase[ID, 3]}";
}

int ChekID(long senderId)
{
	for (int i = 0; i < 200; i++)
	{
		if (dbase[i,0] == senderId.ToString())
		{
			return i;
		}
	}
	for (int i = 0; i < 200; i++)
	{
		if (dbase[i,0] == null)
		{
			dbase[i,0] = senderId.ToString();
			return i;
		}
	}
	return 0;
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
