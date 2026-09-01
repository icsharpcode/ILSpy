// Copyright (c) 2026 Siegfried Pammer
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
using System.Linq;

using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

[assembly: TreeThreadAffinityGuard]

// Deliberately outside any namespace: this is applied to the assembly, and the attribute has to be
// nameable from the assembly-level attribute list.

/// <summary>
/// Fails any test that mutated a displayed tree from a thread other than the tree's owner.
/// </summary>
/// <remarks>
/// The affinity check records into <see cref="TreeThreadAffinity.Violations"/> instead of relying
/// on a throw, because tree mutation happens inside callers that catch Exception - the background
/// decompile turns any exception a node raises into text in the output pane. A throw there is
/// swallowed and the run still passes, so something has to read the collector, and it has to be
/// per test: an assembly-level teardown failure is reported but does not fail the run or change
/// the exit code. Debug builds only - the check compiles away in release, where the collector
/// stays empty and this is a no-op.
///
/// A fixture that provokes violations on purpose clears the collector in its own
/// <c>[TearDown]</c>, which runs before this.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class TreeThreadAffinityGuardAttribute : Attribute, ITestAction
{
	public ActionTargets Targets => ActionTargets.Test;

	public void BeforeTest(ITest test)
	{
		TreeThreadAffinity.Clear();
	}

	public void AfterTest(ITest test)
	{
		var violations = TreeThreadAffinity.Violations;
		if (violations.Count == 0)
			return;
		TreeThreadAffinity.Clear();
		Assert.Fail($"{violations.Count} tree thread-affinity violation(s) were recorded while this test ran:"
			+ Environment.NewLine
			+ string.Join(Environment.NewLine, violations.Select(v => v.ToString())));
	}
}
