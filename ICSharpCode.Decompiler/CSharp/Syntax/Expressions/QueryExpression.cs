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
	[DecompilerAstNode]
	public sealed partial class QueryExpression : Expression
	{
		[Slot("Clause")]
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
	[DecompilerAstNode]
	public sealed partial class QueryContinuationClause : QueryClause
	{
		public const string IntoKeyword = "into";

		[Slot("PrecedingQuery")]
		public partial QueryExpression PrecedingQuery { get; set; }

		[Slot("Identifier")]
		public partial string Identifier { get; set; }
	}

	/// <summary>
	/// <c>from_clause ::= 'from' type? identifier 'in' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryFromClause : QueryClause
	{
		public const string FromKeyword = "from";
		public const string InKeyword = "in";

		[Slot("Type")]
		public partial AstType? Type { get; set; }

		[Slot("Identifier")]
		public partial string Identifier { get; set; }

		[Slot("Expression")]
		public partial Expression Expression { get; set; }
	}

	/// <summary>
	/// <c>let_clause ::= 'let' identifier '=' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryLetClause : QueryClause
	{
		public const string LetKeyword = "let";

		[Slot("Identifier")]
		public partial string Identifier { get; set; }

		[Slot("Expression")]
		public partial Expression Expression { get; set; }
	}

	/// <summary>
	/// <c>where_clause ::= 'where' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryWhereClause : QueryClause
	{
		public const string WhereKeyword = "where";

		[Slot("Condition")]
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
	[DecompilerAstNode]
	public sealed partial class QueryJoinClause : QueryClause
	{
		public const string JoinKeyword = "join";
		public const string InKeyword = "in";
		public const string OnKeyword = "on";
		public const string EqualsKeyword = "equals";
		public const string IntoKeyword = "into";

		// Derived from IntoIdentifier (which DoMatch already compares); exclude it to avoid a redundant compare.
		[ExcludeFromMatch]
		public bool IsGroupJoin {
			get { return !string.IsNullOrEmpty(this.IntoIdentifier); }
		}

		[Slot("Type")]
		public partial AstType? Type { get; set; }

		[Slot("JoinIdentifier")]
		public partial string JoinIdentifier { get; set; }

		[Slot("InExpression")]
		public partial Expression InExpression { get; set; }

		[Slot("OnExpression")]
		public partial Expression OnExpression { get; set; }

		[Slot("EqualsExpression")]
		public partial Expression EqualsExpression { get; set; }

		[Slot("IntoIdentifier")]
		public partial string? IntoIdentifier { get; set; }
	}

	/// <summary>
	/// <c>orderby_clause ::= 'orderby' ordering ( ',' ordering )*</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryOrderClause : QueryClause
	{
		public const string OrderbyKeyword = "orderby";

		[Slot("Ordering")]
		public partial AstNodeCollection<QueryOrdering> Orderings { get; }
	}

	/// <summary>
	/// <c>ordering ::= expression ( 'ascending' | 'descending' )?</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryOrdering : AstNode
	{
		public const string AscendingKeyword = "ascending";
		public const string DescendingKeyword = "descending";

		[Slot("Expression")]
		public partial Expression Expression { get; set; }

		public QueryOrderingDirection Direction { get; set; }
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
	[DecompilerAstNode]
	public sealed partial class QuerySelectClause : QueryClause
	{
		public const string SelectKeyword = "select";

		[Slot("Expression")]
		public partial Expression Expression { get; set; }
	}

	/// <summary>
	/// <c>group_clause ::= 'group' expression 'by' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class QueryGroupClause : QueryClause
	{
		public const string GroupKeyword = "group";
		public const string ByKeyword = "by";

		[Slot("Projection")]
		public partial Expression Projection { get; set; }

		[Slot("Key")]
		public partial Expression Key { get; set; }
	}
}
