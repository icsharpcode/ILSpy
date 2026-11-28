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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
	/// <summary>
	/// Stores a file name as the content of an element in the settings file.
	/// </summary>
	/// <remarks>
	/// A file name can contain characters that XML cannot represent, even as a character
	/// reference. Such names are stored using the \xHHHH format and marked with an attribute.
	/// The marker distinguishes escaped values from existing paths that happen to look like
	/// escapes.
	/// </remarks>
	public static class FileNameXml
	{
		const string EscapedAttribute = "Escaped";

		static readonly Regex EscapedCharPattern = new(@"\\x(?<num>[0-9A-Fa-f]{4})", RegexOptions.Compiled);

		/// <summary>
		/// Gets the XML content for <paramref name="fileName"/>.
		/// </summary>
		public static object ToXmlContent(string fileName)
		{
			if (!NeedsEscaping(fileName))
				return fileName;
			return new object[] { new XAttribute(EscapedAttribute, true), Escape(fileName) };
		}

		/// <summary>
		/// Gets the file name stored in <paramref name="element"/>.
		/// </summary>
		public static string FromXml(XElement element)
		{
			if ((string?)element.Attribute(EscapedAttribute) != "true")
				return element.Value;
			return EscapedCharPattern.Replace(element.Value,
				m => ((char)int.Parse(m.Groups["num"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture)).ToString());
		}

		static bool NeedsEscaping(string fileName)
		{
			for (int i = 0; i < fileName.Length; i++)
			{
				if (char.IsSurrogatePair(fileName, i))
					i++;
				else if (NeedsEscaping(fileName[i]))
					return true;
			}
			return false;
		}

		// XML normalizes line breaks and cannot represent control characters or unpaired surrogates.
		static bool NeedsEscaping(char ch) => ch < ' ' || !XmlConvert.IsXmlChar(ch);

		static string Escape(string fileName)
		{
			var builder = new StringBuilder(fileName.Length);
			for (int i = 0; i < fileName.Length; i++)
			{
				char ch = fileName[i];
				if (char.IsSurrogatePair(fileName, i))
					builder.Append(ch).Append(fileName[++i]);
				else if (ch == '\\' || NeedsEscaping(ch))
					builder.AppendFormat(CultureInfo.InvariantCulture, @"\x{0:X4}", (int)ch);
				else
					builder.Append(ch);
			}
			return builder.ToString();
		}
	}
}
