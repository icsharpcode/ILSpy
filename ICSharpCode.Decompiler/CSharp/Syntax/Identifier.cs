// 
// Identifier.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#nullable enable

using System;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <code>
	/// identifier ::=
	///       Simple_Identifier
	///     | contextual_keyword
	///     | discard_token
	/// </code>
	/// (C# lexical grammar §6.4.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class Identifier : AstNode
	{
		string name;
		public string Name {
			get { return this.name; }
			set {
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				this.name = value;
			}
		}

		TextLocation startLocation;
		public override TextLocation StartLocation {
			get {
				return startLocation;
			}
		}

		internal void SetStartLocation(TextLocation value)
		{
			this.startLocation = value;
		}

		// The @-escaping is a lexical detail, not structural; exclude it from matching.
		[ExcludeFromMatch]
		public bool IsVerbatim { get; set; }

		public override TextLocation EndLocation {
			get {
				return new TextLocation(StartLocation.Line, StartLocation.Column + (Name ?? "").Length + (IsVerbatim ? 1 : 0));
			}
		}

		Identifier()
		{
			this.name = string.Empty;
		}

		private Identifier(string name, TextLocation location)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			this.name = name;
			this.startLocation = location;
		}

		public static Identifier Create(string name)
		{
			return Create(name, TextLocation.Empty);
		}

		// Convenience for optional names (a [Slot] string? property): an empty or null name maps to a
		// null token (no name), any other name to an Identifier.
		public static Identifier? CreateIfNotEmpty(string? name)
		{
			return string.IsNullOrEmpty(name) ? null : Create(name);
		}

		public static Identifier Create(string name, TextLocation location)
		{
			if (string.IsNullOrEmpty(name))
				return new Identifier(string.Empty, location);
			if (name[0] == '@')
				return new Identifier(name.Substring(1), new TextLocation(location.Line, location.Column + 1)) { IsVerbatim = true };
			else
				return new Identifier(name, location);
		}

		public static Identifier Create(string name, TextLocation location, bool isVerbatim)
		{
			if (string.IsNullOrEmpty(name))
				return new Identifier(string.Empty, location);

			if (isVerbatim)
				return new Identifier(name, location) { IsVerbatim = true };
			return new Identifier(name, location);
		}
	}
}