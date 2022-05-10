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
            var userInfo = await _userInfoRepository.GetUserInfoByUserIdAsync(long.Parse(update.Message.Text.Trim()));

            if (!userInfo.ChatId.HasValue)
            {
                userInfo.ChatId = update.Message.Chat.Id;
            }

            if (string.IsNullOrEmpty(userInfo.UserName))
            {
                userInfo.UserName = string.Join(' ', update.Message.Chat.FirstName, update.Message.Chat.LastName);
            }

            if (await _userInfoRepository.SaveChangesAsync() != null)
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.ErrorTextToSendAdmin,
                    replyMarkup: new ReplyKeyboardRemove());

                return;
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton(TelegramLoc.TrcButton){CallbackData = CommandNames.InputTrcCommand},
                    new InlineKeyboardButton(TelegramLoc.BepButton){CallbackData = CommandNames.InputBepCommand},
                }
            });

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.ChoosePurse,
                replyMarkup: inlineKeyboard);
        }
    }
}