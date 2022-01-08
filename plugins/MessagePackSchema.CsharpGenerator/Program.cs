using MessagePackSchema.CsharpGenerator;

try
{
    // Log the name of the plugin to the compiler
    Console.WriteLine("plugin:csharp_generator");

    // Open stdin
    using Stream stdin = Console.OpenStandardInput();
    using Stream output = new MemoryStream();
    
    // Create a buffer to read
    byte[] buffer = new byte[2048];
    int bytes;
    while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
        output.Write(buffer, 0, bytes);

    PluginLogger.LogInfo("Generated files from buffer.");
}
catch (Exception ex)
{
    PluginLogger.LogError("Could not execute plugin.");
    PluginLogger.LogError(ex.ToString());
}