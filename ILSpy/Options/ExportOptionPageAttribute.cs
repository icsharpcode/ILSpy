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
using System.Composition;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Marks a class as an Options-tab panel. MEF discovers all such exports under the
	/// shared contract "OptionPages" with metadata <see cref="IOptionsMetadata"/>; the
	/// Options host orders them by <see cref="Order"/> and renders them as inner tabs.
	/// </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ExportOptionPageAttribute() : ExportAttribute("OptionPages", typeof(IOptionPage))
	{
		// Public property is reflected by System.Composition into the IOptionsMetadata
		// metadata view (matched by name). The metadata view is a concrete class in this
		// MEF host, so there's no "implements IOptionsMetadata" — the wire-up is purely
		// nominal property matching.
		public int Order { get; set; }
	}
}
