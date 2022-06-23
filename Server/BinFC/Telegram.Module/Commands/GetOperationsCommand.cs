using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
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
                    CommandNames.GetPrivateCommand,
                    CommandNames.ChangeLanguageCommand),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}