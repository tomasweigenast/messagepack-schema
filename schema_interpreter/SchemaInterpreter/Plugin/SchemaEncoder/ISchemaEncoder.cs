using SchemaInterpreter.Parser.Definition;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchemaInterpreter.Plugin.Encoder
{
    /// <summary>
    /// Encodes a schema.
    /// </summary>
    public interface ISchemaEncoder
    {
        /// <summary>
        /// Encodes a list of schema files.
        /// </summary>
        /// <param name="files">The files to encode.</param>
        public Task<ReadOnlyMemory<byte>> Encode(IEnumerable<SchemaFile> files);
    }
}