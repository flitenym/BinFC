using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class ChangePurseCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public ChangePurseCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.ChangePurseCommand;

        public override async Task ExecuteAsync(Update update)
        {
            var userInfo = await _userInfoRepository.GetUserInfoByChatId(update.Message.Chat.Id);

            userInfo.TrcAddress = null;
            userInfo.BepAddress = null;

            await _userInfoRepository.DataContextSaveChanges();

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton("TRC-20"){CallbackData = CommandNames.InputTrcCommand},
                    new InlineKeyboardButton("BEP-20"){CallbackData = CommandNames.InputBepCommand},
                }
            });

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                "Выберите кошелек.",
                ParseMode.Markdown,
                replyMarkup: inlineKeyboard);
        }
    }
}