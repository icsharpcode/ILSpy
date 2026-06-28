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

using System.Reflection.Metadata;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// The IL view renders a macro opcode such as "ldarg.0" by writing the mnemonic as an opcode
/// reference with the index omitted, then writing the index as a separate clickable local or
/// parameter reference. The omitSuffix path must keep the mnemonic up to the dot ("ldarg.") so the
/// index is written exactly once; otherwise the opcode reads "ldarg.00".
/// </summary>
[TestFixture]
public class OpCodeReferenceTests
{
	static OpCodeInfo MacroOpCode(ILOpCode opCode) => new(opCode, opCode.GetDisplayName());

	[Test]
	public void WriteReference_OmitSuffix_Keeps_The_Mnemonic_Up_To_The_Dot()
	{
		var output = new AvaloniaEditTextOutput();

		output.WriteReference(MacroOpCode(ILOpCode.Ldarg_0), omitSuffix: true);

		output.GetText().Should().Be("ldarg.");
	}

	[Test]
	public void WriteReference_Without_OmitSuffix_Writes_The_Full_Opcode()
	{
		var output = new AvaloniaEditTextOutput();

		output.WriteReference(MacroOpCode(ILOpCode.Ldarg_0));

		output.GetText().Should().Be("ldarg.0");
	}

	[Test]
	public void Macro_Opcode_Renders_Its_Index_Exactly_Once()
	{
		// Mirrors MethodBodyDisassembler.WriteOpCode for a macro opcode: the mnemonic (index omitted)
		// followed by the index as a local/parameter reference.
		var output = new AvaloniaEditTextOutput();

		output.WriteReference(MacroOpCode(ILOpCode.Ldarg_0), omitSuffix: true);
		output.WriteLocalReference("0", "param_0");

		output.GetText().Should().Be("ldarg.0");
	}
}
