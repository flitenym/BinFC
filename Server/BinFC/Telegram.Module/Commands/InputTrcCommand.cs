using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
{
    public class InputTrcCommand : BaseCommand
    {
        public InputTrcCommand()
        {
        }

        public override string Name => CommandNames.InputTrcCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id,
                TelegramLoc.InputTrc,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}