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
    public class AcceptNameCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public AcceptNameCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.AcceptNameCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            string userName = update.Message.Text;
            string existedUserName = null;

            if (string.IsNullOrEmpty(userName))
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.InputNameEmpty, CommandNames.InputNameCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            var userInfo = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id);

            existedUserName = userInfo.UserName;
            userInfo.UserName = userName;

            if (await _userInfoRepository.SaveChangesAsync() != null)
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.ErrorTextToSendAdmin,
                    replyMarkup: new ReplyKeyboardRemove());

                return;
            }

            if (string.IsNullOrEmpty(existedUserName))
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.AcceptName,
                    replyMarkup: new ReplyKeyboardRemove());
            }
            else
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.AcceptNameWithExisted, existedUserName),
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
    }
}