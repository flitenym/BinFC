using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class AcceptEmailCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public AcceptEmailCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.AcceptEmailCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            string userEmail = update.Message.Text;
            string existedUserEmail = null;

            if (string.IsNullOrEmpty(userEmail))
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.InputEmailEmpty, CommandNames.InputEmailCommand));
                return;
            }

            var userInfo = await _userInfoRepository.GetUserInfoByChatId(update.Message.Chat.Id);

            existedUserEmail = userInfo.UserEmail;
            userInfo.UserEmail = userEmail;

            await _userInfoRepository.DataContextSaveChanges();

            if (string.IsNullOrEmpty(existedUserEmail))
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.AcceptEmail);
            }
            else
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.AcceptEmailWithExisted, existedUserEmail));
            }
        }
    }
}