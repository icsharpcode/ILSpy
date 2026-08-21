// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AIChatFeatureCommandsTests
{
	[AvaloniaTest]
	public async Task RunExplainAsync_WithoutSelection_ReturnsNull()
	{
		IAIChatFeatureCommands commands = AppComposition.Current.GetExport<IAIChatFeatureCommands>();

		var stream = await commands.RunExplainAsync(null, CancellationToken.None);

		stream.Should().BeNull();
	}

	[AvaloniaTest]
	public async Task RunRenameAsync_WithoutSelection_ReturnsGuidance()
	{
		IAIChatFeatureCommands commands = AppComposition.Current.GetExport<IAIChatFeatureCommands>();

		string result = await commands.RunRenameAsync(null, CancellationToken.None);

		result.Should().Contain("/rename requires a selected type, method, property, or field");
	}

	[Test]
	public void FormatRenameSuggestions_RendersNameConfidenceReasoningAndApplyPointer()
	{
		IEntity entity = FullNameEntityProxy.Create("Sample.Type.method_5");
		var suggestions = new List<RenameSuggestion> {
			new("ParseHeader", 0.92, "reads the file header"),
			new("ReadFileHeader", 0.81, "")
		};

		string formatted = AIChatFeatureCommands.FormatRenameSuggestions(entity, suggestions);

		formatted.Should().Contain("Rename candidates for Sample.Type.method_5");
		formatted.Should().Contain("- **ParseHeader** (92%) — reads the file header");
		formatted.Should().Contain("- **ReadFileHeader** (81%) — no reasoning provided");
		formatted.Should().Contain("Suggest Name with AI");
	}

	class FullNameEntityProxy : DispatchProxy
	{
		string? fullName;

		public static IEntity Create(string fullName)
		{
			IEntity entity = Create<IEntity, FullNameEntityProxy>();
			((FullNameEntityProxy)(object)entity).fullName = fullName;
			return entity;
		}

		protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
			=> targetMethod?.Name == "get_FullName" ? fullName : throw new NotSupportedException(targetMethod?.Name);
	}
}
