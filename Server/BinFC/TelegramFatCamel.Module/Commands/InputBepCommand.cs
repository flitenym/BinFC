using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class InputBepCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public InputBepCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.InputBepCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            await _client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id,
                TelegramLoc.InputBep);
        }
    }
}