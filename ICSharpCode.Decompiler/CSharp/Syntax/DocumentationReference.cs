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

#nullable enable

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// No C# spec grammar production: this models a 'cref' reference inside XML documentation comments, not C# source syntax.
	/// <code>
	/// documentation_reference ::=
	///       type_name
	///     | type_name '.' member_name
	///     | member_name
	/// </code>
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class DocumentationReference : AstNode
	{
		/// <summary>
		/// Gets/Sets the entity type.
		/// Possible values are:
		///   <c>SymbolKind.Operator</c> for operators,
		///   <c>SymbolKind.Indexer</c> for indexers,
		///   <c>SymbolKind.TypeDefinition</c> for references to primitive types,
		///   and <c>SymbolKind.None</c> for everything else.
		/// </summary>
		public SymbolKind SymbolKind { get; set; }

		/// <summary>
		/// Gets/Sets the operator type.
		/// This property is only used when SymbolKind==Operator.
		/// </summary>
		public OperatorType OperatorType { get; set; }

		/// <summary>
		/// Gets/Sets whether a parameter list was provided.
		/// </summary>
		public bool HasParameterList { get; set; }

		/// <summary>
		/// Gets/Sets the declaring type.
		/// </summary>
		[Slot("DeclaringType")]
		public partial AstType? DeclaringType { get; set; }

		/// <summary>
		/// Gets/sets the member name.
		/// This property is only used when SymbolKind==None.
		/// </summary>
		public string MemberName {
			get { return NameToken.Name; }
			set { NameToken = Identifier.Create(value); }
		}

		[Slot("Identifier")]
		public partial Identifier NameToken { get; set; }

		/// <summary>
		/// Gets/Sets the return type of conversion operators.
		/// This property is only used when SymbolKind==Operator and OperatorType is explicit or implicit.
		/// </summary>
		[Slot("ConversionOperatorReturnType")]
		public partial AstType ConversionOperatorReturnType { get; set; }

		[Slot("TypeArgument")]
		public partial AstNodeCollection<AstType> TypeArguments { get; }

		[Slot("Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> Parameters { get; }

		protected internal override bool DoMatch(AstNode? other, PatternMatching.Match match)
		{
			DocumentationReference? o = other as DocumentationReference;
			if (!(o != null && this.SymbolKind == o.SymbolKind && this.HasParameterList == o.HasParameterList))
				return false;
			if (this.SymbolKind == SymbolKind.Operator)
			{
				if (this.OperatorType != o.OperatorType)
					return false;
				if (this.OperatorType == OperatorType.Implicit || this.OperatorType == OperatorType.Explicit)
				{
					if (!this.ConversionOperatorReturnType.DoMatch(o.ConversionOperatorReturnType, match))
						return false;
				}
			}
			else if (this.SymbolKind == SymbolKind.None)
			{
				if (!MatchString(this.MemberName, o.MemberName))
					return false;
				if (!this.TypeArguments.DoMatch(o.TypeArguments, match))
					return false;
			}
			return this.Parameters.DoMatch(o.Parameters, match);
		}
	}
}
