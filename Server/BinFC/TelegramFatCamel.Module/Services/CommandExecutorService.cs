using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Storage.Module;
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

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case CommandNames.StartCommand:
                        await ExecuteCommand(CommandNames.StartCommand, update);
                        return;
                    case CommandNames.GetOperationsCommand:
                        await ExecuteCommand(CommandNames.GetOperationsCommand, update);
                        return;
                    case CommandNames.InputIdCommand:
                        await ExecuteCommand(CommandNames.InputIdCommand, update);
                        return;
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains(CommandNames.InputTrcCommand))
                {
                    await ExecuteCommand(CommandNames.InputTrcCommand, update);
                    return;
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.InputBepCommand))
                {
                    await ExecuteCommand(CommandNames.InputBepCommand, update);
                    return;
                }
            }

            switch (_lastCommand?.Name)
            {
                case CommandNames.InputIdCommand:
                    {
                        string inputedId = update.Message?.Text.Trim();
                        if (!string.IsNullOrEmpty(inputedId) && long.TryParse(inputedId, out var inputedIdLong))
                        {
                            //введен ИД корректно, проверим его в БД
                            var existedUserId = await _userInfoRepository.GetUserInfoByUserId(inputedIdLong);

                            if (existedUserId == null)
                            {
                                // выведем сообщение, что "Данный id не зарегистрирован в базе, попробуйте снова"
                                await ExecuteCommand(CommandNames.NotExistIdCommand, update);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(existedUserId.TrcAddress ?? existedUserId.BepAddress))
                                {
                                    // т.к. кошельки пустые, можем перейти на установку кошелька
                                    await ExecuteCommand(CommandNames.SelectPurseCommand, update);
                                }
                                else
                                {
                                    // т.к. один из кошельков не пустой, тогда выполним команду смены кошелька
                                    await ExecuteCommand(CommandNames.ChangePurseCommand, update);
                                }
                            }
                        }
                        else
                        {
                            // выведем сообщение, что "Неверно введен Id."
                            await ExecuteCommand(CommandNames.ErrorInputIdCommand, update);
                        }

                        break;
                    }
                case CommandNames.InputTrcCommand:
                case CommandNames.InputBepCommand:
                    {
                        await ExecuteCommand(CommandNames.FinishInputsCommand, update);
                        break;
                    }
            }

            // по дефолту просто пропишем операции.
            await ExecuteCommand(CommandNames.GetOperationsCommand, update);
        }

        private async Task ExecuteCommand(string commandName, Update update)
        {
            _lastCommand = _commands.First(x => x.Name == commandName);
            await _lastCommand.ExecuteAsync(update);
        }
    }
}