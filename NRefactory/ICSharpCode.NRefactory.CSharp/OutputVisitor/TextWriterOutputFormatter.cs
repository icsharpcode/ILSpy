// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Globalization;
using System.IO;
using System.Text;

namespace ICSharpCode.NRefactory.CSharp
{
	/// <summary>
	/// Writes C# code into a TextWriter.
	/// </summary>
	public class TextWriterTokenWriter : TokenWriter, ILocatable
	{
		readonly TextWriter textWriter;
		int indentation;
		bool needsIndent = true;
		bool isAtStartOfLine = true;
		int line, column;

		public int Indentation
		{
			get { return this.indentation; }
			set { this.indentation = value; }
		}

		public TextLocation Location
		{
			get { return new TextLocation(line, column + (needsIndent ? indentation * IndentationString.Length : 0)); }
		}

		public string IndentationString { get; set; }

		public TextWriterTokenWriter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException("textWriter");
			this.textWriter = textWriter;
			this.IndentationString = "\t";
			this.line = 1;
			this.column = 1;
		}

		public override void WriteIdentifier(Identifier identifier)
		{
			WriteIndentation();
			if (identifier.IsVerbatim || CSharpOutputVisitor.IsKeyword(identifier.Name, identifier))
			{
				Write('@');
			}
			Write(identifier.Name);
			isAtStartOfLine = false;
		}

		public override void WriteKeyword(Role role, string keyword)
		{
			WriteIndentation();
			Write(keyword);
			isAtStartOfLine = false;
		}

		public override void WriteToken(Role role, string token)
		{
			WriteIndentation();
			Write(token);
			isAtStartOfLine = false;
		}

		public override void Space()
		{
			WriteIndentation();
			Write(' ');
		}

		protected void WriteIndentation()
		{
			if (needsIndent)
			{
				needsIndent = false;
				for (int i = 0; i < indentation; i++)
				{
					Write(this.IndentationString);
				}
			}
		}

		public override void NewLine()
		{
			textWriter.WriteLine();
			column = 1;
			line++;
			needsIndent = true;
			isAtStartOfLine = true;
		}

		public override void Indent()
		{
			indentation++;
		}

		public override void Unindent()
		{
			indentation--;
		}

		public override void WriteComment(CommentType commentType, string content)
		{
			WriteIndentation();
			switch (commentType)
			{
				case CommentType.SingleLine:
					Write("//");
					Write(content);
					textWriter.WriteLine();
					needsIndent = true;
					isAtStartOfLine = true;
					break;
				case CommentType.MultiLine:
					Write("/*");
					textWriter.Write(content);
					UpdateEndLocation(content, ref line, ref column);
					Write("*/");
					isAtStartOfLine = false;
					break;
				case CommentType.Documentation:
					Write("///");
					Write(content);
					textWriter.WriteLine();
					needsIndent = true;
					isAtStartOfLine = true;
					break;
				case CommentType.MultiLineDocumentation:
					Write("/**");
					textWriter.Write(content);
					UpdateEndLocation(content, ref line, ref column);
					Write("*/");
					isAtStartOfLine = false;
					break;
				default:
					Write("content");
					break;
			}
		}

		static void UpdateEndLocation(string content, ref int line, ref int column)
		{
			if (string.IsNullOrEmpty(content))
				return;
			for (int i = 0; i < content.Length; i++)
			{
				char ch = content[i];
				switch (ch)
				{
					case '\r':
						if (i + 1 < content.Length && content[i + 1] == '\n')
							i++;
						goto case '\n';
					case '\n':
						line++;
						column = 0;
						break;
				}
				column++;
			}
		}

		public override void WritePreProcessorDirective(PreProcessorDirectiveType type, string argument)
		{
			// pre-processor directive must start on its own line
			if (!isAtStartOfLine)
				NewLine();
			WriteIndentation();
			Write('#');
			Write(type.ToString().ToLowerInvariant());
			if (!string.IsNullOrEmpty(argument))
			{
				Write(' ');
				Write(argument);
			}
			NewLine();
		}

		public static string PrintPrimitiveValue(object value)
		{
			TextWriter writer = new StringWriter();
			TextWriterTokenWriter tokenWriter = new TextWriterTokenWriter(writer);
			tokenWriter.WritePrimitiveValue(value);
			return writer.ToString();
		}

