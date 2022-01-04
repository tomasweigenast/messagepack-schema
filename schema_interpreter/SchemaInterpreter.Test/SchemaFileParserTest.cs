using SchemaInterpreter.Exceptions;
using SchemaInterpreter.Helpers;
using SchemaInterpreter.Parser;
using SchemaInterpreter.Parser.Definition;
using SchemaInterpreter.Parser.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace SchemaInterpreter.Test
{
    public class SchemaFileParserTest
    {
        [Fact]
        public async Task TestParseType()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test2.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test2", 1));
            await parser.ParseFile(schema, "test2");

            ParserContext.Current.GenerateDefaultValues();

            var schemaFile = ParserContext.Current.GetCompiledAndClear().First();
            var type = schemaFile.Types[0];
            var fields = type.Fields.ToDictionary(k => k.Index, v => v);

            Assert.Equal(1, schemaFile.Version);
            Assert.Equal("test2", schemaFile.Name);
            Assert.Equal("Animal", type.Name);
            Assert.Equal("test2", type.Package);
            Assert.Equal("test2.Animal", type.FullName);
            Assert.Equal(CommonHelpers.CalculateMD5("Animal"), type.Id);
            Assert.Null(type.Modifier);

            AssertExt.True(fields[0], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "is_mammal",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.Boolean,
                (field) => ((bool)field.DefaultValue) == false
            });

            AssertExt.True(fields[1], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "name",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.String,
                (field) => !field.HasDefaultValue
            });

            AssertExt.True(fields[2], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "discovered_date",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.Int64,
                (field) => !field.HasDefaultValue,
                (field) => !fields[2].HasDefaultValue,
                (field) => fields[2].IsNullable
            });
        }

        [Fact]
        public async Task TestParseEnum()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test3.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test3", 1));
            await parser.ParseFile(schema, "test3");

            ParserContext.Current.GenerateDefaultValues();

            var schemaFile = ParserContext.Current.GetCompiledAndClear().First();
            var type = schemaFile.Types[0];
            var fields = type.Fields.ToDictionary(k => k.Index, v => v);

            Assert.Equal(1, schemaFile.Version);
            Assert.Equal("test3", schemaFile.Name);
            Assert.Equal("VehicleType", type.Name);
            Assert.Equal("test3", type.Package);
            Assert.Equal("test3.VehicleType", type.FullName);
            Assert.Equal(CommonHelpers.CalculateMD5("VehicleType"), type.Id);
            Assert.Equal(SchemaTypeModifier.Enum, type.Modifier);

            Assert.True(fields[0].Name == "undefined");
            Assert.True(fields[1].Name == "car");
            Assert.True(fields[2].Name == "motorbike");
            Assert.True(fields[3].Name == "bike");
            Assert.True(fields[4].Name == "furgo");
        }

        [Fact]
        public async Task TestParseComplexType()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test4.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test4", 1));
            await parser.ParseFile(schema, "test4");

            ParserContext.Current.GenerateDefaultValues();

            var schemaFile = ParserContext.Current.GetCompiledAndClear().First();
            var types = schemaFile.Types;

            Assert.Equal(1, schemaFile.Version);
            Assert.Equal("test4", schemaFile.Name);

            // Verify schema types
            AssertExt.True(types[0], new Expression<Func<SchemaType, bool>>[]
            {
                (type) => type.Name == "User",
                (type) => type.Package == "test4",
                (type) => type.FullName == "test4.User",
                (type) => type.Id == CommonHelpers.CalculateMD5("User"),
                (type) => type.Modifier == null,
            });
            AssertExt.True(types[1], new Expression<Func<SchemaType, bool>>[]
            {
                (type) => type.Name == "PaymentMethod",
                (type) => type.Package == "test4",
                (type) => type.FullName == "test4.PaymentMethod",
                (type) => type.Id == CommonHelpers.CalculateMD5("PaymentMethod"),
                (type) => type.Modifier == SchemaTypeModifier.Enum,
            });
            AssertExt.True(types[2], new Expression<Func<SchemaType, bool>>[]
            {
                (type) => type.Name == "Address",
                (type) => type.Package == "test4",
                (type) => type.FullName == "test4.Address",
                (type) => type.Id == CommonHelpers.CalculateMD5("Address"),
                (type) => type.Modifier == null,
            });

            // Verify schema type fields
            var userFields = types[0].Fields.ToList();
            AssertExt.True(userFields[0], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "uid",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.String,
            });
            AssertExt.True(userFields[4], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "phone_number",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.String,
                (field) => field.IsNullable,
            });
            AssertExt.True(userFields[6], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "phone_number_verified",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.Boolean,
                (field) => field.IsNullable,
            });
            AssertExt.True(userFields[7], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "payment_methods",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.Map,
                (field) => field.ValueType is MapSchemaFieldValueType,
                (field) => ((MapSchemaFieldValueType)field.ValueType).KeyType.TypeName == SchemaFieldValueTypes.Custom,
                (field) => ((MapSchemaFieldValueType)field.ValueType).ValueType.TypeName == SchemaFieldValueTypes.Boolean,
                (field) => ((CustomSchemaFieldValueType)((MapSchemaFieldValueType)field.ValueType).KeyType).CustomType == "PaymentMethod",
                (field) => field.Metadata.SequenceEqual(new Dictionary<string, object>
                {
                    { "ignore_unknown", true }
                }),
            });
            AssertExt.True(userFields[8], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "addresses",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.List,
                (field) => field.ValueType is ListSchemaFieldValueType,
                (field) => ((ListSchemaFieldValueType)field.ValueType).ElementType.TypeName == SchemaFieldValueTypes.Custom,
                (field) => ((CustomSchemaFieldValueType)((ListSchemaFieldValueType)field.ValueType).ElementType).CustomType == "Address",
                (field) => field.Metadata == null
            });
            AssertExt.True(userFields[9], new Expression<Func<SchemaTypeField, bool>>[]
            {
                (field) => field.Name == "default_payment_method",
                (field) => field.ValueType.TypeName == SchemaFieldValueTypes.Custom,
                (field) => field.ValueType is CustomSchemaFieldValueType,
                (field) => ((CustomSchemaFieldValueType)field.ValueType).CustomType == "PaymentMethod",
                (field) => field.Metadata == null,
                (field) => field.HasDefaultValue,
                (field) => ((CustomTypeValue)field.DefaultValue).Value == "cash"
            });
        }

        [Fact]
        public async Task TestParseUnknownEnumValue()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test5.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test5", 1));
            await parser.ParseFile(schema, "test5");

            Assert.Throws<InvalidSchemaException>(() => ParserContext.Current.GenerateDefaultValues());
        }

        [Fact]
        public async Task TestParseInvalidTypeName()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test6.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test6", 1));
            await parser.ParseFile(schema, "test6");

            Record.Exception(() => ParserContext.Current.VerifyAllTypes());
        }

        [Fact]
        public async Task TestParseInvalidTypeNameInDefaultValue()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test7.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test7", 1));
            await parser.ParseFile(schema, "test7");

            Assert.Throws<InvalidSchemaException>(() => ParserContext.Current.GenerateDefaultValues());
        }


        [Fact]
        public async Task TestParseTypes()
        {
            ParserContext.Create();

            var schema = new StreamReader(File.OpenRead("test1.mpack"));
            var parser = new SchemaFileParser();

            ParserContext.Current.AddPackage(new SchemaFile("test1", 1));
            await parser.ParseFile(schema, "test1");

            Assert.Null(Record.Exception(() =>
            {
                ParserContext.Current.GenerateDefaultValues();
                ParserContext.Current.VerifyAllTypes();
            }));
        }
    }
}