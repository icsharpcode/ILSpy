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
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class Loops
	{
		public void ForEach(IEnumerable<string> enumerable)
		{
			foreach (string current in enumerable) {
				current.ToLower();
			}
		}

		public void ForEachOverList(List<string> list)
		{
			// List has a struct as enumerator, so produces quite different IL than foreach over the IEnumerable interface
			foreach (string current in list) {
				current.ToLower();
			}
		}

		public void ForEachOverNonGenericEnumerable(IEnumerable enumerable)
		{
			foreach (object current in enumerable) {
				current.ToString();
			}
		}

		public void ForEachOverNonGenericEnumerableWithAutomaticCast(IEnumerable enumerable)
		{
			foreach (int num in enumerable) {
				num.ToString();
			}
		}

		//		public void ForEachOverArray(string[] array)
		//		{
		//			foreach (string text in array)
		//			{
		//				text.ToLower();
		//			}
		//		}

		public void ForOverArray(string[] array)
		{
			for (int i = 0; i < array.Length; i++) {
				array[i].ToLower();
			}
		}

		public void NestedLoops()
		{
			for (int i = 0; i < 10; i++) {
				if (i % 2 == 0) {
					for (int j = 0; j < 5; j++) {
						Console.WriteLine("Y");
					}
				} else {
					Console.WriteLine("X");
				}
			}
		}

		public int MultipleExits()
		{
			int i = 0;
			while (true) {
				if (i % 4 == 0) { return 4; }
				if (i % 7 == 0) { break; }
				if (i % 9 == 0) { return 5; }
				if (i % 11 == 0) { break; }
				i++;
			}
			i = int.MinValue;
			return i;
		}

		public int InterestingLoop()
		{
			int i = 0;
			if (i % 11 == 0) {
				while (true) {
					if (i % 4 == 0) {
						if (i % 7 == 0) {
							if (i % 11 == 0) {
								continue; // use a continue here to prevent moving the if (i%7) outside the loop
							}
							Console.WriteLine("7");
						} else {
							// this block is not part of the natural loop
							Console.WriteLine("!7");
						}
						break;
					}
					i++;
				}
				// This instruction is still dominated by the loop header
				i = int.MinValue;
			}
			return i;
		}

		bool Condition(string arg)
		{
			Console.WriteLine("Condition: " + arg);
			return false;
		}

		public void WhileLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if")) {
				while (Condition("while")) {
					Console.WriteLine("Loop Body");
					if (Condition("test")) {
						if (Condition("continue"))
							continue;
						if (!Condition("break"))
							break;
					}
					Console.WriteLine("End of loop body");
				}
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		public void WhileWithGoto()
		{
			while (Condition("Main Loop")) {
				if (!Condition("Condition"))
					goto block2;
				block1:
				Console.WriteLine("Block1");
				if (Condition("Condition2"))
					continue;
				block2:
				Console.WriteLine("Block2");
				goto block1;
			}
		}

		public void DoWhileLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if")) {
				do {
					Console.WriteLine("Loop Body");
					if (Condition("test")) {
						if (Condition("continue"))
							continue;
						if (!Condition("break"))
							break;
					}
					Console.WriteLine("End of loop body");
				} while (Condition("while"));
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		public void ForLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if")) {
				for (int i = 0; Condition("for"); i++) {
					Console.WriteLine("Loop Body");
					if (Condition("test")) {
						if (Condition("continue"))
							continue;
						if (!Condition("not-break"))
							break;
					}
					Console.WriteLine("End of loop body");
				}
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		public void DoubleForEachWithSameVariable(IEnumerable<string> enumerable)
		{
			foreach (string current in enumerable) {
				current.ToLower();
			}
			foreach (string current in enumerable) {
				current.ToLower();
			}
		}

		static void ForeachExceptForNameCollision(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachWithNameCollision");
			int input;
			using (var enumerator = inputs.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					input = enumerator.Current;
					Console.WriteLine(input);
				}
			}
			input = 1;
			Console.WriteLine(input);
		}

		static void ForeachExceptForContinuedUse(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachExceptForContinuedUse");
			int input = 0;
			using (var enumerator = inputs.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					input = enumerator.Current;
					Console.WriteLine(input);
				}
			}
			Console.WriteLine("Last: " + input);
		}
	}
}
