// 
// FullTypeName.cs
//
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
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

using System;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	public class FunctionPointerAstType : AstType
	{
		public static readonly TokenRole PointerRole = new TokenRole("*");
		public static readonly Role<AstType> CallingConventionRole = new Role<AstType>("CallConv", AstType.Null);

		public AstType CallingConvention {
			get {
				return GetChildByRole(CallingConventionRole);
			}
			set {
				SetChildByRole(CallingConventionRole, value);
			}
		}

		public AstNodeCollection<ParameterDeclaration> Parameters {
			get { return GetChildrenByRole(Roles.Parameter); }
		}

		public AstType ReturnType {
			get { return GetChildByRole(Roles.Type); }
			set { SetChildByRole(Roles.Type, value); }
		}

		public override void AcceptVisitor(IAstVisitor visitor)
		{
			visitor.VisitFunctionPointerType(this);
		}

		public override T AcceptVisitor<T>(IAstVisitor<T> visitor)
		{
			return visitor.VisitFunctionPointerType(this);
		}

		public override S AcceptVisitor<T, S>(IAstVisitor<T, S> visitor, T data)
		{
			return visitor.VisitFunctionPointerType(this, data);
		}

		protected internal override bool DoMatch(AstNode other, PatternMatching.Match match)
		{
			return other is FunctionPointerAstType o
				&& this.CallingConvention.DoMatch(o.CallingConvention, match)
				&& this.Parameters.DoMatch(o.Parameters, match)
				&& this.ReturnType.DoMatch(o.ReturnType, match);
		}

		public override ITypeReference ToTypeReference(NameLookupMode lookupMode, InterningProvider interningProvider = null)
		{
			throw new NotImplementedException();
		}
	}
}
