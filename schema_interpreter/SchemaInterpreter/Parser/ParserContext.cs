using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;
using System.Linq;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// The current parsing context which contains references between
    /// schemas and types.
    /// </summary>
    public class ParserContext
    {
        private readonly IList<SchemaType> mTypes;

        public ParserContext()
        {
            mTypes = new List<SchemaType>();
        }

        /// <summary>
        /// Finds a type by its id.
        /// </summary>
        /// <param name="id">The type id.</param>
        public SchemaType FindType(int id) 
            => mTypes.FirstOrDefault(x => x.Id == id);

        /// <summary>
        /// Finds a type by its full name.
        /// </summary>
        /// <param name="nane">The full name of the type.</param>
        public SchemaType FindTypeByFullName(string fullName) 
            => mTypes.FirstOrDefault(x => x.FullName == fullName);

        /// <summary>
        /// Parses and generates all the default values for all types.
        /// </summary>
        public void GenerateDefaultValues()
        {
            foreach(SchemaTypeField field in mTypes.SelectMany(x => x.Fields).Where(x => x.HasDefaultValue && !x.IsDefaultValueGenerated))
            {
                string rawDefaultValue = field.DefaultValue as string;
                object value = ValueParser.Parse(rawDefaultValue, field.ValueType)
            }
        }
    }
}