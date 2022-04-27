using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class InputNameCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public InputNameCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.InputNameCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            var existedUser = await _userInfoRepository.GetUserInfoByChatId(update.Message.Chat.Id);

            if (existedUser == null)
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    CommandMessages.IdUnspecified);
                return;
            }

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                CommandMessages.InputName);
        }
    }
}