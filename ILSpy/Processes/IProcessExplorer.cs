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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Enumerates running .NET processes and the managed assemblies loaded in them.
	/// Abstracted so the "Open from Running Process" dialog's view model can be driven by a
	/// fake in headless tests, without live processes or IPC.
	/// </summary>
	public interface IProcessExplorer
	{
		/// <summary>
		/// Lists the running .NET processes visible to the current user. Unreachable
		/// processes (exited mid-scan, access denied, hung) are silently skipped.
		/// </summary>
		Task<IReadOnlyList<RunningDotNetProcess>> GetProcessesAsync(CancellationToken cancellationToken);

		/// <summary>
		/// Lists the managed assemblies currently loaded in <paramref name="process"/>.
		/// </summary>
		Task<IReadOnlyList<ProcessModuleInfo>> GetModulesAsync(RunningDotNetProcess process, CancellationToken cancellationToken);
	}
}
