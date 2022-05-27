using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class ErrorInputIdCommand : BaseCommand
    {
        public ErrorInputIdCommand()
        {
            
        }

        public override string Name => CommandNames.ErrorInputIdCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.IncorrectId,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}