// 
// NamespaceDeclaration.cs
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
#nullable enable

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// <code>
	/// namespace_declaration ::=
	///       'namespace' type '{' namespace_member* '}' ';'?
	///     | 'namespace' type ';' namespace_member*
	/// </code>
	/// (C# grammar §14.3)
	/// </summary>
	[DecompilerAstNode]
	public sealed partial class NamespaceDeclaration : AstNode
	{
		public bool IsFileScoped { get; set; }

		[Slot("NamespaceName")]
		public partial AstType NamespaceName { get; set; }

		// Computed from NamespaceName (the matched slot); exclude as redundant.
		[ExcludeFromMatch]
		public string Name {
			get {
				return UsingDeclaration.ConstructNamespace(NamespaceName);
			}
			set {
				var arr = value.Split('.');
				NamespaceName = ConstructType(arr, arr.Length - 1);
			}
		}

		static AstType ConstructType(string[] arr, int i)
		{
			if (i < 0 || i >= arr.Length)
				throw new ArgumentOutOfRangeException(nameof(i));
			if (i == 0)
				return new SimpleType(arr[i]);
			return new MemberType(ConstructType(arr, i - 1), arr[i]);
		}

		/// <summary>
		/// Gets the full namespace name (including any parent namespaces)
		/// </summary>
		// Computed by walking the parent chain; exclude as redundant and costly.
		[ExcludeFromMatch]
		public string FullName {
			get {
				NamespaceDeclaration? parentNamespace = Parent as NamespaceDeclaration;
				if (parentNamespace != null)
					return BuildQualifiedName(parentNamespace.FullName, Name);
				return Name;
			}
		}

		// Computed from NamespaceName (the matched slot); exclude it (and it is not reference-comparable).
		[ExcludeFromMatch]
		public IEnumerable<string> Identifiers {
			get {
				var result = new Stack<string>();
				AstType type = NamespaceName;
				while (type is MemberType)
				{
					var mt = (MemberType)type;
					result.Push(mt.MemberName);
					type = mt.Target;
				}
				if (type is SimpleType simpleType)
					result.Push(simpleType.Identifier ?? string.Empty);
				return result;
			}
		}

		[Slot("Member")]
		public partial AstNodeCollection<AstNode> Members { get; }

		public NamespaceDeclaration(string name)
		{
			this.Name = name;
		}

		public static string BuildQualifiedName(string name1, string name2)
		{
			if (string.IsNullOrEmpty(name1))
				return name2;
			if (string.IsNullOrEmpty(name2))
				return name1;
			return name1 + "." + name2;
		}

		public void AddMember(AstNode child)
		{
			AddChild(child, Slots.Member);
		}
	}
};
