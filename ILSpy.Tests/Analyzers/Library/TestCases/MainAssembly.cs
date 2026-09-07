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

namespace ICSharpCode.ILSpy.Tests.Analyzers.Library.TestCases.Main
{
	// Fixture for the analyzer-library tests. Methods are picked up by metadata-token
	// lookup from the test assembly itself, so each one's IL must reach the operation
	// the analyser is meant to find — that's why the bodies look trivial.
	class MainAssembly
	{
		public string UsesSystemStringEmpty()
		{
			return string.Empty;
		}

		public int UsesInt32()
		{
			return int.Parse("1234");
		}
	}

	// Fixture for the field-access analysers. A field of a value type reached through a
	// method call is loaded by address (ldflda/ldsflda), which says nothing about whether
	// the call writes to it - issue #2372.
	class FieldAccess
	{
		public bool instanceFlag;
		public static bool staticFlag;

		public string ReadsInstanceFlagByAddress()
		{
			// callvirt Boolean::ToString(ldflda instanceFlag)
			return instanceFlag.ToString();
		}

		public static string ReadsStaticFlagByAddress()
		{
			// call Boolean::ToString(ldsflda staticFlag)
			return staticFlag.ToString();
		}

		public bool ReadsInstanceFlag()
		{
			return instanceFlag;
		}

		public void WritesInstanceFlag()
		{
			instanceFlag = true;
		}

		public static void WritesStaticFlag()
		{
			staticFlag = true;
		}
	}
}
