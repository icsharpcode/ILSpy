/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.BamlDecompiler.Xaml
{
	internal static class XamlUtils
	{
		static readonly char[] markupExtensionSpecialCharacters = { ',', '=', '\'', '"', '\\' };

		/// <summary>
		/// Quotes an argument of a markup extension if the parser reading the document again would
		/// take part of it for grammar: ',' and '=' separate arguments from one another, a quote
		/// character starts a quoted value, '\' escapes whatever follows it, and whitespace at
		/// either end is dropped. A value carrying none of those is left as it is, because quoting
		/// every value would rewrite every document that never needed it.
		/// <para>
		/// Braces are grammar only where they are unbalanced: a stray '{' opens an extension and a
		/// stray '}' closes the surrounding one, while a matched pair inside a value ("{0:C}",
		/// "Element[{ns}Name]") is read as text and stays unquoted. A value beginning with '{' is
		/// a nested extension that is already written as one, so it is left alone; the "{}" that
		/// escapes a leading brace is not, because inside an extension it would open one.
		/// </para>
		/// </summary>
		public static string QuoteMarkupExtensionValue(string value)
		{
			if (value == null)
				return null;
			if (value.StartsWith("{", StringComparison.Ordinal) && !value.StartsWith("{}", StringComparison.Ordinal))
			{
				return value; // a nested markup extension, already written as one
			}
			if (value.Length > 0
				&& !value.StartsWith("{}", StringComparison.Ordinal)
				&& value.IndexOfAny(markupExtensionSpecialCharacters) < 0
				&& BracesAreBalanced(value)
				&& !char.IsWhiteSpace(value[0])
				&& !char.IsWhiteSpace(value[value.Length - 1]))
			{
				return value;
			}

			var quoted = new StringBuilder(value.Length + 2);
			quoted.Append('\'');
			foreach (char c in value)
			{
				if (c == '\'' || c == '\\')
					quoted.Append('\\');
				quoted.Append(c);
			}
			quoted.Append('\'');
			return quoted.ToString();
		}

		static bool BracesAreBalanced(string value)
		{
			int depth = 0;
			foreach (char c in value)
			{
				if (c == '{')
					depth++;
				else if (c == '}' && --depth < 0)
					return false;
			}
			return depth == 0;
		}

		/// <summary>
		/// Reads the CLR namespace out of a "clr-namespace:Some.Namespace;assembly=Some.Assembly"
		/// declaration. Such a declaration names its CLR namespace itself; the other form of XML
		/// namespace ("http://...") maps to CLR namespaces through XmlnsDefinition attributes
		/// instead, and has none of its own.
		/// </summary>
		public static bool TryParseClrNamespace(string xmlNamespace, out string clrNamespace)
		{
			const string prefix = "clr-namespace:";
			clrNamespace = null;
			if (xmlNamespace == null || !xmlNamespace.StartsWith(prefix, StringComparison.Ordinal))
				return false;
			clrNamespace = xmlNamespace.Substring(prefix.Length);
			int assembly = clrNamespace.IndexOf(';');
			if (assembly >= 0)
				clrNamespace = clrNamespace.Substring(0, assembly);
			return true;
		}

		public static string Escape(string value)
		{
			if (value.Length == 0)
				return value;
			if (value[0] == '{')
				return "{}" + value;
			return value;
		}

		/// <summary>
		/// Escapes the characters XML cannot carry - obfuscators put them into BAML strings, and
		/// XML 1.0 has no representation for them at all, not even a numeric character reference.
		/// The escapes are spelled the way the C# output spells them, so one convention covers
		/// both languages: the short form where C# has one, "\uXXXX" otherwise.
		/// Characters XML can carry - tab, newline, astral characters - are left untouched, and a
		/// literal backslash is not doubled, because XAML itself has no escape syntax to undo.
		/// </summary>
		public static string EscapeInvalidXmlCharacters(string value)
		{
			if (string.IsNullOrEmpty(value))
				return value;

			StringBuilder escaped = null;
			for (int i = 0; i < value.Length; i++)
			{
				char c = value[i];
				if (char.IsHighSurrogate(c) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
				{
					escaped?.Append(c).Append(value[i + 1]);
					i++;
					continue;
				}
				if (XmlConvert.IsXmlChar(c))
				{
					escaped?.Append(c);
					continue;
				}
				escaped ??= new StringBuilder(value.Length).Append(value, 0, i);
				escaped.Append(EscapeChar(c));
			}
			return escaped?.ToString() ?? value;
		}

		static string EscapeChar(char c)
		{
			switch (c)
			{
				case '\0':
					return "\\0";
				case '\a':
					return "\\a";
				case '\b':
					return "\\b";
				case '\f':
					return "\\f";
				case '\v':
					return "\\v";
				default:
					return "\\u" + ((int)c).ToString("x4");
			}
		}

		public static string ToString(this XamlContext ctx, XElement elem, XamlType type)
		{
			type.ResolveNamespace(elem, ctx);
			return ctx.ToString(elem, type.ToXName(ctx));
		}

		public static string ToString(this XamlContext ctx, XElement elem, XName name)
		{
			var sb = new StringBuilder();
			if (name.Namespace != elem.GetDefaultNamespace())
			{
				var prefix = elem.GetPrefixOfNamespace(name.Namespace);
				if (!string.IsNullOrEmpty(prefix))
				{
					sb.Append(prefix);
					sb.Append(':');
				}
			}
			sb.Append(name.LocalName);
			return sb.ToString();
		}

		public static double ReadXamlDouble(this BinaryReader reader, bool scaledInt = false)
		{
			if (!scaledInt)
			{
				switch (reader.ReadByte())
				{
					case 1:
						return 0;
					case 2:
						return 1;
					case 3:
						return -1;
					case 4:
						break;
					case 5:
						return reader.ReadDouble();
					default:
						throw new InvalidDataException("Unknown double type.");
				}
			}
			// Dividing by 1000000.0 is important to get back the original numbers, we can't
			// multiply by the inverse of it (0.000001).
			// (11700684 * 0.000001) != (11700684 / 1000000.0) => 11.700683999999999 != 11.700684
			return reader.ReadInt32() / 1000000.0;
		}

		/// <summary>
		/// Escape characters that cannot be used in XML.
		/// </summary>
		public static StringBuilder EscapeName(StringBuilder sb, string name)
		{
			foreach (char ch in name)
			{
				if (char.IsWhiteSpace(ch) || char.IsControl(ch) || char.IsSurrogate(ch))
					sb.AppendFormat("\\u{0:x4}", (int)ch);
				else
					sb.Append(ch);
			}
			return sb;
		}

		/// <summary>
		/// Escape characters that cannot be displayed in the UI.
		/// </summary>
		public static string EscapeName(string name)
		{
			return EscapeName(new StringBuilder(name.Length), name).ToString();
		}
	}
}