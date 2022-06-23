using System.Threading.Tasks;
using Telegram.Bot;

namespace Telegram.Module.Services.Interfaces
{
    public interface ITelegramBotService
    {
        public Task<TelegramBotClient> GetTelegramBotAsync(bool isNeedHandlers = true);
        public Task StopTelegramBotAsync();
    }
}
