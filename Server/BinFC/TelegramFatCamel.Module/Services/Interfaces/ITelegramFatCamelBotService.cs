using System.Threading.Tasks;
using Telegram.Bot;

namespace TelegramFatCamel.Module.Services.Interfaces
{
    public interface ITelegramFatCamelBotService
    {
        public Task<TelegramBotClient> GetTelegramBotAsync(bool isNeedHandlers = true);
        public Task StopTelegramBotAsync();
    }
}
