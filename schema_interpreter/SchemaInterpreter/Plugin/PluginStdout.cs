using SchemaInterpreter.Helpers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SchemaInterpreter.Plugin
{
    /// <summary>
    /// Listens for stdout writes and interprets them as log events
    /// </summary>
    public static class PluginStdout
    {
        /// <summary>
        /// The name of the plugin
        /// </summary>
        public static string PluginName { get; set; }

        public static void Listen(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            LogEvent logEvent = GetLogEvent(e.Data);
            if(logEvent == null)
            {
                if (e.Data.Contains("plugin:"))
                    PluginName = e.Data.Split(':')[1];

                return;
            }

            switch (logEvent.Level)
            {
                case LogEvent.LogEventLevel.Error:
                    Logger.Error(logEvent.Message, prefix: PluginName);
                    break;

                case LogEvent.LogEventLevel.Warning:
                    Logger.Warning(logEvent.Message, prefix: PluginName);
                    break;

                case LogEvent.LogEventLevel.Info:
                    Logger.Info(logEvent.Message, prefix: PluginName);
                    break;

                case LogEvent.LogEventLevel.Debug:
                    Logger.Debug(logEvent.Message, prefix: PluginName);
                    break;

                default:
                    throw new Exception($"Unknown log event '{logEvent}'");
            }
        }

        private static LogEvent GetLogEvent(string input)
        {
            try
            {
                return JsonSerializer.Deserialize<LogEvent>(input);
            }
            catch
            {
                return null;
            }
        }

        private class LogEvent
        {
            [JsonPropertyName("level")]
            public LogEventLevel Level { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            public enum LogEventLevel
            {
                Debug,
                Info,
                Warning,
                Error
            }
        }
    }
}