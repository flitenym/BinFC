using System.Threading.Tasks;

namespace TelegramFatCamel.Module.Services.Interfaces
{
    public interface ITelegramFatCamelBotService
    {
        public Task FatCamelBotStartAsync();
        public Task FatCamelBotStopAsync();
    }
}
