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

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <c>query_expression ::= query_clause+</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: true)]
	public partial class QueryExpression : Expression
	{
		public static readonly Role<QueryClause> ClauseRole = new Role<QueryClause>("Clause", null);

		[Slot("ClauseRole")]
		public partial AstNodeCollection<QueryClause> Clauses { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryExpression o = other as QueryExpression;
			return o != null && !o.IsNull && this.Clauses.DoMatch(o.Clauses, match);
		}
	}

	public abstract class QueryClause : AstNode
	{
		public override NodeType NodeType {
			get { return NodeType.QueryClause; }
		}
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
		public static readonly Role<QueryExpression> PrecedingQueryRole = new Role<QueryExpression>("PrecedingQuery", QueryExpression.Null);
		public static readonly TokenRole IntoKeywordRole = new TokenRole("into");

		[Slot("PrecedingQueryRole")]
		public partial QueryExpression PrecedingQuery { get; set; }

		public string Identifier {
			get {
				return IdentifierToken.Name;
			}
			set {
				IdentifierToken = Decompiler.CSharp.Syntax.Identifier.Create(value);
			}
		}

		[Slot("Roles.Identifier")]
		public partial Identifier IdentifierToken { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryContinuationClause o = other as QueryContinuationClause;
			return o != null && MatchString(this.Identifier, o.Identifier) && this.PrecedingQuery.DoMatch(o.PrecedingQuery, match);
		}
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
		public partial AstType Type { get; set; }

		public string Identifier {
			get {
				return IdentifierToken.Name;
			}
			set {
				IdentifierToken = Decompiler.CSharp.Syntax.Identifier.Create(value);
			}
		}

		[Slot("Roles.Identifier")]
		public partial Identifier IdentifierToken { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryFromClause o = other as QueryFromClause;
			return o != null && this.Type.DoMatch(o.Type, match) && MatchString(this.Identifier, o.Identifier)
				&& this.Expression.DoMatch(o.Expression, match);
		}
	}

	/// <summary>
	/// <c>let_clause ::= 'let' identifier '=' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryLetClause : QueryClause
	{
		public readonly static TokenRole LetKeywordRole = new TokenRole("let");

		public string Identifier {
			get {
				return IdentifierToken.Name;
			}
			set {
				IdentifierToken = Decompiler.CSharp.Syntax.Identifier.Create(value);
			}
		}

		[Slot("Roles.Identifier")]
		public partial Identifier IdentifierToken { get; set; }

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryLetClause o = other as QueryLetClause;
			return o != null && MatchString(this.Identifier, o.Identifier) && this.Expression.DoMatch(o.Expression, match);
		}
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

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryWhereClause o = other as QueryWhereClause;
			return o != null && this.Condition.DoMatch(o.Condition, match);
		}
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
		public static readonly Role<AstType> TypeRole = Roles.Type;
		public static readonly Role<Identifier> JoinIdentifierRole = Roles.Identifier;
		public static readonly TokenRole InKeywordRole = new TokenRole("in");
		public static readonly Role<Expression> InExpressionRole = Roles.Expression;
		public static readonly TokenRole OnKeywordRole = new TokenRole("on");
		public static readonly Role<Expression> OnExpressionRole = new Role<Expression>("OnExpression", Expression.Null);
		public static readonly TokenRole EqualsKeywordRole = new TokenRole("equals");
		public static readonly Role<Expression> EqualsExpressionRole = new Role<Expression>("EqualsExpression", Expression.Null);
		public static readonly TokenRole IntoKeywordRole = new TokenRole("into");
		public static readonly Role<Identifier> IntoIdentifierRole = new Role<Identifier>("IntoIdentifier", Identifier.Null);

		public bool IsGroupJoin {
			get { return !string.IsNullOrEmpty(this.IntoIdentifier); }
		}

		[Slot("TypeRole")]
		public partial AstType Type { get; set; }

		public string JoinIdentifier {
			get {
				return JoinIdentifierToken.Name;
			}
			set {
				JoinIdentifierToken = Identifier.Create(value);
			}
		}

		[Slot("JoinIdentifierRole")]
		public partial Identifier JoinIdentifierToken { get; set; }

		[Slot("InExpressionRole")]
		public partial Expression InExpression { get; set; }

		[Slot("OnExpressionRole")]
		public partial Expression OnExpression { get; set; }

		[Slot("EqualsExpressionRole")]
		public partial Expression EqualsExpression { get; set; }

		public string IntoIdentifier {
			get {
				return IntoIdentifierToken.Name;
			}
			set {
				IntoIdentifierToken = Identifier.Create(value);
			}
		}

		[Slot("IntoIdentifierRole")]
		public partial Identifier IntoIdentifierToken { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryJoinClause o = other as QueryJoinClause;
			return o != null && this.IsGroupJoin == o.IsGroupJoin
				&& this.Type.DoMatch(o.Type, match) && MatchString(this.JoinIdentifier, o.JoinIdentifier)
				&& this.InExpression.DoMatch(o.InExpression, match) && this.OnExpression.DoMatch(o.OnExpression, match)
				&& this.EqualsExpression.DoMatch(o.EqualsExpression, match)
				&& MatchString(this.IntoIdentifier, o.IntoIdentifier);
		}
	}

	/// <summary>
	/// <c>orderby_clause ::= 'orderby' ordering ( ',' ordering )*</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryOrderClause : QueryClause
	{
		public static readonly TokenRole OrderbyKeywordRole = new TokenRole("orderby");
		public static readonly Role<QueryOrdering> OrderingRole = new Role<QueryOrdering>("Ordering", null);

		[Slot("OrderingRole")]
		public partial AstNodeCollection<QueryOrdering> Orderings { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryOrderClause o = other as QueryOrderClause;
			return o != null && this.Orderings.DoMatch(o.Orderings, match);
		}
	}

	/// <summary>
	/// <c>ordering ::= expression ( 'ascending' | 'descending' )?</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryOrdering : AstNode
	{
		public readonly static TokenRole AscendingKeywordRole = new TokenRole("ascending");
		public readonly static TokenRole DescendingKeywordRole = new TokenRole("descending");

		public override NodeType NodeType {
			get { return NodeType.Unknown; }
		}

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		public QueryOrderingDirection Direction {
			get;
			set;
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryOrdering o = other as QueryOrdering;
			return o != null && this.Direction == o.Direction && this.Expression.DoMatch(o.Expression, match);
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

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QuerySelectClause o = other as QuerySelectClause;
			return o != null && this.Expression.DoMatch(o.Expression, match);
		}
	}

	/// <summary>
	/// <c>group_clause ::= 'group' expression 'by' expression</c> (C# grammar §12.23.1)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryGroupClause : QueryClause
	{
		public static readonly TokenRole GroupKeywordRole = new TokenRole("group");
		public static readonly Role<Expression> ProjectionRole = new Role<Expression>("Projection", Expression.Null);
		public static readonly TokenRole ByKeywordRole = new TokenRole("by");
		public static readonly Role<Expression> KeyRole = new Role<Expression>("Key", Expression.Null);

		[Slot("ProjectionRole")]
		public partial Expression Projection { get; set; }

		[Slot("KeyRole")]
		public partial Expression Key { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryGroupClause o = other as QueryGroupClause;
			return o != null && this.Projection.DoMatch(o.Projection, match) && this.Key.DoMatch(o.Key, match);
		}
	}
}
