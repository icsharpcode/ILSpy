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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>query_expression ::= query_clause+</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: true)]
	public partial class QueryExpression : Expression
	{

		[Slot("ClauseRole")]
		public partial AstNodeCollection<QueryClause> Clauses { get; }
	}

	public abstract class QueryClause : AstNode
	{
	}

	/// <summary>
	/// Represents a query continuation, e.g. "(from .. select ..) into Identifier".
	/// Note that "join .. into .." is not a query continuation.
	/// A continuation is always the first clause of the query expression that contains it.
	/// <c>query_continuation ::= query_expression 'into' identifier</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryContinuationClause : QueryClause
	{
		public static readonly TokenRole IntoKeywordRole = new TokenRole("into");

		[Slot("PrecedingQueryRole")]
		public partial QueryExpression PrecedingQuery { get; set; }

		[NameSlot("Roles.Identifier")]
		public partial string Identifier { get; set; }
	}

	/// <summary>
	/// <c>from_clause ::= 'from' type? identifier 'in' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryFromClause : QueryClause
	{
		public static readonly TokenRole FromKeywordRole = new TokenRole("from");
		public static readonly TokenRole InKeywordRole = new TokenRole("in");

		[Slot("Roles.Type")]
		public partial AstType? Type { get; set; }

		[NameSlot("Roles.Identifier")]
		public partial string Identifier { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }
	}

	/// <summary>
	/// <c>let_clause ::= 'let' identifier '=' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryLetClause : QueryClause
	{
		public readonly static TokenRole LetKeywordRole = new TokenRole("let");

		[NameSlot("Roles.Identifier")]
		public partial string Identifier { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }
	}


	/// <summary>
	/// <c>where_clause ::= 'where' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryWhereClause : QueryClause
	{
		public readonly static TokenRole WhereKeywordRole = new TokenRole("where");

		[Slot("Roles.Condition")]
		public partial Expression Condition { get; set; }
	}

	/// <summary>
	/// Represents a join or group join clause.
	/// <code>
	/// join_clause ::=
	///       'join' type? identifier 'in' expression 'on' expression 'equals' expression
	///     | 'join' type? identifier 'in' expression 'on' expression 'equals' expression 'into' identifier
	/// </code>
	/// (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryJoinClause : QueryClause
	{
		public static readonly TokenRole JoinKeywordRole = new TokenRole("join");
		public static readonly TokenRole InKeywordRole = new TokenRole("in");
		public static readonly TokenRole OnKeywordRole = new TokenRole("on");
		public static readonly TokenRole EqualsKeywordRole = new TokenRole("equals");
		public static readonly TokenRole IntoKeywordRole = new TokenRole("into");

		// Derived from IntoIdentifier (which DoMatch already compares); exclude it to avoid a redundant compare.
		[ExcludeFromMatch]
		public bool IsGroupJoin {
			get { return !string.IsNullOrEmpty(this.IntoIdentifier); }
		}

		[Slot("TypeRole")]
		public partial AstType? Type { get; set; }

		[NameSlot("JoinIdentifierRole")]
		public partial string JoinIdentifier { get; set; }

		[Slot("InExpressionRole")]
		public partial Expression InExpression { get; set; }

		[Slot("OnExpressionRole")]
		public partial Expression OnExpression { get; set; }

		[Slot("EqualsExpressionRole")]
		public partial Expression EqualsExpression { get; set; }

		[NameSlot("IntoIdentifierRole", nullOnEmpty: true)]
		public partial string IntoIdentifier { get; set; }
	}

	/// <summary>
	/// <c>orderby_clause ::= 'orderby' ordering ( ',' ordering )*</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryOrderClause : QueryClause
	{
		public static readonly TokenRole OrderbyKeywordRole = new TokenRole("orderby");

		[Slot("OrderingRole")]
		public partial AstNodeCollection<QueryOrdering> Orderings { get; }
	}

	/// <summary>
	/// <c>ordering ::= expression ( 'ascending' | 'descending' )?</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryOrdering : AstNode
	{
		public readonly static TokenRole AscendingKeywordRole = new TokenRole("ascending");
		public readonly static TokenRole DescendingKeywordRole = new TokenRole("descending");

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		public QueryOrderingDirection Direction {
			get;
			set;
		}
	}

	public enum QueryOrderingDirection
	{
		None,
		Ascending,
		Descending
	}

	/// <summary>
	/// <c>select_clause ::= 'select' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QuerySelectClause : QueryClause
	{
		public readonly static TokenRole SelectKeywordRole = new TokenRole("select");

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }
	}

	/// <summary>
	/// <c>group_clause ::= 'group' expression 'by' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryGroupClause : QueryClause
	{
		public static readonly TokenRole GroupKeywordRole = new TokenRole("group");
		public static readonly TokenRole ByKeywordRole = new TokenRole("by");

		[Slot("ProjectionRole")]
		public partial Expression Projection { get; set; }

		[Slot("KeyRole")]
		public partial Expression Key { get; set; }
	}
}
