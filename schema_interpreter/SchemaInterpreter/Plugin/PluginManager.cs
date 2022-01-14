using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Plugin.Encoder;
using SchemaInterpreter.Plugin.SchemaEncoder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
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
        /// <param name="outputFolder">The path to the folder where to generate the files.</param>
        /// <param name="files">The list of files to serialize and send to the plugin.</param>
        public static async Task RunPluginAsync_(string path, string outputFolder, IEnumerable<SchemaPackage> files, PluginEncoding encoding)
        {
            Logger.Debug("Creating process info to run the plugin.");
            if (CommonHelpers.IsDirectory(outputFolder) != true)
                throw new InvalidOperationException("'output' must be a valid directory path.");

            Logger.Debug($"Generating source files on {outputFolder}.");

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
            PluginStdout.Encoding = encoding;
            //process.OutputDataReceived += new DataReceivedEventHandler(PluginStdout.Listen);

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
            var buffer = await encoder.Encode(encoding, files);

            Logger.Debug($"Buffer size: {buffer.Length}");

            // Write to plugin
            Logger.Debug("Writing buffer to the plugin...");
            await writer.BaseStream.WriteAsync(buffer);
            writer.Flush();
            writer.Close();

            Logger.Debug("Waiting until the plugin ends...");

            // Wait until the plugin finishes
            var outputFiles = await PluginStdout.WaitForOutputAsync();
            await WriteOutputFilesAsync(outputFiles);

            Logger.Debug("Plugin ended.");
        }

        /// <summary>
        /// Runs a plugin passing compiled data
        /// </summary>
        /// <param name="path">The path to the binary.</param>
        /// <param name="outputFolder">The path to the folder where to generate the files.</param>
        /// <param name="packages">The list of packages to serialize and send to the plugin.</param>
        public static async Task RunPluginAsync(string path, string outputFolder, IEnumerable<SchemaPackage> packages, PluginEncoding encoding)
        {
            Logger.Debug($"Running plugin {path}");
            Logger.Debug($"Working directory {Path.GetDirectoryName(path)}");
            
            if (CommonHelpers.IsDirectory(outputFolder) != true)
                throw new InvalidOperationException("'output' must be a valid directory path.");

            Logger.Debug("Serializing entries...");

            // Serialize entries
            var encoder = mEncoders[encoding]();
            var buffer = await encoder.Encode(encoding, packages);
            using var output = new MemoryStream();

            Logger.Debug($"Buffer size: {buffer.Length}");

            // Run plugin
            //var plugin = PipeSource.FromBytes(buffer.ToArray()) 
            //    | Cli.Wrap(path)
            //        .WithValidation(CommandResultValidation.ZeroExitCode)
            //        .WithArguments(new string[] {"first", "second"})
            //    | output;
            //var result = await plugin.ExecuteAsync();
            var result = await Cli.Wrap(path)
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory(Path.GetDirectoryName(path))
                .ExecuteBufferedAsync();

            Console.WriteLine(JsonSerializer.Serialize(result));

            //await foreach(var pluginEvent in plugin.ListenAsync())
            //{
            //    switch (pluginEvent)
            //    {
            //        case StartedCommandEvent started:
            //            Logger.Debug($"Plugin started. Process id: {started.ProcessId}");
            //            break;

            //        case StandardOutputCommandEvent stdout:
            //            PluginStdout.OnData(stdout.Text);
            //            break;

            //        case ExitedCommandEvent exited:
            //            Logger.Debug($"Plugin finished. Code {exited.ExitCode}");
            //            break;
            //    }
            //}
            output.Position = 0;
            Logger.Debug($"Output: {output.Length}");

            Logger.Debug("Closing.");
        }

        private static async Task WriteOutputFilesAsync(PluginOutput output)
        {
            Logger.Debug($"Writing {output.Files.Count} output files.");

            foreach(var file in output.Files)
            {
                using var fileStream = new FileStream(file.Path, FileMode.OpenOrCreate);
                await fileStream.WriteAsync(file.Buffer.AsMemory(0, file.Buffer.Length));

                Logger.Debug($"File '{file.Path}' wrote with {file.Buffer.Length} bytes.");
            }
        }
    }
}