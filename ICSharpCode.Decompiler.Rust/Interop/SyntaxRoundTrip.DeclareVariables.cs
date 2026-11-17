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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answers and annotation kinds of the engine's <c>DeclareVariables</c>
	/// (<c>csharp/transforms/declare_variables.rs</c>, which documents each kind).
	/// <para>
	/// ILVariables, BlockContainers and ILFunctions travel as object ids, stable for the round trip.
	/// Syntax the host builds (types, deconstruction designations) travels as a subtree in the wire
	/// format whose handles index <see cref="minted"/>; the engine attaches each such object with an
	/// annotation of the kind <see cref="AnnotationMinted"/>.
	/// </para>
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte DeclareVariablesFirstQuery = 96;
		const byte QueryAnnotation = 96;
		const byte QueryVariable = 97;
		const byte QuerySettings = 98;
		const byte QueryHidesDiscard = 99;
		const byte QueryTemporaryVariable = 100;
		const byte QueryVariableType = 101;
		const byte QueryUnsafeType = 102;
		const byte QueryDeconstruction = 103;
		const byte QueryDesignation = 104;
		const byte QueryAnnotationTags = 105;
		const byte DeclareVariablesLastQuery = 105;

		const byte AnnotationVariable = 96;
		const byte AnnotationOutVar = 97;
		const byte AnnotationMinted = 98;
		const byte AnnotationUnknownError = 99;

		readonly List<object> objects = new List<object>();
		readonly Dictionary<object, int> objectIds = new Dictionary<object, int>(ReferenceComparer.Instance);
		readonly List<object> minted = new List<object>();

		int ObjectId(object o)
		{
			if (!objectIds.TryGetValue(o, out int id))
			{
				id = objects.Count;
				objects.Add(o);
				objectIds.Add(o, id);
			}
			return id;
		}

		T ObjectAt<T>(int id) where T : class
			=> id >= 0 && id < objects.Count && objects[id] is T t ? t : throw new RustEngineException($"object {id} is no {typeof(T).Name}");

		TransformContext Context => context ?? throw new RustEngineException("DeclareVariables needs the transform context");

		object? CreateDeclareVariablesAnnotation(byte kind, ArraySegment<byte> payload)
		{
			int pos = 0;
			byte[] b = payload.ToArray();
			switch (kind)
			{
				case AnnotationVariable:
				{
					var v = ObjectAt<ILVariable>((int)ReadUVar(b, ref pos));
					return new ILVariableResolveResult(v, v.Type);
				}
				case AnnotationOutVar:
					return new OutVarResolveResult(ObjectAt<ILVariable>((int)ReadUVar(b, ref pos)).Type);
				case AnnotationMinted:
					return minted[(int)ReadUVar(b, ref pos)];
				case AnnotationUnknownError:
					return ErrorResolveResult.UnknownError;
				default:
					return null;
			}
		}

		byte[]? AnswerDeclareVariables(byte[] query)
		{
			int pos = 1;
			int arg = (int)ReadUVar(query, ref pos);
			var s = new MemoryStream();
			switch (query[0])
			{
				case QueryAnnotationTags:
					foreach (var h in handles)
						s.WriteByte(Tag(h));
					break;
				case QueryAnnotation:
					if (arg < 0 || arg >= handles.Count)
						return null;
					WriteAnnotation(s, handles[arg]);
					break;
				case QueryVariable:
				{
					var v = ObjectAt<ILVariable>(arg);
					var name = System.Text.Encoding.UTF8.GetBytes(v.Name ?? string.Empty);
					UVar(s, (ulong)name.Length);
					s.Write(name, 0, name.Length);
					s.WriteByte((byte)v.Kind);
					int flags = (v.UsesInitialValue ? 1 : 0)
						| (v.InitialValueIsInitialized ? 2 : 0)
						| (v.IsRefReadOnly ? 4 : 0)
						| (v.IsScoped ? 8 : 0)
						| (v.IsScopedWithoutInitializer ? 16 : 0)
						| (v.Type.IsByRefLike ? 32 : 0)
						| (v.Type.ContainsAnonymousType() ? 64 : 0);
					UVar(s, (ulong)flags);
					BlockContainer? captureScope = v.CaptureScope;
					while (captureScope != null && !IsRelevantScope(captureScope))
						captureScope = BlockContainer.FindClosestContainer(captureScope.Parent);
					UVar(s, captureScope == null ? 0 : (ulong)ObjectId(captureScope) + 1);
					UVar(s, (ulong)v.LoadCount);
					UVar(s, (ulong)v.StoreCount);
					UVar(s, (ulong)v.AddressCount);
					break;
				}
				case QuerySettings:
				{
					var settings = Context.Settings;
					int bits = (settings.Discards ? 1 : 0)
						| (settings.AnonymousTypes ? 2 : 0)
						| (settings.SeparateLocalVariableDeclarations ? 4 : 0)
						| (settings.ScopedRef ? 8 : 0)
						| (settings.DefaultLiterals ? 16 : 0)
						| (settings.OutVariables ? 32 : 0);
					UVar(s, (ulong)bits);
					break;
				}
				case QueryHidesDiscard:
					s.WriteByte(ExpressionBuilder.HidesVariableWithName(ObjectAt<ILFunction>(arg), "_") ? (byte)1 : (byte)0);
					break;
				case QueryTemporaryVariable:
				{
					var function = ObjectAt<ILFunction>(arg);
					int count = (int)ReadUVar(query, ref pos);
					var annotations = new List<object>();
					for (int i = 0; i < count; i++)
					{
						int h = (int)ReadUVar(query, ref pos);
						if (h < handles.Count)
							annotations.Add(handles[h]);
					}
					var type = (annotations.OfType<ResolveResult>().FirstOrDefault() ?? ErrorResolveResult.UnknownError).Type;
					var v = function.RegisterVariable(
						VariableKind.StackSlot,
						type,
						AssignVariableNames.GenerateVariableName(function, type, Context.DecompileRun.UsingScope,
							annotations.OfType<ILInstruction>()
								.Where(AssignVariableNames.IsSupportedInstruction).FirstOrDefault(),
							mustResolveConflicts: true)
					);
					UVar(s, (ulong)ObjectId(v));
					break;
				}
				case QueryVariableType:
					WriteMinted(s, Context.TypeSystemAstBuilder.ConvertType(ObjectAt<ILVariable>(arg).Type));
					break;
				case QueryUnsafeType:
					WriteMinted(s, Context.TypeSystemAstBuilder.ConvertType(Context.TypeSystem.FindType(KnownTypeCode.Unsafe)));
					break;
				case QueryDeconstruction:
				{
					if (arg < 0 || arg >= handles.Count || handles[arg] is not DeconstructInstruction deconstruct)
						return null;
					s.WriteByte(deconstruct.Init.Count == 0 && deconstruct.Conversions.Instructions.Count == 0 ? (byte)1 : (byte)0);
					UVar(s, (ulong)deconstruct.Assignments.Instructions.Count);
					foreach (var inst in deconstruct.Assignments.Instructions)
					{
						if (!inst.MatchStLoc(out var v, out var value))
						{
							s.WriteByte(0);
							continue;
						}
						s.WriteByte(value is LdLoc ldloc ? (v.Type.Equals(ldloc.Variable.Type) ? (byte)2 : (byte)1) : (byte)3);
						UVar(s, (ulong)ObjectId(v));
					}
					break;
				}
				case QueryDesignation:
				{
					if (arg < 0 || arg >= handles.Count || handles[arg] is not DeconstructInstruction deconstruct)
						return null;
					WriteMinted(s, StatementBuilder.TranslateDeconstructionDesignation(deconstruct, isForeach: false));
					break;
				}
				default:
					return null;
			}
			return s.ToArray();
		}

		static bool IsRelevantScope(BlockContainer scope)
			=> scope.EntryPoint.IncomingEdgeCount > 1 || scope.Parent is ILFunction;

		/// <summary>
		/// The tag of an annotation for <see cref="QueryAnnotationTags"/>, as the first byte of its
		/// <see cref="QueryAnnotation"/> answer.
		/// </summary>
		static byte Tag(object annotation) => annotation switch {
			ILVariableResolveResult => 2,
			ResolveResult => 1,
			BlockContainer => 3,
			ILFunction => 4,
			DeconstructInstruction => 5,
			UseImplicitlyTypedOutAnnotation => 6,
			_ => 0
		};

		void WriteAnnotation(MemoryStream s, object annotation)
		{
			s.WriteByte(Tag(annotation));
			switch (annotation)
			{
				case ILVariableResolveResult rr:
					UVar(s, (ulong)ObjectId(rr.Variable));
					break;
				case BlockContainer container:
					WriteContainer(s, container);
					break;
				case ILFunction function:
					UVar(s, (ulong)ObjectId(function));
					s.WriteByte((byte)((function.Kind == ILFunctionKind.LocalFunction ? 1 : 0) | (function.Parent == null ? 2 : 0)));
					if (function.Body is BlockContainer body)
					{
						s.WriteByte(1);
						WriteContainer(s, body);
					}
					else
					{
						s.WriteByte(0);
					}
					if (function.Kind == ILFunctionKind.LocalFunction)
					{
						UVar(s, (ulong)function.CapturedVariables.Count);
						foreach (var v in function.CapturedVariables)
							UVar(s, (ulong)ObjectId(v));
					}
					else
					{
						UVar(s, 0);
					}
					break;
			}
		}

		void WriteContainer(MemoryStream s, BlockContainer container)
		{
			UVar(s, (ulong)ObjectId(container));
			s.WriteByte((byte)((container.EntryPoint.IncomingEdgeCount > 1 ? 1 : 0) | (container.Parent is ILFunction ? 2 : 0)));
		}

		/// <summary>Writes <paramref name="node"/> with its handles indexing <see cref="minted"/>.</summary>
		void WriteMinted(MemoryStream s, AstNode node)
		{
			mintTarget = minted;
			try
			{
				WriteNode(s, node);
			}
			finally
			{
				mintTarget = null;
			}
		}
	}
}
