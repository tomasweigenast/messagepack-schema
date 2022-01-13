#!/bin/sh
PLUGIN_SOURCE_PATH="../SchemaInterpreter.ExamplePlugin/"
PLUGIN_EXE_PATH="../SchemaInterpreter.ExamplePlugin/outputs/SchemaInterpreter.ExamplePlugin.dll"

echo "Compiling ExamplePlugin..."

rm $PLUGIN_SOURCE_PATH"/outputs"
dotnet clean $PLUGIN_SOURCE_PATH
dotnet build $PLUGIN_SOURCE_PATH -c Release -o $PLUGIN_EXE_PATH

echo "ExamplePlugin compiled."

dotnet $PLUGIN_EXE_PATH --help

