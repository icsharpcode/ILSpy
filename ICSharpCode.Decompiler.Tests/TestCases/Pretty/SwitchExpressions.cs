// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class SwitchExpressions
	{
		public enum State
		{
			False,
			True,
			Null
		}

		public static bool? SwitchOverNullableEnum(State? state)
		{
			return state switch {
				State.False => false,
				State.True => true,
				State.Null => null,
				_ => throw new InvalidOperationException(),
			};
		}

		public static string SparseIntegerSwitch(int i)
		{
			Console.WriteLine("SparseIntegerSwitch: " + i);
			return i switch {
				-10000000 => "-10 mln",
				-100 => "-hundred",
				-1 => "-1",
				0 => "0",
				1 => "1",
				2 => "2",
				4 => "4",
				100 => "hundred",
				10000 => "ten thousand",
				10001 => "ten thousand and one",
				int.MaxValue => "int.MaxValue",
				_ => "something else",
			};
		}

		public static bool SparseIntegerSwitch3(int i)
		{
			// not using a switch expression because we'd have to duplicate the 'true' branch
			switch (i) {
				case 0:
				case 10:
				case 11:
				case 12:
				case 100:
				case 101:
				case 200:
					return true;
				default:
					return false;
			}
		}

		public static string SwitchOverNullableInt(int? i, int? j)
		{
			return (i + j) switch {
				null => "null",
				0 => "zero",
				5 => "five",
				10 => "ten",
				_ => "large",
			};
		}

		public static void SwitchOverInt(int i)
		{
			Console.WriteLine(i switch {
				0 => "zero",
				5 => "five",
				10 => "ten",
				15 => "fifteen",
				20 => "twenty",
				25 => "twenty-five",
				30 => "thirty",
				_ => throw new NotImplementedException(),
			});
		}

		public static string SwitchOverString1(string text)
		{
			Console.WriteLine("SwitchOverString1: " + text);
			return text switch {
				"First case" => "Text1",
				"Second case" => "Text2",
				"Third case" => "Text3",
				"Fourth case" => "Text4",
				"Fifth case" => "Text5",
				"Sixth case" => "Text6",
				null => null,
				_ => "Default",
			};
		}

		public static string SwitchOverString2(string text)
		{
			Console.WriteLine("SwitchOverString1: " + text);
			// Cannot use switch expression, because "return Text2;" would need to be duplicated
			switch (text) {
				case "First case":
					return "Text1";
				case "Second case":
				case "2nd case":
					return "Text2";
				case "Third case":
					return "Text3";
				case "Fourth case":
					return "Text4";
				case "Fifth case":
					return "Text5";
				case "Sixth case":
					return "Text6";
				case null:
					return null;
				default:
					return "Default";
			}
		}
	}
}