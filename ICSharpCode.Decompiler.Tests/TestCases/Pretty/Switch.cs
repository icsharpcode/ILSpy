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
using System.Reflection;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class Switch
	{
		public class SetProperty
		{
			public readonly PropertyInfo Property;

			public int Set {
				get;
				set;
			}

			public SetProperty(PropertyInfo property)
			{
				this.Property = property;
			}
		}

		public enum State
		{
			False,
			True,
			Null
		}

		public static State SwitchOverNullableBool(bool? value)
		{
			switch (value) {
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

		public static bool? SwitchOverNullableEnum(State? state)
		{
			switch (state) {
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
			switch (i) {
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
				case 2147483647:
					return "int.MaxValue";
				default:
					return "something else";
			}
		}

		public static string SwitchOverNullableInt(int? i)
		{
			switch (i) {
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
			switch (i) {
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
			switch (i + 5) {
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
			switch (i + 5) {
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
			switch (i) {
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
			switch (i + 5) {
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
			switch (i) {
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

		public static string ShortSwitchOverString(string text)
		{
			Console.WriteLine("ShortSwitchOverString: " + text);
			switch (text) {
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
			switch (text) {
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
			Console.WriteLine("SwitchOverString2:");
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

		public static string SwitchOverBool(bool b)
		{
			Console.WriteLine("SwitchOverBool: " + b.ToString());
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
			Console.WriteLine("SwitchInLoop: " + i);
			while (true) {
				switch (i) {
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
			switch (i) {
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

		private static SetProperty[] GetProperties()
		{
			return new SetProperty[0];
		}

		public static void SwitchOnStringInForLoop()
		{
			List<SetProperty> list = new List<SetProperty>();
			List<SetProperty> list2 = new List<SetProperty>();
			SetProperty[] properties = Switch.GetProperties();
			for (int i = 0; i < properties.Length; i++) {
				Console.WriteLine("In for-loop");
				SetProperty setProperty = properties[i];
				switch (setProperty.Property.Name) {
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

		public static void SwitchWithComplexCondition(string[] args)
		{
			switch ((args.Length == 0) ? "dummy" : args[0]) {
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
			switch (args[0]) {
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
	}
}