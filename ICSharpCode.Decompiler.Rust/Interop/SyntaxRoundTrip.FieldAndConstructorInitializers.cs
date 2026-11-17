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
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answers and annotation kinds of the engine's <c>TransformFieldAndConstructorInitializers</c>
	/// (<c>csharp/transforms/field_and_constructor_initializers.rs</c>, which documents each kind).
	/// Symbols travel as ids of the symbol table (<see cref="SymbolId"/>), ILVariables as object ids
	/// (<see cref="ObjectId"/>), attribute syntax as subtrees whose handles index <see cref="minted"/>.
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte FieldInitializersFirstQuery = 112;
		const byte QueryFciCurrentType = 112;
		const byte QueryFciType = 113;
		const byte QueryFciMethod = 114;
		const byte QueryFciRecord = 115;
		const byte QueryFciBackingStores = 116;
		const byte QueryFciMember = 117;
		const byte QueryFciDependsOnParameters = 118;
		const byte QueryFciResolveResult = 119;
		const byte QueryFciDecimalMismatch = 120;
		const byte QueryFciAttributes = 121;
		const byte QueryFciValuesEqual = 122;
		const byte QueryFciDocumentation = 123;
		const byte FieldInitializersLastQuery = 123;

		const byte AnnotationFciOtherConstructors = 112;

		object? CreateFieldInitializersAnnotation(byte kind, ArraySegment<byte> payload)
		{
			if (kind != AnnotationFciOtherConstructors)
				return null;
			byte[] b = payload.ToArray();
			int pos = 0;
			int count = (int)ReadUVar(b, ref pos);
			var initializers = new List<Expression>(count);
			for (int i = 0; i < count; i++)
				initializers.Add((Expression)ReadNode(b, ref pos));
			return new MemberInitializerInOtherConstructorsAnnotation(initializers);
		}

		T SymbolAt<T>(int id) where T : class, ISymbol
			=> id >= 0 && id < symbols.Count && symbols[id] is T t ? t : throw new RustEngineException($"symbol {id} is no {typeof(T).Name}");

		/// <summary>The resolve result a handle argument (the handle plus one) names; UnknownError for 0.</summary>
		ResolveResult ResolveResultArgument(int handlePlusOne)
			=> handlePlusOne > 0 && handlePlusOne <= handles.Count && handles[handlePlusOne - 1] is ResolveResult rr ? rr : ErrorResolveResult.UnknownError;

		byte[]? AnswerFieldInitializers(byte[] query)
		{
			int pos = 1;
			var s = new MemoryStream();
			switch (query[0])
			{
				case QueryFciCurrentType:
				{
					var current = Context.CurrentTypeDefinition;
					UVar(s, current == null ? 0 : (ulong)SymbolId(current) + 1);
					break;
				}
				case QueryFciType:
				{
					var td = SymbolAt<ITypeDefinition>((int)ReadUVar(query, ref pos));
					var record = Context.DecompileRun.RecordDecompilers.TryGetValue(td, out var r) ? r : null;
					bool beforeFieldInit = false;
					if (!td.MetadataToken.IsNil)
					{
						var md = Context.TypeSystem.MainModule.MetadataFile.Metadata;
						beforeFieldInit = md.GetTypeDefinition((SRM.TypeDefinitionHandle)td.MetadataToken).HasFlag(TypeAttributes.BeforeFieldInit);
					}
					var metadata = Context.TypeSystem.MainModule.metadata;
					var typeDef = metadata.GetTypeDefinition((SRM.TypeDefinitionHandle)td.MetadataToken);
					int firstMethodRowNumber = MetadataTokens.GetRowNumber(typeDef.GetMethods().FirstOrDefault());
					s.WriteByte((byte)((td.Kind == TypeKind.Struct ? 1 : 0)
						| (td.IsAbstract ? 2 : 0)
						| (td.IsRecord ? 4 : 0)
						| (beforeFieldInit ? 8 : 0)
						| (record != null ? 16 : 0)));
					UVar(s, (ulong)firstMethodRowNumber);
					UVar(s, record?.PrimaryConstructor is { } primary ? (ulong)SymbolId(primary) + 1 : 0);
					break;
				}
				case QueryFciMethod:
				{
					int id = (int)ReadUVar(query, ref pos);
					if (id < 0 || id >= symbols.Count || symbols[id] is not IMethod m)
						return null;
					s.WriteByte((byte)((m.IsStatic ? 1 : 0)
						| (m.IsConstructor ? 2 : 0)
						| (m.DeclaringType.Kind == TypeKind.Struct ? 4 : 0)));
					s.WriteByte((byte)m.Accessibility);
					UVar(s, (ulong)MetadataTokens.GetRowNumber(m.MetadataToken));
					UVar(s, (ulong)m.Parameters.Count);
					UVar(s, m.DeclaringTypeDefinition == null ? 0 : (ulong)SymbolId(m.DeclaringTypeDefinition) + 1);
					break;
				}
				case QueryFciRecord:
				{
					var td = SymbolAt<ITypeDefinition>((int)ReadUVar(query, ref pos));
					var method = SymbolAt<IMethod>((int)ReadUVar(query, ref pos));
					var record = Context.DecompileRun.RecordDecompilers[td];
					s.WriteByte(record.IsCopyConstructor(method) ? (byte)1 : (byte)0);
					break;
				}
				case QueryFciBackingStores:
				{
					var td = SymbolAt<ITypeDefinition>((int)ReadUVar(query, ref pos));
					var ctor = SymbolAt<IMethod>((int)ReadUVar(query, ref pos));
					var map = Context.DecompileRun.RecordDecompilers[td].GetParameterToBackingStoreMap();
					UVar(s, (ulong)ctor.Parameters.Count);
					foreach (var p in ctor.Parameters)
					{
						if (!map.TryGetValue(p, out var store))
						{
							s.WriteByte(0);
							continue;
						}
						s.WriteByte(1);
						UVar(s, store.Item1 == null ? 0 : (ulong)SymbolId(store.Item1) + 1);
						UVar(s, store.Item2 == null ? 0 : (ulong)SymbolId(store.Item2) + 1);
					}
					break;
				}
				case QueryFciMember:
				{
					var symbol = SymbolAt<ISymbol>((int)ReadUVar(query, ref pos));
					var field = symbol as IField;
					s.WriteByte((byte)((field != null ? 1 : 0)
						| (field is { IsConst: true } ? 2 : 0)
						| (field != null && TransformFieldAndConstructorInitializers.IsGeneratedPrimaryConstructorBackingField(field) ? 4 : 0)
						| (field?.DeclaringTypeDefinition?.Kind == TypeKind.Struct ? 8 : 0)));
					UVar(s, symbol is IProperty property && PatternStatementTransform.TryGetBackingField(property, out var backingField)
						? (ulong)SymbolId(backingField) + 1 : 0);
					break;
				}
				case QueryFciDependsOnParameters:
				{
					int functionHandle = (int)ReadUVar(query, ref pos);
					var function = functionHandle > 0 && functionHandle <= handles.Count ? handles[functionHandle - 1] as ILFunction : null;
					int count = (int)ReadUVar(query, ref pos);
					bool depends = false;
					for (int i = 0; i < count; i++)
					{
						int h = (int)ReadUVar(query, ref pos);
						if (h >= handles.Count || handles[h] is not ILInstruction instruction)
							continue;
						foreach (var inst in instruction.Descendants)
						{
							if (inst is IInstructionWithVariableOperand { Variable: var v }
								&& v.Function == function && v.Kind == VariableKind.Parameter)
							{
								depends = true;
							}
						}
					}
					s.WriteByte(depends ? (byte)1 : (byte)0);
					break;
				}
				case QueryFciResolveResult:
				{
					int h = (int)ReadUVar(query, ref pos);
					if (h >= handles.Count || handles[h] is not ResolveResult rr)
						return null;
					bool referencesInstanceMember = rr switch {
						ThisResolveResult => true,
						MemberResolveResult {
							TargetResult: ThisResolveResult,
							Member: { IsStatic: false } member
						} when member is not IField field || !TransformFieldAndConstructorInitializers.IsGeneratedPrimaryConstructorBackingField(field) => true,
						_ => false
					};
					var variable = (rr as ILVariableResolveResult)?.Variable;
					s.WriteByte((byte)((referencesInstanceMember ? 1 : 0)
						| (rr is MemberResolveResult ? 2 : 0)
						| (IsDefaultValueConstant(rr) ? 4 : 0)
						| (variable != null ? 8 : 0)));
					if (variable != null)
					{
						s.WriteByte((byte)variable.Kind);
						UVar(s, variable.Index is int index && index >= 0 ? (ulong)index + 1 : 0);
						UVar(s, (ulong)ObjectId(variable));
						WriteUtf16(s, variable.Name ?? string.Empty);
					}
					break;
				}
				case QueryFciDecimalMismatch:
				{
					var constant = ResolveResultArgument((int)ReadUVar(query, ref pos));
					var expression = ResolveResultArgument((int)ReadUVar(query, ref pos));
					bool mismatch = constant.IsCompileTimeConstant && TryEvaluateDecimalConstant(expression, out decimal value)
						&& !value.Equals(constant.ConstantValue);
					s.WriteByte(mismatch ? (byte)1 : (byte)0);
					break;
				}
				case QueryFciAttributes:
				{
					var entity = SymbolAt<IEntity>((int)ReadUVar(query, ref pos));
					bool isField = query[pos] != 0;
					var attributes = entity.GetAttributes()
						.Where(a => !isField || !PatternStatementTransform.attributeTypesToRemoveFromAutoProperties.Contains(a.AttributeType.FullName))
						.Select(attr => Context.TypeSystemAstBuilder.ConvertAttribute(attr)).ToArray();
					UVar(s, (ulong)attributes.Length);
					foreach (var attribute in attributes)
					{
						var node = new MemoryStream();
						WriteMinted(node, attribute);
						UVar(s, (ulong)node.Length);
						node.WriteTo(s);
					}
					break;
				}
				case QueryFciValuesEqual:
				{
					int a = (int)ReadUVar(query, ref pos);
					int b = (int)ReadUVar(query, ref pos);
					if (a >= handles.Count || b >= handles.Count)
						return null;
					s.WriteByte(Equals(handles[a], handles[b]) ? (byte)1 : (byte)0);
					break;
				}
				case QueryFciDocumentation:
				{
					var method = SymbolAt<IMethod>((int)ReadUVar(query, ref pos));
					byte flags = 0;
					if (Context.Settings.ShowXmlDocumentation && Context.DecompileRun.DocumentationProvider is { } provider)
					{
						var classDoc = provider.GetDocumentation(method.DeclaringTypeDefinition);
						var ctorDoc = provider.GetDocumentation(method);
						if (ctorDoc != null && ctorDoc != classDoc)
							flags |= 1;
						if (ctorDoc != null)
							flags |= 2;
					}
					s.WriteByte(flags);
					break;
				}
				default:
					return null;
			}
			return s.ToArray();
		}

		/// <summary>The constant part of <c>InitializerSequence.IsDefaultValueInitializer</c>.</summary>
		static bool IsDefaultValueConstant(ResolveResult rr)
		{
			if (!rr.IsCompileTimeConstant)
				return false;
			return rr.ConstantValue switch {
				null or false or '\0'
					or (sbyte)0 or (byte)0 or (short)0 or (ushort)0
					or 0 or 0u or 0L or 0uL => true,
				// -0.0 and 0.00m equal zero but are observably different, so the representation counts.
				float f => BitConverter.ToInt32(BitConverter.GetBytes(f), 0) == 0,
				double d => BitConverter.DoubleToInt64Bits(d) == 0,
				decimal m => m == 0m && decimal.GetBits(m)[3] == 0,
				_ => false,
			};
		}

		/// <summary><c>TryEvaluateDecimalConstant</c> of the managed transform: evaluates a call to the decimal constructor.</summary>
		static bool TryEvaluateDecimalConstant(ResolveResult expression, out decimal value)
		{
			value = 0;
			if (!expression.Type.IsKnownType(KnownTypeCode.Decimal))
				return false;
			switch (expression)
			{
				case CSharpInvocationResolveResult rr:
					if (!(rr.GetSymbol() is IMethod { SymbolKind: SymbolKind.Constructor }))
						return false;
					var args = rr.GetArgumentsForCall();
					if (args.Count == 1)
					{
						switch (args[0].ConstantValue)
						{
							case double d:
								value = new decimal(d);
								return true;
							case float f:
								value = new decimal(f);
								return true;
							case long l:
								value = new decimal(l);
								return true;
							case int i:
								value = new decimal(i);
								return true;
							case ulong ul:
								value = new decimal(ul);
								return true;
							case uint ui:
								value = new decimal(ui);
								return true;
							case int[] bits when bits.Length == 4 && (bits[3] & 0x7F00FFFF) == 0 && (bits[3] & 0xFF000000) <= 0x1C000000:
								value = new decimal(bits);
								return true;
							default:
								return false;
						}
					}
					else if (args.Count == 5 &&
						args[0].ConstantValue is int lo &&
						args[1].ConstantValue is int mid &&
						args[2].ConstantValue is int hi &&
						args[3].ConstantValue is bool isNegative &&
						args[4].ConstantValue is byte scale)
					{
						value = new decimal(lo, mid, hi, isNegative, scale);
						return true;
					}
					return false;
				default:
					if (expression.ConstantValue is decimal v)
					{
						value = v;
						return true;
					}
					return false;
			}
		}
	}
}
