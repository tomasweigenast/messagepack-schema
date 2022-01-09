using MessagePack;
using SchemaInterpreter.ExamplePlugin;
using SchemaInterpreter.Plugin.SchemaEncoder;
using System.Security.Cryptography;

try
{
    // Log the name of the plugin to the compiler
    Console.WriteLine("plugin:csharp_generator");

    PluginLogger.LogDebug("Starting plugin...");

    using Stream stdin = Console.OpenStandardInput();
    using Stream output = new MemoryStream();
    
    byte[] buffer = new byte[2048];
    int bytes;
    while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
        output.Write(buffer, 0, bytes);

    byte[] outputFileBuffer = new byte[2048];
    PluginInterpretedSchema interpretedSchema = MessagePackSerializer.Deserialize<PluginInterpretedSchema>(buffer);
    RandomNumberGenerator.Fill(outputFileBuffer);

    PluginLogger.LogInfo("Generated files from buffer.");
    using Stream stdout = Console.OpenStandardOutput();
    stdout.Write(outputFileBuffer, 0, outputFileBuffer.Length);
}
catch (Exception ex)
{
    PluginLogger.LogError("Could not execute plugin.");
    PluginLogger.LogError(ex.ToString());
}