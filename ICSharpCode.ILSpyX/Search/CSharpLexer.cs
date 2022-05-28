// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ICSharpCode.ILSpyX.Search
{
	class LATextReader : TextReader
	{
		List<int> buffer;
		TextReader reader;

		public LATextReader(TextReader reader)
		{
			this.buffer = new List<int>();
			this.reader = reader;
		}

		public override int Peek()
		{
			return Peek(0);
		}

		public override int Read()
		{
			int c = Peek();
			buffer.RemoveAt(0);
			return c;
		}

		public int Peek(int step)
		{
			while (step >= buffer.Count)
			{
				buffer.Add(reader.Read());
			}

			if (step < 0)
				return -1;

			return buffer[step];
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				reader.Dispose();
			base.Dispose(disposing);
		}
	}

	enum LiteralFormat : byte
	{
		None,
		DecimalNumber,
		HexadecimalNumber,
		OctalNumber,
		StringLiteral,
		VerbatimStringLiteral,
		CharLiteral
	}

	class Literal
	{
		internal readonly LiteralFormat literalFormat;
		internal readonly object literalValue;
		internal readonly string val;
		internal Literal next;

		public LiteralFormat LiteralFormat {
			get { return literalFormat; }
		}

		public object LiteralValue {
			get { return literalValue; }
		}

		public string Value {
			get { return val; }
		}

		public Literal(string val, object literalValue, LiteralFormat literalFormat)
		{
			this.val = val;
			this.literalValue = literalValue;
			this.literalFormat = literalFormat;
		}
	}

	internal abstract class AbstractLexer : IDisposable
	{
		LATextReader reader;
		int col = 1;
		int line = 1;

		protected Literal lastToken = null;
		protected Literal curToken = null;
		protected Literal peekToken = null;

		protected StringBuilder sb = new StringBuilder();

		// used for the original value of strings (with escape sequences).
		protected StringBuilder originalValue = new StringBuilder();

		protected int Line {
			get {
				return line;
			}
		}
		protected int Col {
			get {
				return col;
			}
		}

		protected bool recordRead = false;
		protected StringBuilder recordedText = new StringBuilder();

		protected int ReaderRead()
		{
			int val = reader.Read();
			if (recordRead && val >= 0)
				recordedText.Append((char)val);
			if ((val == '\r' && reader.Peek() != '\n') || val == '\n')
			{
				++line;
				col = 1;
				LineBreak();
			}
			else if (val >= 0)
			{
				col++;
			}
			return val;
		}

		protected int ReaderPeek()
		{
			return reader.Peek();
		}

		protected int ReaderPeek(int step)
		{
			return reader.Peek(step);
		}

		protected void ReaderSkip(int steps)
		{
			for (int i = 0; i < steps; i++)
			{
				ReaderRead();
			}
		}

		protected string ReaderPeekString(int length)
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < length; i++)
			{
				int peek = ReaderPeek(i);
				if (peek != -1)
					builder.Append((char)peek);
			}

			return builder.ToString();
		}

		/// <summary>
		/// The current Token. <seealso cref="ICSharpCode.NRefactory.Parser.Token"/>
		/// </summary>
		public Literal Token {
			get {
				return lastToken;
			}
		}

		/// <summary>
		/// The next Token (The <see cref="Token"/> after <see cref="NextToken"/> call) . <seealso cref="ICSharpCode.NRefactory.Parser.Token"/>
		/// </summary>
		public Literal LookAhead {
			get {
				return curToken;
			}
		}

		/// <summary>
		/// Constructor for the abstract lexer class.
		/// </summary>
		protected AbstractLexer(TextReader reader)
		{
			this.reader = new LATextReader(reader);
		}

		#region System.IDisposable interface implementation
		public virtual void Dispose()
		{
			reader.Close();
			reader = null;
			lastToken = curToken = peekToken = null;
			sb = originalValue = null;
		}
		#endregion

		/// <summary>
		/// Must be called before a peek operation.
		/// </summary>
		public void StartPeek()
		{
			peekToken = curToken;
		}

		/// <summary>
		/// Gives back the next token. A second call to Peek() gives the next token after the last call for Peek() and so on.
		/// </summary>
		/// <returns>An <see cref="Token"/> object.</returns>
		public Literal Peek()
		{
			//			Console.WriteLine("Call to Peek");
			if (peekToken.next == null)
			{
				peekToken.next = Next();
			}
			peekToken = peekToken.next;
			return peekToken;
		}

		/// <summary>
		/// Reads the next token and gives it back.
		/// </summary>
		/// <returns>An <see cref="Token"/> object.</returns>
		public virtual Literal NextToken()
		{
			if (curToken == null)
			{
				curToken = Next();
				//Console.WriteLine(ICSharpCode.NRefactory.Parser.CSharp.Tokens.GetTokenString(curToken.kind) + " -- " + curToken.val + "(" + curToken.kind + ")");
				return curToken;
			}

			lastToken = curToken;

			if (curToken.next == null)
			{
				curToken.next = Next();
			}

			curToken = curToken.next;
			//Console.WriteLine(ICSharpCode.NRefactory.Parser.CSharp.Tokens.GetTokenString(curToken.kind) + " -- " + curToken.val + "(" + curToken.kind + ")");
			return curToken;
		}

		protected abstract Literal Next();

		protected static bool IsIdentifierPart(int ch)
		{
			if (ch == 95)
				return true;  // 95 = '_'
			if (ch == -1)
				return false;
			return char.IsLetterOrDigit((char)ch); // accept unicode letters
		}

		protected static bool IsHex(char digit)
		{
			return Char.IsDigit(digit) || ('A' <= digit && digit <= 'F') || ('a' <= digit && digit <= 'f');
		}

		protected int GetHexNumber(char digit)
		{
			if (Char.IsDigit(digit))
			{
				return digit - '0';
			}
			if ('A' <= digit && digit <= 'F')
			{
				return digit - 'A' + 0xA;
			}
			if ('a' <= digit && digit <= 'f')
			{
				return digit - 'a' + 0xA;
			}
			return 0;
		}
		protected void LineBreak()
		{
		}
		protected bool HandleLineEnd(char ch)
		{
			// Handle MS-DOS or MacOS line ends.
			if (ch == '\r')
			{
				if (reader.Peek() == '\n')
				{ // MS-DOS line end '\r\n'
					ReaderRead(); // LineBreak (); called by ReaderRead ();
					return true;
				}
				else
				{ // assume MacOS line end which is '\r'
					LineBreak();
					return true;
				}
			}
			if (ch == '\n')
			{
				LineBreak();
				return true;
			}
			return false;
		}

		protected void SkipToEndOfLine()
		{
			int nextChar;
			while ((nextChar = reader.Read()) != -1)
			{
				if (nextChar == '\r')
				{
					if (reader.Peek() == '\n')
						reader.Read();
					nextChar = '\n';
				}
				if (nextChar == '\n')
				{
					++line;
					col = 1;
					break;
				}
			}
		}

		protected string ReadToEndOfLine()
		{
			sb.Length = 0;
			int nextChar;
			while ((nextChar = reader.Read()) != -1)
			{
				char ch = (char)nextChar;

				if (nextChar == '\r')
				{
					if (reader.Peek() == '\n')
						reader.Read();
					nextChar = '\n';
				}
				// Return read string, if EOL is reached
				if (nextChar == '\n')
				{
					++line;
					col = 1;
					return sb.ToString();
				}

				sb.Append(ch);
			}

			// Got EOF before EOL
			string retStr = sb.ToString();
			col += retStr.Length;
			return retStr;
		}
	}

	internal sealed class Lexer : AbstractLexer
	{
		public Lexer(TextReader reader) : base(reader)
		{
		}

		protected override Literal Next()
		{
			char ch;
			while (true)
			{
				int nextChar = ReaderRead();
				if (nextChar == -1)
					break;

				Literal token = null;

				switch (nextChar)
				{
					case ' ':
					case '\t':
						continue;
					case '\r':
					case '\n':
						HandleLineEnd((char)nextChar);
						continue;
					case '"':
						token = ReadString();
						break;
					case '\'':
						token = ReadChar();
						break;
					case '@':
						int next = ReaderRead();
						if (next == -1)
						{
							Error(Line, Col, String.Format("EOF after @"));
							continue;
						}
						else
						{
							int x = Col - 1;
							int y = Line;
							ch = (char)next;
							if (ch == '"')
							{
								token = ReadVerbatimString();
							}
							else if (Char.IsLetterOrDigit(ch) || ch == '_')
							{
								bool canBeKeyword;
								string s = ReadIdent(ch, out canBeKeyword);
								return new Literal(null, null, LiteralFormat.None);
							}
							else
							{
								HandleLineEnd(ch);
								Error(y, x, String.Format("Unexpected char in Lexer.Next() : {0}", ch));
								continue;
							}
						}
						break;
					default: // non-ws chars are handled here
						ch = (char)nextChar;
						if (Char.IsLetter(ch) || ch == '_' || ch == '\\')
						{
							int x = Col - 1; // Col was incremented above, but we want the start of the identifier
							int y = Line;
							bool canBeKeyword;
							string s = ReadIdent(ch, out canBeKeyword);
							return new Literal(null, null, LiteralFormat.None);
						}
						else if (Char.IsDigit(ch))
						{
							token = ReadDigit(ch, Col - 1);
						}
						break;
				}

				// try error recovery (token = null -> continue with next char)
				if (token != null)
				{
					return token;
				}
			}

			return new Literal(null, null, LiteralFormat.None);
		}

		// The C# compiler has a fixed size length therefore we'll use a fixed size char array for identifiers
		// it's also faster than using a string builder.
		const int MAX_IDENTIFIER_LENGTH = 512;
		char[] identBuffer = new char[MAX_IDENTIFIER_LENGTH];

		string ReadIdent(char ch, out bool canBeKeyword)
		{
			int peek;
			int curPos = 0;
			canBeKeyword = true;
			while (true)
			{
				if (ch == '\\')
				{
					peek = ReaderPeek();
					if (peek != 'u' && peek != 'U')
					{
						Error(Line, Col, "Identifiers can only contain unicode escape sequences");
					}
					canBeKeyword = false;
					string surrogatePair;
					ReadEscapeSequence(out ch, out surrogatePair);
					if (surrogatePair != null)
					{
						if (!char.IsLetterOrDigit(surrogatePair, 0))
						{
							Error(Line, Col, "Unicode escape sequences in identifiers cannot be used to represent characters that are invalid in identifiers");
						}
						for (int i = 0; i < surrogatePair.Length - 1; i++)
						{
							if (curPos < MAX_IDENTIFIER_LENGTH)
							{
								identBuffer[curPos++] = surrogatePair[i];
							}
						}
						ch = surrogatePair[surrogatePair.Length - 1];
					}
					else
					{
						if (!IsIdentifierPart(ch))
						{
							Error(Line, Col, "Unicode escape sequences in identifiers cannot be used to represent characters that are invalid in identifiers");
						}
					}
				}

				if (curPos < MAX_IDENTIFIER_LENGTH)
				{
					if (ch != '\0') // only add character, if it is valid
									// prevents \ from being added
						identBuffer[curPos++] = ch;
				}
				else
				{
					Error(Line, Col, String.Format("Identifier too long"));
					while (IsIdentifierPart(ReaderPeek()))
					{
						ReaderRead();
					}
					break;
				}
				peek = ReaderPeek();
				if (IsIdentifierPart(peek) || peek == '\\')
				{
					ch = (char)ReaderRead();
				}
				else
				{
					break;
				}
			}
			return new String(identBuffer, 0, curPos);
		}

		Literal ReadDigit(char ch, int x)
		{
			unchecked
			{ // prevent exception when ReaderPeek() = -1 is cast to char
				int y = Line;
				sb.Length = 0;
				sb.Append(ch);
				string prefix = null;
				string suffix = null;

				bool ishex = false;
				bool isunsigned = false;
				bool islong = false;
				bool isfloat = false;
				bool isdouble = false;
				bool isdecimal = false;

				char peek = (char)ReaderPeek();

				if (ch == '.')
				{
					isdouble = true;

					while (Char.IsDigit((char)ReaderPeek()))
					{ // read decimal digits beyond the dot
						sb.Append((char)ReaderRead());
					}
					peek = (char)ReaderPeek();
				}
				else if (ch == '0' && (peek == 'x' || peek == 'X'))
				{
					ReaderRead(); // skip 'x'
					sb.Length = 0; // Remove '0' from 0x prefix from the stringvalue
					while (IsHex((char)ReaderPeek()))
					{
						sb.Append((char)ReaderRead());
					}
					if (sb.Length == 0)
					{
						sb.Append('0'); // dummy value to prevent exception
						Error(y, x, "Invalid hexadecimal integer literal");
					}
					ishex = true;
					prefix = "0x";
					peek = (char)ReaderPeek();
				}
				else
				{
					while (Char.IsDigit((char)ReaderPeek()))
					{
						sb.Append((char)ReaderRead());
					}
					peek = (char)ReaderPeek();
				}

				Literal nextToken = null; // if we accidently read a 'dot'
				if (peek == '.')
				{ // read floating point number
					ReaderRead();
					peek = (char)ReaderPeek();
					if (!Char.IsDigit(peek))
					{
						nextToken = new Literal(null, null, LiteralFormat.None);
						peek = '.';
					}
					else
					{
						isdouble = true; // double is default
						if (ishex)
						{
							Error(y, x, "No hexadecimal floating point values allowed");
						}
						sb.Append('.');

						while (Char.IsDigit((char)ReaderPeek()))
						{ // read decimal digits beyond the dot
							sb.Append((char)ReaderRead());
						}
						peek = (char)ReaderPeek();
					}
				}

				if (peek == 'e' || peek == 'E')
				{ // read exponent
					isdouble = true;
					sb.Append((char)ReaderRead());
					peek = (char)ReaderPeek();
					if (peek == '-' || peek == '+')
					{
						sb.Append((char)ReaderRead());
					}
					while (Char.IsDigit((char)ReaderPeek()))
					{ // read exponent value
						sb.Append((char)ReaderRead());
					}
					isunsigned = true;
					peek = (char)ReaderPeek();
				}

				if (peek == 'f' || peek == 'F')
				{ // float value
					ReaderRead();
					suffix = "f";
					isfloat = true;
				}
				else if (peek == 'd' || peek == 'D')
				{ // double type suffix (obsolete, double is default)
					ReaderRead();
					suffix = "d";
					isdouble = true;
				}
				else if (peek == 'm' || peek == 'M')
				{ // decimal value
					ReaderRead();
					suffix = "m";
					isdecimal = true;
				}
				else if (!isdouble)
				{
					if (peek == 'u' || peek == 'U')
					{
						ReaderRead();
						suffix = "u";
						isunsigned = true;
						peek = (char)ReaderPeek();
					}

					if (peek == 'l' || peek == 'L')
					{
						ReaderRead();
						peek = (char)ReaderPeek();
						islong = true;
						if (!isunsigned && (peek == 'u' || peek == 'U'))
						{
							ReaderRead();
							suffix = "Lu";
							isunsigned = true;
						}
						else
						{
							suffix = isunsigned ? "uL" : "L";
						}
					}
				}

				string digit = sb.ToString();
				string stringValue = prefix + digit + suffix;

				if (isfloat)
				{
					float num;
					if (float.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
					{
						return new Literal(stringValue, num, LiteralFormat.DecimalNumber);
					}
					else
					{
						Error(y, x, String.Format("Can't parse float {0}", digit));
						return new Literal(stringValue, 0f, LiteralFormat.DecimalNumber);
					}
				}
				if (isdecimal)
				{
					decimal num;
					if (decimal.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
					{
						return new Literal(stringValue, num, LiteralFormat.DecimalNumber);
					}
					else
					{
						Error(y, x, String.Format("Can't parse decimal {0}", digit));
						return new Literal(stringValue, 0m, LiteralFormat.DecimalNumber);
					}
				}
				if (isdouble)
				{
					double num;
					if (double.TryParse(digit, NumberStyles.Any, CultureInfo.InvariantCulture, out num))
					{
						return new Literal(stringValue, num, LiteralFormat.DecimalNumber);
					}
					else
					{
						Error(y, x, String.Format("Can't parse double {0}", digit));
						return new Literal(stringValue, 0d, LiteralFormat.DecimalNumber);
					}
				}

				// Try to determine a parsable value using ranges.
				ulong result;
				if (ishex)
				{
					if (!ulong.TryParse(digit, NumberStyles.HexNumber, null, out result))
					{
						Error(y, x, String.Format("Can't parse hexadecimal constant {0}", digit));
						return new Literal(stringValue.ToString(), 0, LiteralFormat.HexadecimalNumber);
					}
				}
				else
				{
					if (!ulong.TryParse(digit, NumberStyles.Integer, null, out result))
					{
						Error(y, x, String.Format("Can't parse integral constant {0}", digit));
						return new Literal(stringValue.ToString(), 0, LiteralFormat.DecimalNumber);
					}
				}

				if (result > long.MaxValue)
				{
					islong = true;
					isunsigned = true;
				}
				else if (result > uint.MaxValue)
				{
					islong = true;
				}
				else if (islong == false && result > int.MaxValue)
				{
					isunsigned = true;
				}

				Literal token;

				LiteralFormat literalFormat = ishex ? LiteralFormat.HexadecimalNumber : LiteralFormat.DecimalNumber;
				if (islong)
				{
					if (isunsigned)
					{
						ulong num;
						if (ulong.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num))
						{
							token = new Literal(stringValue, num, literalFormat);
						}
						else
						{
							Error(y, x, String.Format("Can't parse unsigned long {0}", digit));
							token = new Literal(stringValue, 0UL, literalFormat);
						}
					}
					else
					{
						long num;
						if (long.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num))
						{
							token = new Literal(stringValue, num, literalFormat);
						}
						else
						{
							Error(y, x, String.Format("Can't parse long {0}", digit));
							token = new Literal(stringValue, 0L, literalFormat);
						}
					}
				}
				else
				{
					if (isunsigned)
					{
						uint num;
						if (uint.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num))
						{
							token = new Literal(stringValue, num, literalFormat);
						}
						else
						{
							Error(y, x, String.Format("Can't parse unsigned int {0}", digit));
							token = new Literal(stringValue, (uint)0, literalFormat);
						}
					}
					else
					{
						int num;
						if (int.TryParse(digit, ishex ? NumberStyles.HexNumber : NumberStyles.Number, CultureInfo.InvariantCulture, out num))
						{
							token = new Literal(stringValue, num, literalFormat);
						}
						else
						{
							Error(y, x, String.Format("Can't parse int {0}", digit));
							token = new Literal(stringValue, 0, literalFormat);
						}
					}
				}
				token.next = nextToken;
				return token;
			}
		}

		Literal ReadString()
		{
			int x = Col - 1;
			int y = Line;

			sb.Length = 0;
			originalValue.Length = 0;
			originalValue.Append('"');
			bool doneNormally = false;
			int nextChar;
			while ((nextChar = ReaderRead()) != -1)
			{
				char ch = (char)nextChar;

				if (ch == '"')
				{
					doneNormally = true;
					originalValue.Append('"');
					break;
				}

				if (ch == '\\')
				{
					originalValue.Append('\\');
					string surrogatePair;
					originalValue.Append(ReadEscapeSequence(out ch, out surrogatePair));
					if (surrogatePair != null)
					{
						sb.Append(surrogatePair);
					}
					else
					{
						sb.Append(ch);
					}
				}
				else if (HandleLineEnd(ch))
				{
					// call HandleLineEnd to ensure line numbers are still correct after the error
					Error(y, x, "No new line is allowed inside a string literal");
					break;
				}
				else
				{
					originalValue.Append(ch);
					sb.Append(ch);
				}
			}

			if (!doneNormally)
			{
				Error(y, x, "End of file reached inside string literal");
			}

			return new Literal(originalValue.ToString(), sb.ToString(), LiteralFormat.StringLiteral);
		}

		Literal ReadVerbatimString()
		{
			sb.Length = 0;
			originalValue.Length = 0;
			originalValue.Append("@\"");
			int nextChar;
			while ((nextChar = ReaderRead()) != -1)
			{
				char ch = (char)nextChar;

				if (ch == '"')
				{
					if (ReaderPeek() != '"')
					{
						originalValue.Append('"');
						break;
					}
					originalValue.Append("\"\"");
					sb.Append('"');
					ReaderRead();
				}
				else if (HandleLineEnd(ch))
				{
					sb.Append("\r\n");
					originalValue.Append("\r\n");
				}
				else
				{
					sb.Append(ch);
					originalValue.Append(ch);
				}
			}

			if (nextChar == -1)
			{
				Error(Line, Col, "End of file reached inside verbatim string literal");
			}

			return new Literal(originalValue.ToString(), sb.ToString(), LiteralFormat.VerbatimStringLiteral);
		}

		readonly char[] escapeSequenceBuffer = new char[12];

		/// <summary>
		/// reads an escape sequence
		/// </summary>
		/// <param name="ch">The character represented by the escape sequence,
		/// or '\0' if there was an error or the escape sequence represents a character that
		/// can be represented only be a suggorate pair</param>
		/// <param name="surrogatePair">Null, except when the character represented
		/// by the escape sequence can only be represented by a surrogate pair (then the string
		/// contains the surrogate pair)</param>
		/// <returns>The escape sequence</returns>
		string ReadEscapeSequence(out char ch, out string surrogatePair)
		{
			surrogatePair = null;

			int nextChar = ReaderRead();
			if (nextChar == -1)
			{
				Error(Line, Col, "End of file reached inside escape sequence");
				ch = '\0';
				return String.Empty;
			}
			int number;
			char c = (char)nextChar;
			int curPos = 1;
			escapeSequenceBuffer[0] = c;
			switch (c)
			{
				case '\'':
					ch = '\'';
					break;
				case '\"':
					ch = '\"';
					break;
				case '\\':
					ch = '\\';
					break;
				case '0':
					ch = '\0';
					break;
				case 'a':
					ch = '\a';
					break;
				case 'b':
					ch = '\b';
					break;
				case 'f':
					ch = '\f';
					break;
				case 'n':
					ch = '\n';
					break;
				case 'r':
					ch = '\r';
					break;
				case 't':
					ch = '\t';
					break;
				case 'v':
					ch = '\v';
					break;
				case 'u':
				case 'x':
					// 16 bit unicode character
					c = (char)ReaderRead();
					number = GetHexNumber(c);
					escapeSequenceBuffer[curPos++] = c;

					if (number < 0)
					{
						Error(Line, Col - 1, String.Format("Invalid char in literal : {0}", c));
					}
					for (int i = 0; i < 3; ++i)
					{
						if (IsHex((char)ReaderPeek()))
						{
							c = (char)ReaderRead();
							int idx = GetHexNumber(c);
							escapeSequenceBuffer[curPos++] = c;
							number = 16 * number + idx;
						}
						else
						{
							break;
						}
					}
					ch = (char)number;
					break;
				case 'U':
					// 32 bit unicode character
					number = 0;
					for (int i = 0; i < 8; ++i)
					{
						if (IsHex((char)ReaderPeek()))
						{
							c = (char)ReaderRead();
							int idx = GetHexNumber(c);
							escapeSequenceBuffer[curPos++] = c;
							number = 16 * number + idx;
						}
						else
						{
							Error(Line, Col - 1, String.Format("Invalid char in literal : {0}", (char)ReaderPeek()));
							break;
						}
					}
					if (number > 0xffff)
					{
						ch = '\0';
						surrogatePair = char.ConvertFromUtf32(number);
					}
					else
					{
						ch = (char)number;
					}
					break;
				default:
					Error(Line, Col, String.Format("Unexpected escape sequence : {0}", c));
					ch = '\0';
					break;
			}
			return new String(escapeSequenceBuffer, 0, curPos);
		}

		Literal ReadChar()
		{
			int x = Col - 1;
			int y = Line;
			int nextChar = ReaderRead();
			if (nextChar == -1 || HandleLineEnd((char)nextChar))
			{
				return null;
			}
			char ch = (char)nextChar;
			char chValue = ch;
			string escapeSequence = String.Empty;
			if (ch == '\\')
			{
				string surrogatePair;
				escapeSequence = ReadEscapeSequence(out chValue, out surrogatePair);
				if (surrogatePair != null)
				{
					Error(y, x, "The unicode character must be represented by a surrogate pair and does not fit into a System.Char");
				}
			}

			unchecked
			{
				if ((char)ReaderRead() != '\'')
				{
					Error(y, x, "Char not terminated");
				}
			}
			return new Literal("'" + ch + escapeSequence + "'", chValue, LiteralFormat.CharLiteral);
		}

		void Error(int y, int x, string message)
		{
		}
	}
}
