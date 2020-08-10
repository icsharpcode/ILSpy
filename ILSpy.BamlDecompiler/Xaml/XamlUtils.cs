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

using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ILSpy.BamlDecompiler.Xaml {
	internal static class XamlUtils {
		public static string Escape(string value) {
			if (value.Length == 0)
				return value;
			if (value[0] == '{')
				return "{}" + value;
			return value;
		}

		public static string ToString(this XamlContext ctx, XElement elem, XamlType type) {
			type.ResolveNamespace(elem, ctx);
			return ctx.ToString(elem, type.ToXName(ctx));
		}

		public static string ToString(this XamlContext ctx, XElement elem, XName name) {
			var sb = new StringBuilder();
			if (name.Namespace != elem.GetDefaultNamespace()) {
				var prefix = elem.GetPrefixOfNamespace(name.Namespace);
				if (!string.IsNullOrEmpty(prefix)) {
					sb.Append(prefix);
					sb.Append(':');
				}
			}
			sb.Append(name.LocalName);
			return sb.ToString();
		}

		public static double ReadXamlDouble(this BinaryReader reader, bool scaledInt = false) {
			if (!scaledInt) {
				switch (reader.ReadByte()) {
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
			foreach (char ch in name) {
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