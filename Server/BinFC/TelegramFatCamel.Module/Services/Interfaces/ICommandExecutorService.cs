using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramFatCamel.Module.Services.Interfaces
{
    public interface ICommandExecutorService
    {
        Task ExecuteAsync(Update update);
    }
}
