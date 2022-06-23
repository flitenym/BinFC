using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Module.Commands.Base;
using Telegram.Module.Commands.CommandSettings;
using Telegram.Module.Localization;

namespace Telegram.Module.Commands
{
    public class GetPrivateCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ISpotScaleRepository _spotScaleRepository;
        private readonly IFuturesScaleRepository _futuresScaleRepository;
        public GetPrivateCommand(
            IUserInfoRepository userInfoRepository, 
            ISpotScaleRepository spotScaleRepository,
            IFuturesScaleRepository futuresScaleRepository)
        {
            _userInfoRepository = userInfoRepository;
            _spotScaleRepository = spotScaleRepository;
            _futuresScaleRepository = futuresScaleRepository;
        }

        public override string Name => CommandNames.GetPrivateCommand;

        public override async Task ExecuteAsync(ITelegramBotClient client, Update update, dynamic param = null)
        {
            var existedUser = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id, false);

            if (existedUser == null)
            {
                await client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.IdUnspecified, CommandNames.InputIdCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            var spots = _spotScaleRepository.GetByUnique(existedUser.UniqueId.Value);
            var futures = _futuresScaleRepository.GetByUnique(existedUser.UniqueId.Value);

            string spotPercentes = string.Join("; ", spots.Select(x => $"{x.Percent}% (>{x.FromValue}){Environment.NewLine}")).Trim();
            string futuresPercentes = string.Join("; ", futures.Select(x => $"{x.Percent}% (>{x.FromValue}){Environment.NewLine}")).Trim();

            List<string> privateInfo = new();

            privateInfo.Add(!existedUser.UserId.HasValue ? string.Empty : string.Format(TelegramLoc.PrivateId, existedUser.UserId)); // id
            privateInfo.Add(string.IsNullOrEmpty(existedUser.UserName) ? string.Empty : string.Format(TelegramLoc.PrivateName, existedUser.UserName)); // name
            privateInfo.Add(string.IsNullOrEmpty(existedUser.UserEmail) ? string.Empty : string.Format(TelegramLoc.PrivateEmail, existedUser.UserEmail)); // email
            privateInfo.Add(string.IsNullOrEmpty(existedUser.BepAddress) ?
                                string.IsNullOrEmpty(existedUser.TrcAddress) ? string.Empty : string.Format(TelegramLoc.PrivateTrc, existedUser.TrcAddress) :
                                string.Format(TelegramLoc.PrivateBep, existedUser.BepAddress)); //purse

            privateInfo.Add(string.Format(TelegramLoc.Spot, spotPercentes)); // spots percent
            privateInfo.Add(string.Format(TelegramLoc.Futures, futuresPercentes)); // futures percent

            privateInfo.Add(existedUser.IsApproved ? TelegramLoc.StatusApprove : TelegramLoc.StatusNotApprove); // status

            await client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Join(Environment.NewLine, privateInfo.Where(x => !string.IsNullOrEmpty(x))),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}