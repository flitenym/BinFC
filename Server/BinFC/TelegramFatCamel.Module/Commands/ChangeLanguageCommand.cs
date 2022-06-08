using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class ChangeLanguageCommand : BaseCommand
    {
        public ChangeLanguageCommand()
        {
        }

        public override string Name => CommandNames.ChangeLanguageCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton(TelegramLoc.RussianLanguage){CallbackData = CommandNames.RussianLanguageCommand},
                    new InlineKeyboardButton(TelegramLoc.EnglishLanguage){CallbackData = CommandNames.EnglishLanguageCommand},
                }
            });

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.ChooseLanguage,
                replyMarkup: inlineKeyboard);
        }
    }
}