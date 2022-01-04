using System;

namespace SchemaInterpreter.Helpers
{
    public static class Logger
    {
        public static void Info(string message) => Log(message, LogLevel.Info);
        public static void Debug(string message) => Log(message, LogLevel.Debug);
        public static void Warning(string message) => Log(message, LogLevel.Warning);
        public static void Error(string message) => Log(message, LogLevel.Error);

        private static void Log(string message, LogLevel level)
        {
            Console.ForegroundColor = level.GetColor();
            Console.Write($"[{level.GetName()}]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }

        private static string GetName(this LogLevel level)
            => level switch
            {
                LogLevel.Debug => "DEBUG",
                LogLevel.Info => "INFO",
                LogLevel.Warning => "WARNING",
                LogLevel.Error => "ERROR",
                _ => throw new FormatException("Log level not found."),
            };

        private static ConsoleColor GetColor(this LogLevel level)
            => level switch
            {
                LogLevel.Debug => ConsoleColor.Gray,
                LogLevel.Info => ConsoleColor.DarkGreen,
                LogLevel.Warning => ConsoleColor.DarkYellow,
                LogLevel.Error => ConsoleColor.DarkRed,
                _ => throw new FormatException("Log level not found."),
            };
    }

    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }
}