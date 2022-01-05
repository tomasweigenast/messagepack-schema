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
            // Create a process to call the plugin
            Process process = new();
            ProcessStartInfo psInfo = new()
            {
                FileName = path,
                UseShellExecute = false,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
            };

            process.StartInfo = psInfo;

            // Get stdin writer
            StreamWriter writer = process.StandardInput;

            // Serialize entries
            var encoder = mEncoders[encoding]();
            var buffer = await encoder.Encode(files);

            // Write to plugin
            await writer.BaseStream.WriteAsync(buffer);

            // Start the plugin
            process.Start();

            // Wait until the plugin finishes
            await process.WaitForExitAsync();
        }
    }
}