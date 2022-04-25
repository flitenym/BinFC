using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class NotExistIdCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public NotExistIdCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.NotExistIdCommand;

        public override async Task ExecuteAsync(Update update)
        {
            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "Данный Id не зарегистрирован в базе. Попробуйте снова.",
                ParseMode.Markdown);
        }
    }
}