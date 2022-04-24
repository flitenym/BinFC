using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramFatCamel.Module.Commands.Base
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract Task ExecuteAsync(Update update);
    }
}
