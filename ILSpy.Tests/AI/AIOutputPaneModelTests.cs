// Copyright (c) 2026 Masroor
using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AppEnv;

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
