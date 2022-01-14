#!/bin/sh
PLUGIN_SOURCE_PATH="../SchemaInterpreter.ExamplePlugin/"
PLUGIN_EXE_PATH="../SchemaInterpreter.ExamplePlugin/outputs/SchemaInterpreter.ExamplePlugin.dll"
SCHEMA_INTERPRETER_SOURCE_PATH="../SchemaInterpreter/"
SCHEMA_INTERPRETER_EXE_PATH="../SchemaInterpreter/outputs/SchemaInterpreter.dll"
COMPILED_SCHEMA_OUTPUTS="generated"

echo "Compiling ExamplePlugin..."

# rm -r $PLUGIN_SOURCE_PATH"outputs"
dotnet clean $PLUGIN_SOURCE_PATH -o $SCHEMA_INTERPRETER_SOURCE_PATH"outputs"
dotnet publish $PLUGIN_SOURCE_PATH -c Release -o $PLUGIN_SOURCE_PATH"outputs" --self-contained true --runtime win-x64

echo "ExamplePlugin compiled."

echo "Compiling SchemaInterpreter..."
# rm -r $SCHEMA_INTERPRETER_SOURCE_PATH"outputs"
dotnet clean $SCHEMA_INTERPRETER_SOURCE_PATH -o $SCHEMA_INTERPRETER_SOURCE_PATH"outputs"
dotnet publish $SCHEMA_INTERPRETER_SOURCE_PATH -c Release -o $SCHEMA_INTERPRETER_SOURCE_PATH"outputs" --self-contained true --runtime win-x64

echo "Compiling schema files..."
dotnet $SCHEMA_INTERPRETER_EXE_PATH generate --input . --output generated --plugin $PLUGIN_EXE_PATH --verbose
