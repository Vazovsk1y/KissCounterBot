using System;
using TelegramBot.Controllers;
using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using TelegramBot.Models;
using System.Linq;

namespace TelegramBot
{
    public class Program        
    {
        private static TelegramBotClient client = new TelegramBotClient(Configuration.BotToken);

        private static void Main(string[] args)
        {
            Console.WriteLine("BOT is working!");
            client.StartReceiving(HandleUpdate, HandleErrors);
            Console.ReadLine();
        }

        private static Task HandleErrors(ITelegramBotClient botClient, Exception exceptinon, CancellationToken token)
        {
            var errorMessage = exceptinon switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n {apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exceptinon.ToString()
            };

            Console.WriteLine($"Error: {errorMessage}");
            return Task.CompletedTask;
        }

        async private static Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            if (IsUpdateCorrectType(update))
            {
                ServerNotification(update.Message);                                     // message in console.
                CommandHandler commandHandler = new CommandHandler(update.Message);
                await commandHandler.ProcessCommand(botClient);
            }
            return;
        }

        private static void ServerNotification(Message message)
        {
            var text = message.Text;
            string serverMessage = $"{message.From.Username ?? message.From.FirstName} is wrote {text} " +
                        $"from {message.Chat.Type} with title {message.Chat.Title}";
            Console.WriteLine(serverMessage);
        }

        public static bool IsUpdateCorrectType(Update update)  
        {
            /* 
               Bot works only with commands that were sent 
               from groups or supergroups and
               if update was not a message bot doing nothing. 
            */
            if (!update.Type.Equals(UpdateType.Message))       
                return false;

            var message = update.Message;
            var permissedMessageType = MessageType.Text;
            var permissedChatTypeFirst = ChatType.Supergroup;
            var permissedChatTypeSecond = ChatType.Group;

            return message.Type.Equals(permissedMessageType) && message.Text.StartsWith("/") 
               && (message.Chat.Type.Equals(permissedChatTypeFirst) || message.Chat.Type.Equals(permissedChatTypeSecond)) ? true : false;
        }
    }
}