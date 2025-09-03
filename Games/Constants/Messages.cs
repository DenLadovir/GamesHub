using MediatR;

namespace Games.Constants
{
    public static class Messages
    {
        //Files JSON & CSV
        public const string LogWarningString = "Игра с ID {0} не найдена";
        public const string FileNotLoaded = "Файл не загружен.";
        public const string JsonParseError = "Не удалось распарсить JSON.";
        public const string CsvParseError = "Не удалось распарсить CSV.";
        public const string GamesAlreadyExist = "Все игры из файла уже существуют в базе.";
        public const string JsonFormatError = "Ошибка формата JSON: ";
        public const string CsvFormatError = "Ошибка формата CSV: ";
        public const string GeneralError = "Общая ошибка: ";
        public const string GamesUploaded = "Загружено {0} игр.";
        public const string GenresUploaded = "Загружено {0} жанров.";

        //User management
        public const string LoginAndPassRequired = "Email и пароль обязательны";
        public const string UserCreated = "Пользователь {0} создан";
        public const string UserNotFound = "Пользователь не найден";
        public const string DeletingMessage = "User for deleting {0} {1}";
        public const string UserRolesUpdated = "Роли пользователя {0} обновлены";
        public const string PasswordChanged = "Пароль для {0} успешно изменён";
        public const string UserDeleted = "Пользователь {0} удалён";

        //Validation messages
        public const string ValueCannotBeNull = "Значение не может быть null.";
        public const string InvalidDataTypeStringExpected = "Некорректный тип данных. Ожидается строка.";
        public const string StringLengthRange = "Длина должна быть от {0} до {1} символов.";
    }
}
