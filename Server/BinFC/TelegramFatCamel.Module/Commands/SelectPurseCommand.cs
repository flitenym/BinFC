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
    public class SelectPurseCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public SelectPurseCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.SelectPurseCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            var userInfo = await _userInfoRepository.GetUserInfoByUserId(long.Parse(update.Message.Text.Trim()));

            if (!userInfo.ChatId.HasValue)
            {
                userInfo.ChatId = update.Message.Chat.Id;
            }

            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                userInfo.UserName = string.Join(' ', update.Message.Chat.FirstName, update.Message.Chat.LastName);
            }

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
                CommandMessages.ChoosePurse,
                replyMarkup: inlineKeyboard);
        }
    }
}