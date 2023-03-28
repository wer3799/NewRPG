using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Text;

//파일 예시
//Option1
//Option2
//Option3
//EnumCodeGenerator.GenerateEnumFromTextFile(Application.dataPath + "/input.txt", "MyEnum", Application.dataPath + "/MyEnum.cs");

public static class EnumCodeGenerator
{
    public static void GenerateEnumFromTextFile(string inputFilePath, string enumName, string outputFilePath)
    {
        // Read the input text file
        string[] inputLines = File.ReadAllLines(inputFilePath);

        // Create a new StringBuilder to hold the C# code for the Enum
        StringBuilder codeBuilder = new StringBuilder();

        // Generate the Enum code
        codeBuilder.AppendLine($"public enum {enumName}");
        codeBuilder.AppendLine("{");

        foreach (string inputLine in inputLines)
        {
            string enumValue = inputLine.Trim().Replace(" ", "");

            // Skip empty lines and lines starting with "//"
            if (string.IsNullOrEmpty(enumValue) || enumValue.StartsWith("//"))
            {
                continue;
            }

            codeBuilder.AppendLine($"    {enumValue},");
        }

        codeBuilder.AppendLine("}");

        // Write the C# code to the output file
        File.WriteAllText(outputFilePath, codeBuilder.ToString());

        // Compile the generated code
        CompileGeneratedCode(outputFilePath);
    }

    private static void CompileGeneratedCode(string codeFilePath)
    {
        // Load the generated code assembly into memory
        Assembly codeAssembly = Assembly.LoadFrom(codeFilePath);

        // Find the generated Enum type in the assembly
        Type enumType = codeAssembly.GetType($"{Path.GetFileNameWithoutExtension(codeFilePath)}");

        if (enumType == null)
        {
            throw new Exception("Failed to load generated Enum type.");
        }

        // Use the generated Enum type in your code
        // For example, you can get the Enum values like this:
        Array enumValues = Enum.GetValues(enumType);
    }
}