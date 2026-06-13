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
	/// Represents a query continuation.
	/// "(from .. select ..) into Identifier" or "(from .. group .. by ..) into Identifier"
	/// Note that "join .. into .." is not a query continuation!
	/// 
	/// This is always the first(!!) clause in a query expression.
	/// The tree for "from a in b select c into d select e" looks like this:
	/// new QueryExpression {
	/// 	new QueryContinuationClause {
	/// 		PrecedingQuery = new QueryExpression {
	/// 			new QueryFromClause(a in b),
	/// 			new QuerySelectClause(c)
	/// 		},
	/// 		Identifier = d
	/// 	},
	/// 	new QuerySelectClause(e)
	/// }
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryContinuationClause : QueryClause
	{
		public static readonly Role<QueryExpression> PrecedingQueryRole = new Role<QueryExpression>("PrecedingQuery", QueryExpression.Null);
		public static readonly TokenRole IntoKeywordRole = new TokenRole("into");

		[Slot("PrecedingQueryRole")]
		public partial QueryExpression PrecedingQuery { get; set; }

		public CSharpTokenNode IntoKeyword {
			get { return GetChildByRole(IntoKeywordRole); }
		}

		public string Identifier {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Decompiler.CSharp.Syntax.Identifier.Create(value));
			}
		}

		public Identifier IdentifierToken {
			get { return GetChildByRole(Roles.Identifier); }
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryContinuationClause o = other as QueryContinuationClause;
			return o != null && MatchString(this.Identifier, o.Identifier) && this.PrecedingQuery.DoMatch(o.PrecedingQuery, match);
		}
	}

	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryFromClause : QueryClause
	{
		public static readonly TokenRole FromKeywordRole = new TokenRole("from");
		public static readonly TokenRole InKeywordRole = new TokenRole("in");

		public CSharpTokenNode FromKeyword {
			get { return GetChildByRole(FromKeywordRole); }
		}

		[Slot("Roles.Type")]
		public partial AstType Type { get; set; }

		public string Identifier {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Decompiler.CSharp.Syntax.Identifier.Create(value));
			}
		}

		public Identifier IdentifierToken {
			get { return GetChildByRole(Roles.Identifier); }
		}

		public CSharpTokenNode InKeyword {
			get { return GetChildByRole(InKeywordRole); }
		}

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryFromClause o = other as QueryFromClause;
			return o != null && this.Type.DoMatch(o.Type, match) && MatchString(this.Identifier, o.Identifier)
				&& this.Expression.DoMatch(o.Expression, match);
		}
	}

	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryLetClause : QueryClause
	{
		public readonly static TokenRole LetKeywordRole = new TokenRole("let");

		public CSharpTokenNode LetKeyword {
			get { return GetChildByRole(LetKeywordRole); }
		}

		public string Identifier {
			get {
				return GetChildByRole(Roles.Identifier).Name;
			}
			set {
				SetChildByRole(Roles.Identifier, Decompiler.CSharp.Syntax.Identifier.Create(value));
			}
		}

		public Identifier IdentifierToken {
			get { return GetChildByRole(Roles.Identifier); }
		}

		public CSharpTokenNode AssignToken {
			get { return GetChildByRole(Roles.Assign); }
		}

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryLetClause o = other as QueryLetClause;
			return o != null && MatchString(this.Identifier, o.Identifier) && this.Expression.DoMatch(o.Expression, match);
		}
	}


	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryWhereClause : QueryClause
	{
		public readonly static TokenRole WhereKeywordRole = new TokenRole("where");

		public CSharpTokenNode WhereKeyword {
			get { return GetChildByRole(WhereKeywordRole); }
		}

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

		public CSharpTokenNode JoinKeyword {
			get { return GetChildByRole(JoinKeywordRole); }
		}

		[Slot("TypeRole")]
		public partial AstType Type { get; set; }

		public string JoinIdentifier {
			get {
				return GetChildByRole(JoinIdentifierRole).Name;
			}
			set {
				SetChildByRole(JoinIdentifierRole, Identifier.Create(value));
			}
		}

		public Identifier JoinIdentifierToken {
			get { return GetChildByRole(JoinIdentifierRole); }
		}

		public CSharpTokenNode InKeyword {
			get { return GetChildByRole(InKeywordRole); }
		}

		[Slot("InExpressionRole")]
		public partial Expression InExpression { get; set; }

		public CSharpTokenNode OnKeyword {
			get { return GetChildByRole(OnKeywordRole); }
		}

		[Slot("OnExpressionRole")]
		public partial Expression OnExpression { get; set; }

		public CSharpTokenNode EqualsKeyword {
			get { return GetChildByRole(EqualsKeywordRole); }
		}

		[Slot("EqualsExpressionRole")]
		public partial Expression EqualsExpression { get; set; }

		public CSharpTokenNode IntoKeyword {
			get { return GetChildByRole(IntoKeywordRole); }
		}

		public string IntoIdentifier {
			get {
				return GetChildByRole(IntoIdentifierRole).Name;
			}
			set {
				SetChildByRole(IntoIdentifierRole, Identifier.Create(value));
			}
		}

		public Identifier IntoIdentifierToken {
			get { return GetChildByRole(IntoIdentifierRole); }
		}

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

	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryOrderClause : QueryClause
	{
		public static readonly TokenRole OrderbyKeywordRole = new TokenRole("orderby");
		public static readonly Role<QueryOrdering> OrderingRole = new Role<QueryOrdering>("Ordering", null);

		public CSharpTokenNode OrderbyToken {
			get { return GetChildByRole(OrderbyKeywordRole); }
		}

		[Slot("OrderingRole")]
		public partial AstNodeCollection<QueryOrdering> Orderings { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryOrderClause o = other as QueryOrderClause;
			return o != null && this.Orderings.DoMatch(o.Orderings, match);
		}
	}

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

		public CSharpTokenNode DirectionToken {
			get { return Direction == QueryOrderingDirection.Ascending ? GetChildByRole(AscendingKeywordRole) : GetChildByRole(DescendingKeywordRole); }
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

	[DecompilerAstNode(hasNullNode: false)]
	public partial class QuerySelectClause : QueryClause
	{
		public readonly static TokenRole SelectKeywordRole = new TokenRole("select");

		public CSharpTokenNode SelectKeyword {
			get { return GetChildByRole(SelectKeywordRole); }
		}

		[Slot("Roles.Expression")]
		public partial Expression Expression { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QuerySelectClause o = other as QuerySelectClause;
			return o != null && this.Expression.DoMatch(o.Expression, match);
		}
	}

	[DecompilerAstNode(hasNullNode: false)]
	public partial class QueryGroupClause : QueryClause
	{
		public static readonly TokenRole GroupKeywordRole = new TokenRole("group");
		public static readonly Role<Expression> ProjectionRole = new Role<Expression>("Projection", Expression.Null);
		public static readonly TokenRole ByKeywordRole = new TokenRole("by");
		public static readonly Role<Expression> KeyRole = new Role<Expression>("Key", Expression.Null);

		public CSharpTokenNode GroupKeyword {
			get { return GetChildByRole(GroupKeywordRole); }
		}

		[Slot("ProjectionRole")]
		public partial Expression Projection { get; set; }

		public CSharpTokenNode ByKeyword {
			get { return GetChildByRole(ByKeywordRole); }
		}

		[Slot("KeyRole")]
		public partial Expression Key { get; set; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			QueryGroupClause o = other as QueryGroupClause;
			return o != null && this.Projection.DoMatch(o.Projection, match) && this.Key.DoMatch(o.Key, match);
		}
	}
}
