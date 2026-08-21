// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.IO;

using ILSpy.BuildTools.PromptEmbedder;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI;

[TestFixture]
public sealed class PromptFileGeneratorTests
{
	private string _directory = null!;

	[SetUp]
	public void SetUp()
	{
		_directory = Path.Combine(Path.GetTempPath(), $"ilspy-prompt-embedder-{Guid.NewGuid():N}");
		Directory.CreateDirectory(_directory);
	}

	[TearDown]
	public void TearDown()
	{
		if (Directory.Exists(_directory))
			Directory.Delete(_directory, recursive: true);
	}

	[Test]
	public void Generate_OrdersBasePromptsAndSkipsModelVariants()
	{
		WritePrompt("zeta.prompt", "Z prompt");
		WritePrompt("alpha.prompt", "A prompt");
		WritePrompt("alpha.claude.prompt", "Variant");

		var generated = PromptFileGenerator.Generate(_directory, "ICSharpCode.ILSpy.AI");

		Assert.That(generated.IndexOf("[\"alpha\"]", StringComparison.Ordinal), Is.LessThan(generated.IndexOf("[\"zeta\"]", StringComparison.Ordinal)));
		Assert.That(generated, Does.Contain("A prompt"));
		Assert.That(generated, Does.Contain("Z prompt"));
		Assert.That(generated, Does.Not.Contain("Variant"));
	}

	[Test]
	public void Generate_RejectsMissingFrontmatterTerminator()
	{
		File.WriteAllText(Path.Combine(_directory, "broken.prompt"), "---\ndescription: broken\nbody");

		var exception = Assert.Throws<InvalidDataException>(() => PromptFileGenerator.Generate(_directory, "ICSharpCode.ILSpy.AI"));

		Assert.That(exception!.Message, Does.Contain("terminator"));
	}

	[Test]
	public void WriteIfChanged_IsIdempotent()
	{
		WritePrompt("alpha.prompt", "A prompt");
		var output = Path.Combine(_directory, "EmbeddedPrompts.g.cs");
		var generated = PromptFileGenerator.Generate(_directory, "ICSharpCode.ILSpy.AI");

		PromptFileGenerator.WriteIfChanged(output, generated);
		var timestamp = File.GetLastWriteTimeUtc(output);
		PromptFileGenerator.WriteIfChanged(output, generated);

		Assert.Multiple(() => {
			Assert.That(File.ReadAllText(output), Is.EqualTo(generated));
			Assert.That(File.GetLastWriteTimeUtc(output), Is.EqualTo(timestamp));
		});
	}

	private void WritePrompt(string fileName, string body)
	{
		File.WriteAllText(Path.Combine(_directory, fileName), $"---\ndescription: test\n---\n{body}\n");
	}
}
