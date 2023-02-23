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

using System.Collections.Generic;
using System.Linq;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Extensions;

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
		HighlightingColor propertyDeclarationColor;
		HighlightingColor propertyAccessColor;
		HighlightingColor eventDeclarationColor;
		HighlightingColor eventAccessColor;

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

			this.visibilityKeywordsColor = GetColor("Visibility") ?? GetColor("Keywords");
			this.namespaceKeywordsColor = GetColor("NamespaceKeywords") ?? GetColor("Keywords");
			this.structureKeywordsColor = GetColor("Keywords");
			this.gotoKeywordsColor = GetColor("GotoKeywords") ?? GetColor("Keywords");
			this.queryKeywordsColor = GetColor("QueryKeywords") ?? GetColor("Keywords");
			this.exceptionKeywordsColor = GetColor("ExceptionKeywords") ?? GetColor("Keywords");
			this.checkedKeywordColor = GetColor("CheckedKeyword") ?? GetColor("Keywords");
			this.unsafeKeywordsColor = GetColor("UnsafeKeywords") ?? GetColor("Keywords");
			this.valueTypeKeywordsColor = GetColor("ValueTypeKeywords") ?? GetColor("Keywords");
			this.referenceTypeKeywordsColor = GetColor("ReferenceTypeKeywords") ?? GetColor("Keywords");
			this.operatorKeywordsColor = GetColor("OperatorKeywords") ?? GetColor("Keywords");
			this.parameterModifierColor = GetColor("ParameterModifiers") ?? GetColor("Keywords");
			this.modifiersColor = GetColor("Modifiers") ?? GetColor("Keywords");
			this.accessorKeywordsColor = GetColor("GetSetAddRemove") ?? GetColor("Keywords");

			this.referenceTypeColor = GetColor("ReferenceTypes") ?? GetColor("Types");
			this.valueTypeColor = GetColor("ValueTypes") ?? GetColor("Types");
			this.interfaceTypeColor = GetColor("InterfaceTypes") ?? GetColor("Types");
			this.enumerationTypeColor = GetColor("EnumTypes") ?? GetColor("Types");
			this.typeParameterTypeColor = GetColor("TypeParameters") ?? GetColor("Types");
			this.delegateTypeColor = GetColor("DelegateTypes") ?? GetColor("Types");
			this.methodDeclarationColor = GetColor("MethodDeclaration") ?? GetColor("MethodCall");
			this.methodCallColor = GetColor("MethodCall") ?? GetColor("MethodDeclaration");
			this.fieldDeclarationColor = GetColor("FieldDeclaration") ?? GetColor("FieldAccess");
			this.fieldAccessColor = GetColor("FieldAccess") ?? GetColor("FieldDeclaration");
			this.propertyDeclarationColor = GetColor("PropertyDeclaration") ?? GetColor("PropertyAccess");
			this.propertyAccessColor = GetColor("PropertyAccess") ?? GetColor("PropertyDeclaration");
			this.eventDeclarationColor = GetColor("EventDeclaration") ?? GetColor("EventAccess");
			this.eventAccessColor = GetColor("EventAccess") ?? GetColor("EventDeclaration");
			//this.variableDeclarationColor = this.variableAccessColor = defaultTextColor;
			//this.parameterDeclarationColor = this.parameterAccessColor = defaultTextColor;
			this.valueKeywordColor = GetColor("NullOrValueKeywords") ?? GetColor("Keywords");
			this.thisKeywordColor = GetColor("ThisOrBaseReference") ?? GetColor("Keywords");
			this.trueKeywordColor = GetColor("TrueFalse") ?? GetColor("Keywords");
			this.typeKeywordsColor = GetColor("TypeKeywords") ?? GetColor("Keywords");
			this.attributeKeywordsColor = GetColor("AttributeKeywords") ?? GetColor("Keywords");
			//this.externAliasKeywordColor = ...;

			HighlightingColor GetColor(string colorName)
			{
				var color = highlighting.GetNamedColor(colorName);
				return color is not { Foreground: null, Background: null, FontFamily: null, FontWeight: null, FontSize: null, FontStyle: null, Strikethrough: null, Underline: null } ? color : null;
			}
		}

		public override void WriteKeyword(Role role, string keyword)
		{
			HighlightingColor color = null;
			switch (keyword)
			{
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
				case "with":
					if (role == WithInitializerExpression.WithKeywordRole)
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
				case "init":
					if (role == PropertyDeclaration.GetKeywordRole ||
						role == PropertyDeclaration.SetKeywordRole ||
						role == PropertyDeclaration.InitKeywordRole ||
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
				case "record":
					color = role == Roles.RecordKeyword ? referenceTypeKeywordsColor : valueTypeKeywordsColor;
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
				case "scoped":
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
			if (color != null)
			{
				BeginSpan(color);
			}
			base.WriteKeyword(role, keyword);
			if (color != null)
			{
				EndSpan();
			}
		}

		public override void WritePrimitiveType(string type)
		{
			HighlightingColor color = null;
			switch (type)
			{
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
				case "nint":
				case "nuint":
					color = valueTypeKeywordsColor;
					break;
				case "class":
				case "object":
				case "string":
				case "void":
				case "dynamic":
					color = referenceTypeKeywordsColor;
					break;
			}
			if (color != null)
			{
				BeginSpan(color);
			}
			base.WritePrimitiveType(type);
			if (color != null)
			{
				EndSpan();
			}
		}

		public override void WriteIdentifier(Identifier identifier)
		{
			HighlightingColor color = null;
			if (identifier.Name == "value"
				&& identifier.Parent?.GetResolveResult() is ILVariableResolveResult rr
				&& rr.Variable.Kind == Decompiler.IL.VariableKind.Parameter
				&& identifier.Ancestors.OfType<Accessor>().FirstOrDefault() is Accessor accessor
				&& accessor.Role != PropertyDeclaration.GetterRole)
			{
				color = valueKeywordColor;
			}
			if (identifier.Parent is AstType)
			{
				switch (identifier.Name)
				{
					case "var":
						color = queryKeywordsColor;
						break;
					case "global":
						color = structureKeywordsColor;
						break;
				}
			}
			switch (GetCurrentDefinition())
			{
				case ITypeDefinition t:
					switch (t.Kind)
					{
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
				case IMethod:
					color = methodDeclarationColor;
					break;
				case IField:
					color = fieldDeclarationColor;
					break;
				case IProperty:
					color = propertyDeclarationColor;
					break;
				case IEvent:
					color = eventDeclarationColor;
					break;
			}
			switch (GetCurrentMemberReference())
			{
				case IType t:
					switch (t.Kind)
					{
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
				case IMethod:
					color = methodCallColor;
					break;
				case IField:
					color = fieldAccessColor;
					break;
				case IProperty:
					color = propertyAccessColor;
					break;
				case IEvent:
					color = eventAccessColor;
					break;
			}
			if (color != null)
			{
				BeginSpan(color);
			}
			base.WriteIdentifier(identifier);
			if (color != null)
			{
				EndSpan();
			}
		}

		public override void WritePrimitiveValue(object value, Decompiler.CSharp.Syntax.LiteralFormat format)
		{
			HighlightingColor color = null;
			if (value is null)
			{
				color = valueKeywordColor;
			}
			if (value is true || value is false)
			{
				color = trueKeywordColor;
			}
			if (color != null)
			{
				BeginSpan(color);
			}
			base.WritePrimitiveValue(value, format);
			if (color != null)
			{
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
			if (symbol == null && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression)
			{
				symbol = node.Parent.GetSymbol();
			}
			if (symbol != null && node.Parent is ObjectCreateExpression)
			{
				symbol = node.Parent.GetSymbol();
			}
			if (node is IdentifierExpression && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression && symbol is IMember member)
			{
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
			if (textOutput != null)
			{
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
			if (textOutput != null)
			{
				textOutput.EndSpan();
				return;
			}

			HighlightingModel.SetHighlighting(currentColorBegin, locatable.Length - currentColorBegin, currentColor);
			currentColor = colorStack.Pop();
			currentColorBegin = locatable.Length;
		}
	}
}
