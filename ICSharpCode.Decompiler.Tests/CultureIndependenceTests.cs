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

using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// The decompiler's output must not depend on the host's culture or globalization mode: a
	/// framework-dependent app on ICU and a Native AOT app in invariant mode (the usual AOT
	/// configuration) have to produce the same text for the same input.
	/// </summary>
	[TestFixture]
	public class CultureIndependenceTests
	{
		class Instruction
		{
			public int Value;
		}

		static int SumOfInstruction()
		{
			Instruction instruction = new Instruction();
			instruction.Value = 1;
			return instruction.Value + instruction.Value;
		}

		[Test]
		public void VariableNamesDerivedFromTypeNamesUseInvariantCaseMapping()
		{
			// Without debug symbols the local is named after its type, "Instruction" -> "instruction".
			// Turkish case mapping lowercases 'I' to the dotless 'ı' (U+0131), which must not leak
			// into generated identifiers.
			var settings = new DecompilerSettings {
				UseDebugSymbols = false,
				ThrowOnAssemblyResolveErrors = false
			};
			var decompiler = new CSharpDecompiler(typeof(CultureIndependenceTests).Assembly.Location, settings);
			var method = typeof(CultureIndependenceTests).GetMethod(nameof(SumOfInstruction), BindingFlags.NonPublic | BindingFlags.Static);
			var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);

			var culture = CultureInfo.CurrentCulture;
			string code;
			try
			{
				CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
				code = decompiler.DecompileAsString(handle);
			}
			finally
			{
				CultureInfo.CurrentCulture = culture;
			}

			Assert.That(code, Does.Contain("Instruction instruction = new Instruction();"));
			Assert.That(code, Does.Not.Contain("ı"), "dotless i from Turkish case mapping");
		}
	}
}
