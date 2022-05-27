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
    public class AcceptPurseCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public AcceptPurseCommand(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.AcceptPurseCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var userInfo = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id);

            if ((string)param == CommandNames.InputBepCommand)
            {
                userInfo.BepAddress = update.Message.Text;
                userInfo.TrcAddress = null;
            }
            else if ((string)param == CommandNames.InputTrcCommand)
            {
                userInfo.BepAddress = null;
                userInfo.TrcAddress = update.Message.Text;
            }

            if (await _userInfoRepository.SaveChangesAsync() != null)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.ErrorTextToSendAdmin,
                    replyMarkup: new ReplyKeyboardRemove());

                return;
            }

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.AcceptedPurse,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}