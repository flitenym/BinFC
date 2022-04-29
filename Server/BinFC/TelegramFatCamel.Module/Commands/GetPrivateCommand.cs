﻿using Storage.Module.Repositories.Interfaces;
using System;
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
    public class GetPrivateCommand : BaseCommand
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly TelegramBotClient _client;
        public GetPrivateCommand(ITelegramFatCamelBotService telegramFatCamelBotService, IUserInfoRepository userInfoRepository)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
            _userInfoRepository = userInfoRepository;
        }

        public override string Name => CommandNames.GetPrivateCommand;

        public override async Task ExecuteAsync(Update update, dynamic param = null)
        {
            var existedUser = await _userInfoRepository.GetUserInfoByChatId(update.Message.Chat.Id, false);

            if (existedUser == null)
            {
                await _client.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    string.Format(TelegramLoc.IdUnspecified, CommandNames.InputIdCommand),
                    replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            string id = !existedUser.UserId.HasValue ? string.Empty : string.Format(TelegramLoc.PrivateId, existedUser.UserId);
            string name = string.IsNullOrEmpty(existedUser.UserName) ? string.Empty : string.Format(TelegramLoc.PrivateName, existedUser.UserName);
            string email = string.IsNullOrEmpty(existedUser.UserEmail) ? string.Empty : string.Format(TelegramLoc.PrivateEmail, existedUser.UserEmail);
            string purse = string.IsNullOrEmpty(existedUser.BepAddress) ?
                                string.IsNullOrEmpty(existedUser.TrcAddress) ? string.Empty : string.Format(TelegramLoc.PrivateTrc, existedUser.TrcAddress) :
                                string.Format(TelegramLoc.PrivateBep, existedUser.BepAddress);

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                string.Join(Environment.NewLine, id, name, email, purse),
                replyMarkup: new ReplyKeyboardRemove());
        }
    }
}