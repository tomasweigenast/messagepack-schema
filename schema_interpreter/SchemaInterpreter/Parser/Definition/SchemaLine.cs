namespace SchemaInterpreter.Parser.Definition
{
    public class SchemaLine
    {
        /// <summary>
        /// The text of the line
        /// </summary>
        public string Line { get; }

        /// <summary>
        /// The line index
        /// </summary>
        public int LineIndex { get; }

        /// <summary>
        /// The name of the file
        /// </summary>
        public string FileName { get; }

        public SchemaLine(int lineIndex, string line, string fileName)
        {
            Line = line;
            LineIndex = lineIndex;
            FileName = fileName;
        }
    }
}