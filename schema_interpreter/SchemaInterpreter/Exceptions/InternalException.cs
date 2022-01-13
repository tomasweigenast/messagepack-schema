using System;

namespace SchemaInterpreter.Exceptions
{
    public class InternalException : Exception
    {
        public InternalException(string message) : base(message) { }
    }
}