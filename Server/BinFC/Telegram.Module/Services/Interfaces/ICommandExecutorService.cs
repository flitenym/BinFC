using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Module.Services.Interfaces
{
    public interface ICommandExecutorService
    {
        Task ExecuteAsync(ITelegramBotClient client, Update update);
    }
}
