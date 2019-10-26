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
using System.Linq;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class CSharpHighlightingTokenWriter : DecoratingTokenWriter
	{
		HighlightingColor visibilityKeywordsColor;
		HighlightingColor namespaceKeywordsColor;
		HighlightingColor structureKeywordsColor;
		HighlightingColor gotoKeywordsColor;
		HighlightingColor queryKeywordsColor;
		HighlightingColor exceptionKeywordsColor;
		HighlightingColor checkedKeywordColor;
		HighlightingColor unsafeKeywordsColor;
		HighlightingColor valueTypeKeywordsColor;
		HighlightingColor referenceTypeKeywordsColor;
		HighlightingColor operatorKeywordsColor;
		HighlightingColor parameterModifierColor;
		HighlightingColor modifiersColor;
		HighlightingColor accessorKeywordsColor;
		HighlightingColor attributeKeywordsColor;

		HighlightingColor referenceTypeColor;
		HighlightingColor valueTypeColor;
		HighlightingColor interfaceTypeColor;
		HighlightingColor enumerationTypeColor;
		HighlightingColor typeParameterTypeColor;
		HighlightingColor delegateTypeColor;

		HighlightingColor methodCallColor;
		HighlightingColor methodDeclarationColor;

		HighlightingColor fieldDeclarationColor;
		HighlightingColor fieldAccessColor;

		HighlightingColor valueKeywordColor;
		HighlightingColor thisKeywordColor;
		HighlightingColor trueKeywordColor;
		HighlightingColor typeKeywordsColor;

		public RichTextModel HighlightingModel { get; } = new RichTextModel();

		public CSharpHighlightingTokenWriter(TokenWriter decoratedWriter, ISmartTextOutput textOutput = null, ILocatable locatable = null)
			: base(decoratedWriter)
		{
			var highlighting = HighlightingManager.Instance.GetDefinition("C#");

			this.locatable = locatable;
			this.textOutput = textOutput;

			this.visibilityKeywordsColor = highlighting.GetNamedColor("Visibility");
			this.namespaceKeywordsColor = highlighting.GetNamedColor("NamespaceKeywords");
			this.structureKeywordsColor = highlighting.GetNamedColor("Keywords");
			this.gotoKeywordsColor = highlighting.GetNamedColor("GotoKeywords");
			this.queryKeywordsColor = highlighting.GetNamedColor("QueryKeywords");
			this.exceptionKeywordsColor = highlighting.GetNamedColor("ExceptionKeywords");
			this.checkedKeywordColor = highlighting.GetNamedColor("CheckedKeyword");
			this.unsafeKeywordsColor = highlighting.GetNamedColor("UnsafeKeywords");
			this.valueTypeKeywordsColor = highlighting.GetNamedColor("ValueTypeKeywords");
			this.referenceTypeKeywordsColor = highlighting.GetNamedColor("ReferenceTypeKeywords");
			this.operatorKeywordsColor = highlighting.GetNamedColor("OperatorKeywords");
			this.parameterModifierColor = highlighting.GetNamedColor("ParameterModifiers");
			this.modifiersColor = highlighting.GetNamedColor("Modifiers");
			this.accessorKeywordsColor = highlighting.GetNamedColor("GetSetAddRemove");

			this.referenceTypeColor = highlighting.GetNamedColor("ReferenceTypes");
			this.valueTypeColor = highlighting.GetNamedColor("ValueTypes");
			this.interfaceTypeColor = highlighting.GetNamedColor("InterfaceTypes");
			this.enumerationTypeColor = highlighting.GetNamedColor("EnumTypes");
			this.typeParameterTypeColor = highlighting.GetNamedColor("TypeParameters");
			this.delegateTypeColor = highlighting.GetNamedColor("DelegateTypes");
			this.methodDeclarationColor = this.methodCallColor = highlighting.GetNamedColor("MethodCall");
			//this.eventDeclarationColor = this.eventAccessColor = defaultTextColor;
			//this.propertyDeclarationColor = this.propertyAccessColor = defaultTextColor;
			this.fieldDeclarationColor = this.fieldAccessColor = highlighting.GetNamedColor("FieldAccess");
			//this.variableDeclarationColor = this.variableAccessColor = defaultTextColor;
			//this.parameterDeclarationColor = this.parameterAccessColor = defaultTextColor;
			this.valueKeywordColor = highlighting.GetNamedColor("NullOrValueKeywords");
			this.thisKeywordColor = highlighting.GetNamedColor("ThisOrBaseReference");
			this.trueKeywordColor = highlighting.GetNamedColor("TrueFalse");
			this.typeKeywordsColor = highlighting.GetNamedColor("TypeKeywords");
			this.attributeKeywordsColor = highlighting.GetNamedColor("AttributeKeywords");
			//this.externAliasKeywordColor = ...;
		}

		public override void WriteKeyword(Role role, string keyword)
		{
			HighlightingColor color = null;
			switch (keyword) {
				case "namespace":
				case "using":
					if (role == UsingStatement.UsingKeywordRole)
						color = structureKeywordsColor;
					else
						color = namespaceKeywordsColor;
					break;
				case "this":
				case "base":
					color = thisKeywordColor;
					break;
				case "true":
				case "false":
					color = trueKeywordColor;
					break;
				case "public":
				case "internal":
				case "protected":
				case "private":
					color = visibilityKeywordsColor;
					break;
				case "if":
				case "else":
				case "switch":
				case "case":
				case "default":
				case "while":
				case "do":
				case "for":
				case "foreach":
				case "lock":
				case "global":
				case "await":
					color = structureKeywordsColor;
					break;
				case "where":
					if (nodeStack.PeekOrDefault() is QueryClause)
						color = queryKeywordsColor;
					else
						color = structureKeywordsColor;
					break;
				case "in":
					if (nodeStack.PeekOrDefault() is ForeachStatement)
						color = structureKeywordsColor;
					else if (nodeStack.PeekOrDefault() is QueryClause)
						color = queryKeywordsColor;
					else
						color = parameterModifierColor;
					break;
				case "as":
				case "is":
				case "new":
				case "sizeof":
				case "typeof":
				case "nameof":
				case "stackalloc":
					color = typeKeywordsColor;
					break;
				case "try":
				case "throw":
				case "catch":
				case "finally":
					color = exceptionKeywordsColor;
					break;
				case "when":
					if (role == CatchClause.WhenKeywordRole)
						color = exceptionKeywordsColor;
					break;
				case "get":
				case "set":
				case "add":
				case "remove":
					if (role == PropertyDeclaration.GetKeywordRole ||
						role == PropertyDeclaration.SetKeywordRole ||
						role == CustomEventDeclaration.AddKeywordRole ||
						role == CustomEventDeclaration.RemoveKeywordRole)
						color = accessorKeywordsColor;
					break;
				case "abstract":
				case "const":
				case "event":
				case "extern":
				case "override":
				case "sealed":
				case "static":
				case "virtual":
				case "volatile":
				case "async":
				case "partial":
					color = modifiersColor;
					break;
				case "readonly":
					if (role == ComposedType.ReadonlyRole)
						color = parameterModifierColor;
					else
						color = modifiersColor;
					break;
				case "checked":
				case "unchecked":
					color = checkedKeywordColor;
					break;
				case "fixed":
				case "unsafe":
					color = unsafeKeywordsColor;
					break;
				case "enum":
				case "struct":
					color = valueTypeKeywordsColor;
					break;
				case "class":
				case "interface":
				case "delegate":
					color = referenceTypeKeywordsColor;
					break;
				case "select":
				case "group":
				case "by":
				case "into":
				case "from":
				case "orderby":
				case "let":
				case "join":
				case "on":
				case "equals":
					if (nodeStack.PeekOrDefault() is QueryClause)
						color = queryKeywordsColor;
					break;
				case "ascending":
				case "descending":
					if (nodeStack.PeekOrDefault() is QueryOrdering)
						color = queryKeywordsColor;
					break;
				case "explicit":
				case "implicit":
				case "operator":
					color = operatorKeywordsColor;
					break;
				case "params":
				case "ref":
				case "out":
					color = parameterModifierColor;
					break;
				case "break":
				case "continue":
				case "goto":
				case "yield":
				case "return":
					color = gotoKeywordsColor;
					break;
			}
			if (nodeStack.PeekOrDefault() is AttributeSection)
				color = attributeKeywordsColor;
			if (color != null) {
				BeginSpan(color);
			}
			base.WriteKeyword(role, keyword);
			if (color != null) {
				EndSpan();
			}
		}

		public override void WritePrimitiveType(string type)
		{
			HighlightingColor color = null;
			switch (type) {
				case "new":
				case "notnull":
					// Not sure if reference type or value type
					color = referenceTypeKeywordsColor;
					break;
				case "bool":
				case "byte":
				case "char":
				case "decimal":
				case "double":
				case "enum":
				case "float":
				case "int":
				case "long":
				case "sbyte":
				case "short":
				case "struct":
				case "uint":
				case "ushort":
				case "ulong":
				case "unmanaged":
					color = valueTypeKeywordsColor;
					break;
				case "class":
				case "object":
				case "string":
				case "void":
					color = referenceTypeKeywordsColor;
					break;
			}
			if (color != null) {
				BeginSpan(color);
			}
			base.WritePrimitiveType(type);
			if (color != null) {
				EndSpan();
			}
		}

		public override void WriteIdentifier(Identifier identifier)
		{
			HighlightingColor color = null;
			if (identifier.Name == "value" && identifier.Ancestors.OfType<Accessor>().FirstOrDefault() is Accessor accessor && accessor.Role != PropertyDeclaration.GetterRole)
				color = valueKeywordColor;
			if ((identifier.Name == "dynamic" || identifier.Name == "var") && identifier.Parent is AstType)
				color = queryKeywordsColor;
			switch (GetCurrentDefinition()) {
				case ITypeDefinition t:
					switch (t.Kind) {
						case TypeKind.Delegate:
							color = delegateTypeColor;
							break;
						case TypeKind.Class:
							color = referenceTypeColor;
							break;
						case TypeKind.Interface:
							color = interfaceTypeColor;
							break;
						case TypeKind.Enum:
							color = enumerationTypeColor;
							break;
						case TypeKind.Struct:
							color = valueTypeColor;
							break;
					}
					break;
				case IMethod m:
					color = methodDeclarationColor;
					break;
				case IField f:
					color = fieldDeclarationColor;
					break;
			}
			switch (GetCurrentMemberReference()) {
				case IType t:
					switch (t.Kind) {
						case TypeKind.Delegate:
							color = delegateTypeColor;
							break;
						case TypeKind.Class:
							color = referenceTypeColor;
							break;
						case TypeKind.Interface:
							color = interfaceTypeColor;
							break;
						case TypeKind.Enum:
							color = enumerationTypeColor;
							break;
						case TypeKind.Struct:
							color = valueTypeColor;
							break;
					}
					break;
				case IMethod m:
					color = methodCallColor;
					break;
				case IField f:
					color = fieldAccessColor;
					break;
			}
			if (color != null) {
				BeginSpan(color);
			}
			base.WriteIdentifier(identifier);
			if (color != null) {
				EndSpan();
			}
		}

		public override void WritePrimitiveValue(object value, string literalValue = null)
		{
			HighlightingColor color = null;
			if (value is null) {
				color = valueKeywordColor;
			}
			if (value is true || value is false) {
				color = trueKeywordColor;
			}
			if (color != null) {
				BeginSpan(color);
			}
			base.WritePrimitiveValue(value, literalValue);
			if (color != null) {
				EndSpan();
			}
		}

		ISymbol GetCurrentDefinition()
		{
			if (nodeStack == null || nodeStack.Count == 0)
				return null;

			var node = nodeStack.Peek();
			if (node is Identifier)
				node = node.Parent;
			if (Decompiler.TextTokenWriter.IsDefinition(ref node))
				return node.GetSymbol();

			return null;
		}

		ISymbol GetCurrentMemberReference()
		{
			if (nodeStack == null || nodeStack.Count == 0)
				return null;

			AstNode node = nodeStack.Peek();
			var symbol = node.GetSymbol();
			if (symbol == null && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression) {
				symbol = node.Parent.GetSymbol();
			}
			if (symbol != null && node.Parent is ObjectCreateExpression) {
				symbol = node.Parent.GetSymbol();
			}
			if (node is IdentifierExpression && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression && symbol is IMember member) {
				var declaringType = member.DeclaringType;
				if (declaringType != null && declaringType.Kind == TypeKind.Delegate)
					return null;
			}
			return symbol;
		}

		readonly Stack<AstNode> nodeStack = new Stack<AstNode>();

		public override void StartNode(AstNode node)
		{
			nodeStack.Push(node);
			base.StartNode(node);
		}

		public override void EndNode(AstNode node)
		{
			base.EndNode(node);
			nodeStack.Pop();
		}

		readonly Stack<HighlightingColor> colorStack = new Stack<HighlightingColor>();
		HighlightingColor currentColor = new HighlightingColor();
		int currentColorBegin = -1;
		readonly ILocatable locatable;
		readonly ISmartTextOutput textOutput;

		private void BeginSpan(HighlightingColor highlightingColor)
		{
			if (textOutput != null) {
				textOutput.BeginSpan(highlightingColor);
				return;
			}

			if (currentColorBegin > -1)
				HighlightingModel.SetHighlighting(currentColorBegin, locatable.Length - currentColorBegin, currentColor);
			colorStack.Push(currentColor);
			currentColor = currentColor.Clone();
			currentColorBegin = locatable.Length;
			currentColor.MergeWith(highlightingColor);
			currentColor.Freeze();
		}

		private void EndSpan()
		{
			if (textOutput != null) {
				textOutput.EndSpan();
				return;
			}

			HighlightingModel.SetHighlighting(currentColorBegin, locatable.Length - currentColorBegin, currentColor);
			currentColor = colorStack.Pop();
			currentColorBegin = locatable.Length;
		}
	}
}
