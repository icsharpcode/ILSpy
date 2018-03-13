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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Semantics
{
	/// <summary>
	/// Represents an unknown member.
	/// </summary>
	public class UnknownMemberResolveResult : ResolveResult
	{
		public UnknownMemberResolveResult(IType targetType, string memberName, IEnumerable<IType> typeArguments)
			: base(SpecialType.UnknownType)
		{
			this.TargetType = targetType ?? throw new ArgumentNullException("targetType");
			this.MemberName = memberName;
			this.TypeArguments = new ReadOnlyCollection<IType>(typeArguments.ToArray());
		}
		
		/// <summary>
		/// The type on which the method is being called.
		/// </summary>
		public IType TargetType { get; }

		public string MemberName { get; }

		public ReadOnlyCollection<IType> TypeArguments { get; }

		public override bool IsError => true;

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}.{2}]", GetType().Name, TargetType, MemberName);
		}
	}
	
	/// <summary>
	/// Represents an unknown method.
	/// </summary>
	public class UnknownMethodResolveResult : UnknownMemberResolveResult
	{
		public UnknownMethodResolveResult(IType targetType, string methodName, IEnumerable<IType> typeArguments, IEnumerable<IParameter> parameters)
			: base(targetType, methodName, typeArguments)
		{
			this.Parameters = new ReadOnlyCollection<IParameter>(parameters.ToArray());
		}
		
		public ReadOnlyCollection<IParameter> Parameters { get; }
	}
	
	/// <summary>
	/// Represents an unknown identifier.
	/// </summary>
	public class UnknownIdentifierResolveResult : ResolveResult
	{
		public UnknownIdentifierResolveResult(string identifier, int typeArgumentCount = 0)
			: base(SpecialType.UnknownType)
		{
			this.Identifier = identifier;
			this.TypeArgumentCount = typeArgumentCount;
		}
		
		public string Identifier { get; }

		public int TypeArgumentCount { get; }

		public override bool IsError => true;

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, Identifier);
		}
	}
}
