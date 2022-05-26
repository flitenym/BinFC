using Microsoft.Extensions.DependencyInjection;
using Storage.Module.Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
        private readonly List<BaseCommand> _commands;

        public CommandExecutorService(IUserInfoRepository userInfoRepository, IServiceProvider serviceProvider)
        {
            _userInfoRepository = userInfoRepository;
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
        }

        public async Task<string> ExecuteAsync(ITelegramBotClient client, string lastCommand, Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return null;

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
                    case CommandNames.StartCommand:
                        return await ExecuteCommandAsync(client, CommandNames.StartCommand, update);
                    case CommandNames.GetOperationsCommand:
                        return await ExecuteCommandAsync(client, CommandNames.GetOperationsCommand, update);
                    case CommandNames.ChangePurseCommand:
                        return await ExecuteCommandAsync(client, CommandNames.ChangePurseCommand, update);
                    case CommandNames.InputIdCommand:
                        return await ExecuteCommandAsync(client, CommandNames.InputIdCommand, update);
                    case CommandNames.InputNameCommand:
                        return await ExecuteCommandAsync(client, CommandNames.InputNameCommand, update);
                    case CommandNames.InputEmailCommand:
                        return await ExecuteCommandAsync(client, CommandNames.InputEmailCommand, update);
                    case CommandNames.GetPrivateCommand:
                        return await ExecuteCommandAsync(client, CommandNames.GetPrivateCommand, update);
                }
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                if (update.CallbackQuery.Data.Contains(CommandNames.InputTrcCommand))
                {
                    return await ExecuteCommandAsync(client, CommandNames.InputTrcCommand, update);
                }
                else if (update.CallbackQuery.Data.Contains(CommandNames.InputBepCommand))
                {
                    return await ExecuteCommandAsync(client, CommandNames.InputBepCommand, update);
                }
            }

            switch (lastCommand)
            {
                case CommandNames.InputNameCommand:
                    {
                        return await ExecuteCommandAsync(client, CommandNames.AcceptNameCommand, update);
                    }
                case CommandNames.InputEmailCommand:
                    {
                        return await ExecuteCommandAsync(client, CommandNames.AcceptEmailCommand, update);
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
                                return await ExecuteCommandAsync(client, CommandNames.NotExistIdCommand, update);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(existedUserId.TrcAddress ?? existedUserId.BepAddress))
                                {
                                    // т.к. кошельки пустые, можем перейти на установку кошелька
                                    return await ExecuteCommandAsync(client, CommandNames.SelectPurseCommand, update);
                                }
                                else
                                {
                                    // т.к. один из кошельков не пустой, тогда выполним команду смены кошелька
                                    return await ExecuteCommandAsync(client, CommandNames.ChangePurseCommand, update);
                                }
                            }
                        }
                        else
                        {
                            // выведем сообщение, что "Неверно введен Id."
                            return await ExecuteCommandAsync(client, CommandNames.ErrorInputIdCommand, update);
                        }
                    }
                case CommandNames.InputTrcCommand:
                case CommandNames.InputBepCommand:
                    {
                        return await ExecuteCommandAsync(client, CommandNames.AcceptPurseCommand, update, lastCommand);
                    }
            }

            // по дефолту просто пропишем операции.
            return await ExecuteCommandAsync(client, CommandNames.GetOperationsCommand, update);
        }

        private async Task<string> ExecuteCommandAsync(ITelegramBotClient client, string commandName, Update update, dynamic param = null)
        {
            var lastCommandService = _commands.First(x => x.Name == commandName);
            await lastCommandService.ExecuteAsync(client, update, param);
            return lastCommandService.Name;
        }
    }
}