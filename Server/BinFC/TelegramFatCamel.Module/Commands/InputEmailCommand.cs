using Storage.Module.Repositories.Interfaces;
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
    public class InputEmailCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public InputEmailCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.InputEmailCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            var existedUser = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id, false);

            if (existedUser == null)
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.IdUnspecified, CommandNames.InputIdCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.InputEmail,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}