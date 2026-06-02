// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Feeds the running test's fixture and method name to <see cref="TestCapture"/> before each test.
/// <para>
/// Visual-breakpoint captures fire from inside the async test body, often after an <c>await</c>.
/// NUnit's <c>TestContext.CurrentContext</c> is unreliable there -- its execution context does not
/// flow onto every async continuation, so a live lookup can fall back to the ad-hoc context and
/// dump frames from unrelated tests under one colliding filename. <see cref="ITestAction.BeforeTest"/>
/// instead hands us the real <see cref="ITest"/> up front, on the same dispatcher thread the test
/// body runs on, so we record the identity once and the captures read it from a plain static.
/// </para>
/// Kept separate from <c>ResetAppStateAttribute</c> so that attribute stays self-contained for the
/// build sweep -- capture instrumentation must not ride along into older commits.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class CaptureContextAttribute : Attribute, ITestAction
{
	public ActionTargets Targets => ActionTargets.Test;

	public void BeforeTest(ITest test)
	{
		ArgumentNullException.ThrowIfNull(test);
		TestCapture.BeginTest(test.ClassName, test.Name);
	}

	public void AfterTest(ITest test)
	{
	}
}
