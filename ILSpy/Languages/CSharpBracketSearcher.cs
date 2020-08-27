// Copyright (c) 2018 Siegfried Pammer
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
using System.Diagnostics;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Searches matching brackets for C#.
	/// </summary>
	class CSharpBracketSearcher : IBracketSearcher
	{
		string openingBrackets = "([{";
		string closingBrackets = ")]}";

		public BracketSearchResult SearchBracket(IDocument document, int offset)
		{
			if (offset > 0)
			{
				char c = document.GetCharAt(offset - 1);
				int index = openingBrackets.IndexOf(c);
				int otherOffset = -1;
				if (index > -1)
					otherOffset = SearchBracketForward(document, offset, openingBrackets[index], closingBrackets[index]);

				index = closingBrackets.IndexOf(c);
				if (index > -1)
					otherOffset = SearchBracketBackward(document, offset - 2, openingBrackets[index], closingBrackets[index]);

				if (otherOffset > -1)
				{
					var result = new BracketSearchResult(Math.Min(offset - 1, otherOffset), 1,
														 Math.Max(offset - 1, otherOffset), 1);
					return result;
				}
			}

			return null;
		}

		#region SearchBracket helper functions
		static int ScanLineStart(IDocument document, int offset)
		{
			for (int i = offset - 1; i > 0; --i)
			{
				if (document.GetCharAt(i) == '\n')
					return i + 1;
			}
			return 0;
		}

		/// <summary>
		/// Gets the type of code at offset.<br/>
		/// 0 = Code,<br/>
		/// 1 = Comment,<br/>
		/// 2 = String<br/>
		/// Block comments and multiline strings are not supported.
		/// </summary>
		static int GetStartType(IDocument document, int linestart, int offset)
		{
			bool inString = false;
			bool inChar = false;
			bool verbatim = false;
			int result = 0;
			for (int i = linestart; i < offset; i++)
			{
				switch (document.GetCharAt(i))
				{
					case '/':
						if (!inString && !inChar && i + 1 < document.TextLength)
						{
							if (document.GetCharAt(i + 1) == '/')
							{
								result = 1;
							}
						}
						break;
					case '"':
						if (!inChar)
						{
							if (inString && verbatim)
							{
								if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
								{
									++i; // skip escaped quote
									inString = false; // let the string go on
								}
								else
								{
									verbatim = false;
								}
							}
							else if (!inString && i > 0 && document.GetCharAt(i - 1) == '@')
							{
								verbatim = true;
							}
							inString = !inString;
						}
						break;
					case '\'':
						if (!inString)
							inChar = !inChar;
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							++i; // skip next character
						break;
				}
			}

			return (inString || inChar) ? 2 : result;
		}
		#endregion

		#region SearchBracketBackward
		int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			if (offset + 1 >= document.TextLength)
				return -1;
			// this method parses a c# document backwards to find the matching bracket

			// first try "quick find" - find the matching bracket if there is no string/comment in the way
			int quickResult = QuickSearchBracketBackward(document, offset, openBracket, closingBracket);
			if (quickResult >= 0)
				return quickResult;

			// we need to parse the line from the beginning, so get the line start position
			int linestart = ScanLineStart(document, offset + 1);

			// we need to know where offset is - in a string/comment or in normal code?
			// ignore cases where offset is in a block comment
			int starttype = GetStartType(document, linestart, offset + 1);
			if (starttype == 1)
			{
				return -1; // start position is in a comment
			}

			// I don't see any possibility to parse a C# document backwards...
			// We have to do it forwards and push all bracket positions on a stack.
			Stack<int> bracketStack = new Stack<int>();
			bool blockComment = false;
			bool lineComment = false;
			bool inChar = false;
			bool inString = false;
			bool verbatim = false;

			for (int i = 0; i <= offset; ++i)
			{
				char ch = document.GetCharAt(i);
				switch (ch)
				{
					case '\r':
					case '\n':
						lineComment = false;
						inChar = false;
						if (!verbatim)
							inString = false;
						break;
					case '/':
						if (blockComment)
						{
							Debug.Assert(i > 0);
							if (document.GetCharAt(i - 1) == '*')
							{
								blockComment = false;
							}
						}
						if (!inString && !inChar && i + 1 < document.TextLength)
						{
							if (!blockComment && document.GetCharAt(i + 1) == '/')
							{
								lineComment = true;
							}
							if (!lineComment && document.GetCharAt(i + 1) == '*')
							{
								blockComment = true;
							}
						}
						break;
					case '"':
						if (!(inChar || lineComment || blockComment))
						{
							if (inString && verbatim)
							{
								if (i + 1 < document.TextLength && document.GetCharAt(i + 1) == '"')
								{
									++i; // skip escaped quote
									inString = false; // let the string go
								}
								else
								{
									verbatim = false;
								}
							}
							else if (!inString && offset > 0 && document.GetCharAt(i - 1) == '@')
							{
								verbatim = true;
							}
							inString = !inString;
						}
						break;
					case '\'':
						if (!(inString || lineComment || blockComment))
						{
							inChar = !inChar;
						}
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							++i; // skip next character
						break;
					default:
						if (ch == openBracket)
						{
							if (!(inString || inChar || lineComment || blockComment))
							{
								bracketStack.Push(i);
							}
						}
						else if (ch == closingBracket)
						{
							if (!(inString || inChar || lineComment || blockComment))
							{
								if (bracketStack.Count > 0)
									bracketStack.Pop();
							}
						}
						break;
				}
			}
			if (bracketStack.Count > 0)
				return (int)bracketStack.Pop();
			return -1;
		}
		#endregion

		#region SearchBracketForward
		int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			bool inString = false;
			bool inChar = false;
			bool verbatim = false;

			bool lineComment = false;
			bool blockComment = false;

			if (offset < 0)
				return -1;

			// first try "quick find" - find the matching bracket if there is no string/comment in the way
			int quickResult = QuickSearchBracketForward(document, offset, openBracket, closingBracket);
			if (quickResult >= 0)
				return quickResult;

			// we need to parse the line from the beginning, so get the line start position
			int linestart = ScanLineStart(document, offset);

			// we need to know where offset is - in a string/comment or in normal code?
			// ignore cases where offset is in a block comment
			int starttype = GetStartType(document, linestart, offset);
			if (starttype != 0)
				return -1; // start position is in a comment/string

			int brackets = 1;

			while (offset < document.TextLength)
			{
				char ch = document.GetCharAt(offset);
				switch (ch)
				{
					case '\r':
					case '\n':
						lineComment = false;
						inChar = false;
						if (!verbatim)
							inString = false;
						break;
					case '/':
						if (blockComment)
						{
							Debug.Assert(offset > 0);
							if (document.GetCharAt(offset - 1) == '*')
							{
								blockComment = false;
							}
						}
						if (!inString && !inChar && offset + 1 < document.TextLength)
						{
							if (!blockComment && document.GetCharAt(offset + 1) == '/')
							{
								lineComment = true;
							}
							if (!lineComment && document.GetCharAt(offset + 1) == '*')
							{
								blockComment = true;
							}
						}
						break;
					case '"':
						if (!(inChar || lineComment || blockComment))
						{
							if (inString && verbatim)
							{
								if (offset + 1 < document.TextLength && document.GetCharAt(offset + 1) == '"')
								{
									++offset; // skip escaped quote
									inString = false; // let the string go
								}
								else
								{
									verbatim = false;
								}
							}
							else if (!inString && offset > 0 && document.GetCharAt(offset - 1) == '@')
							{
								verbatim = true;
							}
							inString = !inString;
						}
						break;
					case '\'':
						if (!(inString || lineComment || blockComment))
						{
							inChar = !inChar;
						}
						break;
					case '\\':
						if ((inString && !verbatim) || inChar)
							++offset; // skip next character
						break;
					default:
						if (ch == openBracket)
						{
							if (!(inString || inChar || lineComment || blockComment))
							{
								++brackets;
							}
						}
						else if (ch == closingBracket)
						{
							if (!(inString || inChar || lineComment || blockComment))
							{
								--brackets;
								if (brackets == 0)
								{
									return offset;
								}
							}
						}
						break;
				}
				++offset;
			}
			return -1;
		}
		#endregion

		int QuickSearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			int brackets = -1;
			// first try "quick find" - find the matching bracket if there is no string/comment in the way
			for (int i = offset; i >= 0; --i)
			{
				char ch = document.GetCharAt(i);
				if (ch == openBracket)
				{
					++brackets;
					if (brackets == 0)
						return i;
				}
				else if (ch == closingBracket)
				{
					--brackets;
				}
				else if (ch == '"')
				{
					break;
				}
				else if (ch == '\'')
				{
					break;
				}
				else if (ch == '/' && i > 0)
				{
					if (document.GetCharAt(i - 1) == '/')
						break;
					if (document.GetCharAt(i - 1) == '*')
						break;
				}
			}
			return -1;
		}

		int QuickSearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
		{
			int brackets = 1;
			// try "quick find" - find the matching bracket if there is no string/comment in the way
			for (int i = offset; i < document.TextLength; ++i)
			{
				char ch = document.GetCharAt(i);
				if (ch == openBracket)
				{
					++brackets;
				}
				else if (ch == closingBracket)
				{
					--brackets;
					if (brackets == 0)
						return i;
				}
				else if (ch == '"')
				{
					break;
				}
				else if (ch == '\'')
				{
					break;
				}
				else if (ch == '/' && i > 0)
				{
					if (document.GetCharAt(i - 1) == '/')
						break;
				}
				else if (ch == '*' && i > 0)
				{
					if (document.GetCharAt(i - 1) == '/')
						break;
				}
			}
			return -1;
		}
	}
}
