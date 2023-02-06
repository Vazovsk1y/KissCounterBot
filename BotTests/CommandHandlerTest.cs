using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Controllers;

namespace BotTests
{
    [TestClass]
    public class CommandHandlerTest
    {
        [TestMethod]
        public void ProcessCommandHandlerTest_IncorrectData()
        {
            List<string> incorrectCommandText = new List<string>()
            {
                "incorrect text",
                "incorrect text",
                "/some_command",
                "/some_command",
                "/JOIN",
                "/KISS",
                "/TOP",
                "/JOIN",
                "/KISS",
                "/TOP",
                "/Join",
                "/Kiss",
                "/Top",
                "/Join",
                "/Kiss",
                "/Top",
                "/jOin",
                "/kIss",
                "/tOp"  ,
                "/JoiN",
                "/kisS",
                "/tOp",
                "some text /kiss",
                "some text /top",
                "some text /join",
                null
            };

            foreach(var item in incorrectCommandText)
            {
                Assert.IsFalse(CommandHandler.ProcessCommand(item ?? string.Empty));
            }
        }

        [TestMethod]
        public void ProcessCommandHandlerTest_CorrectData()
        {
            List<string> correctCommandText = new List<string>()
            {
                "/top",
                "/kiss",
                "/top",
                "/kiss Some text",
                "/join Some text",
                "/top Some text"
            };

            foreach (var item in correctCommandText)
            {
                Assert.IsTrue(CommandHandler.ProcessCommand(item));
            }
        }
    }
}
