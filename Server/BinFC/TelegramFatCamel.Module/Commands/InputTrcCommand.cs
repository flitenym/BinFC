using Storage.Module;
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
        private readonly DataContext _dataContext;
        private readonly TelegramBotClient _client;
        public InputTrcCommand(ITelegramFatCamelBotService telegramFatCamelBotService, DataContext dataContext)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _dataContext = dataContext;
        }

        public override string Name => CommandNames.InputTrcCommand;

        public override async Task ExecuteAsync(Update update)
        {
            // TODO реализовать логику

            await _client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id, 
                "TRC принят.",
                ParseMode.Markdown);
        }
    }
}
