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
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;

namespace ICSharpCode.NRefactory.ConsistencyCheck
{
	/// <summary>
	/// Validates pattern matching:
	/// Every compilation must match its clone;
	/// and mutations to the cloned compilation must prevent the match.
	/// </summary>
	/// <remarks>
	/// This test is SLOW! (due to O(n^2) algorithm).
	/// Expect it to take a whole hour!
	/// </remarks>
	public class PatternMatchingTest
	{
		public static void RunTest(CSharpFile file)
		{
			AstNode copy = file.SyntaxTree.Clone();
			if (!copy.IsMatch(file.SyntaxTree))
				throw new InvalidOperationException("Clone must match the compilation itself; in " + file.FileName);
			
			// Mutate identifiers:
			foreach (var id in copy.Descendants.OfType<Identifier>()) {
				if (id.Parent is ConstructorDeclaration || id.Parent is DestructorDeclaration)
					continue; // identifier in ctor/dtor isn't relevant for matches
				string oldName = id.Name;
				id.Name = "mutatedName";
				if (copy.IsMatch(file.SyntaxTree))
					throw new InvalidOperationException("Mutation in " + id.StartLocation + " did not prevent the match; in " + file.FileName);
				id.Name = oldName;
				//if (!copy.IsMatch(file.SyntaxTree))
				//	throw new InvalidOperationException("Clone must match the compilation itself after resetting the mutation");
			}
			// Mutate primitive values:
			foreach (var pe in copy.Descendants.OfType<PrimitiveExpression>()) {
				if (pe.Ancestors.Any(a => a is PreProcessorDirective))
					continue;
				object oldVal = pe.Value;
				pe.Value = "Mutated " + "Value";
				if (copy.IsMatch(file.SyntaxTree))
					throw new InvalidOperationException("Mutation in " + pe.StartLocation + " did not prevent the match; in " + file.FileName);
				pe.Value = oldVal;
			}
			Console.Write('.');
		}
	}
}
