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
			TestCase(SwitchOverString1, "First case", "Second case", "2nd case", "Third case", "Fourth case", "Fifth case", "Sixth case", null, "default", "else");
			Console.WriteLine(SwitchOverString2());
			Console.WriteLine(SwitchOverBool(true));
			Console.WriteLine(SwitchOverBool(false));
		}

		static void TestCase<T>(Func<T, string> target, params T[] args)
		{
			foreach (var arg in args) {
				Console.WriteLine(target(arg));
			}
		}

		public static string SparseIntegerSwitch(int i)
		{
			switch (i) {
				case -10000000: return "-10 mln";
				case -100: return "-hundred";
				case -1: return "-1";
				case 0: return "0";
				case 1: return "1";
				case 2: return "2";
				case 4: return "4";
				case 100: return "hundred";
				case 10000: return "ten thousand";
				case 10001: return "ten thousand and one";
				case int.MaxValue: return "int.MaxValue";
				default: return "something else";
			}
		}

		public static string ShortSwitchOverString(string text)
		{
			switch (text) {
				case "First case":
					return "Text";
				default:
					return "Default";
			}
		}

		public static string SwitchOverString1(string text)
		{
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

		public static string SwitchOverString2()
		{
			switch (Environment.UserName) {
				case "First case":
					return "Text1";
				case "Second case":
					return "Text2";
				case "Third case":
					return "Text3";
				case "Fourth case":
					return "Text4";
				case "Fifth case":
					return "Text5";
				case "Sixth case":
					return "Text6";
				default:
					return "Default";
			}
		}

		public static string SwitchOverBool(bool b)
		{
			switch (b) {
				case true:
					return bool.TrueString;
				case false:
					return bool.FalseString;
				default:
					return null;
			}
		}

		public static void SwitchInLoop(int i)
		{
			while (true) {
				switch (i) {
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
						throw new ArgumentException();
				}
				i++;
			}
		}
	}
}