		public override void WritePrimitiveValue(object value, string literalValue = null)
		{
			if (literalValue != null)
			{
				Write(literalValue);
				return;
			}

			if (value == null)
			{
				// usually NullReferenceExpression should be used for this, but we'll handle it anyways
				Write("null");
				return;
			}

			if (value is bool)
			{
				if ((bool)value)
				{
					Write("true");
				}
				else
				{
					Write("false");
				}
				return;
			}

			if (value is string)
			{
				Write("\"" + ConvertString(value.ToString()) + "\"");
			}
			else if (value is char)
			{
				Write("'" + ConvertCharLiteral((char)value) + "'");
			}
			else if (value is decimal)
			{
				decimal f = (decimal)value;
				if (f == decimal.MinValue || f == decimal.MaxValue)
				{
					Write("decimal");
					WriteToken(Roles.Dot, ".");
					if (f == decimal.MaxValue)
					{
						Write("MaxValue");
					}
					else if (f == decimal.MinValue)
					{
						Write("MinValue");
					}
					return;
				}
				Write(f.ToString(NumberFormatInfo.InvariantInfo) + "m");
			}
			else if (value is float)
			{
				float f = (float)value;
				if (float.IsInfinity(f) || float.IsNaN(f) || f == float.MaxValue || f == float.MinValue || f == float.Epsilon)
				{
					// Strictly speaking, these aren't PrimitiveExpressions;
					// but we still support writing these to make life easier for code generators.
					Write("float");
					WriteToken(Roles.Dot, ".");
					if (float.IsPositiveInfinity(f))
					{
						Write("PositiveInfinity");
					}
					else if (float.IsNegativeInfinity(f))
					{
						Write("NegativeInfinity");
					}
					else if (float.IsNaN(f))
					{
						Write("NaN");
					}
					else if (f == float.MaxValue)
					{
						Write("MaxValue");
					}
					else if (f == float.MinValue)
					{
						Write("MinValue");
					}
					else if (f == float.Epsilon)
					{
						Write("Epsilon");
					}
					return;
				}
				if (f == 0 && 1 / f == float.NegativeInfinity)
				{
					// negative zero is a special case
					// (again, not a primitive expression, but it's better to handle
					// the special case here than to do it in all code generators)
					Write('-');
				}
				string number = f.ToString("R", NumberFormatInfo.InvariantInfo);
				if (number.IndexOf('.') < 0 && number.IndexOf('E') < 0)
				{
					number += ".0";
				}
				number += "f";
				Write(number);
			}
			else if (value is double)
			{
				double f = (double)value;
				if (double.IsInfinity(f) || double.IsNaN(f) || f == double.MaxValue || f == double.MinValue || f == double.Epsilon)
				{
					// Strictly speaking, these aren't PrimitiveExpressions;
					// but we still support writing these to make life easier for code generators.
					Write("double");
					WriteToken(Roles.Dot, ".");
					if (double.IsPositiveInfinity(f))
					{
						Write("PositiveInfinity");
					}
					else if (double.IsNegativeInfinity(f))
					{
						Write("NegativeInfinity");
					}
					else if (double.IsNaN(f))
					{
						Write("NaN");
					}
					else if (f == double.MaxValue)
					{
						Write("MaxValue");
					}
					else if (f == double.MinValue)
					{
						Write("MinValue");
					}
					else if (f == double.Epsilon)
					{
						Write("Epsilon");
					}
					return;
				}
				if (f == 0 && 1 / f == double.NegativeInfinity)
				{
					// negative zero is a special case
					// (again, not a primitive expression, but it's better to handle
					// the special case here than to do it in all code generators)
					Write('-');
				}
				string number = f.ToString("R", NumberFormatInfo.InvariantInfo);
				if (number.IndexOf('.') < 0 && number.IndexOf('E') < 0)
				{
					number += ".0";
				}
				Write(number);
			}
			else if (value is IFormattable)
			{

				if (value is byte && (byte)value == byte.MaxValue)
				{
					Write("byte");
					WriteToken(Roles.Dot, ".");
					Write("MaxValue");
					return;
				}
				else if (value is sbyte)
				{
					sbyte sb = (sbyte)value;
					if (sb == sbyte.MaxValue || sb == sbyte.MinValue)
					{
						Write("sbyte");
						WriteToken(Roles.Dot, ".");
						if (sb == sbyte.MaxValue)
						{
							Write("MaxValue");
						}
						else if (sb == sbyte.MinValue)
						{
							Write("MinValue");
						}
						return;
					}
				}
				else if (value is ushort && (ushort)value == ushort.MaxValue)
				{
					Write("ushort");
					WriteToken(Roles.Dot, ".");
					Write("MaxValue");
					return;
				}
				else if (value is short)
				{
					short s = (short)value;
					if (s == short.MaxValue || s == short.MinValue)
					{
						Write("short");
						WriteToken(Roles.Dot, ".");
						if (s == short.MaxValue)
						{
							Write("MaxValue");
						}
						else if (s == short.MinValue)
						{
							Write("MinValue");
						}
						return;
					}
				}
				else if (value is uint && (uint)value == uint.MaxValue)
				{
					Write("uint");
					WriteToken(Roles.Dot, ".");
					Write("MaxValue");
					return;
				}
				else if (value is int)
				{
					int i = (int)value;
					if (i == int.MaxValue || i == int.MinValue)
					{
						Write("int");
						WriteToken(Roles.Dot, ".");
						if (i == int.MaxValue)
						{
							Write("MaxValue");
						}
						else if (i == int.MinValue)
						{
							Write("MinValue");
						}
						return;
					}
				}
				else if (value is ulong && (ulong)value == ulong.MaxValue)
				{
					Write("ulong");
					WriteToken(Roles.Dot, ".");
					Write("MaxValue");
					return;
				}
				else if (value is long)
				{
					long l = (long)value;
					if (l == long.MaxValue || l == long.MinValue)
					{
						Write("long");
						WriteToken(Roles.Dot, ".");
						if (l == long.MaxValue)
						{
							Write("MaxValue");
						}
						else if (l == long.MinValue)
						{
							Write("MinValue");
						}
						return;
					}
				}

				StringBuilder b = new StringBuilder();
				b.Append(((IFormattable)value).ToString(null, NumberFormatInfo.InvariantInfo));
				if (value is uint || value is ulong)
				{
					b.Append('u');
				}
				if (value is long || value is ulong)
				{
					b.Append('L');
				}
				Write(b.ToString());
			}
			else
			{
				Write(value.ToString());
			}
		}

