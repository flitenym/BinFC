namespace TelegramFatCamel.Module.Commands.CommandSettings
{
    public static class CommandMessages
    {
        public const string InputEmailEmpty = $"E-mail пустой. Повторите снова, указав команду {CommandNames.InputEmailCommand}";
        public const string AcceptEmail = "Ваш указанный e-mail принят.";
        public const string AcceptEmailWithExisted = "Ваш указанный e-mail принят. E-mail был указан: {0}";

        public const string InputNameEmpty = $"ФИ пустое. Повторите снова, указав команду {CommandNames.InputNameCommand}";
        public const string AcceptName = "Ваше указанное ФИ принято.";
        public const string AcceptNameWithExisted = "Ваше указанное ФИ принято. ФИ было указано: {0}";

        public const string AcceptedPurse = "Кошелек принят. Администратор проверит данные и вам придет сообщение.";

        public const string ChoosePurse = "Выберите кошелек.";

        public const string IdUnspecified = $"Id не указан, привяжите сперва Id. Указать Id можно написав команду {CommandNames.InputIdCommand}";

        public const string IncorrectId = "Неверно введен Id. Попробуйте снова.";

        public const string Operations = $"Доступные команды:\r\n" +
                $"Указание Id: {CommandNames.InputIdCommand}\r\n" +
                $"Изменение кошелька: {CommandNames.ChangePurseCommand}\r\n" +
                $"Указание ФИ: {CommandNames.InputNameCommand}\r\n" +
                $"Указание e-mail: {CommandNames.InputEmailCommand}";

        public const string InputBep = "Введите BEP кошелек.";

        public const string InputTrc = "Введите TRC кошелек.";

        public const string InputEmail = "Введите e-mail.";

        public const string InputId = "Введите Id.";

        public const string InputName = "Введите ФИ.";

        public const string IdNotRegister = $"Данный Id не зарегистрирован в базе. Попробуйте снова указав команду {CommandNames.InputIdCommand}";

        public const string StartCommand = $"Добро пожаловать! Я буду помогать в регистрации Fat Camel!\r\n" +
                $"Для получения списка комманд напишите \"{CommandNames.GetOperationsCommand}\" или нажмите на кнопку.";
    }
}