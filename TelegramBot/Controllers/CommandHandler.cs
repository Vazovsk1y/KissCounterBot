using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Resourses;
using System;

namespace TelegramBot.Controllers
{
    internal class CommandHandler
    {
        /*
         If you will delete the bot from group he lose the acsess to the message
         that were sent before his deleting, therefore after if you will add it 
         back and will try to use the /kiss command on each message that were
         sent before bot was added, bot will send you message that you used
         this command not like an answer, but you used it correct.
        */

        private static string _Join = "/join";
        private static string _Kiss = "/kiss";
        private static string _Top = "/top";
        private long _currentChatID;
        private long _currentUserID;
        private string _currentCommandText;
        private string _currentUsername;
        private Message _currentMessage;                           // for database methods.

        public CommandHandler(Message message) 
        {
            _currentChatID = message.Chat.Id;
            _currentUserID = message.From.Id;
            _currentCommandText = message.Text;
            _currentMessage = message;
            _currentUsername = message.From.Username ?? message.From.FirstName ?? string.Empty;
        }

        async public Task ProcessCommand(ITelegramBotClient botClient)
        {
            // if i will have more than 3 command it would be wise to rewrite with another construction.
            try
            {
                if (_currentCommandText.Contains(_Join))
                    await JoinHandler(botClient);
                else if (_currentCommandText.Contains(_Kiss))
                    await KissHandler(botClient);
                else if (_currentCommandText.Contains(_Top))
                    await TopHandler(botClient);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        async private Task TopHandler(ITelegramBotClient botClient)
        {
            /*
             Bot could used in different groups at the same time where he can have different top for each of this groups, 
             also one user can be exist in two groups simultaneously, where this bot is using. 
             In each group user can have the different score, so kissed in one group will 
             not influence on score in other group, MANY USERS - MANY GROUPS - DIFFERENT SCORES.
            */

            string messageReplyTop = string.Empty;
            if(DataBase.IsInGroupAnybodyPlay(_currentMessage))
            {
                messageReplyTop = DataBase.GetTop(_currentMessage);
                await SendMessageToChat(botClient, messageReplyTop);
            }
            else
            {
                messageReplyTop = "Here is nobody playing...";
                await SendMessageToChat(botClient, messageReplyTop);
            }
        }     

        async private Task KissHandler(ITelegramBotClient botClient)
        {
            string messageReplyKiss = string.Empty;
            if (!IsMessageReply())
            {
                messageReplyKiss = $"@{_currentUsername} used this command not like an answer!";
                await SendMessageToChat(botClient, messageReplyKiss);
            }
            else if (IsUsedOnYourself())
            {
                messageReplyKiss = $"@{_currentUsername}, you can't kiss yourself!";
                await SendMessageToChat(botClient, messageReplyKiss);
            }
            else if (IsBothPlaying())
            {
                var username = _currentMessage.ReplyToMessage.From.Username ?? _currentMessage.ReplyToMessage.From.FirstName ?? string.Empty;   // who was kissed.
                messageReplyKiss = $"@{_currentUsername} kissed the @{username}";
                DataBase.UpdateRecord(_currentMessage.ReplyToMessage);
                await SendMessageToChat(botClient, messageReplyKiss);
            }
            else
            {
                var username = _currentMessage.ReplyToMessage.From.Username ?? _currentMessage.ReplyToMessage.From.FirstName ?? string.Empty;   // who was kissed.
                messageReplyKiss = $"@{username} or @{_currentUsername} isn't playing!";
                await SendMessageToChat(botClient, messageReplyKiss);
            }

            /* The correct /kiss using must be like
             - Both players are playing
             - Not used on yourself 
             - Must be using like reply on another message */
            bool IsBothPlaying() => DataBase.IsUserPlayingGame(_currentMessage) && DataBase.IsUserPlayingGame(_currentMessage.ReplyToMessage);
            bool IsUsedOnYourself() => _currentUserID.Equals(_currentMessage.ReplyToMessage.From.Id);
            bool IsMessageReply() => _currentMessage.ReplyToMessage != null;
        }    

        async private Task JoinHandler(ITelegramBotClient botClient)
        {
            string messageReplyJoin = string.Empty;
            if (DataBase.IsUserPlayingGame(_currentMessage))
            {
                messageReplyJoin = $"{_currentUsername}, you are already playing!";
                await SendMessageToChat(botClient, _currentChatID, messageReplyJoin);
            }
            else
            {
                DataBase.AddUserWithCurrentGroup(_currentMessage);
                messageReplyJoin = $"{_currentUsername}, have joined to the game!";
                await SendMessageToChat(botClient, messageReplyJoin);
            }
        }    

        /// <summary>
        /// Send a message to the chat from what message was sent, have one overload that send a message to
        /// chat that you could point.
        /// </summary>
        /// <param name="client"> BotClient</param>
        /// <param name="chatID"> ChatID</param>
        /// <param name="text"> Message text</param>
        /// <returns></returns>
        async private static Task SendMessageToChat(ITelegramBotClient client, long chatID, string text) 
            => await client.SendTextMessageAsync(chatID, text);

        async private Task SendMessageToChat(ITelegramBotClient client, string text)
            => await client.SendTextMessageAsync(_currentChatID, text);                                   
    }
}


