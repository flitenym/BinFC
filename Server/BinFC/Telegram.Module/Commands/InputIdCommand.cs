using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
{
    public class InputIdCommand : BaseCommand
    {
        public InputIdCommand()
        {
        }

        public override string Name => CommandNames.InputIdCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.InputId,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}