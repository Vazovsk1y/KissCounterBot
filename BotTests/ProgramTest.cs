using System;
using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;

namespace BotTests
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void IsUpdateCorrectType_IncorrectUpdateTypes()
        {
            List<Update> updates = new List<Update>()
            {
                new Update { Poll = new Poll() },
                new Update { CallbackQuery = new CallbackQuery() },
                new Update { InlineQuery = new InlineQuery() },
                new Update { ChannelPost = new Message() },
                new Update { ChatJoinRequest = new ChatJoinRequest() },
                new Update { MyChatMember = new ChatMemberUpdated() },
                new Update { EditedMessage = new Message() },
                new Update { EditedChannelPost = new Message() },
                new Update { ChosenInlineResult = new ChosenInlineResult() },
                new Update() { Message= new Message { Text = "/join" , Chat = new Chat { Type = ChatType.Private} } },
                new Update() { Message= new Message { Text = "/kiss" , Chat = new Chat { Type = ChatType.Private} } },
                new Update() { Message= new Message { Text = "/top" , Chat = new Chat { Type = ChatType.Private} } },
                new Update() { Message= new Message { Text = "some text from private chat" , Chat = new Chat { Type = ChatType.Private} } },
                new Update() { Message= new Message { Text = "text from supergroup" , Chat = new Chat { Type = ChatType.Supergroup} } },
                new Update() { Message= new Message { Text = "text that contains /kiss" , Chat = new Chat { Type = ChatType.Supergroup} } },
                new Update() { Message= new Message { Text = "text that contains / " , Chat = new Chat { Type = ChatType.Group} } },
                new Update() { Message= new Message { Text = "also text from group" , Chat = new Chat { Type = ChatType.Group} } },
                new Update() { Message= new Message { Text = "show /top" , Chat = new Chat { Type = ChatType.Group} } },
                new Update { Message = null },
                new Update { Message = new Message { Text = null} },
            };

            foreach(var item in updates)
            {
                Assert.IsFalse(Program.IsUpdateCorrectType(item));
            }
        }

        [TestMethod]
        public void IsUpdateCorrectType_CorrectUpdateTypes()
        {
            List<Update> updates = new List<Update>()
            {
              new Update() { Message= new Message { Text = "/join" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/kiss" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/top" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/join" , Chat = new Chat { Type = ChatType.Group} } },
              new Update() { Message= new Message { Text = "/kiss" , Chat = new Chat { Type = ChatType.Group} } },
              new Update() { Message= new Message { Text = "/top" , Chat = new Chat { Type = ChatType.Group} } },
              new Update() { Message= new Message { Text = "/join some text" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/kiss some text" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/top sometext" , Chat = new Chat { Type = ChatType.Supergroup} } },
              new Update() { Message= new Message { Text = "/join sometext" , Chat = new Chat { Type = ChatType.Group} } },
              new Update() { Message= new Message { Text = "/kiss sometext" , Chat = new Chat { Type = ChatType.Group} } },
              new Update() { Message= new Message { Text = "/top sometext" , Chat = new Chat { Type = ChatType.Group} } },
            };

            foreach (var item in updates)
            {
                Assert.IsTrue(Program.IsUpdateCorrectType(item));
            }
        }
    }
}