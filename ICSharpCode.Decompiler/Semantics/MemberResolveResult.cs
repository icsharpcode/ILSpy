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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Semantics
{
	/// <summary>
	/// Represents the result of a member invocation.
	/// Used for field/property/event access.
	/// Also, <see cref="InvocationResolveResult"/> derives from MemberResolveResult.
	/// </summary>
	public class MemberResolveResult : ResolveResult
	{
		readonly bool isConstant;

		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnTypeOverride = null)
			: base(returnTypeOverride ?? ComputeType(member))
		{
			this.TargetResult = targetResult;
			this.Member = member;
			var thisRR = targetResult as ThisResolveResult;
			this.IsVirtualCall = member.IsOverridable && !(thisRR != null && thisRR.CausesNonVirtualInvocation);
			
			var field = member as IField;
			if (field != null) {
				isConstant = field.IsConst;
				if (isConstant)
					ConstantValue = field.ConstantValue;
			}
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, bool isVirtualCall, IType returnTypeOverride = null)
			: base(returnTypeOverride ?? ComputeType(member))
		{
			this.TargetResult = targetResult;
			this.Member = member;
			this.IsVirtualCall = isVirtualCall;
			var field = member as IField;
			if (field != null) {
				isConstant = field.IsConst;
				if (isConstant)
					ConstantValue = field.ConstantValue;
			}
		}
		
		static IType ComputeType(IMember member)
		{
			switch (member.SymbolKind) {
				case SymbolKind.Constructor:
					return member.DeclaringType ?? SpecialType.UnknownType;
				case SymbolKind.Field:
					if (((IField)member).IsFixed)
						return new PointerType(member.ReturnType);
					break;
			}
			if (member.ReturnType.Kind == TypeKind.ByReference)
				return ((ByReferenceType)member.ReturnType).ElementType;
			return member.ReturnType;
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnType, bool isConstant, object constantValue)
			: base(returnType)
		{
			this.TargetResult = targetResult;
			this.Member = member;
			this.isConstant = isConstant;
			this.ConstantValue = constantValue;
		}
		
		public MemberResolveResult(ResolveResult targetResult, IMember member, IType returnType, bool isConstant, object constantValue, bool isVirtualCall)
			: base(returnType)
		{
			this.TargetResult = targetResult;
			this.Member = member;
			this.isConstant = isConstant;
			this.ConstantValue = constantValue;
			this.IsVirtualCall = isVirtualCall;
		}
		
		public ResolveResult TargetResult { get; }

		/// <summary>
		/// Gets the member.
		/// This property never returns null.
		/// </summary>
		public IMember Member { get; }

		/// <summary>
		/// Gets whether this MemberResolveResult is a virtual call.
		/// </summary>
		public bool IsVirtualCall { get; }

		public override bool IsCompileTimeConstant => isConstant;

		public override object ConstantValue { get; }

		public override IEnumerable<ResolveResult> GetChildResults()
		{
			if (TargetResult != null)
				return new[] { TargetResult };
			else
				return Enumerable.Empty<ResolveResult>();
		}
		
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[{0} {1}]", GetType().Name, Member);
		}
	}
}
