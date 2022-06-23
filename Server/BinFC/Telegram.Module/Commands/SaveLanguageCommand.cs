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
    public class SaveLanguageCommand : BaseCommand
    {
        private readonly ITelegramUserInfoRepository _telegramUserInfoRepository;
        public SaveLanguageCommand(ITelegramUserInfoRepository telegramUserInfoRepository)
        {
            _telegramUserInfoRepository = telegramUserInfoRepository;
        }

        public override string Name => CommandNames.SaveLanguageCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var telegramUserInfo = await _telegramUserInfoRepository.GetByChatIdAsync(update.CallbackQuery.Message.Chat.Id);

            if (param != null)
            {
                telegramUserInfo.Language = param.ToString();
            }

            await _telegramUserInfoRepository.UpdateAsync(telegramUserInfo);

            await client.SendTextMessageAsync(
                update.CallbackQuery.Message.Chat.Id,
                TelegramLoc.EndChooseLanguage,
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}