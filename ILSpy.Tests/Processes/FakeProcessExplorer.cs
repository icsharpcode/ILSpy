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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.ILSpy.Processes;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// Stands in for the live process explorer so the dialog's view model can be driven without
/// running processes or IPC. Records every call, and can be made to fail or to park a
/// module query on a gate so cancellation and superseded-selection behavior is observable.
/// </summary>
sealed class FakeProcessExplorer : IProcessExplorer
{
	public List<RunningDotNetProcess> ProcessesToReturn { get; set; } = new();
	public Dictionary<int, IReadOnlyList<ProcessModuleInfo>> ModulesByPid { get; } = new();

	public List<int> ModuleCalls { get; } = new();
	public int ProcessCalls { get; private set; }

	public Exception? ProcessesException { get; set; }
	public Exception? ModulesException { get; set; }

	/// <summary>When set, module queries wait for it before returning.</summary>
	public TaskCompletionSource? ModulesGate { get; set; }

	public CancellationToken LastModulesToken { get; private set; }

	public static RunningDotNetProcess Process(int pid, string name, string? entryAssembly = null,
		RuntimeKind kind = RuntimeKind.CoreClr, string? commandLine = null)
		=> new(pid, name, kind, RuntimeVersion: "10.0.0", Architecture: "x64",
			CommandLine: commandLine, EntryAssemblyName: entryAssembly);

	public async Task<IReadOnlyList<RunningDotNetProcess>> GetProcessesAsync(CancellationToken cancellationToken)
	{
		ProcessCalls++;
		if (ProcessesException != null)
			throw ProcessesException;
		await Task.Yield();
		cancellationToken.ThrowIfCancellationRequested();
		return ProcessesToReturn;
	}

	public async Task<IReadOnlyList<ProcessModuleInfo>> GetModulesAsync(
		RunningDotNetProcess process, CancellationToken cancellationToken)
	{
		ModuleCalls.Add(process.Pid);
		LastModulesToken = cancellationToken;
		if (ModulesException != null)
			throw ModulesException;
		if (ModulesGate != null)
			await ModulesGate.Task.WaitAsync(cancellationToken);
		await Task.Yield();
		cancellationToken.ThrowIfCancellationRequested();
		return ModulesByPid.TryGetValue(process.Pid, out var modules)
			? modules
			: Array.Empty<ProcessModuleInfo>();
	}
}
