// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ILSpy.BuildTools.PromptEmbedder;

public static class Program
{
	public static int Main(string[] args)
	{
		try
		{
			var options = GeneratorOptions.Parse(args);
			var generated = PromptFileGenerator.Generate(options.InputDirectory);
			if (options.CheckOnly)
				return PromptFileGenerator.IsCurrent(options.OutputFile, generated) ? 0 : 1;

			PromptFileGenerator.WriteIfChanged(options.OutputFile, generated);
			return 0;
		}
		catch (Exception ex) when (ex is ArgumentException or IOException or InvalidDataException)
		{
			Console.Error.WriteLine($"Prompt embedding failed: {ex.Message}");
			return 1;
		}
	}
}

public sealed record GeneratorOptions(string InputDirectory, string OutputFile, bool CheckOnly)
{
	public static GeneratorOptions Parse(string[] args)
	{
		if (args.Length is < 2 or > 3)
			throw new ArgumentException("Usage: PromptEmbedder <input-directory> <output-file> [--check]");

		var checkOnly = args.Length == 3 && string.Equals(args[2], "--check", StringComparison.Ordinal);
		if (args.Length == 3 && !checkOnly)
			throw new ArgumentException("The optional third argument must be --check.");

		return new GeneratorOptions(Path.GetFullPath(args[0]), Path.GetFullPath(args[1]), checkOnly);
	}
}

public static class PromptFileGenerator
{
	private const string Header = "// Copyright (c) 2026 Dr. Masroor Ehsan\n\n";

	public static string Generate(string inputDirectory)
	{
		if (!Directory.Exists(inputDirectory))
			throw new DirectoryNotFoundException($"Prompt directory does not exist: {inputDirectory}");

		var prompts = Directory.EnumerateFiles(inputDirectory, "*.prompt", SearchOption.TopDirectoryOnly)
			.Select(path => (Path: path, Id: Path.GetFileNameWithoutExtension(path)))
			.Where(item => item.Id.IndexOf('.', StringComparison.Ordinal) < 0)
			.OrderBy(item => item.Id, StringComparer.Ordinal)
			.ToArray();

		if (prompts.Length == 0)
			throw new InvalidDataException($"No base .prompt files found in {inputDirectory}.");

		var builder = new StringBuilder();
		builder.Append(Header);
		builder.AppendLine("using System;");
		builder.AppendLine("using System.Collections.Generic;");
		builder.AppendLine();
		builder.AppendLine("namespace ICSharpCode.ILSpyX.AI");
		builder.AppendLine("{");
		builder.AppendLine("\t/// <summary>");
		builder.AppendLine("\t/// Generated embedded fallback prompts. DO NOT EDIT - regenerate with BuildTools/PromptEmbedder.");
		builder.AppendLine("\t/// </summary>");
		builder.AppendLine("\tinternal static class EmbeddedPrompts");
		builder.AppendLine("\t{");
		builder.AppendLine("\t\tprivate static readonly Dictionary<string, string> _prompts = new(StringComparer.Ordinal)");
		builder.AppendLine("\t\t{");

		foreach (var prompt in prompts)
		{
			ValidatePromptId(prompt.Id);
			var text = ReadPromptText(prompt.Path);
			builder.Append("\t\t\t[").Append(JsonSerializer.Serialize(prompt.Id)).Append("] = ")
				.Append(JsonSerializer.Serialize(text, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
				.AppendLine(",");
		}

		builder.AppendLine("\t\t};");
		builder.AppendLine();
		builder.AppendLine("\t\tpublic static string Get(string promptId)");
		builder.AppendLine("\t\t{");
		builder.AppendLine("\t\t\tif (_prompts.TryGetValue(promptId, out var prompt))");
		builder.AppendLine("\t\t\t\treturn prompt;");
		builder.AppendLine();
		builder.AppendLine("\t\t\tthrow new ArgumentException($\"Unknown prompt ID: {promptId}\", nameof(promptId));");
		builder.AppendLine("\t\t}");
		builder.AppendLine("\t}");
		builder.AppendLine("}");
		return builder.ToString();
	}

	public static bool IsCurrent(string outputFile, string generated)
	{
		return File.Exists(outputFile) && string.Equals(File.ReadAllText(outputFile, Encoding.UTF8), generated, StringComparison.Ordinal);
	}

	public static void WriteIfChanged(string outputFile, string generated)
	{
		if (IsCurrent(outputFile, generated))
			return;

		var directory = Path.GetDirectoryName(outputFile);
		if (string.IsNullOrEmpty(directory))
			throw new ArgumentException("Output file must include a directory.", nameof(outputFile));
		Directory.CreateDirectory(directory);

		var temporaryFile = outputFile + ".tmp";
		File.WriteAllText(temporaryFile, generated, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
		File.Move(temporaryFile, outputFile, overwrite: true);
	}

	private static string ReadPromptText(string path)
	{
		var content = File.ReadAllText(path, Encoding.UTF8).Replace("\r\n", "\n");
		if (content.StartsWith("\uFEFF", StringComparison.Ordinal))
			content = content[1..];
		if (!content.StartsWith("---\n", StringComparison.Ordinal))
			throw new InvalidDataException($"Prompt file must start with YAML frontmatter: {path}");

		var separator = content.IndexOf("\n---\n", 4, StringComparison.Ordinal);
		if (separator < 0)
			throw new InvalidDataException($"Prompt file is missing the YAML frontmatter terminator: {path}");

		var promptText = content[(separator + 5)..].Trim();
		if (promptText.Length == 0)
			throw new InvalidDataException($"Prompt file has an empty prompt body: {path}");
		return promptText;
	}

	private static void ValidatePromptId(string promptId)
	{
		if (promptId.Length == 0 || !char.IsLower(promptId[0]) || promptId.Any(c => !(char.IsLower(c) || char.IsDigit(c) || c == '_')))
			throw new InvalidDataException($"Invalid prompt ID '{promptId}'. Use lowercase letters, digits, and underscores.");
	}
}
