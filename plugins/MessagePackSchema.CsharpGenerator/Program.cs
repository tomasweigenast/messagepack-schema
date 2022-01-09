using MessagePack;
using MessagePackSchema.CsharpGenerator;

try
{
    // Send the name of the plugin to the compiler
    Console.WriteLine("plugin:csharp_generator");

    // Open stdin
    using Stream stdin = Console.OpenStandardInput();
    using Stream output = new MemoryStream();
    
    // Create a buffer to read
    byte[] buffer = new byte[2048];
    int bytes;

    // Read the whole input
    while ((bytes = stdin.Read(buffer, 0, buffer.Length)) > 0)
        output.Write(buffer, 0, bytes);

    var fields = GetSchemaFields(buffer);
}
catch (Exception ex)
{
    PluginLogger.LogError("Could not execute plugin.");
    PluginLogger.LogError(ex.ToString());
}

static IEnumerable<SchemaField> GetSchemaFields(byte[] buffer)
{
    List<SchemaField> fields = new();
    var reader = new MessagePackReader(buffer);
    int filesCount = reader.ReadArrayHeader();
    for (int i = 0; i < filesCount; i++)
    {
        fields.Add(MessagePackSerializer.Deserialize<SchemaField>(ref reader));
    }

    return fields;
}