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
    public class AcceptPurseCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public AcceptPurseCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.AcceptPurseCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            var userInfo = await _userInfoRepository.GetUserInfoByChatId(update.Message.Chat.Id);

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

            await _userInfoRepository.DataContextSaveChanges();

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.AcceptedPurse,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}