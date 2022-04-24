using Microsoft.Extensions.DependencyInjection;
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
        private readonly List<BaseCommand> _commands;
        private BaseCommand _lastCommand;

        public CommandExecutorService(IServiceProvider serviceProvider)
        {
            _commands = serviceProvider.GetServices<BaseCommand>().ToList();
            Console.WriteLine("Пиздарики");
        }

        public async Task Execute(Update update)
        {
            if (update?.Message?.Chat == null && update?.CallbackQuery == null)
                return;

            switch (_lastCommand?.Name)
            {
                case CommandNames.InputIdCommand:
                    {
                        await ExecuteCommand(CommandNames.SelectPurseCommand, update);
                        break;
                    }
                case CommandNames.InputTrcCommand:
                case CommandNames.InputBepCommand:
                    {
                        await ExecuteCommand(CommandNames.FinishInputsCommand, update);
                        break;
                    }
            }

            if (update.Type == UpdateType.Message)
            {
                switch (update.Message?.Text)
                {
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

            if (update.Message?.Text.Contains(CommandNames.StartCommand) == true)
            {
                await ExecuteCommand(CommandNames.StartCommand, update);
                return;
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
