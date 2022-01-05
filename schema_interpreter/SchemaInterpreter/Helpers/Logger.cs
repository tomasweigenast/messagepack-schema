using System;

namespace SchemaInterpreter.Helpers
{
    public static class Logger
    {
        private static LogLevel mMinimumLogLevel = LogLevel.Info;

        public static void Info(string message, string prefix = null) => Log(message, LogLevel.Info, prefix);
        public static void Debug(string message, string prefix = null) => Log(message, LogLevel.Debug, prefix);
        public static void Warning(string message, string prefix = null) => Log(message, LogLevel.Warning, prefix);
        public static void Error(string message, string prefix = null) => Log(message, LogLevel.Error, prefix);

        public static void SetMinimumLogLevel(LogLevel level)
        {
            mMinimumLogLevel = level;
        }

        private static void Log(string message, LogLevel level, string prefix)
        {
            if (level < mMinimumLogLevel)
                return;

            ConsoleColor logColor = level.GetColor();
            Console.ForegroundColor = logColor;
            Console.Write($"[{level.GetName()}] ");

            if(prefix != null)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write($"[{prefix}] ");
                Console.ForegroundColor = logColor;
            }

            Console.WriteLine(message);
            Console.ResetColor();
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
                LogLevel.Debug => ConsoleColor.DarkGray,
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