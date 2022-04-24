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
        private readonly TelegramBotClient _client;
        public SelectPurseCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.SelectPurseCommand;

        public override async Task ExecuteAsync(Update update)
        {
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