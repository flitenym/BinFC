using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
{
    public class NotExistIdCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public NotExistIdCommand(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.NotExistIdCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            UserInfo newUserInfo = new UserInfo()
            {
                ChatId = update.Message.Chat.Id,
                UserId = (long)param,
                UserName = string.Join(' ', update.Message.Chat.FirstName, update.Message.Chat.LastName),
                UserNickName = update.Message.Chat.Username
            };

            await _userInfoRepository.CreateAsync(newUserInfo);

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.IdNotRegister,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}