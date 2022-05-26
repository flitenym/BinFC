using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramFatCamel.Module.Services.Interfaces
{
    public interface ICommandExecutorService
    {
        Task<string> ExecuteAsync(ITelegramBotClient client, string lastCommand, Update update);
    }
}
