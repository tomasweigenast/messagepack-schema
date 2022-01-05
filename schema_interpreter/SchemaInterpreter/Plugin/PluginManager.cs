using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Plugin.Encoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin
{
    public static class PluginManager
    {
        private static readonly IDictionary<PluginEncoding, Func<ISchemaEncoder>> mEncoders = new Dictionary<PluginEncoding, Func<ISchemaEncoder>>()
        {
            { PluginEncoding.Json, () => new JsonSchemaEncoder() },
            { PluginEncoding.MessagePack, () => new MessagePackSchemaEncoder() }
        };

        /// <summary>
        /// Runs a plugin passing compiled data
        /// </summary>
        /// <param name="path">The path to the binary.</param>
        /// <param name="files">The list of files to serialize and send to the plugin.</param>
        public static async Task RunPluginAsync(string path, IEnumerable<SchemaFile> files, PluginEncoding encoding)
        {
            Logger.Debug("Creating process info to run the plugin.");

            // Create a process to call the plugin
            Process process = new();
            ProcessStartInfo psInfo = new()
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            };

            Logger.Debug($"Running plugin {Path.GetFileName(path)}.");

            // Setup a standard output listener
            process.OutputDataReceived += new DataReceivedEventHandler(PluginStdout.Listen);

            // Start the plugin
            process.StartInfo = psInfo;
            process.Start();
            process.BeginOutputReadLine();

            Logger.Debug("Getting stdin...");

            // Get stdin writer
            using StreamWriter writer = process.StandardInput;

            Logger.Debug("Serializing entries...");

            // Serialize entries
            var encoder = mEncoders[encoding]();
            var buffer = await encoder.Encode(files);

            Logger.Debug($"Buffer size: {buffer.Length}");
            Logger.Debug("Writing buffer to the plugin...");

            // Write to plugin
            await writer.BaseStream.WriteAsync(buffer);
            writer.Flush();
            writer.Close();

            Logger.Debug("Waiting until the plugin ends...");
            
            // Wait until the plugin finishes
            await process.WaitForExitAsync();

            Logger.Debug("Plugin ended.");
        }
    }
}