using ClinicRequestBot.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ClinicRequestBot
{
	public class Program() //ніяк не була змінена
	{
		static ReplyKeyboardMarkup replyKeyboardMarkup;
		public async static Task Main()
		{
			Console.OutputEncoding = System.Text.Encoding.UTF8;
			ITelegramBotClient tgbclient = new TelegramBotClient("6898246655:AAFcpzxcN5ukOdRQ9Z-4XobXrrf6DXDU-IE");
			var _receiverOptions = new ReceiverOptions 
			{
				AllowedUpdates = Array.Empty<UpdateType>(),
				ThrowPendingUpdates = true,
			};
			using var cts = new CancellationTokenSource();
			replyKeyboardMarkup = new(new[]
			{
				KeyboardButton.WithRequestContact("Надіслати заявку (потрібен номер телефону)")
			})
			{
				ResizeKeyboard = true
			};
			tgbclient.StartReceiving(
				updateHandler: HandleUpdateAsync,
				pollingErrorHandler: HandlePollingErrorAsync,
				receiverOptions: _receiverOptions,
				cancellationToken: cts.Token);

			var me = await tgbclient.GetMeAsync();
			Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
			await Task.Delay(-1);
		}
		static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
		{
			if (update.Message is not { } message)
				return;

			var chatId = message.Chat.Id;

			if(update.Type==UpdateType.Message&&update.Message.Type==MessageType.Contact)
			{
				Storage.Add(new Request
				{
					FirstName = update.Message.Contact.FirstName,
					LastName = update.Message.Contact.LastName,
					PhoneNumber = update.Message.Contact.PhoneNumber
				});
				await botClient.SendTextMessageAsync(
					chatId: chatId,
					text: "Ваша заявка була відправлена! Вам зателефонують",
					cancellationToken: cancellationToken);
			}

			// Echo received message text
			Message sentMessage = await botClient.SendTextMessageAsync(
				chatId: chatId,
				text: "Оберіть дію",
				replyMarkup: replyKeyboardMarkup,
				cancellationToken: cancellationToken);
		}

		static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
}