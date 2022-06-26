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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	public static class Switch
	{
		public static void Main()
		{
			TestCase(SparseIntegerSwitch, -100, 1, 2, 3, 4);
			TestCase(ShortSwitchOverString, "First case", "Else");
			TestCase(ShortSwitchOverString2, "First case", "Second case", "Third case", "Else");
			TestCase(ShortSwitchOverStringNoExplicitDefault, "First case", "Second case", "Third case", "Else");
			TestCase(SwitchOverString1, "First case", "Second case", "2nd case", "Third case", "Fourth case", "Fifth case", "Sixth case", null, "default", "else");
			Console.WriteLine(SwitchOverString2());
			Console.WriteLine(SwitchOverBool(true));
			Console.WriteLine(SwitchOverBool(false));
			SwitchInLoop(0);
			SwitchWithGoto(1);
			SwitchWithGoto(2);
			SwitchWithGoto(3);
			SwitchWithGoto(4);
		}

		static void TestCase<T>(Func<T, string> target, params T[] args)
		{
			foreach (var arg in args)
			{
				Console.WriteLine(target(arg));
			}
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
				_ => "something else"
			};
		}

		public static string ShortSwitchOverString(string text)
		{
			Console.WriteLine("ShortSwitchOverString: " + text);
			return text switch {
				"First case" => "Text",
				_ => "Default"
			};
		}

		public static string ShortSwitchOverString2(string text)
		{
			Console.WriteLine("ShortSwitchOverString2: " + text);
			return text switch {
				"First case" => "Text1",
				"Second case" => "Text2",
				"Third case" => "Text3",
				_ => "Default"
			};
		}

		public static string ShortSwitchOverStringNoExplicitDefault(string text)
		{
			Console.WriteLine("ShortSwitchOverStringNoExplicitDefault: " + text);
			return text switch {
				"First case" => "Text1",
				"Second case" => "Text2",
				"Third case" => "Text3",
				_ => "Default"
			};
		}

		public static string SwitchOverString1(string text)
		{
			Console.WriteLine("SwitchOverString1: " + text);
			switch (text)
			{
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

		public static string SwitchOverString2()
		{
			Console.WriteLine("SwitchOverString2:");
			return Environment.UserName switch {
				"First case" => "Text1",
				"Second case" => "Text2",
				"Third case" => "Text3",
				"Fourth case" => "Text4",
				"Fifth case" => "Text5",
				"Sixth case" => "Text6",
				"Seventh case" => "Text7",
				"Eighth case" => "Text8",
				"Ninth case" => "Text9",
				"Tenth case" => "Text10",
				"Eleventh case" => "Text11",
				_ => "Default"
			};
		}

		public static string SwitchOverBool(bool b)
		{
			Console.WriteLine("SwitchOverBool: " + b);
			switch (b)
			{
				case true:
					return bool.TrueString;
				case false:
					return bool.FalseString;
				default:
#pragma warning disable CS0162
					return null;
#pragma warning restore CS0162
			}
		}

		public static void SwitchInLoop(int i)
		{
			Console.WriteLine("SwitchInLoop: " + i);
			while (true)
			{
				switch (i)
				{
					case 1:
						Console.WriteLine("one");
						break;
					case 2:
						Console.WriteLine("two");
						break;
					case 3:
						Console.WriteLine("three");
						continue;
					case 4:
						Console.WriteLine("four");
						return;
					default:
						Console.WriteLine("default");
						Console.WriteLine("more code");
						return;
				}
				i++;
			}
		}

		public static void SwitchWithGoto(int i)
		{
			Console.WriteLine("SwitchWithGoto: " + i);
			switch (i)
			{
				case 1:
					Console.WriteLine("one");
					goto default;
				case 2:
					Console.WriteLine("two");
					goto case 3;
				case 3:
					Console.WriteLine("three");
					break;
				case 4:
					Console.WriteLine("four");
					return;
				default:
					Console.WriteLine("default");
					break;
			}
		}
	}
}