		/// <summary>
		/// Gets the escape sequence for the specified character within a char literal.
		/// Does not include the single quotes surrounding the char literal.
		/// </summary>
		public static string ConvertCharLiteral(char ch)
		{
			if (ch == '\'')
			{
				return "\\'";
			}
			return ConvertChar(ch);
		}

		/// <summary>
		/// Gets the escape sequence for the specified character.
		/// </summary>
		/// <remarks>This method does not convert ' or ".</remarks>
		static string ConvertChar(char ch)
		{
			switch (ch)
			{
				case '\\':
					return "\\\\";
				case '\0':
					return "\\0";
				case '\a':
					return "\\a";
				case '\b':
					return "\\b";
				case '\f':
					return "\\f";
				case '\n':
					return "\\n";
				case '\r':
					return "\\r";
				case '\t':
					return "\\t";
				case '\v':
					return "\\v";
				default:
					if (char.IsControl(ch) || char.IsSurrogate(ch) ||
						// print all uncommon white spaces as numbers
						(char.IsWhiteSpace(ch) && ch != ' '))
					{
						return "\\u" + ((int)ch).ToString("x4");
					}
					else
					{
						return ch.ToString();
					}
			}
		}

		/// <summary>
		/// Converts special characters to escape sequences within the given string.
		/// </summary>
		public static string ConvertString(string str)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char ch in str)
			{
				if (ch == '"')
				{
					sb.Append("\\\"");
				}
				else
				{
					sb.Append(ConvertChar(ch));
				}
			}
			return sb.ToString();
		}

		public override void WritePrimitiveType(string type)
		{
			Write(type);
			if (type == "new")
			{
				Write("()");
			}
		}

		public override void Write(char c)
		{
			textWriter.Write(c);
			column++;
		}

		public override void Write(string str)
		{
			textWriter.Write(str);
			column += str.Length;
		}

		public override void StartNode(AstNode node)
		{
			// Write out the indentation, so that overrides of this method
			// can rely use the current output length to identify the position of the node
			// in the output.
			WriteIndentation();
		}

		public override void EndNode(AstNode node)
		{
		}
	}
}