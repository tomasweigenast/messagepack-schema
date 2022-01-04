﻿using SchemaInterpreter.Parser.Definition;
using System.Collections.Generic;

namespace SchemaInterpreter.Parser
{
    /// <summary>
    /// Provides methods to build a <see cref="SchemaType"/>
    /// </summary>
    public class SchemaTypeBuilder
    {
        private readonly IDictionary<int, SchemaTypeField> mFields;
        private string mTypeName;
        private SchemaTypeModifier? mModifier;

        public SchemaTypeModifier? Modifier => mModifier;

        public SchemaTypeBuilder() { }

        public SchemaTypeBuilder(string typeName, SchemaTypeModifier? modifier) 
        {
            mTypeName = typeName;
            mModifier = modifier;
            mFields = new Dictionary<int, SchemaTypeField>();
        }

        public SchemaTypeBuilder SetTypeName(string typeName)
        {
            mTypeName = typeName;
            return this;
        }

        public SchemaTypeBuilder SetModifier(SchemaTypeModifier modifier)
        {
            mModifier = modifier;
            return this;
        }

        public SchemaTypeBuilder AddField(SchemaTypeField field)
        {
            mFields[field.Index] = field;
            return this;
        }

        public bool ExistsIndex(int index) => mFields.ContainsKey(index);

        public SchemaType Build() => new(mTypeName, mModifier, mFields.Values);
    }
}