using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly List<BaseCommand> _commands;
        private BaseCommand _lastCommand;

        public CommandExecutorService(IUserInfoRepository userInfoRepository, IServiceProvider serviceProvider)
        {
            _userInfoRepository = userInfoRepository;
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
        }

        public async Task ExecuteAsync(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case CommandNames.StartCommand:
                        await ExecuteCommandAsync(CommandNames.StartCommand, update);
                        return;
                    case CommandNames.GetOperationsCommand:
                        await ExecuteCommandAsync(CommandNames.GetOperationsCommand, update);
                        return;
                    case CommandNames.ChangePurseCommand:
                        await ExecuteCommandAsync(CommandNames.ChangePurseCommand, update);
                        return;
                    case CommandNames.InputIdCommand:
                        await ExecuteCommandAsync(CommandNames.InputIdCommand, update);
                        return;
                    case CommandNames.InputNameCommand:
                        await ExecuteCommandAsync(CommandNames.InputNameCommand, update);
                        return;
                    case CommandNames.InputEmailCommand:
                        await ExecuteCommandAsync(CommandNames.InputEmailCommand, update);
                        return;
                    case CommandNames.GetPrivateCommand:
                        await ExecuteCommandAsync(CommandNames.GetPrivateCommand, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains(CommandNames.InputTrcCommand))
                {
                    await ExecuteCommandAsync(CommandNames.InputTrcCommand, update);
                    return;
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.InputBepCommand))
                {
                    await ExecuteCommandAsync(CommandNames.InputBepCommand, update);
                    return;
                }
            }

            switch (_lastCommand?.Name)
            {
                case CommandNames.InputNameCommand:
                    {
                        await ExecuteCommandAsync(CommandNames.AcceptNameCommand, update);
                        return;
                    }
                case CommandNames.InputEmailCommand:
                    {
                        await ExecuteCommandAsync(CommandNames.AcceptEmailCommand, update);
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
                                // выведем сообщение, что "Данный id не зарегистрирован в базе, попробуйте снова"
                                await ExecuteCommandAsync(CommandNames.NotExistIdCommand, update);
                                return;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(existedUserId.TrcAddress ?? existedUserId.BepAddress))
                                {
                                    // т.к. кошельки пустые, можем перейти на установку кошелька
                                    await ExecuteCommandAsync(CommandNames.SelectPurseCommand, update);
                                    return;
                                }
                                else
                                {
                                    // т.к. один из кошельков не пустой, тогда выполним команду смены кошелька
                                    await ExecuteCommandAsync(CommandNames.ChangePurseCommand, update);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            // выведем сообщение, что "Неверно введен Id."
                            await ExecuteCommandAsync(CommandNames.ErrorInputIdCommand, update);
                            return;
                        }
                    }
                case CommandNames.InputTrcCommand:
                case CommandNames.InputBepCommand:
                    {
                        await ExecuteCommandAsync(CommandNames.AcceptPurseCommand, update, _lastCommand.Name);
                        return;
                    }
            }

            // по дефолту просто пропишем операции.
            await ExecuteCommandAsync(CommandNames.GetOperationsCommand, update);
        }

        private async Task ExecuteCommandAsync(string commandName, Update update, dynamic param = null)
        {
            _lastCommand = _commands.First(x => x.Name == commandName);
            await _lastCommand.ExecuteAsync(update, param);
        }
    }
}