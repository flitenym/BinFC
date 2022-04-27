using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class GetOperationsCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;

        public GetOperationsCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.GetOperationsCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Format(TelegramLoc.Operations, 
                    CommandNames.InputIdCommand, 
                    CommandNames.ChangePurseCommand, 
                    CommandNames.InputNameCommand, 
                    CommandNames.InputEmailCommand),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}