// Copyright (c) 2026 Masroor
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class AIOutputPaneModelTests
{
	[AvaloniaTest]
	public async Task Cancel_InvalidatesLateProviderChunks()
	{
		var pane = AppComposition.Current.GetExport<AIOutputPaneModel>();
		pane.ClearCommand.Execute(null);
		var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
		var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

		Task request = pane.StartAsync("Sample", token => LateChunkStream(started, release));
		await started.Task;

		pane.CancelCommand.Execute(null);
		pane.IsBusy.Should().BeFalse();
		pane.StatusMessage.Should().Be("Canceled");
		pane.CanCopy.Should().BeFalse();

		release.SetResult();
		await request;

		pane.Response.Should().BeEmpty();
		pane.StatusMessage.Should().Be("Canceled");
		pane.ClearCommand.Execute(null);
	}

	[AvaloniaTest]
	public async Task CopyBecomesAvailableOnlyAfterCompletion()
	{
		var pane = AppComposition.Current.GetExport<AIOutputPaneModel>();
		pane.ClearCommand.Execute(null);

		await pane.StartAsync("Sample", _ => SingleChunkStream());

		pane.Response.Should().Be("complete");
		pane.StatusMessage.Should().Be("Complete");
		pane.CanCopy.Should().BeTrue();
		pane.ClearCommand.Execute(null);
	}

	[AvaloniaTest]
	public async Task StartAsync_WithEntity_ReresolvesEntityFromNewDecompiler()
	{
		// This test verifies the fix for the "The entity does not belong to the decompiler module" error.
		// The issue occurred when an IEntity from one decompiler instance was passed to StartAsync,
		// which creates a new decompiler instance internally. The fix re-resolves the entity
		// from the new decompiler's type system using the metadata token.

		// Get a loaded assembly from the current environment
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		var listName = settingsService.AssemblyListManager.AssemblyLists.FirstOrDefault();
		if (listName == null)
		{
			Assert.Ignore("No assembly lists available");
			return;
		}

		var list = settingsService.AssemblyListManager.LoadList(listName);
		var assemblies = list.GetAssemblies();
		if (assemblies.Length == 0)
		{
			Assert.Ignore("Assembly list is empty");
			return;
		}

		var loadedAsm = assemblies[0];
		if (loadedAsm.GetMetadataFileOrNull() is not { } metadataFile)
		{
			Assert.Ignore("First assembly has no metadata file");
			return;
		}

		// Create a decompiler and get an entity from its type system
		var settings = new DecompilerSettings();
		var resolver = metadataFile.GetAssemblyResolver();
		var decompiler = new CSharpDecompiler(metadataFile, resolver, settings);
		var type = decompiler.TypeSystem.MainModule.TypeDefinitions.FirstOrDefault();

		if (type == null)
		{
			Assert.Ignore("Module has no type definitions");
			return;
		}

		// Now call StartAsync with that entity - it creates a new decompiler internally
		// and should re-resolve the entity without throwing
		var pane = AppComposition.Current.GetExport<AIOutputPaneModel>();
		pane.ClearCommand.Execute(null);

		// This should not throw - the fix re-resolves the entity from the new decompiler
		Assert.DoesNotThrowAsync(async () => {
			await pane.StartAsync(type);
		}, "StartAsync should re-resolve entity from new decompiler without throwing");

		pane.ClearCommand.Execute(null);
	}

	[Test]
	public void ResolveEntity_WithTypeFromDifferentDecompiler_ReresolvesSuccessfully()
	{
		// Unit test for the entity re-resolution logic without requiring full app environment.
		// This directly tests the scenario: an ITypeDefinition from one decompiler is passed
		// to code that creates a new decompiler, and the entity must be re-resolved.

		// Use this test assembly itself as the metadata source
		var testAssemblyPath = typeof(AIOutputPaneModelTests).Assembly.Location;
		var metadataFile = new PEFile(testAssemblyPath);

		// Create first decompiler and get an entity
		var settings = new DecompilerSettings();
		var resolver = new UniversalAssemblyResolver(testAssemblyPath, false, metadataFile.Metadata.DetectTargetFrameworkId());
		var decompiler1 = new CSharpDecompiler(metadataFile, resolver, settings);
		var type1 = decompiler1.TypeSystem.MainModule.TypeDefinitions.First();

		// Create a second decompiler (simulating what happens in StartAsync)
		var decompiler2 = new CSharpDecompiler(metadataFile, resolver, settings);

		// Try to re-resolve the entity from the first decompiler using the second decompiler's type system
		// This is the fix - using MetadataToken to re-resolve
		var token = type1.MetadataToken;
		var type2 = decompiler2.TypeSystem.MainModule.GetDefinition((TypeDefinitionHandle)token);

		// Verify the re-resolved entity is valid and equivalent
		Assert.That(type2, Is.Not.Null);
		Assert.That(type2.FullName, Is.EqualTo(type1.FullName));
		Assert.That(type2.MetadataToken, Is.EqualTo(type1.MetadataToken));
	}

	static async IAsyncEnumerable<string> LateChunkStream(
		TaskCompletionSource started,
		TaskCompletionSource release)
	{
		started.SetResult();
		await release.Task;
		yield return "late";
	}

	static async IAsyncEnumerable<string> SingleChunkStream()
	{
		yield return "complete";
		await Task.CompletedTask;
	}
}
