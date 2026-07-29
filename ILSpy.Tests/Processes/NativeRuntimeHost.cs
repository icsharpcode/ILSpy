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
using System.Runtime.InteropServices;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// The native CoreCLR host that backs the running test process. It serves as the fixture for
/// two things the process explorer must get right: a real file that carries no IL, and a
/// module the managed rundown must never report.
/// </summary>
/// <remarks>
/// It is located through the runtime directory rather than the process' own module list,
/// because <see cref="System.Diagnostics.Process.Modules"/> is not implemented on macOS -
/// there it reports the main module and nothing else.
/// </remarks>
static class NativeRuntimeHost
{
	/// <summary>
	/// File name of the runtime host on the current OS.
	/// </summary>
	public static string FileName { get; } =
		OperatingSystem.IsWindows() ? "coreclr.dll"
		: OperatingSystem.IsMacOS() ? "libcoreclr.dylib"
		: "libcoreclr.so";

	/// <summary>
	/// Full path of the runtime host loaded by the current process. It sits next to
	/// System.Private.CoreLib, whether the app is framework-dependent or self-contained.
	/// </summary>
	public static string FullPath { get; } =
		Path.Combine(RuntimeEnvironment.GetRuntimeDirectory(), FileName);
}
