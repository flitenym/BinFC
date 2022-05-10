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
    public class ErrorInputIdCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public ErrorInputIdCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.ErrorInputIdCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.IncorrectId,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}