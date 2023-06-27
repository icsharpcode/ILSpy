// Copyright (c) 2022 AlphaSierraPapa for the SharpDevelop Team
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

#nullable enable

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Information used for (optional) progress reporting by the decompiler.
	/// </summary>
	public struct DecompilationProgress
	{
		/// <summary>
		/// The total number of units to process. If set to a value &lt;= 0, an indeterminate progress bar is displayed.
		/// </summary>
		public int TotalUnits;

		/// <summary>
		/// The number of units currently completed. Should be a positive number.
		/// </summary>
		public int UnitsCompleted;

		/// <summary>
		/// Optional information displayed alongside the progress bar.
		/// </summary>
		public string? Status;

		/// <summary>
		/// Optional custom title for the operation.
		/// </summary>
		public string? Title;
	}
}
