using SchemaInterpreter.Parser;
using System;

namespace SchemaInterpreter.Exceptions
{
    public class InvalidSchemaException : Exception
    {
        /// <summary>
        /// Information about the line where the error has been thrown.
        /// </summary>
        public FileLine Line { get; }

        public InvalidSchemaException(string message, FileLine line) : base(message)
        {
            Line = line;
        }

        public InvalidSchemaException(string message) : base(message) { }
    }
}