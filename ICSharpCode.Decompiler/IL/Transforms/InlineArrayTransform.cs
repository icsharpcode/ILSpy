// Copyright (c) 2025 Siegfried Pammer
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

using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.IL.Transforms
{
	static class InlineArrayTransform
	{
		internal static bool RunOnExpression(Call inst, StatementTransformContext context)
		{
			if (MatchSpanIndexerWithInlineArrayAsSpan(inst, out var type, out var addr, out var index, out bool isReadOnly))
			{
				if (isReadOnly)
				{
					context.Step("call get_Item(addressof System.ReadOnlySpan{T}(call InlineArrayAsReadOnlySpan(addr)), index) -> readonly.ldelema.inlinearray(addr, index)", inst);
				}
				else
				{
					context.Step("call get_Item(addressof System.Span{T}(call InlineArrayAsSpan(addr)), index) -> ldelema.inlinearray(addr, index)", inst);
				}
				inst.ReplaceWith(new LdElemaInlineArray(type, addr, index) { IsReadOnly = isReadOnly }.WithILRange(inst));
				return true;
			}

			if (MatchInlineArrayElementRef(inst, out type, out addr, out index, out isReadOnly))
			{
				if (isReadOnly)
				{
					context.Step("call InlineArrayElementRefReadOnly(addr, index) -> readonly.ldelema.inlinearray(addr, index)", inst);
				}
				else
				{
					context.Step("call InlineArrayElementRef(addr, index) -> ldelema.inlinearray(addr, index)", inst);
				}
				inst.ReplaceWith(new LdElemaInlineArray(type, addr, index) { IsReadOnly = isReadOnly }.WithILRange(inst));
				return true;
			}

			if (MatchInlineArrayFirstElementRef(inst, out type, out addr, out isReadOnly))
			{
				if (isReadOnly)
				{
					context.Step("call InlineArrayFirstElementRefReadOnly(addr) -> readonly.ldelema.inlinearray(addr, ldc.i4 0)", inst);
				}
				else
				{
					context.Step("call InlineArrayFirstElementRef(addr) -> ldelema.inlinearray(addr, ldc.i4 0)", inst);
				}
				inst.ReplaceWith(new LdElemaInlineArray(type, addr, new LdcI4(0)) { IsReadOnly = isReadOnly }.WithILRange(inst));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Matches call get_Item(addressof System.(ReadOnly)Span[[T]](call InlineArrayAs(ReadOnly)Span(addr, length)), index)
		/// </summary>
		static bool MatchSpanIndexerWithInlineArrayAsSpan(Call inst, [NotNullWhen(true)] out IType? type, [NotNullWhen(true)] out ILInstruction? addr, [NotNullWhen(true)] out ILInstruction? index, out bool isReadOnly)
		{
			isReadOnly = false;
			type = null;
			addr = null;
			index = null;

			if (MatchSpanGetItem(inst.Method, "ReadOnlySpan"))
			{
				isReadOnly = true;

				if (inst.Arguments is not [AddressOf { Value: Call targetInst, Type: var typeInfo }, var indexInst])
					return false;

				if (!MatchInlineArrayHelper(targetInst.Method, "InlineArrayAsReadOnlySpan", out var inlineArrayType))
					return false;

				if (targetInst.Arguments is not [var addrInst, LdcI4 { Value: var length }])
					return false;

				if (length < 0 || length > inlineArrayType.GetInlineArrayLength())
					return false;

				type = inlineArrayType;
				addr = addrInst;
				index = indexInst;

				return true;
			}
			else if (MatchSpanGetItem(inst.Method, "Span"))
			{
				if (inst.Arguments is not [AddressOf { Value: Call targetInst, Type: var typeInfo }, var indexInst])
					return false;

				if (!MatchInlineArrayHelper(targetInst.Method, "InlineArrayAsSpan", out var inlineArrayType))
					return false;

				if (targetInst.Arguments is not [var addrInst, LdcI4 { Value: var length }])
					return false;

				if (length < 0 || length > inlineArrayType.GetInlineArrayLength())
					return false;

				type = inlineArrayType;
				addr = addrInst;
				index = indexInst;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Matches call InlineArrayElementRef(ReadOnly)(addr, index)
		/// </summary>
		static bool MatchInlineArrayElementRef(Call inst, [NotNullWhen(true)] out IType? type, [NotNullWhen(true)] out ILInstruction? addr, [NotNullWhen(true)] out ILInstruction? index, out bool isReadOnly)
		{
			type = null;
			addr = null;
			index = null;
			isReadOnly = false;

			if (inst.Arguments is not [var addrInst, LdcI4 { Value: var indexValue } indexInst])
				return false;

			addr = addrInst;
			index = indexInst;

			if (MatchInlineArrayHelper(inst.Method, "InlineArrayElementRef", out var inlineArrayType))
			{
				isReadOnly = false;
				type = inlineArrayType;
			}
			else if (MatchInlineArrayHelper(inst.Method, "InlineArrayElementRefReadOnly", out inlineArrayType))
			{
				isReadOnly = true;
				type = inlineArrayType;
			}
			else
			{
				return false;
			}

			if (indexValue < 0 || indexValue >= inlineArrayType.GetInlineArrayLength())
			{
				return false;
			}

			return true;
		}

		private static bool MatchInlineArrayFirstElementRef(Call inst, [NotNullWhen(true)] out IType? type, [NotNullWhen(true)] out ILInstruction? addr, out bool isReadOnly)
		{
			type = null;
			addr = null;
			isReadOnly = false;

			if (inst.Arguments is not [var addrInst])
				return false;

			if (MatchInlineArrayHelper(inst.Method, "InlineArrayFirstElementRef", out var inlineArrayType))
			{
				isReadOnly = false;
				type = inlineArrayType;
				addr = addrInst;
				return true;
			}

			if (MatchInlineArrayHelper(inst.Method, "InlineArrayFirstElementRefReadOnly", out inlineArrayType))
			{
				isReadOnly = true;
				type = inlineArrayType;
				addr = addrInst;
				return true;
			}

			return false;
		}

		static bool MatchSpanGetItem(IMethod method, string typeName)
		{
			return method is {
				IsStatic: false,
				Name: "get_Item",
				DeclaringType: { Namespace: "System", Name: string name, TypeParameterCount: 1, DeclaringType: null }
			} && typeName == name;
		}

		static bool MatchInlineArrayHelper(IMethod method, string methodName, [NotNullWhen(true)] out IType? inlineArrayType)
		{
			inlineArrayType = null;
			if (method is not {
				IsStatic: true, Name: var name,
				DeclaringType: { FullName: "<PrivateImplementationDetails>", TypeParameterCount: 0 },
				TypeArguments: [var bufferType, _],
				Parameters: var parameters
			})
			{
				return false;
			}

			if (methodName != name)
				return false;

			if (methodName.Contains("FirstElement"))
			{
				if (parameters is not [{ Type: ByReferenceType { ElementType: var type } }])
					return false;
				if (!type.Equals(bufferType))
					return false;
			}
			else
			{
				if (parameters is not [{ Type: ByReferenceType { ElementType: var type } }, { Type: var lengthOrIndexParameterType }])
					return false;
				if (!type.Equals(bufferType) || !lengthOrIndexParameterType.IsKnownType(KnownTypeCode.Int32))
					return false;
			}

			inlineArrayType = bufferType;
			return true;
		}
	}
}
