﻿using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class InputIdCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public InputIdCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.InputIdCommand;

        public override async Task ExecuteAsync(Update update)
        {
            // TODO реализовать логику

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id, 
                "ИД принят.",
                ParseMode.Markdown);
        }
    }
}
