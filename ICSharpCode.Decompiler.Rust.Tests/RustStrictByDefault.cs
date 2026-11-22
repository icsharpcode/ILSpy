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
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Test runs are strict by default: seams 1 and 2 fail a request the engine cannot serve instead of
/// falling back to the managed pipeline. Setting <c>ILSPY_RUST_STRICT</c> before the run (for
/// example to 0) overrides this. Linked into every test project that loads the shim; the
/// variable is inherited by the processes a test starts.
/// </summary>
static class RustStrictByDefault
{
#pragma warning disable CA2255 // Runs before any test, which is the point.
	[ModuleInitializer]
#pragma warning restore CA2255
	internal static void Initialize()
	{
		if (Environment.GetEnvironmentVariable("ILSPY_RUST_STRICT") == null)
			Environment.SetEnvironmentVariable("ILSPY_RUST_STRICT", "1");
	}
}
