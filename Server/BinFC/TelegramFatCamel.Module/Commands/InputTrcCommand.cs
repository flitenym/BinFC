using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class InputTrcCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public InputTrcCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.InputTrcCommand;

        public override async Task ExecuteAsync(Update update)
        {
            // TODO реализовать логику

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id, 
                "TRC принят.",
                ParseMode.Markdown);
        }
    }
}
