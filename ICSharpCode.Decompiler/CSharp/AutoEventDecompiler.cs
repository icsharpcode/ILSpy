// Copyright (c) 2026 Siegfried Pammer
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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Recognizes automatic events by structurally matching the ILAst of the compiler-generated
	/// add/remove accessors. The accessor bodies are decompiled with a fixed set of decompiler
	/// settings (see <see cref="CSharpDecompiler.DecompileBodyForAnalysis"/>), so recognition is
	/// independent of the user-visible settings that shape the final C# output.
	/// </summary>
	static class AutoEventDecompiler
	{
		/// <summary>
		/// Determines whether <paramref name="ev"/> is an automatic event, memoizing the verdict
		/// in <paramref name="decompileRun"/> so that all consumers decide from the same analysis.
		/// </summary>
		public static bool IsAutomaticEvent(IDecompilerTypeSystem typeSystem, IEvent ev, DecompileRun decompileRun, CancellationToken cancellationToken, [NotNullWhen(true)] out IField? backingField)
		{
			if (!decompileRun.AutomaticEvents.TryGetValue(ev, out backingField))
			{
				backingField = IsAutomaticEvent(typeSystem, ev, cancellationToken, out var field) ? field : null;
				decompileRun.AutomaticEvents.Add(ev, backingField);
			}
			return backingField != null;
		}

		static bool IsAutomaticEvent(IDecompilerTypeSystem typeSystem, IEvent ev, CancellationToken cancellationToken, [NotNullWhen(true)] out IField? backingField)
		{
			backingField = null;
			if (ev.IsExplicitInterfaceImplementation || ev.DeclaringTypeDefinition == null)
				return false;
			if (ev.AddAccessor is not { HasBody: true } addAccessor || ev.RemoveAccessor is not { HasBody: true } removeAccessor)
				return false;
			backingField = FindBackingField(ev);
			if (backingField == null)
				return false;
			// ignore tuple element names, dynamic and nullability
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(ev.ReturnType, backingField.ReturnType))
				return false;
			return IsAutomaticAccessor(typeSystem, addAccessor, backingField, isAddAccessor: true, cancellationToken)
				&& IsAutomaticAccessor(typeSystem, removeAccessor, backingField, isAddAccessor: false, cancellationToken);
		}

		static readonly string[] attributeTypesToRemoveFromAutoEventAccessors = new[] {
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Diagnostics.DebuggerBrowsableAttribute",
			"System.Runtime.CompilerServices.MethodImplAttribute"
		};

		static readonly string[] attributeTypesToRemoveFromAutoEventFields = new[] {
			"System.Runtime.CompilerServices.CompilerGeneratedAttribute",
			"System.Diagnostics.DebuggerBrowsableAttribute"
		};

		/// <summary>
		/// Adds the add-accessor and backing-field attributes of an automatic event to its
		/// field-like declaration (as "method:" and "field:" sections), dropping the attributes
		/// the compiler puts on automatic events.
		/// </summary>
		internal static void AddFieldLikeEventAttributes(EventDeclaration eventDecl, TypeSystemAstBuilder astBuilder, IEvent ev, IField backingField)
		{
			eventDecl.Attributes.AddRange(astBuilder.ConvertAttributes(
				WithoutAttributeTypes(ev.AddAccessor!.GetAttributes(), attributeTypesToRemoveFromAutoEventAccessors), "method"));
			eventDecl.Attributes.AddRange(astBuilder.ConvertAttributes(
				WithoutAttributeTypes(backingField.GetAttributes(), attributeTypesToRemoveFromAutoEventFields), "field"));
		}

		static IEnumerable<IAttribute> WithoutAttributeTypes(IEnumerable<IAttribute> attributes, string[] attributeTypesToRemove)
		{
			return attributes.Where(a => !attributeTypesToRemove.Contains(a.AttributeType.FullName));
		}

		static IField? FindBackingField(IEvent ev)
		{
			if (ev.ParentModule is not MetadataModule module || ev.DeclaringTypeDefinition is not { } declaringType)
				return null;
			var lookup = module.MetadataFile.PropertyAndEventBackingFieldLookup;
			foreach (var field in declaringType.Fields)
			{
				if (field.MetadataToken.IsNil)
					continue;
				if (!lookup.IsEventBackingField((FieldDefinitionHandle)field.MetadataToken, out var eventHandle))
					continue;
				if (eventHandle != (EventDefinitionHandle)ev.MetadataToken)
					continue;
				return field.Accessibility == Accessibility.Private && field.IsStatic == ev.IsStatic ? field : null;
			}
			return null;
		}

		static bool IsAutomaticAccessor(IDecompilerTypeSystem typeSystem, IMethod accessor, IField field, bool isAddAccessor, CancellationToken cancellationToken)
		{
			var body = CSharpDecompiler.DecompileBodyForAnalysis(accessor, typeSystem, cancellationToken);
			if (body == null)
				return false;
			if (body.Parent is not BlockContainer functionBody || functionBody.Blocks.Count != 1)
				return false;
			return MatchCompareExchangeLoop(body, functionBody, accessor, field, isAddAccessor)
				|| MatchSimpleCombineAssignment(body, functionBody, accessor, field, isAddAccessor)
				|| MatchCompareExchangeLoopMcs(body, functionBody, accessor, field, isAddAccessor);
		}

		// Matches the thread-safe compare-exchange loop generated by csc 4 and Roslyn (C# and VB):
		//   T oldValue = this.field;
		//   while (true) {
		//       T comparand = oldValue;
		//       T newValue = (T)Delegate.Combine(comparand, value);
		//       oldValue = Interlocked.CompareExchange(ref this.field, newValue, comparand);
		//       if (oldValue == comparand) break;
		//   }
		static bool MatchCompareExchangeLoop(Block body, BlockContainer functionBody, IMethod accessor, IField field, bool isAddAccessor)
		{
			if (body.Instructions.Count != 3)
				return false;
			if (!body.Instructions[0].MatchStLoc(out var oldValue, out var init))
				return false;
			if (!MatchLoadOfField(init, accessor, field))
				return false;
			if (body.Instructions[1] is not BlockContainer { Kind: ContainerKind.Loop } loop || loop.Blocks.Count != 1)
				return false;
			if (!body.Instructions[2].MatchLeave(functionBody))
				return false;
			var loopBody = loop.EntryPoint;
			if (loopBody.Instructions.Count != 5)
				return false;
			if (!loopBody.Instructions[0].MatchStLoc(out var comparand, out var comparandInit) || comparand == oldValue)
				return false;
			if (!comparandInit.MatchLdLoc(oldValue))
				return false;
			if (!loopBody.Instructions[1].MatchStLoc(out var newValue, out var newValueInit) || newValue == oldValue || newValue == comparand)
				return false;
			if (!MatchCastedDelegateCombineCall(newValueInit, field, comparand, isAddAccessor))
				return false;
			if (!loopBody.Instructions[2].MatchStLoc(oldValue, out var compareExchangeCall))
				return false;
			if (!MatchCompareExchangeCall(compareExchangeCall, accessor, field, valueArg: newValue, comparandArg: comparand))
				return false;
			return MatchLoopExit(loopBody, loop, oldValue, comparand);
		}

		// Matches the compare-exchange loop generated by mcs, which passes the combined delegate
		// directly to Interlocked.CompareExchange and uses the previous exchange result as comparand:
		//   T oldValue = this.field;
		//   while (true) {
		//       T comparand = oldValue;
		//       oldValue = Interlocked.CompareExchange(ref this.field, (T)Delegate.Combine(comparand, value), oldValue);
		//       if (oldValue == comparand) break;
		//   }
		static bool MatchCompareExchangeLoopMcs(Block body, BlockContainer functionBody, IMethod accessor, IField field, bool isAddAccessor)
		{
			if (body.Instructions.Count != 3)
				return false;
			if (!body.Instructions[0].MatchStLoc(out var oldValue, out var init))
				return false;
			if (!MatchLoadOfField(init, accessor, field))
				return false;
			if (body.Instructions[1] is not BlockContainer { Kind: ContainerKind.Loop } loop || loop.Blocks.Count != 1)
				return false;
			if (!body.Instructions[2].MatchLeave(functionBody))
				return false;
			var loopBody = loop.EntryPoint;
			if (loopBody.Instructions.Count != 4)
				return false;
			if (!loopBody.Instructions[0].MatchStLoc(out var comparand, out var comparandInit) || comparand == oldValue)
				return false;
			if (!comparandInit.MatchLdLoc(oldValue))
				return false;
			if (!loopBody.Instructions[1].MatchStLoc(oldValue, out var compareExchangeCall))
				return false;
			if (compareExchangeCall is not Call call || !IsCompareExchangeMethod(call.Method) || call.Arguments.Count != 3)
				return false;
			if (!MatchAddressOfField(call.Arguments[0], accessor, field))
				return false;
			if (!MatchCastedDelegateCombineCall(call.Arguments[1], field, comparand, isAddAccessor))
				return false;
			if (!call.Arguments[2].MatchLdLoc(oldValue))
				return false;
			return MatchLoopExit(loopBody, loop, oldValue, comparand);
		}

		// Matches the non-thread-safe shape generated by csc before 4.0 (the accessors are
		// [MethodImpl(Synchronized)] instead):
		//   this.field = (T)Delegate.Combine(this.field, value);
		// mcs 2.x compiles the accessor as a compound assignment, evaluating 'this' once into
		// a temporary (IL 'dup'), so the store may be preceded by "stloc S(ldloc this)" with
		// both field accesses going through S.
		static bool MatchSimpleCombineAssignment(Block body, BlockContainer functionBody, IMethod accessor, IField field, bool isAddAccessor)
		{
			ILVariable? thisAlias = null;
			int pos = 0;
			if (!accessor.IsStatic && body.Instructions.Count == 3)
			{
				if (!body.Instructions[0].MatchStLoc(out thisAlias, out var aliasInit) || !aliasInit.MatchLdThis())
					return false;
				if (thisAlias.StoreCount != 1 || thisAlias.LoadCount != 2 || thisAlias.AddressCount != 0)
					return false;
				pos = 1;
			}
			else if (body.Instructions.Count != 2)
			{
				return false;
			}
			ILInstruction? value;
			if (accessor.IsStatic)
			{
				if (!body.Instructions[pos].MatchStsFld(out var f, out value) || !IsSameField(f, field))
					return false;
			}
			else
			{
				if (!body.Instructions[pos].MatchStFld(out var target, out var f, out value) || !MatchLdThisOrAlias(target, thisAlias) || !IsSameField(f, field))
					return false;
			}
			if (!body.Instructions[pos + 1].MatchLeave(functionBody))
				return false;
			if (value is not CastClass castclass)
				return false;
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(castclass.Type, field.ReturnType))
				return false;
			if (castclass.Argument is not Call call || !IsDelegateCombineMethod(call.Method, isAddAccessor) || call.Arguments.Count != 2)
				return false;
			return MatchLoadOfField(call.Arguments[0], accessor, field, thisAlias)
				&& MatchLdValueParameter(call.Arguments[1]);
		}

		static bool MatchLdThisOrAlias(ILInstruction inst, ILVariable? thisAlias)
		{
			return inst.MatchLdThis() || (thisAlias != null && inst.MatchLdLoc(thisAlias));
		}

		// Matches the loop exit at the end of the compare-exchange loop body:
		//   if (oldValue == comparand) break;
		// i.e. "if (comp.o(ldloc oldValue == ldloc comparand)) leave loop" followed by the back-branch.
		static bool MatchLoopExit(Block loopBody, BlockContainer loop, ILVariable oldValue, ILVariable comparand)
		{
			if (!loopBody.Instructions[loopBody.Instructions.Count - 2].MatchIfInstruction(out var condition, out var trueInst))
				return false;
			if (condition is not Comp { Kind: ComparisonKind.Equality } comp)
				return false;
			if (!comp.Left.MatchLdLoc(oldValue) || !comp.Right.MatchLdLoc(comparand))
				return false;
			if (!trueInst.MatchLeave(loop))
				return false;
			return loopBody.Instructions[loopBody.Instructions.Count - 1].MatchBranch(loopBody);
		}

		// Matches "(T)Delegate.Combine(comparand, value)" (Delegate.Remove for the remove accessor).
		static bool MatchCastedDelegateCombineCall(ILInstruction inst, IField field, ILVariable comparand, bool isAddAccessor)
		{
			if (inst is not CastClass castclass)
				return false;
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(castclass.Type, field.ReturnType))
				return false;
			if (castclass.Argument is not Call call || !IsDelegateCombineMethod(call.Method, isAddAccessor) || call.Arguments.Count != 2)
				return false;
			return call.Arguments[0].MatchLdLoc(comparand)
				&& MatchLdValueParameter(call.Arguments[1]);
		}

		// Matches "Interlocked.CompareExchange(ref this.field, valueArg, comparandArg)".
		static bool MatchCompareExchangeCall(ILInstruction inst, IMethod accessor, IField field, ILVariable valueArg, ILVariable comparandArg)
		{
			if (inst is not Call call || !IsCompareExchangeMethod(call.Method) || call.Arguments.Count != 3)
				return false;
			return MatchAddressOfField(call.Arguments[0], accessor, field)
				&& call.Arguments[1].MatchLdLoc(valueArg)
				&& call.Arguments[2].MatchLdLoc(comparandArg);
		}

		static bool IsDelegateCombineMethod(IMethod method, bool isAddAccessor)
		{
			return method.IsStatic
				&& method.Name == (isAddAccessor ? "Combine" : "Remove")
				&& method.DeclaringType.FullName == "System.Delegate";
		}

		static bool IsCompareExchangeMethod(IMethod method)
		{
			return method.IsStatic
				&& method.Name == "CompareExchange"
				&& method.DeclaringType.FullName == "System.Threading.Interlocked";
		}

		static bool MatchLoadOfField(ILInstruction inst, IMethod accessor, IField field, ILVariable? thisAlias = null)
		{
			if (accessor.IsStatic)
			{
				return inst.MatchLdsFld(out var f) && IsSameField(f, field);
			}
			else
			{
				return inst.MatchLdFld(out var target, out var f) && MatchLdThisOrAlias(target, thisAlias) && IsSameField(f, field);
			}
		}

		static bool MatchAddressOfField(ILInstruction inst, IMethod accessor, IField field)
		{
			if (accessor.IsStatic)
			{
				return inst.MatchLdsFlda(out var f) && IsSameField(f, field);
			}
			else
			{
				return inst.MatchLdFlda(out var target, out var f) && target.MatchLdThis() && IsSameField(f, field);
			}
		}

		static bool MatchLdValueParameter(ILInstruction inst)
		{
			return inst.MatchLdLoc(out var v) && v.Kind == VariableKind.Parameter && v.Index == 0;
		}

		static bool IsSameField(IField a, IField b)
		{
			return a.MemberDefinition.Equals(b.MemberDefinition);
		}
	}
}
