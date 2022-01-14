using MessagePack;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Plugin.Encoder;
using SchemaInterpreter.Plugin.SchemaEncoder;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin
{
    /// <summary>
    /// Listens for stdout writes and interprets them as log events
    /// </summary>
    public static class PluginStdout
    {
        private static readonly TaskCompletionSource<PluginOutput> mOutputCompleter = new();

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public static string PluginName { get; set; }

        /// <summary>
        /// The encoding used to send and receive data to/from plugins.
        /// </summary>
        public static PluginEncoding Encoding { get; set; }

        public static void OnData(string data)
        {
            if (data == null)
                return;

            Logger.Debug($"Receiving data from plugin: {data}");

            LogEvent logEvent = GetLogEvent(data);
            if(logEvent == null)
            {
                if (data.Contains("plugin:"))
                    PluginName = data.Split(':')[1];
                else
                {
                    PluginOutput output;
                    if (Encoding == PluginEncoding.Json)
                        output = JsonSerializer.Deserialize<PluginOutput>(data);
                    else
                    {
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
                        output = MessagePackSerializer.Deserialize<PluginOutput>(buffer);
                    }

                    mOutputCompleter.SetResult(output);
                }

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

        public static Task<PluginOutput> WaitForOutputAsync()
            => mOutputCompleter.Task;

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