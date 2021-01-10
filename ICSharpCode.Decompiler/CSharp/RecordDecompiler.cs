// Copyright (c) 2020 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.CSharp
{
	class RecordDecompiler
	{
		readonly IDecompilerTypeSystem typeSystem;
		readonly ITypeDefinition recordTypeDef;
		readonly DecompilerSettings settings;
		readonly CancellationToken cancellationToken;
		readonly List<IMember> orderedMembers;
		readonly bool isInheritedRecord;
		readonly IMethod primaryCtor;
		readonly IType baseClass;
		readonly Dictionary<IField, IProperty> backingFieldToAutoProperty = new Dictionary<IField, IProperty>();
		readonly Dictionary<IProperty, IField> autoPropertyToBackingField = new Dictionary<IProperty, IField>();
		readonly Dictionary<IParameter, IProperty> primaryCtorParameterToAutoProperty = new Dictionary<IParameter, IProperty>();
		readonly Dictionary<IProperty, IParameter> autoPropertyToPrimaryCtorParameter = new Dictionary<IProperty, IParameter>();

		public RecordDecompiler(IDecompilerTypeSystem dts, ITypeDefinition recordTypeDef, DecompilerSettings settings, CancellationToken cancellationToken)
		{
			this.typeSystem = dts;
			this.recordTypeDef = recordTypeDef;
			this.settings = settings;
			this.cancellationToken = cancellationToken;
			this.baseClass = recordTypeDef.DirectBaseTypes.FirstOrDefault(b => b.Kind == TypeKind.Class);
			this.isInheritedRecord = !baseClass.IsKnownType(KnownTypeCode.Object);
			DetectAutomaticProperties();
			this.orderedMembers = DetectMemberOrder(recordTypeDef, backingFieldToAutoProperty);
			this.primaryCtor = DetectPrimaryConstructor();
		}

		void DetectAutomaticProperties()
		{
			var subst = recordTypeDef.AsParameterizedType().GetSubstitution();
			foreach (var property in recordTypeDef.Properties)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var p = (IProperty)property.Specialize(subst);
				if (IsAutoProperty(p, out var field))
				{
					backingFieldToAutoProperty.Add(field, p);
					autoPropertyToBackingField.Add(p, field);
				}
			}

			bool IsAutoProperty(IProperty p, out IField field)
			{
				field = null;
				if (p.Parameters.Count != 0)
					return false;
				if (p.Getter != null)
				{
					if (!IsAutoGetter(p.Getter, out field))
						return false;
				}
				if (p.Setter != null)
				{
					if (!IsAutoSetter(p.Setter, out var field2))
						return false;
					if (field != null)
					{
						if (!field.Equals(field2))
							return false;
					}
					else
					{
						field = field2;
					}
				}
				if (field == null)
					return false;
				if (!IsRecordType(field.DeclaringType))
					return false;
				return field.Name == $"<{p.Name}>k__BackingField";
			}

			bool IsAutoGetter(IMethod method, out IField field)
			{
				field = null;
				var body = DecompileBody(method);
				if (body == null)
					return false;
				// return this.field;
				if (!body.Instructions[0].MatchReturn(out var retVal))
					return false;
				if (method.IsStatic)
				{
					return retVal.MatchLdsFld(out field);
				}
				else
				{
					if (!retVal.MatchLdFld(out var target, out field))
						return false;
					return target.MatchLdThis();
				}
			}

			bool IsAutoSetter(IMethod method, out IField field)
			{
				field = null;
				Debug.Assert(!method.IsStatic);
				var body = DecompileBody(method);
				if (body == null)
					return false;
				// this.field = value;
				ILInstruction valueInst;
				if (method.IsStatic)
				{
					if (!body.Instructions[0].MatchStsFld(out field, out valueInst))
						return false;
				}
				else
				{
					if (!body.Instructions[0].MatchStFld(out var target, out field, out valueInst))
						return false;
					if (!target.MatchLdThis())
						return false;
				}
				if (!valueInst.MatchLdLoc(out var value))
					return false;
				if (!(value.Kind == VariableKind.Parameter && value.Index == 0))
					return false;
				return body.Instructions[1].MatchReturn(out var retVal) && retVal.MatchNop();
			}
		}

		IMethod DetectPrimaryConstructor()
		{
			if (!settings.UsePrimaryConstructorSyntax)
				return null;

			var subst = recordTypeDef.AsParameterizedType().GetSubstitution();
			foreach (var method in recordTypeDef.Methods)
			{
				cancellationToken.ThrowIfCancellationRequested();
				if (method.IsStatic || !method.IsConstructor)
					continue;
				var m = method.Specialize(subst);
				if (IsPrimaryConstructor(m))
					return method;
			}

			return null;

			bool IsPrimaryConstructor(IMethod method)
			{
				Debug.Assert(method.IsConstructor);
				var body = DecompileBody(method);
				if (body == null)
					return false;

				if (method.Parameters.Count == 0)
					return false;

				if (body.Instructions.Count != method.Parameters.Count + 2)
					return false;

				for (int i = 0; i < body.Instructions.Count - 2; i++)
				{
					if (!body.Instructions[i].MatchStFld(out var target, out var field, out var valueInst))
						return false;
					if (!target.MatchLdThis())
						return false;
					if (!valueInst.MatchLdLoc(out var value))
						return false;
					if (!(value.Kind == VariableKind.Parameter && value.Index == i))
						return false;
					if (!backingFieldToAutoProperty.TryGetValue(field, out var property))
						return false;
					primaryCtorParameterToAutoProperty.Add(method.Parameters[i], property);
					autoPropertyToPrimaryCtorParameter.Add(property, method.Parameters[i]);
				}

				var baseCtorCall = body.Instructions.SecondToLastOrDefault() as CallInstruction;
				if (baseCtorCall == null)
					return false;

				var returnInst = body.Instructions.LastOrDefault();
				return returnInst != null && returnInst.MatchReturn(out var retVal) && retVal.MatchNop();
			}
		}

		static List<IMember> DetectMemberOrder(ITypeDefinition recordTypeDef, Dictionary<IField, IProperty> backingFieldToAutoProperty)
		{
			// For records, the order of members is important:
			// Equals/GetHashCode/PrintMembers must agree on an order of fields+properties.
			// The IL metadata has the order of fields and the order of properties, but we
			// need to detect the correct interleaving.
			// We could try to detect this from the PrintMembers body, but let's initially
			// restrict ourselves to the common case where the record only uses properties.
			var subst = recordTypeDef.AsParameterizedType().GetSubstitution();
			return recordTypeDef.Properties.Select(p => p.Specialize(subst)).Concat(
				recordTypeDef.Fields.Select(f => (IField)f.Specialize(subst)).Where(f => !backingFieldToAutoProperty.ContainsKey(f))
			).ToList();
		}

		/// <summary>
		/// Gets the fields and properties of the record type, interleaved as necessary to
		/// maintain Equals/ToString/etc. semantics.
		/// </summary>
		public IEnumerable<IMember> FieldsAndProperties => orderedMembers;

		/// <summary>
		/// Gets the detected primary constructor. Returns null, if there was no primary constructor detected.
		/// </summary>
		public IMethod PrimaryConstructor => primaryCtor;

		bool IsRecordType(IType type)
		{
			return type.GetDefinition() == recordTypeDef
				&& type.TypeArguments.SequenceEqual(recordTypeDef.TypeParameters);
		}

		/// <summary>
		/// Gets whether the member of the record type will be automatically generated by the compiler.
		/// </summary>
		public bool MethodIsGenerated(IMethod method)
		{
			if (method.IsConstructor)
			{
				if (method.Parameters.Count == 1
					&& IsRecordType(method.Parameters[0].Type))
				{
					return IsGeneratedCopyConstructor(method);
				}
			}

			switch (method.Name)
			{
				// Some members in records are always compiler-generated and lead to a
				// "duplicate definition" error if we emit the generated code.
				case "op_Equality":
				case "op_Inequality":
				{
					// Don't emit comparison operators into C# record definition
					// Note: user can declare additional operator== as long as they have
					// different parameter types.
					return method.Parameters.Count == 2
						&& method.Parameters.All(p => IsRecordType(p.Type));
				}
				case "Equals" when method.Parameters.Count == 1:
				{
					IType paramType = method.Parameters[0].Type;
					if (paramType.IsKnownType(KnownTypeCode.Object) && method.IsOverride)
					{
						// override bool Equals(object? obj): always generated
						return true;
					}
					else if (IsRecordType(paramType))
					{
						// virtual bool Equals(R? other): generated unless user-declared
						return IsGeneratedEquals(method);
					}
					else if (isInheritedRecord && NormalizeTypeVisitor.TypeErasure.EquivalentTypes(paramType, baseClass) && method.IsOverride)
					{
						// override bool Equals(BaseClass? obj): always generated
						return true;
					}
					else
					{
						return false;
					}
				}
				case "GetHashCode":
					return IsGeneratedGetHashCode(method);
				case "<Clone>$" when method.Parameters.Count == 0:
					// Always generated; Method name cannot be expressed in C#
					return true;
				case "PrintMembers":
					return IsGeneratedPrintMembers(method);
				case "ToString" when method.Parameters.Count == 0:
					return IsGeneratedToString(method);
				case "Deconstruct" when primaryCtor != null && method.Parameters.Count == primaryCtor.Parameters.Count:
					return IsGeneratedDeconstruct(method);
				default:
					return false;
			}
		}

		internal bool PropertyIsGenerated(IProperty property)
		{
			switch (property.Name)
			{
				case "EqualityContract":
					return IsGeneratedEqualityContract(property);
				default:
					return IsPropertyDeclaredByPrimaryConstructor(property);
			}
		}

		public bool IsPropertyDeclaredByPrimaryConstructor(IProperty property)
		{
			var subst = recordTypeDef.AsParameterizedType().GetSubstitution();
			return primaryCtor != null
				&& autoPropertyToPrimaryCtorParameter.ContainsKey((IProperty)property.Specialize(subst));
		}

		private bool IsGeneratedCopyConstructor(IMethod method)
		{
			/* 
				call BaseClass..ctor(ldloc this, ldloc original)
				stfld <X>k__BackingField(ldloc this, ldfld <X>k__BackingField(ldloc original))
				leave IL_0000 (nop)
			 */
			Debug.Assert(method.IsConstructor && method.Parameters.Count == 1);
			if (method.GetAttributes().Any() || method.GetReturnTypeAttributes().Any())
				return false;
			if (method.Accessibility != Accessibility.Protected)
				return false;
			if (orderedMembers == null)
				return false;
			var body = DecompileBody(method);
			if (body == null)
				return false;
			var variables = body.Ancestors.OfType<ILFunction>().Single().Variables;
			var other = variables.Single(v => v.Kind == VariableKind.Parameter && v.Index == 0);
			Debug.Assert(IsRecordType(other.Type));
			int pos = 0;
			// First instruction is the base constructor call
			if (!(body.Instructions[pos] is Call { Method: { IsConstructor: true } } baseCtorCall))
				return false;
			if (!object.Equals(baseCtorCall.Method.DeclaringType, baseClass))
				return false;
			if (baseCtorCall.Arguments.Count != (isInheritedRecord ? 2 : 1))
				return false;
			if (!baseCtorCall.Arguments[0].MatchLdThis())
				return false;
			if (isInheritedRecord)
			{
				if (!baseCtorCall.Arguments[1].MatchLdLoc(other))
					return false;
			}
			pos++;
			// Then all the fields are copied over
			foreach (var member in orderedMembers)
			{
				if (!(member is IField field))
				{
					if (!autoPropertyToBackingField.TryGetValue((IProperty)member, out field))
						continue;
				}
				if (pos >= body.Instructions.Count)
					return false;
				if (!body.Instructions[pos].MatchStFld(out var lhsTarget, out var lhsField, out var valueInst))
					return false;
				if (!lhsTarget.MatchLdThis())
					return false;
				if (!lhsField.Equals(field))
					return false;

				if (!valueInst.MatchLdFld(out var rhsTarget, out var rhsField))
					return false;
				if (!rhsTarget.MatchLdLoc(other))
					return false;
				if (!rhsField.Equals(field))
					return false;
				pos++;
			}
			return body.Instructions[pos] is Leave;
		}

		private bool IsGeneratedEqualityContract(IProperty property)
		{
			// Generated member:
			// protected virtual Type EqualityContract {
			//    [CompilerGenerated] get => typeof(R);
			// }
			Debug.Assert(property.Name == "EqualityContract");
			if (property.Accessibility != Accessibility.Protected)
				return false;
			if (!(property.IsVirtual || property.IsOverride))
				return false;
			if (property.IsSealed)
				return false;
			var getter = property.Getter;
			if (!(getter != null && !property.CanSet))
				return false;
			if (property.GetAttributes().Any())
				return false;
			if (getter.GetReturnTypeAttributes().Any())
				return false;
			var attrs = getter.GetAttributes().ToList();
			if (attrs.Count != 1)
				return false;
			if (!attrs[0].AttributeType.IsKnownType(KnownAttribute.CompilerGenerated))
				return false;
			var body = DecompileBody(getter);
			if (body == null || body.Instructions.Count != 1)
				return false;
			if (!(body.Instructions.Single() is Leave leave))
				return false;
			// leave IL_0000 (call GetTypeFromHandle(ldtypetoken R))
			if (!TransformExpressionTrees.MatchGetTypeFromHandle(leave.Value, out IType ty))
				return false;
			return IsRecordType(ty);
		}

		private bool IsGeneratedPrintMembers(IMethod method)
		{
			Debug.Assert(method.Name == "PrintMembers");
			if (method.Parameters.Count != 1)
				return false;
			if (!method.IsOverridable)
				return false;
			if (method.GetAttributes().Any() || method.GetReturnTypeAttributes().Any())
				return false;
			if (method.Accessibility != Accessibility.Protected)
				return false;
			if (orderedMembers == null)
				return false;
			var body = DecompileBody(method);
			if (body == null)
				return false;
			var variables = body.Ancestors.OfType<ILFunction>().Single().Variables;
			var builder = variables.Single(v => v.Kind == VariableKind.Parameter && v.Index == 0);
			if (builder.Type.ReflectionName != "System.Text.StringBuilder")
				return false;
			int pos = 0;
			if (isInheritedRecord)
			{
				// Special case: inherited record adding no new members
				if (body.Instructions[pos].MatchReturn(out var returnValue)
					&& IsBaseCall(returnValue) && !orderedMembers.Any(IsPrintedMember))
				{
					return true;
				}
				// if (call PrintMembers(ldloc this, ldloc builder)) Block IL_000f {
				//   callvirt Append(ldloc builder, ldstr ", ")
				// }
				if (!body.Instructions[pos].MatchIfInstruction(out var condition, out var trueInst))
					return false;
				if (!IsBaseCall(condition))
					return false;
				// trueInst = callvirt Append(ldloc builder, ldstr ", ")
				trueInst = Block.Unwrap(trueInst);
				if (!MatchStringBuilderAppend(trueInst, builder, out var val))
					return false;
				if (!(val.MatchLdStr(out string text) && text == ", "))
					return false;
				pos++;

				bool IsBaseCall(ILInstruction inst)
				{
					if (!(inst is CallInstruction { Method: { Name: "PrintMembers" } } call))
						return false;
					if (call.Arguments.Count != 2)
						return false;
					if (!call.Arguments[0].MatchLdThis())
						return false;
					if (!call.Arguments[1].MatchLdLoc(builder))
						return false;
					return true;
				}
			}
			bool needsComma = false;
			foreach (var member in orderedMembers)
			{
				if (!IsPrintedMember(member))
					continue;
				cancellationToken.ThrowIfCancellationRequested();
				/* 
				callvirt Append(ldloc builder, ldstr "A")
				callvirt Append(ldloc builder, ldstr " = ")
				callvirt Append(ldloc builder, constrained[System.Int32].callvirt ToString(addressof System.Int32(call get_A(ldloc this))))
				callvirt Append(ldloc builder, ldstr ", ")
				callvirt Append(ldloc builder, ldstr "B")
				callvirt Append(ldloc builder, ldstr " = ")
				callvirt Append(ldloc builder, constrained[System.Int32].callvirt ToString(ldflda B(ldloc this)))
				leave IL_0000 (ldc.i4 1) */
				if (!MatchStringBuilderAppendConstant(out string text))
					return false;
				string expectedText = (needsComma ? ", " : "") + member.Name + " = ";
				if (text != expectedText)
					return false;
				if (!MatchStringBuilderAppend(body.Instructions[pos], builder, out var val))
					return false;
				if (val is CallInstruction { Method: { Name: "ToString", IsStatic: false } } toStringCall)
				{
					if (toStringCall.Arguments.Count != 1)
						return false;
					val = toStringCall.Arguments[0];
					if (val is AddressOf addressOf)
					{
						val = addressOf.Value;
					}
				}
				else if (val is Box box)
				{
					if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(box.Type, member.ReturnType))
						return false;
					val = box.Argument;
				}
				if (val is CallInstruction getterCall && member is IProperty property)
				{
					if (!getterCall.Method.Equals(property.Getter))
						return false;
					if (getterCall.Arguments.Count != 1)
						return false;
					if (!getterCall.Arguments[0].MatchLdThis())
						return false;
				}
				else if (val.MatchLdFld(out var target, out var field) || val.MatchLdFlda(out target, out field))
				{
					if (!target.MatchLdThis())
						return false;
					if (!field.Equals(member))
						return false;
				}
				else
				{
					return false;
				}
				pos++;
				needsComma = true;
			}
			// leave IL_0000 (ldc.i4 1)
			return body.Instructions[pos].MatchReturn(out var retVal)
				&& retVal.MatchLdcI4(needsComma ? 1 : 0);

			bool IsPrintedMember(IMember member)
			{
				if (member.IsStatic)
				{
					return false; // static fields/properties are not printed
				}
				if (member.Name == "EqualityContract")
				{
					return false; // EqualityContract is never printed
				}
				if (member.IsExplicitInterfaceImplementation)
				{
					return false; // explicit interface impls are not printed
				}
				if (member.IsOverride)
				{
					return false; // override is not printed (again), the virtual base property was already printed
				}
				return true;
			}

			bool MatchStringBuilderAppendConstant(out string text)
			{
				text = null;
				while (MatchStringBuilderAppend(body.Instructions[pos], builder, out var val) && val.MatchLdStr(out string valText))
				{
					text += valText;
					pos++;
				}
				return text != null;
			}
		}

		private bool MatchStringBuilderAppend(ILInstruction inst, ILVariable sb, out ILInstruction val)
		{
			val = null;
			if (!(inst is CallVirt { Method: { Name: "Append", DeclaringType: { Namespace: "System.Text", Name: "StringBuilder" } } } call))
				return false;
			if (call.Arguments.Count != 2)
				return false;
			if (!call.Arguments[0].MatchLdLoc(sb))
				return false;
			val = call.Arguments[1];
			return true;
		}

		private bool IsGeneratedToString(IMethod method)
		{
			Debug.Assert(method.Name == "ToString" && method.Parameters.Count == 0);
			if (!method.IsOverride)
				return false;
			if (method.IsSealed)
				return false;
			if (method.GetAttributes().Any() || method.GetReturnTypeAttributes().Any())
				return false;
			var body = DecompileBody(method);
			if (body == null)
				return false;
			// stloc stringBuilder(newobj StringBuilder..ctor())
			if (!body.Instructions[0].MatchStLoc(out var stringBuilder, out var stringBuilderInit))
				return false;
			if (!(stringBuilderInit is NewObj { Arguments: { Count: 0 }, Method: { DeclaringTypeDefinition: { Name: "StringBuilder", Namespace: "System.Text" } } }))
				return false;
			// callvirt Append(ldloc stringBuilder, ldstr "R")
			if (!MatchAppendCallWithValue(body.Instructions[1], recordTypeDef.Name))
				return false;
			// callvirt Append(ldloc stringBuilder, ldstr " { ")
			if (!MatchAppendCallWithValue(body.Instructions[2], " { "))
				return false;
			// if (callvirt PrintMembers(ldloc this, ldloc stringBuilder)) { trueInst }
			if (!body.Instructions[3].MatchIfInstruction(out var condition, out var trueInst))
				return true;
			if (!(condition is CallVirt { Method: { Name: "PrintMembers" } } printMembersCall))
				return false;
			if (printMembersCall.Arguments.Count != 2)
				return false;
			if (!printMembersCall.Arguments[0].MatchLdThis())
				return false;
			if (!printMembersCall.Arguments[1].MatchLdLoc(stringBuilder))
				return false;
			// trueInst: callvirt Append(ldloc stringBuilder, ldstr " ")
			if (!MatchAppendCallWithValue(Block.Unwrap(trueInst), " "))
				return false;
			// callvirt Append(ldloc stringBuilder, ldstr "}")
			if (!MatchAppendCallWithValue(body.Instructions[4], "}"))
				return false;
			// leave IL_0000 (callvirt ToString(ldloc stringBuilder))
			if (!(body.Instructions[5] is Leave leave))
				return false;
			if (!(leave.Value is CallVirt { Method: { Name: "ToString" } } toStringCall))
				return false;
			if (toStringCall.Arguments.Count != 1)
				return false;
			return toStringCall.Arguments[0].MatchLdLoc(stringBuilder);

			bool MatchAppendCall(ILInstruction inst, out string val)
			{
				val = null;
				if (!(inst is CallVirt { Method: { Name: "Append" } } call))
					return false;
				if (call.Arguments.Count != 2)
					return false;
				if (!call.Arguments[0].MatchLdLoc(stringBuilder))
					return false;
				return call.Arguments[1].MatchLdStr(out val);
			}

			bool MatchAppendCallWithValue(ILInstruction inst, string val)
			{
				return MatchAppendCall(inst, out string tmp) && tmp == val;
			}
		}

		private bool IsGeneratedEquals(IMethod method)
		{
			// virtual bool Equals(R? other) {
			//    return other != null && EqualityContract == other.EqualityContract && EqualityComparer<int>.Default.Equals(A, other.A) && ...;
			// }
			Debug.Assert(method.Name == "Equals" && method.Parameters.Count == 1);
			if (method.Parameters.Count != 1)
				return false;
			if (!method.IsOverridable)
				return false;
			if (method.GetAttributes().Any() || method.GetReturnTypeAttributes().Any())
				return false;
			if (orderedMembers == null)
				return false;
			var body = DecompileBody(method);
			if (body == null)
				return false;
			if (!body.Instructions[0].MatchReturn(out var returnValue))
				return false;
			var variables = body.Ancestors.OfType<ILFunction>().Single().Variables;
			var other = variables.Single(v => v.Kind == VariableKind.Parameter && v.Index == 0);
			Debug.Assert(IsRecordType(other.Type));
			var conditions = UnpackLogicAndChain(returnValue);
			Debug.Assert(conditions.Count >= 1);
			int pos = 0;
			if (isInheritedRecord)
			{
				// call BaseClass::Equals(ldloc this, ldloc other)
				if (pos >= conditions.Count)
					return false;
				if (!(conditions[pos] is Call { Method: { Name: "Equals" } } call))
					return false;
				if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(call.Method.DeclaringType, baseClass))
					return false;
				if (call.Arguments.Count != 2)
					return false;
				if (!call.Arguments[0].MatchLdThis())
					return false;
				if (!call.Arguments[1].MatchLdLoc(other))
					return false;
				pos++;
			}
			else
			{
				// comp.o(ldloc other != ldnull)
				if (pos >= conditions.Count)
					return false;
				if (!conditions[pos].MatchCompNotEqualsNull(out var arg))
					return false;
				if (!arg.MatchLdLoc(other))
					return false;
				pos++;
				// call op_Equality(callvirt get_EqualityContract(ldloc this), callvirt get_EqualityContract(ldloc other))
				// Special-cased because Roslyn isn't using EqualityComparer<T> here.
				if (pos >= conditions.Count)
					return false;
				if (!(conditions[pos] is Call { Method: { IsOperator: true, Name: "op_Equality" } } opEqualityCall))
					return false;
				if (!opEqualityCall.Method.DeclaringType.IsKnownType(KnownTypeCode.Type))
					return false;
				if (opEqualityCall.Arguments.Count != 2)
					return false;
				if (!MatchGetEqualityContract(opEqualityCall.Arguments[0], out var target1))
					return false;
				if (!MatchGetEqualityContract(opEqualityCall.Arguments[1], out var target2))
					return false;
				if (!target1.MatchLdThis())
					return false;
				if (!target2.MatchLdLoc(other))
					return false;
				pos++;
			}
			foreach (var member in orderedMembers)
			{
				if (!MemberConsideredForEquality(member))
					continue;
				if (member.Name == "EqualityContract")
				{
					continue; // already special-cased
				}
				// EqualityComparer<int>.Default.Equals(A, other.A)
				// callvirt Equals(call get_Default(), ldfld <A>k__BackingField(ldloc this), ldfld <A>k__BackingField(ldloc other))
				if (pos >= conditions.Count)
					return false;
				if (!(conditions[pos] is CallVirt { Method: { Name: "Equals" } } equalsCall))
					return false;
				if (equalsCall.Arguments.Count != 3)
					return false;
				if (!IsEqualityComparerGetDefaultCall(equalsCall.Arguments[0], member.ReturnType))
					return false;
				if (!MatchMemberAccess(equalsCall.Arguments[1], out var target1, out var member1))
					return false;
				if (!MatchMemberAccess(equalsCall.Arguments[2], out var target2, out var member2))
					return false;
				if (!target1.MatchLdThis())
					return false;
				if (!member1.Equals(member))
					return false;
				if (!target2.MatchLdLoc(other))
					return false;
				if (!member2.Equals(member))
					return false;
				pos++;
			}
			return pos == conditions.Count;
		}

		static List<ILInstruction> UnpackLogicAndChain(ILInstruction rootOfChain)
		{
			var result = new List<ILInstruction>();
			Visit(rootOfChain);
			return result;

			void Visit(ILInstruction inst)
			{
				if (inst.MatchLogicAnd(out var lhs, out var rhs))
				{
					Visit(lhs);
					Visit(rhs);
				}
				else
				{
					result.Add(inst);
				}
			}
		}

		private static bool MatchGetEqualityContract(ILInstruction inst, out ILInstruction target)
		{
			target = null;
			if (!(inst is CallVirt { Method: { Name: "get_EqualityContract" } } call))
				return false;
			if (call.Arguments.Count != 1)
				return false;
			target = call.Arguments[0];
			return true;
		}

		private static bool IsEqualityComparerGetDefaultCall(ILInstruction inst, IType type)
		{
			if (!(inst is Call { Method: { Name: "get_Default", IsStatic: true } } call))
				return false;
			if (!(call.Method.DeclaringType is { Name: "EqualityComparer", Namespace: "System.Collections.Generic" }))
				return false;
			if (call.Method.DeclaringType.TypeArguments.Count != 1)
				return false;
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(call.Method.DeclaringType.TypeArguments[0], type))
				return false;
			return call.Arguments.Count == 0;
		}

		bool MemberConsideredForEquality(IMember member)
		{
			if (member.IsStatic)
				return false;
			if (member is IProperty property)
			{
				if (property.Name == "EqualityContract")
					return !isInheritedRecord;
				return autoPropertyToBackingField.ContainsKey(property);
			}
			else
			{
				return member is IField;
			}
		}

		bool IsGeneratedGetHashCode(IMethod method)
		{
			/*
			 return (
				(
					EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(A)
				) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(B)
			 ) * -1521134295 + EqualityComparer<object>.Default.GetHashCode(C);
			*/
			Debug.Assert(method.Name == "GetHashCode");
			if (method.Parameters.Count != 0)
				return false;
			if (!method.IsOverride || method.IsSealed)
				return false;
			if (method.GetAttributes().Any() || method.GetReturnTypeAttributes().Any())
				return false;
			if (orderedMembers == null)
				return false;
			var body = DecompileBody(method);
			if (body == null)
				return false;
			if (!body.Instructions[0].MatchReturn(out var returnValue))
				return false;
			var hashedMembers = new List<IMember>();
			bool foundBaseClassHash = false;
			if (!Visit(returnValue))
				return false;
			if (foundBaseClassHash != isInheritedRecord)
				return false;
			return orderedMembers.Where(MemberConsideredForEquality).SequenceEqual(hashedMembers);

			bool Visit(ILInstruction inst)
			{
				if (inst is BinaryNumericInstruction
				{
					Operator: BinaryNumericOperator.Add,
					CheckForOverflow: false,
					Left: BinaryNumericInstruction
					{
						Operator: BinaryNumericOperator.Mul,
						CheckForOverflow: false,
						Left: var left,
						Right: LdcI4 { Value: -1521134295 }
					},
					Right: var right
				})
				{
					if (!Visit(left))
						return false;
					return ProcessIndividualHashCode(right);
				}
				else
				{
					return ProcessIndividualHashCode(inst);
				}
			}

			bool ProcessIndividualHashCode(ILInstruction inst)
			{
				// base.GetHashCode(): call GetHashCode(ldloc this)
				if (inst is Call { Method: { Name: "GetHashCode" } } baseHashCodeCall)
				{
					if (baseHashCodeCall.Arguments.Count != 1)
						return false;
					if (!baseHashCodeCall.Arguments[0].MatchLdThis())
						return false;
					if (foundBaseClassHash || hashedMembers.Count > 0)
						return false; // must be first
					foundBaseClassHash = true;
					return baseHashCodeCall.Method.DeclaringType.Equals(baseClass);
				}
				// callvirt GetHashCode(call get_Default(), callvirt get_EqualityContract(ldloc this))
				// callvirt GetHashCode(call get_Default(), ldfld <A>k__BackingField(ldloc this)))
				if (!(inst is CallVirt { Method: { Name: "GetHashCode" } } getHashCodeCall))
					return false;
				if (getHashCodeCall.Arguments.Count != 2)
					return false;
				// getHashCodeCall.Arguments[0] checked later
				if (!MatchMemberAccess(getHashCodeCall.Arguments[1], out var target, out var member))
					return false;
				if (!target.MatchLdThis())
					return false;
				if (!IsEqualityComparerGetDefaultCall(getHashCodeCall.Arguments[0], member.ReturnType))
					return false;
				hashedMembers.Add(member);
				return true;
			}
		}

		bool IsGeneratedDeconstruct(IMethod method)
		{
			Debug.Assert(method.Name == "Deconstruct" && method.Parameters.Count == primaryCtor.Parameters.Count);

			if (!method.ReturnType.IsKnownType(KnownTypeCode.Void))
				return false;

			for (int i = 0; i < method.Parameters.Count; i++)
			{
				var deconstruct = method.Parameters[i];
				var ctor = primaryCtor.Parameters[i];

				if (!deconstruct.IsOut)
					return false;

				if (!ctor.Type.Equals(((ByReferenceType)deconstruct.Type).ElementType))
					return false;

				if (ctor.Name != deconstruct.Name)
					return false;
			}

			var body = DecompileBody(method);
			if (body == null || body.Instructions.Count != method.Parameters.Count + 1)
				return false;

			for (int i = 0; i < body.Instructions.Count - 1; i++)
			{
				// stobj T(ldloc parameter, call getter(ldloc this))
				if (!body.Instructions[i].MatchStObj(out var targetInst, out var getter, out _))
					return false;
				if (!targetInst.MatchLdLoc(out var target))
					return false;
				if (!(target.Kind == VariableKind.Parameter && target.Index == i))
					return false;

				if (getter is not Call call || call.Arguments.Count != 1)
					return false;
				if (!call.Arguments[0].MatchLdThis())
					return false;

				if (!call.Method.IsAccessor)
					return false;
				var autoProperty = (IProperty)call.Method.AccessorOwner;
				if (!autoPropertyToBackingField.ContainsKey(autoProperty))
					return false;
			}

			var returnInst = body.Instructions.LastOrDefault();
			return returnInst != null && returnInst.MatchReturn(out var retVal) && retVal.MatchNop();
		}

		bool MatchMemberAccess(ILInstruction inst, out ILInstruction target, out IMember member)
		{
			target = null;
			member = null;
			if (inst is CallVirt
			{
				Method:
				{
					AccessorKind: System.Reflection.MethodSemanticsAttributes.Getter,
					AccessorOwner: IProperty property
				}
			} call)
			{
				if (call.Arguments.Count != 1)
					return false;
				target = call.Arguments[0];
				member = property;
				return true;
			}
			else if (inst.MatchLdFld(out target, out IField field))
			{
				if (backingFieldToAutoProperty.TryGetValue(field, out property))
					member = property;
				else
					member = field;
				return true;
			}
			else
			{
				return false;
			}
		}

		Block DecompileBody(IMethod method)
		{
			if (method == null || method.MetadataToken.IsNil)
				return null;
			var metadata = typeSystem.MainModule.metadata;

			var methodDefHandle = (MethodDefinitionHandle)method.MetadataToken;
			var methodDef = metadata.GetMethodDefinition(methodDefHandle);
			if (!methodDef.HasBody())
				return null;

			var genericContext = new GenericContext(
				classTypeParameters: recordTypeDef.TypeParameters,
				methodTypeParameters: null);
			var body = typeSystem.MainModule.PEFile.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
			var ilReader = new ILReader(typeSystem.MainModule);
			var il = ilReader.ReadIL(methodDefHandle, body, genericContext, ILFunctionKind.TopLevelFunction, cancellationToken);
			var settings = new DecompilerSettings(LanguageVersion.CSharp1);
			var transforms = CSharpDecompiler.GetILTransforms();
			// Remove the last couple transforms -- we don't need variable names etc. here
			int lastBlockTransform = transforms.FindLastIndex(t => t is BlockILTransform);
			transforms.RemoveRange(lastBlockTransform + 1, transforms.Count - (lastBlockTransform + 1));
			// Use CombineExitsTransform so that "return other != null && ...;" is a single statement even in release builds
			transforms.Add(new CombineExitsTransform());
			il.RunTransforms(transforms,
				new ILTransformContext(il, typeSystem, debugInfo: null, settings) {
					CancellationToken = cancellationToken
				});
			if (il.Body is BlockContainer container)
			{
				return container.EntryPoint;
			}
			else if (il.Body is Block block)
			{
				return block;
			}
			else
			{
				return null;
			}
		}
	}
}
