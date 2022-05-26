using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Storage.Module.Repositories
{
    public class TelegramUserInfoRepository : ITelegramUserInfoRepository
    {
        private readonly DataContext _dataContext;
        private readonly IBaseRepository _baseRepository;
        private readonly ILogger<TelegramUserInfoRepository> _logger;
        public TelegramUserInfoRepository(
            DataContext dataContext,
            IBaseRepository baseRepository,
            ILogger<TelegramUserInfoRepository> logger)
        {
            _dataContext = dataContext;
            _baseRepository = baseRepository;
            _logger = logger;
        }

        public async Task<TelegramUserInfo> GetByChatIdAsync(long? chatId)
        {
            if (chatId == null)
            {
                return null;
            }

            return await _dataContext
                .TelegramUsersInfo
                .AsNoTracking()
                .Where(x => x.ChatId == chatId)
                .FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(TelegramUserInfo obj)
        {
            _dataContext.TelegramUsersInfo.Add(obj);

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(long? chatId, string lastCommand)
        {
            if (chatId.HasValue)
            {
                TelegramUserInfo telegramUserInfo = await _dataContext
                    .TelegramUsersInfo
                    .Where(x => x.ChatId == chatId)
                    .FirstOrDefaultAsync();

                if (telegramUserInfo == null)
                {
                    telegramUserInfo = new TelegramUserInfo();
                    telegramUserInfo.ChatId = chatId;
                    telegramUserInfo.LastCommand = lastCommand;
                    _dataContext.TelegramUsersInfo.Add(telegramUserInfo);
                }

                telegramUserInfo.LastCommand = lastCommand;
                _dataContext.TelegramUsersInfo.Update(telegramUserInfo);
            }
            else
            {
                TelegramUserInfo telegramUserInfo = new TelegramUserInfo();
                telegramUserInfo.ChatId = chatId;
                telegramUserInfo.LastCommand = lastCommand;
                _dataContext.TelegramUsersInfo.Add(telegramUserInfo);
            }

            return await _baseRepository.SaveChangesAsync();
        }

        public async Task<string> SaveChangesAsync()
        {
            return await _baseRepository.SaveChangesAsync();
        }
    }
}