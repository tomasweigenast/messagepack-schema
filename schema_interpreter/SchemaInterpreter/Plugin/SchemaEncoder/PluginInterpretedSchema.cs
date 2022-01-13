using MessagePack;
using SchemaInterpreter.Plugin.Encoder;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SchemaInterpreter.Plugin.SchemaEncoder
{
    /// <summary>
    /// Class used to send compiled schemas to the generator plugin.
    /// </summary>
    [MessagePackObject]
    public class PluginInterpretedSchema
    {
        [JsonPropertyName("packages")]
        [Key(0)]
        public List<object> Files { get; set; }

        /// <summary>
        /// The encoding to use
        /// </summary>
        //[JsonPropertyName("encoding")]
        //[Key(1)]
        [JsonIgnore]
        [IgnoreMember]
        public PluginEncoding Encoding { get; set; }

        public PluginInterpretedSchema()
        {
            Files = new();
        }
    }
}