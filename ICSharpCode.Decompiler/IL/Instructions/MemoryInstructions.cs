// Copyright (c) 2014 Daniel Grunwald
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
	interface ISupportsUnalignedPrefix
	{
		/// <summary>
		/// Returns the alignment specified by the 'unaligned' prefix; or 0 if there was no 'unaligned' prefix.
		/// </summary>
		byte UnalignedPrefix { get; set; }
	}

	interface ISupportsVolatilePrefix
	{
		/// <summary>
		/// Gets/Sets whether the memory access is volatile.
		/// </summary>
		bool IsVolatile { get; set; }
	}

	partial class LdObj
	{
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			if (options.UseFieldSugar)
			{
				if (this.MatchLdFld(out var target, out var field))
				{
					WriteILRange(output, options);
					output.Write("ldfld ");
					field.WriteTo(output);
					output.Write('(');
					target.WriteTo(output, options);
					output.Write(')');
					return;
				}
				else if (this.MatchLdsFld(out field))
				{
					WriteILRange(output, options);
					output.Write("ldsfld ");
					field.WriteTo(output);
					return;
				}
			}
			OriginalWriteTo(output, options);
		}
	}

	partial class StObj
	{
		public override void WriteTo(ITextOutput output, ILAstWritingOptions options)
		{
			if (options.UseFieldSugar)
			{
				if (this.MatchStFld(out var target, out var field, out var value))
				{
					WriteILRange(output, options);
					output.Write("stfld ");
					field.WriteTo(output);
					output.Write('(');
					target.WriteTo(output, options);
					output.Write(", ");
					value.WriteTo(output, options);
					output.Write(')');
					return;
				}
				else if (this.MatchStsFld(out field, out value))
				{
					WriteILRange(output, options);
					output.Write("stsfld ");
					field.WriteTo(output);
					output.Write('(');
					value.WriteTo(output, options);
					output.Write(')');
					return;
				}
			}
			OriginalWriteTo(output, options);
		}
	}
}
