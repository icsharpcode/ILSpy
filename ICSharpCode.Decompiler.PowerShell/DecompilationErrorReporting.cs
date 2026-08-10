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

using System.Collections.Generic;
using System.Management.Automation;

namespace ICSharpCode.Decompiler.PowerShell
{
	static class DecompilationErrorReporting
	{
		/// <summary>
		/// Raises one non-terminating error per member the decompiler could not handle. The output
		/// is produced either way - with the error text in place of the affected code - so without
		/// this a script would take known-broken source for a clean decompilation.
		/// </summary>
		public static void WriteDecompilationErrors(this Cmdlet cmdlet, IReadOnlyList<DecompilerException> errors)
		{
			foreach (var error in errors)
			{
				cmdlet.WriteError(new ErrorRecord(error, ErrorIds.DecompilationFailed, ErrorCategory.NotSpecified, null));
			}
		}
	}
}
