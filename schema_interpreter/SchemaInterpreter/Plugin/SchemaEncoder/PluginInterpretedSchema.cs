namespace SchemaInterpreter.Plugin.SchemaEncoder
{
    /// <summary>
    /// Class used to send compiled schemas to the generator plugin.
    /// </summary>
    public class PluginInterpretedSchema
    {
        [JsonPropertyName("types")]
        public List<object> Types { get; set; }

        public PluginInterpretedSchema()
        {
            Types = new();
        }
    }
}