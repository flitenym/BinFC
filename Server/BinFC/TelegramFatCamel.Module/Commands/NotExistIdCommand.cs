using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class NotExistIdCommand : BaseCommand
    {
        public NotExistIdCommand()
        {
        }

        public override string Name => CommandNames.NotExistIdCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Format(TelegramLoc.IdNotRegister, CommandNames.InputIdCommand),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}