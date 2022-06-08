using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Entities;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly IUserInfoRepository _userInfoRepository;
        private readonly ITelegramUserInfoRepository _telegramUserInfoRepository;
        private readonly List<BaseCommand> _commands;

        public CommandExecutorService(
            IUserInfoRepository userInfoRepository,
            ITelegramUserInfoRepository telegramUserInfoRepository,
            IServiceProvider serviceProvider)
        {
            _userInfoRepository = userInfoRepository;
            _telegramUserInfoRepository = telegramUserInfoRepository;
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
        }

        public async Task ExecuteAsync(ITelegramBotClient client, Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            long? chatId = update.Message?.Chat?.Id ?? update?.CallbackQuery?.Message?.Chat?.Id;

            TelegramUserInfo telegramUserInfo =
                await _telegramUserInfoRepository.GetByChatIdAsync(chatId, isNeedTracking: false);

            string language = telegramUserInfo?.Language;

            if (telegramUserInfo == null)
            {
                var newTelegramUserInfo = new TelegramUserInfo();
                newTelegramUserInfo.ChatId = chatId;
                language = newTelegramUserInfo.Language;
                await _telegramUserInfoRepository.CreateAsync(newTelegramUserInfo);
            }

            var culture = new CultureInfo(language);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case CommandNames.StartCommand:
                        await ExecuteCommandAsync(client, CommandNames.StartCommand, update);
                        return;
                    case CommandNames.GetOperationsCommand:
                        await ExecuteCommandAsync(client, CommandNames.GetOperationsCommand, update);
                        return;
                    case CommandNames.ChangePurseCommand:
                        await ExecuteCommandAsync(client, CommandNames.ChangePurseCommand, update);
                        return;
                    case CommandNames.InputIdCommand:
                        {
                            var userInfo = await _userInfoRepository.GetUserInfoByChatIdAsync(update.Message.Chat.Id, false);

                            if (userInfo != null)
                            {
                                await ExecuteCommandAsync(client, CommandNames.AlreadyInputIdCommand, update);
                                return;
                            }
                            else
                            {
                                await ExecuteCommandAsync(client, CommandNames.InputIdCommand, update);
                                return;
                            }
                        }
                    case CommandNames.InputNameCommand:
                        await ExecuteCommandAsync(client, CommandNames.InputNameCommand, update);
                        return;
                    case CommandNames.InputEmailCommand:
                        await ExecuteCommandAsync(client, CommandNames.InputEmailCommand, update);
                        return;
                    case CommandNames.GetPrivateCommand:
                        await ExecuteCommandAsync(client, CommandNames.GetPrivateCommand, update);
                        return;
                    case CommandNames.ChangeLanguageCommand:
                        await ExecuteCommandAsync(client, CommandNames.ChangeLanguageCommand, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains(CommandNames.InputTrcCommand))
                {
                    await ExecuteCommandAsync(client, CommandNames.InputTrcCommand, update);
                    return;
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.InputBepCommand))
                {
                    await ExecuteCommandAsync(client, CommandNames.InputBepCommand, update);
                    return;
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.RussianLanguageCommand))
                {
                    await ExecuteCommandAsync(client, CommandNames.SaveLanguageCommand, update, CommandNames.RussianLanguageCommand);
                    return;
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.EnglishLanguageCommand))
                {
                    await ExecuteCommandAsync(client, CommandNames.SaveLanguageCommand, update, CommandNames.EnglishLanguageCommand);
                    return;
                }
            }

            switch (telegramUserInfo?.LastCommand)
            {
                case CommandNames.InputNameCommand:
                    {
                        await ExecuteCommandAsync(client, CommandNames.AcceptNameCommand, update);
                        return;
                    }
                case CommandNames.InputEmailCommand:
                    {
                        await ExecuteCommandAsync(client, CommandNames.AcceptEmailCommand, update);
                        return;
                    }
                case CommandNames.InputIdCommand:
                    {
                        string inputedId = update.Message?.Text.Trim();
                        if (!string.IsNullOrEmpty(inputedId) && long.TryParse(inputedId, out var inputedIdLong))
                        {
                            //введен ИД корректно, проверим его в БД
                            var existedUserId = await _userInfoRepository.GetUserInfoByUserIdAsync(inputedIdLong, false);

                            if (existedUserId == null)
                            {
                                await ExecuteCommandAsync(client, CommandNames.NotExistIdCommand, update, inputedIdLong);
                                return;
                            }
                            // данный ид уже занят
                            else if (existedUserId.ChatId.HasValue)
                            {
                                await ExecuteCommandAsync(client, CommandNames.InputIdAlreadyExistChat, update);
                                return;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(existedUserId.TrcAddress ?? existedUserId.BepAddress))
                                {
                                    // т.к. кошельки пустые, можем перейти на установку кошелька
                                    await ExecuteCommandAsync(client, CommandNames.SelectPurseCommand, update);
                                    return;
                                }
                                else
                                {
                                    // т.к. один из кошельков не пустой, тогда выполним команду смены кошелька
                                    await ExecuteCommandAsync(client, CommandNames.ChangePurseCommand, update);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // выведем сообщение, что "Неверно введен Id."
                            await ExecuteCommandAsync(client, CommandNames.ErrorInputIdCommand, update);
                            return;
                        }
                    }
                case CommandNames.InputTrcCommand:
                case CommandNames.InputBepCommand:
                    {
                        await ExecuteCommandAsync(client, CommandNames.AcceptPurseCommand, update, telegramUserInfo?.LastCommand);
                        return;
                    }
            }

            // по дефолту просто пропишем операции.
            await ExecuteCommandAsync(client, CommandNames.GetOperationsCommand, update);
        }

        private async Task ExecuteCommandAsync(ITelegramBotClient client, string commandName, Update update, dynamic param = null)
        {
            var lastCommandService = _commands.First(x => x.Name == commandName);
            await _telegramUserInfoRepository.UpdateAsync(update.Message?.Chat?.Id ?? update?.CallbackQuery?.Message?.Chat?.Id, lastCommandService.Name);
            await lastCommandService.ExecuteAsync(client, update, param);
        }
    }
}