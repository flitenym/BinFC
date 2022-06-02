using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;

namespace TelegramFatCamel.Module.Commands
{
    public class InputIdAlreadyExistChatCommand : BaseCommand
    {
        public InputIdAlreadyExistChatCommand()
        {
        }

        public override string Name => CommandNames.InputIdAlreadyExistChat;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Format(TelegramLoc.ChatIdExist, CommandNames.InputIdCommand),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}