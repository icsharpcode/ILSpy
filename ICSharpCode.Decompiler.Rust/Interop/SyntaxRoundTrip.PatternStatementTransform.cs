// Copyright (c) 2026 Christoph Wille
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle queries (kinds 16 to 31) and annotations of the engine's port of
	/// <see cref="PatternStatementTransform"/> (<c>csharp/transforms/pattern_statement_transform.rs</c>).
	/// Symbols, IL variables, types and block containers travel as references: indices into a table
	/// of host objects, equal objects sharing one index.
	/// </summary>
	internal sealed partial class SyntaxRoundTrip
	{
		readonly List<object> pstRefs = new List<object>();
		readonly Dictionary<object, int> pstRefIds = new Dictionary<object, int>();
		/// <summary>The nodes of the tree in the order the engine numbers them (<c>SyntaxId</c>).</summary>
		readonly List<AstNode> pstWritten = new List<AstNode>();
		Dictionary<AstNode, int>? pstWrittenIds;
		DeclareVariables? pstDeclareVariables;

		/// <summary>Records a node of the tree as it is written, for the engine's node ids.</summary>
		void PstRecordWritten(AstNode node)
		{
			if (mintTarget == null)
				pstWritten.Add(node);
		}

		const byte PstQueryResolve = 16;
		const byte PstQuerySettings = 17;
		const byte PstQueryVariable = 18;
		const byte PstQueryForeachLocal = 19;
		const byte PstQueryDeclaration = 20;
		const byte PstQueryType = 21;
		const byte PstQueryConvertType = 22;
		const byte PstQueryMember = 23;
		const byte PstQueryProperty = 24;
		const byte PstQueryField = 25;
		const byte PstQueryPropertyField = 26;
		const byte PstQueryEventField = 27;
		const byte PstQueryLiteral = 28;
		const byte PstQueryLiteralEquals = 29;
		const byte PstQueryStoreSurvives = 30;

		const byte PstAnnotationForeachVariable = 16;
		const byte PstAnnotationPropertyReference = 17;

		int PstRef(object value)
		{
			if (!pstRefIds.TryGetValue(value, out int id))
			{
				id = pstRefs.Count;
				pstRefs.Add(value);
				pstRefIds.Add(value, id);
			}
			return id;
		}

		/// <summary>A reference plus one, 0 for null.</summary>
		ulong PstOptRef(object? value) => value == null ? 0 : (ulong)PstRef(value) + 1;

		/// <summary>The host object of a handle in the tree; null for one the engine created.</summary>
		object? PstTarget(uint handle) => handle < (uint)handles.Count ? handles[(int)handle] : null;

		byte[]? AnswerPatternStatementTransform(byte[] query)
		{
			int pos = 1;
			var s = new MemoryStream();
			switch (query[0])
			{
				case PstQueryResolve:
				{
					var target = PstTarget((uint)ReadUVar(query, ref pos));
					byte flags = 0;
					ILVariable? variable = null;
					ISymbol? symbol = null;
					IType? type = null;
					IMember? member = null;
					if (target is ResolveResult rr)
					{
						flags |= 1;
						type = rr.Type;
						symbol = rr is MethodGroupResolveResult mgrr ? mgrr.ChosenMethod : rr.GetSymbol();
						if (rr is ILVariableResolveResult vrr)
						{
							flags |= 2;
							variable = vrr.Variable;
						}
						if (rr is MemberResolveResult mrr)
						{
							flags |= 4;
							member = mrr.Member;
						}
						if (rr is TypeResolveResult)
							flags |= 8;
					}
					else if (target is BlockContainer)
					{
						flags |= 16;
					}
					s.WriteByte(flags);
					UVar(s, PstOptRef(variable));
					UVar(s, PstOptRef(symbol));
					UVar(s, PstOptRef(type));
					UVar(s, PstOptRef(member));
					UVar(s, PstOptRef(target as BlockContainer));
					break;
				}
				case PstQuerySettings:
				{
					if (context == null)
						return null;
					var settings = context.Settings;
					bool[] bits = {
						settings.ForStatement, settings.ForEachStatement, settings.InlineArrays, settings.AnonymousTypes,
						settings.AutomaticProperties, settings.FieldKeyword, settings.GetterOnlyAutomaticProperties,
						settings.AutomaticEvents, settings.PatternBasedFixedStatement, settings.UseEnhancedUsing,
					};
					ulong value = 0;
					for (int i = 0; i < bits.Length; i++)
					{
						if (bits[i])
							value |= 1ul << i;
					}
					UVar(s, value);
					UVar(s, PstOptRef(context.CurrentTypeDefinition));
					UVar(s, PstOptRef(context.CurrentMember as IMethod));
					break;
				}
				case PstQueryVariable:
				{
					var v = (ILVariable)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot));
					s.WriteByte(PstBool(DeclareVariables.VariableNeedsDeclaration(v.Kind)));
					s.WriteByte(PstBool(v.IsSingleDefinition));
					UVar(s, (ulong)v.StoreCount);
					UVar(s, (ulong)v.LoadCount);
					UVar(s, (ulong)v.AddressCount);
					UVar(s, PstOptRef(v.Type));
					WriteUtf8(s, v.Name ?? string.Empty);
					break;
				}
				case PstQueryForeachLocal:
				{
					var v = (ILVariable)pstRefs[(int)ReadUVar(query, ref pos)];
					int container = (int)ReadUVar(query, ref pos);
					var blockContainer = container == 0 ? null : (BlockContainer)pstRefs[container - 1];
					bool result = (v.Kind == VariableKind.Local || v.Kind == VariableKind.StackSlot)
						&& (v.IsSingleDefinition || AddressUsedForSingleCall(v, blockContainer))
						&& (v.CaptureScope == null || v.CaptureScope == blockContainer);
					s.WriteByte(PstBool(result));
					break;
				}
				case PstQueryDeclaration:
				{
					var v = (ILVariable)pstRefs[(int)ReadUVar(query, ref pos)];
					if (pstDeclareVariables == null)
					{
						pstDeclareVariables = new DeclareVariables();
						pstDeclareVariables.Analyze(pstWritten[0]);
					}
					pstWrittenIds ??= pstWritten.Select((node, index) => (node, index)).ToDictionary(p => p.node, p => p.index);
					var point = pstDeclareVariables.GetDeclarationPoint(v);
					if (!pstWrittenIds.TryGetValue(point, out int id))
						return null;
					UVar(s, (ulong)id);
					s.WriteByte(PstBool(pstDeclareVariables.WasMerged(v)));
					break;
				}
				case PstQueryType:
				{
					var t = (IType)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(t.Kind == TypeKind.Array));
					s.WriteByte(PstBool(t.IsKnownType(KnownTypeCode.String)));
					UVar(s, t is TypeSystem.ArrayType arrayType ? (ulong)arrayType.Dimensions : 0);
					s.WriteByte(PstBool(t.IsByRefLike));
					s.WriteByte((byte)(t.IsReferenceType switch { null => 0, false => 1, true => 2 }));
					s.WriteByte(PstBool(t.ContainsAnonymousType()));
					UVar(s, t.GetInlineArrayLength() is int length ? (ulong)length + 1 : 0);
					WriteUtf8(s, t.FullName);
					s.WriteByte(PstBool(t is ITypeDefinitionOrUnknown td && td.FullTypeName == KnownAttribute.CompilerGenerated.GetTypeName()));
					s.WriteByte(PstBool(t is ITypeDefinitionOrUnknown td2 && td2.FullTypeName == KnownAttribute.DebuggerBrowsable.GetTypeName()));
					break;
				}
				case PstQueryConvertType:
				{
					if (context == null)
						return null;
					var t = (IType)pstRefs[(int)ReadUVar(query, ref pos)];
					var sub = new SyntaxRoundTrip();
					byte[] tree = sub.Write(context.TypeSystemAstBuilder.ConvertType(t));
					answered.Add(sub.handles);
					UVar(s, (ulong)(answered.Count - 1));
					UVar(s, (ulong)sub.handles.Count);
					s.Write(tree, 0, tree.Length);
					break;
				}
				case PstQueryMember:
				{
					var symbol = pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte((byte)(symbol switch {
						IMethod => 1,
						IProperty => 2,
						IField => 3,
						IEvent => 4,
						ITypeDefinition => 5,
						IType => 6,
						_ => 0,
					}));
					WriteUtf8(s, (symbol as ISymbol)?.Name ?? (symbol as INamedElement)?.Name ?? string.Empty);
					var member = symbol as IMember;
					var entity = symbol as IEntity;
					UVar(s, PstOptRef(member?.MemberDefinition));
					s.WriteByte(PstBool(entity?.IsStatic == true));
					s.WriteByte(PstBool(entity?.IsCompilerGenerated() == true));
					UVar(s, PstOptRef(entity?.DeclaringTypeDefinition));
					WriteUtf8(s, member?.DeclaringType?.FullName ?? string.Empty);
					var method = symbol as IMethod;
					s.WriteByte(PstBool(method?.IsConstructor == true));
					UVar(s, PstOptRef(method?.AccessorOwner));
					break;
				}
				case PstQueryProperty:
				{
					var property = (IProperty)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(property.CanGet));
					s.WriteByte(PstBool(property.CanSet));
					s.WriteByte(PstBool(property.CanGet && property.Getter!.IsCompilerGenerated()));
					s.WriteByte(PstBool(property.Setter != null));
					s.WriteByte(PstBool(property.Setter?.IsCompilerGenerated() == true));
					s.WriteByte(PstBool(property.Setter?.HasReadonlyModifier() == true));
					UVar(s, PstOptRef(PatternStatementTransform.TryGetBackingField(property, out var field) ? field : null));
					break;
				}
				case PstQueryField:
				{
					var field = (IField)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(PatternStatementTransform.HasAccessedThroughPropertyAttribute(field)));
					UVar(s, PstOptRef(PatternStatementTransform.IsBackingFieldOfAutomaticProperty(field, out var property) ? property : null));
					break;
				}
				case PstQueryStoreSurvives:
				{
					var field = (IField)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(context != null && StoreSurvivesAsInitializer(field, context)));
					break;
				}
				case PstQueryPropertyField:
				{
					var property = (IProperty)pstRefs[(int)ReadUVar(query, ref pos)];
					var field = (IField)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(IsPropertyBackingField(property, field)));
					break;
				}
				case PstQueryEventField:
				{
					var field = (IField)pstRefs[(int)ReadUVar(query, ref pos)];
					var ev = (IEvent)pstRefs[(int)ReadUVar(query, ref pos)];
					s.WriteByte(PstBool(field.ParentModule is MetadataModule module
						&& field.Accessibility == Accessibility.Private
						&& ev.ReturnType.Equals(field.ReturnType)
						&& module.MetadataFile.PropertyAndEventBackingFieldLookup.IsEventBackingField((FieldDefinitionHandle)field.MetadataToken, out _)));
					break;
				}
				case PstQueryLiteral:
				{
					var value = PstTarget((uint)ReadUVar(query, ref pos));
					if (value is int i)
					{
						s.WriteByte(1);
						UVar(s, ZigZag(i));
					}
					else
					{
						s.WriteByte(0);
					}
					if (int.TryParse(value?.ToString() ?? "", out int parsed))
					{
						s.WriteByte(1);
						UVar(s, ZigZag(parsed));
					}
					else
					{
						s.WriteByte(0);
					}
					break;
				}
				case PstQueryLiteralEquals:
				{
					var a = PstTarget((uint)ReadUVar(query, ref pos));
					var b = PstTarget((uint)ReadUVar(query, ref pos));
					s.WriteByte(PstBool(object.Equals(a, b)));
					break;
				}
				default:
					return null;
			}
			return s.ToArray();
		}

		object? CreatePatternStatementAnnotation(byte kind, ArraySegment<byte> payload)
		{
			var bytes = payload.ToArray();
			int pos = 0;
			switch (kind)
			{
				case PstAnnotationForeachVariable:
				{
					// The variable becomes the foreach variable as its annotation is made.
					var v = (ILVariable)pstRefs[(int)ReadUVar(bytes, ref pos)];
					v.Kind = VariableKind.ForeachLocal;
					return new ILVariableResolveResult(v, v.Type);
				}
				case PstAnnotationPropertyReference:
				{
					var old = (MemberResolveResult)PstTarget((uint)ReadUVar(bytes, ref pos))!;
					var property = (IProperty)pstRefs[(int)ReadUVar(bytes, ref pos)];
					return new MemberResolveResult(old.TargetResult, property);
				}
				default:
					return null;
			}
		}

		static byte PstBool(bool value) => value ? (byte)1 : (byte)0;

		// The predicates below repeat private members of PatternStatementTransform.

		static bool AddressUsedForSingleCall(ILVariable v, BlockContainer? loop)
		{
			if (v.StoreCount == 1 && v.AddressCount == 1 && v.LoadCount == 0 && v.Type.IsReferenceType == false)
			{
				if (v.AddressInstructions[0].Parent is Call call
					&& v.AddressInstructions[0].ChildIndex == 0
					&& !call.Method.IsStatic)
				{
					for (var node = call.Parent; node != null; node = node.Parent)
					{
						if (node == loop)
							return true;
						else if (node is BlockContainer)
							break;
					}
				}
			}
			return false;
		}

		static bool IsPropertyBackingField(IProperty property, IField field)
		{
			return field.DeclaringTypeDefinition == property.DeclaringTypeDefinition
				&& field.IsStatic == property.IsStatic
				&& (field.IsCompilerGenerated() || IsMetadataPropertyBackingField(property, field));
		}

		static bool IsMetadataPropertyBackingField(IProperty property, IField field)
		{
			if (property.MetadataToken.IsNil || field.MetadataToken.IsNil)
				return false;
			if (field.ParentModule is not MetadataModule module)
				return false;
			return module.MetadataFile.PropertyAndEventBackingFieldLookup.IsPropertyBackingField((FieldDefinitionHandle)field.MetadataToken, out var propertyHandle)
				&& property.MetadataToken == propertyHandle;
		}

		static bool StoreSurvivesAsInitializer(IField field, TransformContext context)
		{
			if (PatternStatementTransform.IsBackingFieldOfAutomaticProperty(field, out var property) && !property.CanSet)
				return true;
			if (field.IsStatic && !context.Settings.AlwaysMoveInitializer
				&& !IsBeforeFieldInit(field.DeclaringTypeDefinition, context))
			{
				return false;
			}
			return true;
		}

		static bool IsBeforeFieldInit(ITypeDefinition? typeDefinition, TransformContext context)
		{
			if (typeDefinition?.MetadataToken.IsNil != false)
				return false;
			var metadata = context.TypeSystem.MainModule.MetadataFile.Metadata;
			var td = metadata.GetTypeDefinition((TypeDefinitionHandle)typeDefinition.MetadataToken);
			return td.HasFlag(System.Reflection.TypeAttributes.BeforeFieldInit);
		}
	}
}
