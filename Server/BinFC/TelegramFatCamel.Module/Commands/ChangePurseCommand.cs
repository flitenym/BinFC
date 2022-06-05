using Storage.Module.Repositories.Interfaces;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Localization;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class ChangePurseCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        public ChangePurseCommand(IUserInfoRepository userInfoRepository)
        {
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.ChangePurseCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var userInfo = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id);

            if (userInfo == null)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.IdUnspecified, CommandNames.InputIdCommand));
                return;
            }

            if (await _userInfoRepository.SaveChangesAsync() != null)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    TelegramLoc.ErrorTextToSendAdmin,
                    replyMarkup: new ReplyKeyboardRemove());

                return;
            }

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    new InlineKeyboardButton(TelegramLoc.TrcButton){CallbackData = CommandNames.InputTrcCommand},
                    new InlineKeyboardButton(TelegramLoc.BepButton){CallbackData = CommandNames.InputBepCommand},
                }
            });

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                TelegramLoc.ChoosePurse,
                replyMarkup: inlineKeyboard);
        }
    }
}