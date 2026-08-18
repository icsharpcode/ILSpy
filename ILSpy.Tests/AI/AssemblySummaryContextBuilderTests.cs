// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AssemblySummaryContextBuilderTests
{
	[AvaloniaTest]
	public async Task Build_IncludesAssemblyMetadataAndPublicTypeSummary()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var assembly = await vm.OpenAssemblyAsync(typeof(AssemblySummaryContextBuilderTests).Assembly.Location);

		string summary = AssemblySummaryContextBuilder.Build(assembly);

		summary.Should().Contain("# Assembly Summary Context");
		summary.Should().Contain("- **Assembly:** " + assembly.GetTypeSystemOrNull()!.MainModule.AssemblyName);
		summary.Should().Contain("- **Version:** ");
		summary.Should().Contain("- **Target Framework:** ");
		summary.Should().Contain("## Top-level namespaces");
		summary.Should().Contain("ICSharpCode.ILSpy.Tests");
		summary.Should().Contain("- **Public types:** ");
		summary.Should().Contain("## Largest public types");
		summary.Should().Contain("## Entry point");
	}
}
