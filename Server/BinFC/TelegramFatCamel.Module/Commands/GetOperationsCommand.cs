using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class GetOperationsCommand : BaseCommand
    {
        public GetOperationsCommand()
        {
        }

        public override string Name => CommandNames.GetOperationsCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Format(TelegramLoc.Operations, 
                    CommandNames.InputIdCommand, 
                    CommandNames.ChangePurseCommand, 
                    CommandNames.InputNameCommand, 
                    CommandNames.InputEmailCommand,
                    CommandNames.GetPrivateCommand),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}