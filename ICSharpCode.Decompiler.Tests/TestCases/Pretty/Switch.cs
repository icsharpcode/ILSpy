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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class Switch
	{
		public class SetProperty
		{
			public readonly PropertyInfo Property;

			public int Set { get; set; }

			public SetProperty(PropertyInfo property)
			{
				Property = property;
			}
		}

		public class ImplicitString
		{
			private readonly string s;

			public ImplicitString(string s)
			{
				this.s = s;
			}

			public static implicit operator string(ImplicitString v)
			{
				return v.s;
			}
		}

		public class ExplicitString
		{
			private readonly string s;

			public ExplicitString(string s)
			{
				this.s = s;
			}

			public static explicit operator string(ExplicitString v)
			{
				return v.s;
			}
		}

		public enum State
		{
			False,
			True,
			Null
		}

		public enum KnownColor
		{
			DarkBlue,
			DarkCyan,
			DarkGoldenrod,
			DarkGray,
			DarkGreen,
			DarkKhaki
		}

		private static char ch1767;

#if !ROSLYN
		public static State SwitchOverNullableBool(bool? value)
		{
			switch (value)
			{
				case false:
					return State.False;
				case true:
					return State.True;
				case null:
					return State.Null;
				default:
					throw new InvalidOperationException();
			}
		}
#endif

		public static bool? SwitchOverNullableEnum(State? state)
		{
			switch (state)
			{
				case State.False:
					return false;
				case State.True:
					return true;
				case State.Null:
					return null;
				default:
					throw new InvalidOperationException();
			}
		}

		public static string SparseIntegerSwitch(int i)
		{
			Console.WriteLine("SparseIntegerSwitch: " + i);
			switch (i)
			{
				case -10000000:
					return "-10 mln";
				case -100:
					return "-hundred";
				case -1:
					return "-1";
				case 0:
					return "0";
				case 1:
					return "1";
				case 2:
					return "2";
				case 4:
					return "4";
				case 100:
					return "hundred";
				case 10000:
					return "ten thousand";
				case 10001:
					return "ten thousand and one";
				case int.MaxValue:
					return "int.MaxValue";
				default:
					return "something else";
			}
		}

		public static void SparseIntegerSwitch2(int i)
		{
			switch (i)
			{
				case 4:
				case 10:
				case 11:
				case 13:
				case 21:
				case 29:
				case 33:
				case 49:
				case 50:
				case 55:
					Console.WriteLine();
					break;
			}
		}

		public static bool SparseIntegerSwitch3(int i)
		{
			switch (i)
			{
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

		public static string SwitchOverNullableInt(int? i)
		{
			switch (i)
			{
				case null:
					return "null";
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "large";
			}
		}

		public static string SwitchOverNullableIntNullCaseCombined(int? i)
		{
			switch (i)
			{
				case null:
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "large";
			}
		}

		public static string SwitchOverNullableIntShifted(int? i)
		{
			switch (i + 5)
			{
				case null:
					return "null";
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "large";
			}
		}

		public static string SwitchOverNullableIntShiftedNullCaseCombined(int? i)
		{
			switch (i + 5)
			{
				case null:
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "large";
			}
		}

		public static string SwitchOverNullableIntNoNullCase(int? i)
		{
			switch (i)
			{
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "other";
			}
		}

		public static string SwitchOverNullableIntNoNullCaseShifted(int? i)
		{
			switch (i + 5)
			{
				case 0:
					return "zero";
				case 5:
					return "five";
				case 10:
					return "ten";
				default:
					return "other";
			}
		}

		public static void SwitchOverInt(int i)
		{
			switch (i)
			{
				case 0:
					Console.WriteLine("zero");
					break;
				case 5:
					Console.WriteLine("five");
					break;
				case 10:
					Console.WriteLine("ten");
					break;
				case 15:
					Console.WriteLine("fifteen");
					break;
				case 20:
					Console.WriteLine("twenty");
					break;
				case 25:
					Console.WriteLine("twenty-five");
					break;
				case 30:
					Console.WriteLine("thirty");
					break;
			}
		}

		// SwitchDetection.UseCSharpSwitch requires more complex heuristic to identify this when compiled with Roslyn
		public static void CompactSwitchOverInt(int i)
		{
			switch (i)
			{
				case 0:
				case 1:
				case 2:
					Console.WriteLine("012");
					break;
				case 3:
					Console.WriteLine("3");
					break;
				default:
					Console.WriteLine("default");
					break;
			}
			Console.WriteLine("end");
		}

		public static string ShortSwitchOverString(string text)
		{
			Console.WriteLine("ShortSwitchOverString: " + text);
			switch (text)
			{
				case "First case":
					return "Text1";
				case "Second case":
					return "Text2";
				case "Third case":
					return "Text3";
				default:
					return "Default";
			}
		}

		public static string ShortSwitchOverStringWithNullCase(string text)
		{
			Console.WriteLine("ShortSwitchOverStringWithNullCase: " + text);
			switch (text)
			{
				case "First case":
					return "Text1";
				case "Second case":
					return "Text2";
				case null:
					return "null";
				default:
					return "Default";
			}
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
			switch (Environment.UserName)
			{
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
				case "Seventh case":
					return "Text7";
				case "Eighth case":
					return "Text8";
				case "Ninth case":
					return "Text9";
				case "Tenth case":
					return "Text10";
				case "Eleventh case":
					return "Text11";
				default:
					return "Default";
			}
		}

		public static string SwitchOverImplicitString(ImplicitString s)
		{
			// we emit an explicit cast, because the rules used by the C# compiler are counter-intuitive:
			// The C# compiler does *not* take the type of the switch labels into account at all.
			switch ((string)s)
			{
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
				case "Seventh case":
					return "Text7";
				case "Eighth case":
					return "Text8";
				case "Ninth case":
					return "Text9";
				case "Tenth case":
					return "Text10";
				case "Eleventh case":
					return "Text11";
				default:
					return "Default";
			}
		}

		public static string SwitchOverExplicitString(ExplicitString s)
		{
			switch ((string)s)
			{
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
				case "Seventh case":
					return "Text7";
				case "Eighth case":
					return "Text8";
				case "Ninth case":
					return "Text9";
				case "Tenth case":
					return "Text10";
				case "Eleventh case":
					return "Text11";
				default:
					return "Default";
			}
		}

#if !ROSLYN
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
					return null;
			}
		}
#endif

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
					//case 3:
					//		Console.WriteLine("three");
					//		continue;
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
			Console.WriteLine("End of method");
		}

		// Needs to be long enough to generate a hashtable
		public static void SwitchWithGotoString(string s)
		{
			Console.WriteLine("SwitchWithGotoString: " + s);
			switch (s)
			{
				case "1":
					Console.WriteLine("one");
					goto default;
				case "2":
					Console.WriteLine("two");
					goto case "3";
				case "3":
					Console.WriteLine("three");
					break;
				case "4":
					Console.WriteLine("four");
					return;
				case "5":
					Console.WriteLine("five");
					return;
				case "6":
					Console.WriteLine("six");
					return;
				case "7":
					Console.WriteLine("seven");
					return;
				case "8":
					Console.WriteLine("eight");
					return;
				case "9":
					Console.WriteLine("nine");
					return;
				default:
					Console.WriteLine("default");
					break;
			}
			Console.WriteLine("End of method");
		}

		public static void SwitchWithGotoComplex(string s)
		{
			Console.WriteLine("SwitchWithGotoComplex: " + s);
			switch (s)
			{
				case "1":
					Console.WriteLine("one");
					goto case "8";
				case "2":
					Console.WriteLine("two");
					goto case "3";
				case "3":
					Console.WriteLine("three");
					if (s.Length != 2)
					{
						break;
					}
					goto case "5";
				case "4":
					Console.WriteLine("four");
					goto case "5";
				case "5":
					Console.WriteLine("five");
					goto case "8";
				case "6":
					Console.WriteLine("six");
					goto case "5";
				case "8":
					Console.WriteLine("eight");
					return;
				// add a default case so that case "7": isn't redundant
				default:
					Console.WriteLine("default");
					break;
				// note that goto case "7" will decompile as break;
				// cases with a single break have the highest IL offset and are moved to the bottom
				case "7":
					break;
			}
			Console.WriteLine("End of method");
		}

		private static SetProperty[] GetProperties()
		{
			return new SetProperty[0];
		}

		public static void SwitchOnStringInForLoop()
		{
			List<SetProperty> list = new List<SetProperty>();
			List<SetProperty> list2 = new List<SetProperty>();
			SetProperty[] properties = GetProperties();
			for (int i = 0; i < properties.Length; i++)
			{
				Console.WriteLine("In for-loop");
				SetProperty setProperty = properties[i];
				switch (setProperty.Property.Name)
				{
					case "Name1":
						setProperty.Set = 1;
						list.Add(setProperty);
						break;
					case "Name2":
						setProperty.Set = 2;
						list.Add(setProperty);
						break;
					case "Name3":
						setProperty.Set = 3;
						list.Add(setProperty);
						break;
					case "Name4":
						setProperty.Set = 4;
						list.Add(setProperty);
						break;
					case "Name5":
					case "Name6":
						list.Add(setProperty);
						break;
					default:
						list2.Add(setProperty);
						break;
				}
			}
		}

		public static void SwitchInTryBlock(string value)
		{
			try
			{
				switch (value.Substring(5))
				{
					case "Name1":
						Console.WriteLine("1");
						break;
					case "Name2":
						Console.WriteLine("Name_2");
						break;
					case "Name3":
						Console.WriteLine("Name_3");
						break;
					case "Name4":
						Console.WriteLine("No. 4");
						break;
					case "Name5":
					case "Name6":
						Console.WriteLine("5+6");
						break;
					default:
						Console.WriteLine("default");
						break;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("catch block");
			}
		}

		public static void SwitchWithComplexCondition(string[] args)
		{
			switch ((args.Length == 0) ? "dummy" : args[0])
			{
				case "a":
					Console.WriteLine("a");
					break;
				case "b":
					Console.WriteLine("b");
					break;
				case "c":
					Console.WriteLine("c");
					break;
				case "d":
					Console.WriteLine("d");
					break;
			}
			Console.WriteLine("end");
		}

		public static void SwitchWithArray(string[] args)
		{
			switch (args[0])
			{
				case "a":
					Console.WriteLine("a");
					break;
				case "b":
					Console.WriteLine("b");
					break;
				case "c":
					Console.WriteLine("c");
					break;
				case "d":
					Console.WriteLine("d");
					break;
			}
			Console.WriteLine("end");
		}

		public static void SwitchWithContinue1(int i, bool b)
		{
			while (true)
			{
				switch (i)
				{
#if OPT
					case 1:
						continue;
#endif
					case 0:
						if (b)
						{
							continue;
						}
						break;
					case 2:
						if (!b)
						{
							continue;
						}
						break;
#if !OPT
					case 1:
						continue;
#endif
				}
				Console.WriteLine();
			}
		}

		// while condition, return and break cases
		public static void SwitchWithContinue2(int i, bool b)
		{
			while (i < 10)
			{
				switch (i)
				{
					case 0:
						if (b)
						{
							Console.WriteLine("0b");
							continue;
						}
						Console.WriteLine("0!b");
						break;
					case 2:
#if OPT
						if (b)
						{
							Console.WriteLine("2b");
							return;
						}
						Console.WriteLine("2!b");
						continue;
#else
						if (!b)
						{
							Console.WriteLine("2!b");
							continue;
						}
						Console.WriteLine("2b");
						return;
#endif
					default:
						Console.WriteLine("default");
						break;
					case 3:
						break;
					case 1:
						continue;
				}
				Console.WriteLine("loop-tail");
				i++;
			}
		}

		// for loop version
		public static void SwitchWithContinue3(bool b)
		{
			for (int i = 0; i < 10; i++)
			{
				switch (i)
				{
					case 0:
						if (b)
						{
							Console.WriteLine("0b");
							continue;
						}
						Console.WriteLine("0!b");
						break;
					case 2:
#if OPT
						if (b)
						{
							Console.WriteLine("2b");
							return;
						}
						Console.WriteLine("2!b");
						continue;
#else
						if (!b)
						{
							Console.WriteLine("2!b");
							continue;
						}
						Console.WriteLine("2b");
						return;
#endif
					default:
						Console.WriteLine("default");
						break;
					case 3:
						break;
					case 1:
						continue;
				}
				Console.WriteLine("loop-tail");
			}
		}

		// foreach version
		public static void SwitchWithContinue4(bool b)
		{
			foreach (int item in Enumerable.Range(0, 10))
			{
				Console.WriteLine("loop: " + item);
				switch (item)
				{
					case 1:
						if (b)
						{
							continue;
						}
						break;
					case 3:
						if (!b)
						{
							continue;
						}
						return;
					case 4:
						Console.WriteLine(4);
						goto case 7;
					case 5:
						Console.WriteLine(5);
						goto default;
					case 6:
						if (b)
						{
							continue;
						}
						goto case 3;
					case 7:
						if (item % 2 == 0)
						{
							goto case 3;
						}
						if (!b)
						{
							continue;
						}
						goto case 8;
					case 8:
						if (b)
						{
							continue;
						}
						goto case 5;
					default:
						Console.WriteLine("default");
						break;
					case 2:
						continue;
				}
				Console.WriteLine("break: " + item);
			}
		}
		// internal if statement, loop increment block not dominated by the switch head
		public static void SwitchWithContinue5(bool b)
		{
			for (int i = 0; i < 10; i++)
			{
				if (i < 5)
				{
					switch (i)
					{
						case 0:
							if (b)
							{
								Console.WriteLine("0b");
								continue;
							}
							Console.WriteLine("0!b");
							break;
						case 2:
#if OPT
							if (b)
							{
								Console.WriteLine("2b");
								return;
							}
							Console.WriteLine("2!b");
							continue;
#else
							if (!b)
							{
								Console.WriteLine("2!b");
								continue;
							}
							Console.WriteLine("2b");
							return;
#endif
						default:
							Console.WriteLine("default");
							break;
						case 3:
							break;
						case 1:
							continue;
					}
					Console.WriteLine("break-target");
				}
				Console.WriteLine("loop-tail");
			}
		}

		// do-while loop version
		public static void SwitchWithContinue6(int i, bool b)
		{
			do
			{
				switch (i)
				{
					case 0:
						if (!b)
						{
							Console.WriteLine("0!b");
							break;
						}
						Console.WriteLine("0b");
						// ConditionDetection doesn't recognise Do-While continues yet
						continue;
					case 2:
						if (b)
						{
							Console.WriteLine("2b");
							return;
						}
						Console.WriteLine("2!b");
						continue;
					default:
						Console.WriteLine("default");
						break;
					case 3:
						break;
					case 1:
						continue;
				}
				Console.WriteLine("loop-tail");
			} while (++i < 10);
		}

		// double break from switch to loop exit requires additional pattern matching in HighLevelLoopTransform
		public static void SwitchWithContinue7()
		{
			for (int num = 0; num >= 0; num--)
			{
				Console.WriteLine("loop-head");
				switch (num)
				{
					default:
						Console.WriteLine("default");
						break;
					case 0:
						continue;
					case 1:
						break;
				}
				break;
			}
			Console.WriteLine("end");
		}

		public static void SwitchWithContinueInDoubleLoop()
		{
			bool value = false;
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					switch (i + j)
					{
						case 1:
						case 3:
						case 5:
						case 7:
						case 11:
						case 13:
						case 17:
							break;
						default:
							continue;
					}
					value = true;
					break;
				}
			}
			Console.WriteLine(value);
		}

		public static void SwitchLoopNesting()
		{
			for (int i = 0; i < 10; i++)
			{
				switch (i)
				{
					case 0:
						Console.WriteLine(0);
						break;
					case 1:
						Console.WriteLine(1);
						break;
					default:
						if (i % 2 == 0)
						{
							while (i % 3 != 0)
							{
								Console.WriteLine(i++);
							}
						}
						Console.WriteLine();
						break;
				}

				if (i > 4)
				{
					Console.WriteLine("high");
				}
				else
				{
					Console.WriteLine("low");
				}
			}
		}

		// These decompile poorly into switch statements and should be left as is
		#region Overagressive Switch Use

