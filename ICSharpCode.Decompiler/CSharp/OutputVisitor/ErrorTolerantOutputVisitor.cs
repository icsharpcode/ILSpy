// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;

using ICSharpCode.Decompiler.CSharp.Syntax;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.OutputVisitor
{
	/// <summary>
	/// Writes a syntax tree like <see cref="CSharpOutputVisitor"/>, but a member whose output throws
	/// is replaced by the error text instead of ending the file half-written. Writing resumes with
	/// the next member, so the reader still gets the rest of the type and a file that closes every
	/// brace it opened.
	/// </summary>
	/// <remarks>
	/// The failures are collected in <see cref="Errors"/>. An <see cref="IOException"/> from the
	/// underlying writer is not something to recover from - every following write would fail the
	/// same way - so it is left to propagate.
	/// </remarks>
	public class ErrorTolerantOutputVisitor : CSharpOutputVisitor
	{
		readonly List<Exception> errors = new List<Exception>();
		int braceDepth;

		public ErrorTolerantOutputVisitor(TextWriter textWriter, CSharpFormattingOptions formattingPolicy)
			: base(textWriter, formattingPolicy)
		{
		}

		/// <summary>
		/// The failures that took the place of a member, in the order they were written.
		/// </summary>
		public IReadOnlyList<Exception> Errors => errors;

		protected override void OpenBrace(BraceStyle style, bool newLine = true)
		{
			base.OpenBrace(style, newLine);
			braceDepth++;
		}

		protected override void CloseBrace(BraceStyle style, bool unindent = true)
		{
			base.CloseBrace(style, unindent);
			braceDepth--;
		}

		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
			=> Write(typeDeclaration, base.VisitTypeDeclaration);

		public override void VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration)
			=> Write(delegateDeclaration, base.VisitDelegateDeclaration);

		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
			=> Write(constructorDeclaration, base.VisitConstructorDeclaration);

		public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
			=> Write(destructorDeclaration, base.VisitDestructorDeclaration);

		public override void VisitEnumMemberDeclaration(EnumMemberDeclaration enumMemberDeclaration)
			=> Write(enumMemberDeclaration, base.VisitEnumMemberDeclaration);

		public override void VisitExtensionDeclaration(ExtensionDeclaration extensionDeclaration)
			=> Write(extensionDeclaration, base.VisitExtensionDeclaration);

		public override void VisitEventDeclaration(EventDeclaration eventDeclaration)
			=> Write(eventDeclaration, base.VisitEventDeclaration);

		public override void VisitCustomEventDeclaration(CustomEventDeclaration customEventDeclaration)
			=> Write(customEventDeclaration, base.VisitCustomEventDeclaration);

		public override void VisitFieldDeclaration(FieldDeclaration fieldDeclaration)
			=> Write(fieldDeclaration, base.VisitFieldDeclaration);

		public override void VisitFixedFieldDeclaration(FixedFieldDeclaration fixedFieldDeclaration)
			=> Write(fixedFieldDeclaration, base.VisitFixedFieldDeclaration);

		public override void VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
			=> Write(indexerDeclaration, base.VisitIndexerDeclaration);

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
			=> Write(methodDeclaration, base.VisitMethodDeclaration);

		public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
			=> Write(operatorDeclaration, base.VisitOperatorDeclaration);

		public override void VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration)
			=> Write(propertyDeclaration, base.VisitPropertyDeclaration);

		void Write<T>(T node, Action<T> write) where T : AstNode
		{
			int braces = braceDepth;
			int containers = containerStack.Count;
			try
			{
				write(node);
			}
			catch (Exception ex) when (!(ex is OperationCanceledException || ex is IOException))
			{
				errors.Add(ex);
				// The failed member left the writer inside its own nodes and braces: unwind both, so
				// what follows is written at the level the member started at.
				while (containerStack.Count > containers)
				{
					writer.EndNode(containerStack.Pop());
				}
				while (braceDepth > braces)
				{
					CloseBrace(BraceStyle.NextLine);
				}
				NewLine();
				foreach (string line in CSharpDecompiler.GetErrorCommentLines(ex))
				{
					writer.WriteComment(CommentType.SingleLine, " " + line);
				}
			}
		}
	}
}
