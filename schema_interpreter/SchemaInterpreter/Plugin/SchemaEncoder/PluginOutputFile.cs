using MessagePack;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SchemaInterpreter.Plugin.SchemaEncoder
{
    /// <summary>
    /// Represents the output of a plugin which contains the generated files.
    /// </summary>
    [MessagePackObject]
    public class PluginOutput
    {
        /// <summary>
        /// The list of output files
        /// </summary>
        [JsonPropertyName("files")]
        [Key(0)]
        public List<PluginOutputFile> Files { get; set; }
    }

    /// <summary>
    /// Contains information about an output file.
    /// </summary>
    public class PluginOutputFile
    {
        /// <summary>
        /// The path to the file
        /// </summary>
        [JsonPropertyName("path")]
        [Key(0)]
        public string Path { get; set; }

        /// <summary>
        /// The buffer to be written
        /// </summary>
        [JsonPropertyName("buffer")]
        [Key(1)]
        public byte[] Buffer { get; set; }
    }
}