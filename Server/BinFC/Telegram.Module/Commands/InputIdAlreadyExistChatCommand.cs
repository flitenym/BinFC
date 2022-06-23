using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
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