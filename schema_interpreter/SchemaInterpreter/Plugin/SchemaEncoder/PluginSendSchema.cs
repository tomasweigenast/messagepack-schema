using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SchemaInterpreter.Plugin.SchemaEncoder
{
    /// <summary>
    /// Class used to send compiled schemas to the generator plugin.
    /// </summary>
    public class PluginSendSchema
    {
        [JsonPropertyName("outputPath")]
        public string OutputPath { get; set; }

        [JsonPropertyName("types")]
        public List<object> Types { get; set; }

        public PluginSendSchema()
        {
            Types = new();
        }
    }
}