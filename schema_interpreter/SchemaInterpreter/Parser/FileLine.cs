namespace SchemaInterpreter.Parser
{
    public class FileLine
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

        public FileLine(int lineIndex, string line, string fileName)
        {
            Line = line;
            LineIndex = lineIndex;
            FileName = fileName;
        }
    }
}