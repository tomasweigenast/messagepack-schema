using System;

namespace SchemaInterpreter.Exceptions
{
    public class InvalidSchemaException : Exception
    {
        public string Line { get; set; }

        public int LineIndex { get; set; }

        public InvalidSchemaException(string message, int lineIndex, string line) : base(message)
        {
            Line = line;
            LineIndex = lineIndex; 
        }
    }
}