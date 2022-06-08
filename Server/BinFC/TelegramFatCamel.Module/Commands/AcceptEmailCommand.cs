using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class AcceptEmailCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public AcceptEmailCommand(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.AcceptEmailCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            string userEmail = update.Message.Text;
            string existedUserEmail = null;

            if (string.IsNullOrEmpty(userEmail))
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.InputEmailEmpty, CommandNames.InputEmailCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            var userInfo = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id);

            existedUserEmail = userInfo.UserEmail;
            userInfo.UserEmail = userEmail;

            (bool isSuccessSave, string saveMessage) = await _userInfoRepository.SaveChangesAsync();

            if (!isSuccessSave)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.ErrorTextToSendAdmin,
                    replyMarkup: new ReplyKeyboardRemove());

                return;
            }

            if (string.IsNullOrEmpty(existedUserEmail))
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.AcceptEmail,
                    replyMarkup: new ReplyKeyboardRemove());
            }
            else
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.AcceptEmailWithExisted, existedUserEmail),
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
    }
}