using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
{
    public class StartCommand : BaseCommand
    {
        public StartCommand()
        {
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton(CommandNames.GetOperationsCommand)
                })
            {
                ResizeKeyboard = true
            };

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Format(TelegramLoc.StartCommand, CommandNames.GetOperationsCommand),
                replyMarkup: replyKeyboardMarkup);
        }
    }
}