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
using System.Text.RegularExpressions;

using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// LinkElementGenerator that matches a single literal phrase and produces a
	/// <see cref="VisualLineLinkText"/> pointing at a fixed <see cref="Uri"/>. Mirrors the
	/// "MyLinkElementGenerator" pattern from the legacy WPF host.
	/// </summary>
	internal sealed class ResourceLinkGenerator : LinkElementGenerator
	{
		readonly Uri target;

		public ResourceLinkGenerator(string phrase, Uri target)
			: base(new Regex(Regex.Escape(phrase)))
		{
			this.target = target ?? throw new ArgumentNullException(nameof(target));
			// AvaloniaEdit's TextEditorOptions.RequireControlModifierForHyperlinkClick only
			// propagates to its *built-in* LinkElementGenerator (via the
			// IBuiltinElementGenerator.FetchOptions hook the editor invokes from
			// UpdateBuiltinElementGeneratorsFromOptions). User-added subclasses inherit
			// whatever the parameterless base constructor set (default true), so we have to
			// opt out explicitly here.
			RequireControlModifierForClick = false;
		}

		protected override Uri GetUriFromMatch(Match match) => target;
	}
}
