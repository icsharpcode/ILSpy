// 
// TypeDeclaration.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2009 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public enum ClassType
	{
		Class,
		Struct,
		Interface,
		Enum,
		/// <summary>
		/// C# 9 'record class'
		/// </summary>
		RecordClass,
		/// <summary>
		/// C# 10 'record struct'
		/// </summary>
		RecordStruct,
	}

	/// <summary>
	/// <c>class_declaration : attributes? class_modifier* 'partial'? class_tag identifier type_parameter_list? delimited_parameter_list? class_base? type_parameter_constraints_clause* class_body ;</c> (C# grammar §15.2.1)
	/// <c>struct_declaration : non_record_struct_declaration | record_struct_declaration ;</c> (C# grammar §16.2.1)
	/// <c>interface_declaration : attributes? interface_modifier* 'partial'? 'interface' identifier variant_type_parameter_list? interface_base? type_parameter_constraints_clause* interface_body ';'? ;</c> (C# grammar §19.2.1)
	/// <c>enum_declaration : attributes? enum_modifier* 'enum' identifier enum_base? enum_body ';'? ;</c> (C# grammar §20.2)
	/// </summary>
	[DecompilerAstNode(hasNullNode: false)]
	public partial class TypeDeclaration : EntityDeclaration
	{
		public override NodeType NodeType {
			get { return NodeType.TypeDeclaration; }
		}

		public override SymbolKind SymbolKind {
			get { return SymbolKind.TypeDefinition; }
		}

		ClassType classType;

		public ClassType ClassType {
			get { return classType; }
			set {
				classType = value;
			}
		}

		[Slot("AttributeRole")]
		public override partial AstNodeCollection<AttributeSection> Attributes { get; }

		[Slot("Roles.Identifier")]
		public override partial Identifier NameToken { get; set; }

		[Slot("Roles.TypeParameter")]
		public partial AstNodeCollection<TypeParameterDeclaration> TypeParameters { get; }

		public bool HasPrimaryConstructor { get; set; }

		[Slot("Roles.Parameter")]
		public partial AstNodeCollection<ParameterDeclaration> PrimaryConstructorParameters { get; }

		[Slot("Roles.BaseType")]
		public partial AstNodeCollection<AstType> BaseTypes { get; }

		[Slot("Roles.Constraint")]
		public partial AstNodeCollection<Constraint> Constraints { get; }

		[Slot("Roles.TypeMemberRole")]
		public partial AstNodeCollection<EntityDeclaration> Members { get; }

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			TypeDeclaration o = other as TypeDeclaration;
			return o != null && this.ClassType == o.ClassType && MatchString(this.Name, o.Name)
				&& this.MatchAttributesAndModifiers(o, match) && this.TypeParameters.DoMatch(o.TypeParameters, match)
				&& this.BaseTypes.DoMatch(o.BaseTypes, match) && this.Constraints.DoMatch(o.Constraints, match)
				&& this.HasPrimaryConstructor == o.HasPrimaryConstructor
				&& this.PrimaryConstructorParameters.DoMatch(o.PrimaryConstructorParameters, match)
				&& this.Members.DoMatch(o.Members, match);
		}
	}
}
