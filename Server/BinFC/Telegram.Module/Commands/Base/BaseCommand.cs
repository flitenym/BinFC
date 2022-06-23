using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Module.Commands.Base
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null);
    }
}
