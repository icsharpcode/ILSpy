// Copyright (c) 2017 Siegfried Pammer
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


namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// IL using instruction.
	/// Equivalent to:
	/// <code>
	/// stloc v(resourceExpression)
	/// try {
	///    body
	/// } finally {
	///    v?.Dispose();
	/// }
	/// </code>
	/// </summary>
	/// <remarks>
	/// The value of v is undefined after the end of the body block.
	/// </remarks>
	partial class UsingInstruction
	{
		public bool IsAsync { get; set; }

		public bool IsRefStruct { get; set; }

		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			WriteILRange(output, options);
			output.Write("using");
			if (IsAsync) {
				output.Write(".async");
			}
			if (IsRefStruct) {
				output.Write(".ref");
			}
			output.Write(" (");
			Variable.WriteTo(output);
			output.Write(" = ");
			ResourceExpression.WriteTo(output, options);
			output.WriteLine(") {");
			output.Indent();
			Body.WriteTo(output, options);
			output.Unindent();
			output.WriteLine();
			output.Write("}");
		}
	}
}
