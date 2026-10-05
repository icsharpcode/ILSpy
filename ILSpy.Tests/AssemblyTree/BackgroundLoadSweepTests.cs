// Copyright (c) 2026 Christoph Wille
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.


using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AssemblyTree;

/// <summary>
/// The background sweep that loads every assembly of the shown list ends by asking the commands
/// to re-evaluate, which is what enables "Remove assemblies with load errors". That has to happen
/// when one of the assemblies fails to load, since that is the only case the command is for.
/// </summary>
[TestFixture]
public class BackgroundLoadSweepTests
{
	/// <summary>
	/// Holds a load open until the test lets go of it, then ends without a single byte, which
	/// no loader accepts. The test thereby chooses the moment the assembly fails to load.
	/// </summary>
	sealed class GatedEmptyStream : Stream
	{
		public ManualResetEventSlim Gate { get; } = new();

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;
		public override long Length => throw new NotSupportedException();
		public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

		public override int Read(byte[] buffer, int offset, int count)
		{
			Gate.Wait();
			return 0;
		}

		public override void Flush() { }
		public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
		public override void SetLength(long value) => throw new NotSupportedException();
		public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	}

	[AvaloniaTest]
	public async Task Sweep_Requeries_Commands_When_An_Assembly_Fails_To_Load()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await Waiters.WaitForIdleAsync();

		// Show a list whose only assembly cannot finish loading before the gate opens, and let
		// everything the switch itself queues run first, so that a requery seen afterwards can
		// only be the one that ends the sweep.
		var stream = new GatedEmptyStream();
		var listManager = AppComposition.Current.GetExport<SettingsService>().AssemblyListManager;
		var list = listManager.CreateList("gated-list");
		var assembly = list.OpenAssembly(Path.Combine(Path.GetTempPath(), "ILSpyGated.dll"), stream);
		vm.AssemblyTreeModel.ShowAssemblyList(list);
		await Waiters.WaitForAsync(
			() => !Dispatcher.UIThread.HasJobsWithPriority(DispatcherPriority.Background),
			description: "the list switch to settle");

		bool requeried = false;
		EventHandler onRequery = (_, _) => requeried = true;
		ICSharpCode.ILSpy.Commands.CommandManager.AddRequerySuggested(onRequery);
		try
		{
			stream.Gate.Set();
			await Waiters.WaitForAsync(() => assembly.HasLoadError, description: "the gated assembly to fail loading");
			await Waiters.WaitForAsync(() => requeried,
				description: "a command requery after the assembly failed to load");
		}
		finally
		{
			stream.Gate.Set();
			ICSharpCode.ILSpy.Commands.CommandManager.RemoveRequerySuggested(onRequery);
		}
	}
}
