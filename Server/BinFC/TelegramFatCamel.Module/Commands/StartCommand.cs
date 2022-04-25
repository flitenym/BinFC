using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramFatCamel.Module.Commands.Base;
using TelegramFatCamel.Module.Commands.CommandSettings;
using TelegramFatCamel.Module.Services.Interfaces;

namespace TelegramFatCamel.Module.Commands
{
    public class StartCommand : BaseCommand
    {
        private readonly TelegramBotClient _client;
        public StartCommand(ITelegramFatCamelBotService telegramFatCamelBotService)
        {
            _client = telegramFatCamelBotService.GetTelegramBotAsync().Result;
        }

        public override string Name => CommandNames.StartCommand;

        public override async Task ExecuteAsync(Update update)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton(CommandNames.GetOperationsCommand)
                })
            {
                ResizeKeyboard = true
            };

            await _client.SendTextMessageAsync(
                update.Message.Chat.Id,
                $"Добро пожаловать! Я буду помогать в регистрации Fat Camel!{Environment.NewLine}" +
                $"Для получения списка комманд напишите \"{CommandNames.GetOperationsCommand}\" или нажмите на кнопку.",
                ParseMode.Markdown, 
                replyMarkup: replyKeyboardMarkup);
        }
    }
}