#if ROSLYN || OPT
		public static void SingleIf1(int i, bool a)
		{
			if (i == 1 || (i == 2 && a))
			{
				Console.WriteLine(1);
			}
			Console.WriteLine(2);
		}
#endif

		public static void SingleIf2(int i, bool a, bool b)
		{
			if (i == 1 || (i == 2 && a) || (i == 3 && b))
			{
				Console.WriteLine(1);
			}
			Console.WriteLine(2);
		}

		public static void SingleIf3(int i, bool a, bool b)
		{
			if (a || i == 1 || (i == 2 && b))
			{
				Console.WriteLine(1);
			}
			Console.WriteLine(2);
		}

		public static void SingleIf4(int i, bool a)
		{
			if (i == 1 || i == 2 || (i != 3 && a) || i != 4)
			{
				Console.WriteLine(1);
			}
			Console.WriteLine(2);
		}

		public static void NestedIf(int i)
		{
			if (i != 1)
			{
				if (i == 2)
				{
					Console.WriteLine(2);
				}
				Console.WriteLine("default");
			}
			Console.WriteLine();
		}

		public static void IfChainWithCondition(int i)
		{
			if (i == 0)
			{
				Console.WriteLine(0);
			}
			else if (i == 1)
			{
				Console.WriteLine(1);
			}
			else if (i == 2)
			{
				Console.WriteLine(2);
			}
			else if (i == 3)
			{
				Console.WriteLine(3);
			}
			else if (i == 4)
			{
				Console.WriteLine(4);
			}
			else if (i == 5 && Console.CapsLock)
			{
				Console.WriteLine("5A");
			}
			else
			{
				Console.WriteLine("default");
			}

			Console.WriteLine();
		}

		public static bool SwitchlikeIf(int i, int j)
		{
			if (i != 0 && j != 0)
			{
				if (i == -1 && j == -1)
				{
					Console.WriteLine("-1, -1");
				}
				if (i == -1 && j == 1)
				{
					Console.WriteLine("-1, 1");
				}
				if (i == 1 && j == -1)
				{
					Console.WriteLine("1, -1");
				}
				if (i == 1 && j == 1)
				{
					Console.WriteLine("1, 1");
				}
				return false;
			}

			if (i != 0)
			{
				if (i == -1)
				{
					Console.WriteLine("-1, 0");
				}
				if (i == 1)
				{
					Console.WriteLine("1, 0");
				}
				return false;
			}

			if (j != 0)
			{
				if (j == -1)
				{
					Console.WriteLine("0, -1");
				}
				if (j == 1)
				{
					Console.WriteLine("0, 1");
				}
				return false;
			}

			return true;
		}

		public static bool SwitchlikeIf2(int i)
		{
			if (i != 0)
			{
				// note that using else-if in this chain creates a nice-looking switch here (as expected)
				if (i == 1)
				{
					Console.WriteLine(1);
				}
				if (i == 2)
				{
					Console.WriteLine(2);
				}
				if (i == 3)
				{
					Console.WriteLine(3);
				}
				return false;
			}
			return false;
		}

		public static void SingleIntervalIf(char c)
		{
			if (c >= 'A' && c <= 'Z')
			{
				Console.WriteLine("alphabet");
			}
			Console.WriteLine("end");
		}

		public static bool Loop8(char c, bool b, Func<char> getChar)
		{
			if (b)
			{
				while ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))
				{
					c = getChar();
				}
			}

			return true;
		}

		public static void Loop9(Func<char> getChar)
		{
			char c;
			do
			{
				c = getChar();
			} while (c != -1 && c != '\n' && c != '\u2028' && c != '\u2029');
		}
		#endregion

		// Ensure correctness of SwitchDetection.UseCSharpSwitch control flow heuristics
		public static void SwitchWithBreakCase(int i, bool b)
		{
			if (b)
			{
				switch (i)
				{
					case 1:
						Console.WriteLine(1);
						break;
					default:
						Console.WriteLine("default");
						break;
					case 2:
						break;
				}
				Console.WriteLine("b");
			}
			Console.WriteLine("end");
		}

		public static void SwitchWithReturnAndBreak(int i, bool b)
		{
			switch (i)
			{
				case 0:
					if (b)
					{
						return;
					}
					break;
				case 1:
					if (!b)
					{
						return;
					}
					break;
			}
			Console.WriteLine();
		}

		public static int SwitchWithReturnAndBreak2(int i, bool b)
		{
			switch (i)
			{
				case 4:
				case 33:
					Console.WriteLine();
					return 1;
				case 334:
					if (b)
					{
						return 2;
					}
					break;
				case 395:
				case 410:
				case 455:
					Console.WriteLine();
					break;
			}
			Console.WriteLine();
			return 0;
		}

		public static void SwitchWithReturnAndBreak3(int i)
		{
			switch (i)
			{
				default:
					return;
				case 0:
					Console.WriteLine(0);
					break;
				case 1:
					Console.WriteLine(1);
					break;
			}
			Console.WriteLine();
		}

		public static string Issue1621(int x)
		{
			if (x == 5)
			{
				return "5";
			}
			switch (x)
			{
				case 1:
					return "1";
				case 2:
				case 6:
				case 7:
					return "2-6-7";
				case 3:
					return "3";
				case 4:
					return "4";
				case 5:
					return "unreachable";
				default:
					throw new Exception();
			}
		}

		public static int Issue1602(string x)
		{
			switch (x)
			{
				case null:
					return 0;
				case "":
					return -1;
				case "A":
					return 65;
				case "B":
					return 66;
				case "C":
					return 67;
				case "D":
					return 68;
				case "E":
					return 69;
				case "F":
					return 70;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static void Issue1745(string aaa)
		{
			switch (aaa)
			{
				case "a":
				case "b":
				case "c":
				case "d":
				case "e":
				case "f":
					Console.WriteLine(aaa);
					break;
				case null:
					Console.WriteLine("<null>");
					break;
				case "":
					Console.WriteLine("<empty>");
					break;
			}
		}

		public static bool DoNotRemoveAssignmentBeforeSwitch(string x, out ConsoleKey key)
		{
			key = (ConsoleKey)0;
			switch (x)
			{
				case "A":
					key = ConsoleKey.A;
					break;
				case "B":
					key = ConsoleKey.B;
					break;
				case "C":
					key = ConsoleKey.C;
					break;
			}
			return key != (ConsoleKey)0;
		}

		public static void Issue1767(string s)
		{
			switch (s)
			{
				case "a":
					ch1767 = s[0];
					break;
				case "b":
					ch1767 = s[0];
					break;
				case "c":
					ch1767 = s[0];
					break;
				case "d":
					ch1767 = s[0];
					break;
				case "e":
					ch1767 = s[0];
					break;
				case "f":
					ch1767 = s[0];
					break;
			}
		}

		public static void Issue2763(int value)
		{
			switch ((KnownColor)value)
			{
				case KnownColor.DarkBlue:
					Console.WriteLine("DarkBlue");
					break;
				case KnownColor.DarkCyan:
					Console.WriteLine("DarkCyan");
					break;
				case KnownColor.DarkGoldenrod:
					Console.WriteLine("DarkGoldenrod");
					break;
				case KnownColor.DarkGray:
					Console.WriteLine("DarkGray");
					break;
				case KnownColor.DarkGreen:
					Console.WriteLine("DarkGreen");
					break;
				case KnownColor.DarkKhaki:
					Console.WriteLine("DarkKhaki");
					break;
			}
		}
	}
}
