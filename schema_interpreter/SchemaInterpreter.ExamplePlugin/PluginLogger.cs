using System.Text;
using System.Text.Json;

namespace SchemaInterpreter.ExamplePlugin
{
    public static class PluginLogger
    {
        public static void LogError(string message)
        {
            Log(3, message);
        }

        public static void LogDebug(string message)
        {
            Log(0, message);
        }

        public static void LogInfo(string message)
        {
            Log(1, message);
        }

        /// <summary>
        /// Writes a log event to the standard output
        /// </summary>
        /// <param name="logLevel">The level of the log.
        /// 0 stands for DEBUG.
        /// 1 stands for INFO.
        /// 2 stands for WARNING.
        /// 3 stands for ERROR.
        /// </param>
        /// <param name="message">The message to log</param>
        private static void Log(int logLevel, string message)
        {
            Console.WriteLine("Logging");

            var map = new Dictionary<string, object>
            {
                { "level", logLevel },
                { "message", message }
            };

            string json = JsonSerializer.Serialize(map);

            var output = Console.Out;
            output.WriteLine(json);
        }
    }
}