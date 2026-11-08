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

using System.Diagnostics;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Reports failed <c>Debug.Assert</c>s of the managed decompiler and continues, as a Release
/// build does. The equality gates compare the engine with what the managed transforms produce;
/// the default listener would instead terminate the test host, e.g. on the ILAst invariant
/// checks the managed <c>AsyncAwaitDecompiler</c> violates on some runtime methods.
/// </summary>
[SetUpFixture]
public class AssertionListener : DefaultTraceListener
{
	[OneTimeSetUp]
	public void Install()
	{
		Trace.Listeners.Clear();
		Trace.Listeners.Add(this);
	}

	[OneTimeTearDown]
	public void Uninstall()
	{
		Trace.Listeners.Remove(this);
		Trace.Listeners.Add(new DefaultTraceListener());
	}

	public override void Fail(string? message, string? detailMessage)
	{
		TestContext.Progress.WriteLine($"Debug.Assert failed (continuing): {message} {detailMessage}");
	}
}
