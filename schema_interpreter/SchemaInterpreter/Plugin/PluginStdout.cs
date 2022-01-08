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

        public static void Listen(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;

            LogEvent logEvent = GetLogEvent(e.Data);
            if(logEvent == null)
            {
                if (e.Data.Contains("plugin:"))
                    PluginName = e.Data.Split(':')[1];
                else
                {
                    PluginOutput output;
                    if (Encoding == PluginEncoding.Json)
                        output = JsonSerializer.Deserialize<PluginOutput>(e.Data);
                    else
                    {
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(e.Data);
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