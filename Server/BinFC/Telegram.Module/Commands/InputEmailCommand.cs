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
    public class InputEmailCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public InputEmailCommand(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.InputEmailCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var existedUser = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id, false);

            if (existedUser == null)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.IdUnspecified, CommandNames.InputIdCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.InputEmail,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}