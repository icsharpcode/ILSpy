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

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The C# syntax tree in the wire format of <c>csharp/syntax.rs</c>: the host writes the tree
	/// it built, the engine transforms it and sends it back, and the host builds the nodes anew.
	/// Annotations and literal values stay host objects, sent as handles into <see cref="Handles"/>
	/// and attached again to the rebuilt nodes.
	/// </summary>
	internal sealed partial class SyntaxRoundTrip
	{
		readonly List<object> handles = new List<object>();
		const byte QueryMaterializeAnnotations = 255;
		readonly List<object> created = new List<object>();
		/// <summary>The handle tables of the subtrees the host built for the engine's queries.</summary>
		readonly List<IReadOnlyList<object>> answered = new List<IReadOnlyList<object>>();
		readonly CSharp.Transforms.TransformContext? context;

		/// <summary>
		/// A round trip for the transform context <paramref name="context"/>, which answers the
		/// queries about settings and builds type syntax; without one those queries have no answer.
		/// </summary>
		public SyntaxRoundTrip(CSharp.Transforms.TransformContext? context = null)
		{
			this.context = context;
		}

		/// <summary>The settings of the request, which the engine may ask about; null where unknown.</summary>
		public DecompilerSettings? Settings => context?.Settings;
		/// <summary>The transform context's builder, for the queries that convert types.</summary>
		CSharp.Syntax.TypeSystemAstBuilder? astBuilder => context?.TypeSystemAstBuilder;
		readonly List<TypeSystem.ISymbol> symbols = new List<TypeSystem.ISymbol>();
		readonly Dictionary<TypeSystem.ISymbol, int> symbolIds = new Dictionary<TypeSystem.ISymbol, int>();
		readonly List<object> answerObjects = new List<object>();

		/// <summary>The host objects the tree refers to by handle.</summary>
		public IReadOnlyList<object> Handles => handles;

		/// <summary>
		/// Where <see cref="Handle"/> puts objects while the oracle writes a subtree into an answer
		/// (see <c>SyntaxRoundTrip.DeclareVariables.cs</c>), instead of the tree's handles.
		/// </summary>
		List<object>? mintTarget;

		int Handle(object value)
		{
			if (mintTarget != null)
			{
				mintTarget.Add(value);
				return mintTarget.Count - 1;
			}
			handles.Add(value);
			return handles.Count - 1;
		}

		/// <summary>Writes <paramref name="root"/> and its subtree.</summary>
		public byte[] Write(AstNode root)
		{
			var stream = new MemoryStream();
			WriteNode(stream, root);
			return stream.ToArray();
		}

		void WriteNode(MemoryStream s, AstNode node)
		{
			var kind = SyntaxSchema.Of(node.GetType());
			PstRecordWritten(node);
			UVar(s, (ulong)kind.Index);
			foreach (var scalar in kind.Scalars)
			{
				var p = scalar.Property;
				object? value = p.GetValue(node);
				var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
				if (type == typeof(bool))
					s.WriteByte((bool)value! ? (byte)1 : (byte)0);
				else if (type == typeof(string))
					WriteString(s, (string?)value);
				else if (type == typeof(object))
					UVar(s, (ulong)Handle(value!));
				else if (type == typeof(TextLocation))
				{
					var location = (TextLocation)value!;
					UVar(s, (ulong)location.Line);
					UVar(s, (ulong)location.Column);
				}
				else
					UVar(s, ZigZag(ToInt64(value!)));
			}
			var annotations = new List<int>();
			foreach (var annotation in node.Annotations)
			{
				if (annotation is not AstNode.NodeTrivia)
					annotations.Add(Handle(annotation));
			}
			UVar(s, (ulong)annotations.Count);
			foreach (int h in annotations)
				UVar(s, (ulong)h);
			foreach (var trivia in new[] { new List<Trivia>(node.LeadingTrivia), new List<Trivia>(node.TrailingTrivia) })
			{
				UVar(s, (ulong)trivia.Count);
				foreach (var t in trivia)
					WriteNode(s, t);
			}
			// The children of each slot, in child order.
			var bySlot = new List<AstNode>[kind.Slots.Count];
			int count = node.GetChildCount();
			for (int i = 0; i < count; i++)
			{
				var child = node.GetChild(i);
				if (child == null)
					continue;
				int slot = kind.SlotIndex(node.GetChildSlotInfo(i));
				(bySlot[slot] ??= new List<AstNode>()).Add(child);
			}
			for (int i = 0; i < kind.Slots.Count; i++)
			{
				var children = bySlot[i];
				if (kind.Slots[i].Info.IsCollection)
				{
					UVar(s, (ulong)(children?.Count ?? 0));
					if (children != null)
					{
						foreach (var c in children)
							WriteNode(s, c);
					}
				}
				else if (children == null)
				{
					s.WriteByte(0);
				}
				else
				{
					s.WriteByte(1);
					WriteNode(s, children[0]);
				}
			}
		}

		/// <summary>
		/// Replaces the children (and scalars) of <paramref name="root"/> by those of the tree in
		/// <paramref name="bytes"/>, whose root must be of the same kind; the root object stays,
		/// since the caller holds it.
		/// </summary>
		public void Rebuild(AstNode root, byte[] bytes)
		{
			int pos = 0;
			int count = (int)ReadUVar(bytes, ref pos);
			for (int i = 0; i < count; i++)
			{
				byte annotationKind = bytes[pos++];
				int length = (int)ReadUVar(bytes, ref pos);
				created.Add(CreateAnnotation(annotationKind, new ArraySegment<byte>(bytes, pos, length)));
				pos += length;
			}
			var rebuilt = ReadNode(bytes, ref pos);
			if (pos != bytes.Length)
				throw new RustEngineException($"{bytes.Length - pos} bytes after the syntax tree");
			if (rebuilt.GetType() != root.GetType())
				throw new RustEngineException($"the engine returned a {rebuilt.GetType().Name} for a {root.GetType().Name}");
			var kind = SyntaxSchema.Of(root.GetType());
			foreach (var scalar in kind.Scalars)
				SyntaxSchema.SetScalar(scalar.Property, root, scalar.Property.GetValue(rebuilt));
			// The root keeps its annotations and gains those the engine attached to it.
			foreach (var annotation in rebuilt.Annotations)
			{
				if (annotation is not AstNode.NodeTrivia && !root.Annotations.Any(a => ReferenceEquals(a, annotation)))
					root.AddAnnotation(annotation);
			}
			foreach (var child in new List<AstNode>(Children(root)))
				child.Remove();
			foreach (var (child, info) in ChildrenWithSlots(rebuilt))
			{
				child.Remove();
				root.AddChildUnsafe(child, info.Kind ?? info);
			}
		}

		static IEnumerable<AstNode> Children(AstNode node)
		{
			foreach (var (child, _) in ChildrenWithSlots(node))
				yield return child;
		}

		static List<(AstNode, CSharpSlotInfo)> ChildrenWithSlots(AstNode node)
		{
			var result = new List<(AstNode, CSharpSlotInfo)>();
			int count = node.GetChildCount();
			for (int i = 0; i < count; i++)
			{
				var child = node.GetChild(i);
				if (child != null)
					result.Add((child, node.GetChildSlotInfo(i)));
			}
			return result;
		}

		AstNode ReadNode(byte[] b, ref int pos)
		{
			var kind = SyntaxSchema.Kinds[(int)ReadUVar(b, ref pos)];
			var node = Create(kind.Type);
			foreach (var scalar in kind.Scalars)
			{
				var p = scalar.Property;
				var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
				object? value;
				if (type == typeof(bool))
					value = b[pos++] != 0;
				else if (type == typeof(string))
					value = ReadString(b, ref pos);
				else if (type == typeof(object))
					value = Lookup((int)ReadUVar(b, ref pos));
				else if (type == typeof(TextLocation))
				{
					int line = (int)ReadUVar(b, ref pos);
					value = new TextLocation(line, (int)ReadUVar(b, ref pos));
				}
				else
				{
					long v = UnZigZag(ReadUVar(b, ref pos));
					value = type.IsEnum ? Enum.ToObject(type, v) : Convert.ChangeType(v, type, System.Globalization.CultureInfo.InvariantCulture);
				}
				SyntaxSchema.SetScalar(p, node, value);
			}
			int annotations = (int)ReadUVar(b, ref pos);
			for (int i = 0; i < annotations; i++)
				node.AddAnnotation(Lookup((int)ReadUVar(b, ref pos)));
			int leading = (int)ReadUVar(b, ref pos);
			for (int i = 0; i < leading; i++)
				node.AddLeadingTrivia((Trivia)ReadNode(b, ref pos));
			int trailing = (int)ReadUVar(b, ref pos);
			for (int i = 0; i < trailing; i++)
				node.AddTrailingTrivia((Trivia)ReadNode(b, ref pos));
			foreach (var slot in kind.Slots)
			{
				var target = slot.Info.Kind ?? slot.Info;
				if (slot.Info.IsCollection)
				{
					int count = (int)ReadUVar(b, ref pos);
					for (int i = 0; i < count; i++)
						node.AddChildUnsafe(ReadNode(b, ref pos), target);
				}
				else if (b[pos++] != 0)
				{
					node.AddChildUnsafe(ReadNode(b, ref pos), target);
				}
			}
			return node;
		}

		/// <summary>An integer or enum value as a long, unsigned values above <c>long.MaxValue</c> wrapped.</summary>
		static long ToInt64(object value)
		{
			var type = value.GetType();
			var code = Type.GetTypeCode(type.IsEnum ? Enum.GetUnderlyingType(type) : type);
			return code == TypeCode.UInt64 ? unchecked((long)Convert.ToUInt64(value, System.Globalization.CultureInfo.InvariantCulture)) : Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture);
		}

		object Lookup(int handle) => handle < handles.Count ? handles[handle] : created[handle - handles.Count];

		/// <summary>
		/// The host object for an annotation the engine created, of the kind <paramref name="kind"/>
		/// (<c>SyntaxTree::new_annotations</c> in <c>csharp/syntax.rs</c>). A transform that creates a
		/// new kind of annotation adds its case here.
		/// </summary>
		object CreateAnnotation(byte kind, ArraySegment<byte> payload)
		{
			if (CreateWithEventsAnnotation(kind, payload) is { } withEvents)
				return withEvents;
			if (CreateDeclareVariablesAnnotation(kind, payload) is { } declareVariables)
				return declareVariables;
			if (kind is >= 16 and < 32 && CreatePatternStatementAnnotation(kind, payload) is { } patternStatement)
				return patternStatement;
			if (CreateFieldInitializersAnnotation(kind, payload) is { } fieldInitializers)
				return fieldInitializers;
			if (CreateIntroduceUsingDeclarationsAnnotation(kind) is { } introduceUsingDeclarations)
				return introduceUsingDeclarations;
			if (CreateQueryExpressionsAnnotation(kind, payload) is { } queryExpressions)
				return queryExpressions;
			if (CreateCombineQueryExpressionsAnnotation(kind) is { } combineQueries)
				return combineQueries;
			switch (kind)
			{
				case AnnotationAnswered:
				{
					var bytes = payload.ToArray();
					int pos = 0;
					int table = (int)ReadUVar(bytes, ref pos);
					return answered[table][(int)ReadUVar(bytes, ref pos)];
				}
				case AnnotationChecked:
					return CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation;
				case AnnotationUnchecked:
					return CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation;
				case AnnotationDecimalOne:
					return 1m;
				default:
					throw new RustEngineException($"the engine created an annotation of the unknown kind {kind}");
			}
		}

		/// <summary>The annotation kinds of <c>csharp/transforms/replace_method_calls_with_operators.rs</c>.</summary>
		const byte AnnotationAnswered = 48;
		const byte AnnotationChecked = 49;
		const byte AnnotationUnchecked = 50;
		const byte AnnotationDecimalOne = 51;

		/// <summary>
		/// The annotations <c>RewriteVisualBasicWithEventsInInitializeComponent</c> creates (kinds 32..47);
		/// null for another kind.
		/// </summary>
		object? CreateWithEventsAnnotation(byte kind, ArraySegment<byte> payload)
		{
			int pos = payload.Offset;
			switch (kind)
			{
				case AnnotationMemberResolveResult:
					return new Semantics.MemberResolveResult(null, (TypeSystem.IMember)symbols[(int)ReadUVar(payload.Array!, ref pos)]);
				case AnnotationReturnTypeResolveResult:
					return new Semantics.TypeResolveResult(((TypeSystem.IMember)symbols[(int)ReadUVar(payload.Array!, ref pos)]).ReturnType);
				case AnnotationHostObject:
					return answerObjects[(int)ReadUVar(payload.Array!, ref pos)];
				default:
					return null;
			}
		}

		/// <summary>The query kinds of <c>csharp/syntax_oracle.rs</c>.</summary>
		const byte QueryClassName = 0;
		const byte QueryResolveResultType = 1;
		const byte QuerySymbol = 2;
		const byte QueryResolveResultFacts = 48;
		const byte QueryMethod = 49;
		const byte QueryHasOperator = 50;
		const byte QuerySetting = 51;
		const byte QueryConvertType = 52;
		const byte QueryILInstruction = 53;
		const byte QueryTypeResolveResultName = 54;
		const byte QueryCheckedState = 80;
		const byte QueryCheckForOverflowUnderflow = 81;
		const byte QueryUnsafeFacts = 64;
		const byte QueryNodeSymbol = 32;
		const byte QueryMemberFacts = 33;
		const byte QueryAccessedThroughPropertyField = 34;
		const byte QueryFieldMatchesProperty = 35;
		const byte QueryAccessedThroughPropertyNames = 36;
		const byte QueryReturnTypeSyntax = 37;

		/// <summary>The annotation kinds the engine creates (<c>csharp/syntax_oracle.rs</c>).</summary>
		const byte AnnotationMemberResolveResult = 32;
		const byte AnnotationReturnTypeResolveResult = 33;
		const byte AnnotationHostObject = 34;

		/// <summary>The id of <paramref name="symbol"/> in the table the symbol queries refer to;
		/// equal symbols share it.</summary>
		int SymbolId(TypeSystem.ISymbol symbol)
		{
			if (!symbolIds.TryGetValue(symbol, out int id))
			{
				id = symbols.Count;
				symbols.Add(symbol);
				symbolIds.Add(symbol, id);
			}
			return id;
		}

		/// <summary>
		/// The answer to a query about the symbol table (kinds 33 to 37), whose first argument is a
		/// symbol id; null where it does not apply.
		/// </summary>
		byte[]? AnswerSymbolQuery(byte kind, TypeSystem.ISymbol symbol, byte[] query, ref int pos)
		{
			var s = new MemoryStream();
			switch (kind)
			{
				case QueryMemberFacts:
				{
					s.WriteByte(symbol switch {
						TypeSystem.IProperty => 1,
						TypeSystem.IField => 2,
						TypeSystem.IEvent => 3,
						TypeSystem.IMethod => 4,
						_ => 0
					});
					UVar(s, (ulong)SymbolId(symbol is TypeSystem.IMember m ? m.MemberDefinition : symbol));
					byte flags = 0;
					if (symbol is TypeSystem.IEntity { IsStatic: true })
						flags |= 1;
					if (symbol is TypeSystem.IProperty { Setter: not null })
						flags |= 2;
					if (symbol is TypeSystem.IMethod method && CSharp.CSharpDecompiler.IsWindowsFormsInitializeComponentMethod(method))
						flags |= 4;
					s.WriteByte(flags);
					UVar(s, (ulong)(symbol is TypeSystem.IProperty p ? p.Parameters.Count : 0));
					WriteUtf16(s, symbol.Name);
					return s.ToArray();
				}
				case QueryAccessedThroughPropertyField:
				{
					if (symbol is not TypeSystem.IProperty property
						|| !CSharp.Transforms.PatternStatementTransform.TryGetBackingField(property, out var field)
						|| !CSharp.Transforms.PatternStatementTransform.HasAccessedThroughPropertyAttribute(field))
						return null;
					UVar(s, (ulong)SymbolId(field));
					return s.ToArray();
				}
				case QueryFieldMatchesProperty:
				{
					var field = symbols[(int)ReadUVar(query, ref pos)] as TypeSystem.IField;
					bool matches = symbol is TypeSystem.IProperty property && field != null
						&& field.DeclaringTypeDefinition != null && field.DeclaringTypeDefinition.Equals(property.DeclaringTypeDefinition)
						&& TypeSystem.NormalizeTypeVisitor.IgnoreNullability.EquivalentTypes(field.ReturnType, property.ReturnType);
					return new[] { matches ? (byte)1 : (byte)0 };
				}
				case QueryAccessedThroughPropertyNames:
				{
					if (symbol is not TypeSystem.IField field)
						return null;
					var names = new List<string>();
					foreach (var attr in field.GetAttributes())
					{
						if (attr.AttributeType.FullName == "Microsoft.VisualBasic.CompilerServices.AccessedThroughPropertyAttribute"
							&& attr.FixedArguments.Length == 1 && attr.FixedArguments[0].Value is string name)
						{
							names.Add(name);
						}
					}
					UVar(s, (ulong)names.Count);
					foreach (var name in names)
					{
						UVar(s, (ulong)name.Length);
						WriteUtf16(s, name);
					}
					return s.ToArray();
				}
				case QueryReturnTypeSyntax:
				{
					if (astBuilder == null || symbol is not TypeSystem.IMember member)
						return null;
					var node = new SyntaxRoundTrip();
					byte[] bytes = node.Write(astBuilder.ConvertType(member.ReturnType));
					UVar(s, (ulong)answerObjects.Count);
					answerObjects.AddRange(node.Handles);
					s.Write(bytes, 0, bytes.Length);
					return s.ToArray();
				}
				default:
					return null;
			}
		}

		/// <summary>
		/// The answer to the engine's question <paramref name="query"/> about one of the tree's host
		/// objects (<c>csharp/syntax_oracle.rs</c>); null where it does not apply. A transform that needs
		/// another fact adds its query kind here.
		/// </summary>
		public byte[]? Answer(byte[] query)
		{
			if (query[0] == QueryMaterializeAnnotations)
			{
				// The annotations the engine created so far become host objects, at the handles the engine gave them.
				int at = 1;
				int count = (int)ReadUVar(query, ref at);
				for (int i = 0; i < count; i++)
				{
					byte annotationKind = query[at++];
					int length = (int)ReadUVar(query, ref at);
					handles.Add(CreateAnnotation(annotationKind, new ArraySegment<byte>(query, at, length)));
					at += length;
				}
				return Array.Empty<byte>();
			}
			if (query[0] is >= DeclareVariablesFirstQuery and <= DeclareVariablesLastQuery)
				return AnswerDeclareVariables(query);
			if (query[0] is >= 16 and < 32)
				return AnswerPatternStatementTransform(query);
			if (query[0] is >= FieldInitializersFirstQuery and <= FieldInitializersLastQuery)
				return AnswerFieldInitializers(query);
			if (query[0] is >= IntroduceUsingDeclarationsFirstQuery and <= IntroduceUsingDeclarationsLastQuery)
				return AnswerIntroduceUsingDeclarations(query);
			if (query[0] == QueryQueryExpressionsAnnotation)
				return AnswerQueryExpressionsAnnotation(query);
			int pos = 1;
			if (query[0] is >= QueryMemberFacts and <= QueryReturnTypeSyntax)
			{
				int id = (int)ReadUVar(query, ref pos);
				return id >= 0 && id < symbols.Count ? AnswerSymbolQuery(query[0], symbols[id], query, ref pos) : null;
			}
			int handle = (int)ReadUVar(query, ref pos);
			if (query[0] == QuerySetting)
			{
				// Not about a host object: the handle is 0 and the argument names the setting.
				if (context == null)
					return null;
				string name = ReadUtf8(query, ref pos);
				return typeof(DecompilerSettings).GetProperty(name) is { PropertyType: var t } p && t == typeof(bool)
					? new[] { (bool)p.GetValue(context.Settings)! ? (byte)1 : (byte)0 }
					: throw new RustEngineException($"the engine asked for the setting '{name}', which is no boolean DecompilerSettings property");
			}
			if (query[0] == QueryCheckForOverflowUnderflow)
				return Settings == null ? null : new[] { Settings.CheckForOverflowUnderflow ? (byte)1 : (byte)0 };
			if (handle < 0 || handle >= handles.Count)
				return null;
			object target = handles[handle];
			switch (query[0])
			{
				case QueryResolveResultFacts:
				{
					if (target is not Semantics.ResolveResult result)
						return null;
					var type = result.Type;
					return new[] {
						KnownCode(type),
						KnownCode(TypeSystem.NullableType.GetUnderlyingType(type)),
						type.IsByRefLike ? (byte)1 : (byte)0,
						type.IsReferenceType switch { false => (byte)0, true => (byte)1, null => (byte)2 },
						result is Semantics.InvocationResolveResult { Member: TypeSystem.IMethod { Name: "Concat" } concat }
							&& TypeSystem.TypeSystemExtensions.IsKnownType(concat.DeclaringType, TypeSystem.KnownTypeCode.String) ? (byte)1 : (byte)0,
					};
				}
				case QueryMethod:
				{
					if (MethodOf(target) is not { } method)
						return null;
					var s = new MemoryStream();
					WriteUtf8(s, method.Name);
					WriteUtf8(s, method.FullName);
					s.WriteByte(KnownCode(method.DeclaringType));
					s.WriteByte(IL.Transforms.ILInlining.MethodRequiresCopyForReadonlyLValue(method) ? (byte)1 : (byte)0);
					UVar(s, (ulong)method.TypeArguments.Count);
					s.WriteByte(method.TypeArguments.Count > 0 && method.TypeArguments[0] is TypeSystem.ITypeParameter { HasDefaultConstructorConstraint: true } ? (byte)1 : (byte)0);
					UVar(s, (ulong)method.Parameters.Count);
					foreach (var p in method.Parameters)
					{
						s.WriteByte(KnownCode(p.Type));
						s.WriteByte(p.IsParams ? (byte)1 : (byte)0);
						var element = (p.Type as TypeSystem.ArrayType)?.ElementType;
						s.WriteByte(element != null ? (byte)1 : (byte)0);
						s.WriteByte(element != null ? KnownCode(element) : (byte)0);
					}
					return s.ToArray();
				}
				case QueryHasOperator:
				{
					if (MethodOf(target) is not { } method)
						return null;
					string name = ReadUtf8(query, ref pos);
					return new[] { method.DeclaringType.GetMethods(m => m.IsOperator && m.Name == name).Any() ? (byte)1 : (byte)0 };
				}
				case QueryConvertType:
				{
					if (context == null || MethodOf(target) is not { } method)
						return null;
					int which = (int)ReadUVar(query, ref pos);
					var type = which switch {
						0 => method.TypeArguments[0],
						1 => method.ReturnType,
						_ => method.Parameters[which - 2].Type,
					};
					var sub = new SyntaxRoundTrip();
					byte[] tree = sub.Write(context.TypeSystemAstBuilder.ConvertType(type));
					answered.Add(sub.handles);
					var s = new MemoryStream();
					UVar(s, (ulong)(answered.Count - 1));
					UVar(s, (ulong)sub.handles.Count);
					s.Write(tree, 0, tree.Length);
					return s.ToArray();
				}
				case QueryILInstruction:
					return target switch {
						IL.ILFunction f => new[] { (byte)f.Kind },
						IL.ILInstruction => new byte[] { 0xFF },
						_ => null
					};
				case QueryTypeResolveResultName:
				{
					if (target is not Semantics.TypeResolveResult trr)
						return null;
					var s = new MemoryStream();
					WriteUtf8(s, trr.Type.Namespace);
					WriteUtf8(s, trr.Type.Name);
					return s.ToArray();
				}
				case QueryNodeSymbol:
				{
					if (target is not Semantics.ResolveResult nodeRR)
						return null;
					var symbol = nodeRR is Semantics.MethodGroupResolveResult mgrr ? mgrr.ChosenMethod : TypeSystem.TypeSystemExtensions.GetSymbol(nodeRR);
					var s = new MemoryStream();
					UVar(s, symbol == null ? 0 : (ulong)SymbolId(symbol) + 1);
					return s.ToArray();
				}
				case QueryClassName:
					return System.Text.Encoding.UTF8.GetBytes(target.GetType().Name);
				case QueryResolveResultType:
					return target is Semantics.ResolveResult rr ? System.Text.Encoding.UTF8.GetBytes(rr.Type.ReflectionName) : null;
				case QuerySymbol:
				{
					if (target is not Semantics.ResolveResult r || TypeSystem.TypeSystemExtensions.GetSymbol(r) is not { } symbol)
						return null;
					var s = new MemoryStream();
					s.WriteByte((byte)symbol.SymbolKind);
					string declaring = symbol is TypeSystem.IEntity { DeclaringType: { } t } ? t.ReflectionName : string.Empty;
					foreach (var text in new[] { declaring, symbol.Name })
					{
						var utf8 = System.Text.Encoding.UTF8.GetBytes(text);
						UVar(s, (ulong)utf8.Length);
						s.Write(utf8, 0, utf8.Length);
					}
					return s.ToArray();
				}
				case QueryCheckedState:
					if (ReferenceEquals(target, CSharp.Transforms.AddCheckedBlocks.CheckedAnnotation))
						return new byte[] { 0 };
					if (ReferenceEquals(target, CSharp.Transforms.AddCheckedBlocks.UncheckedAnnotation))
						return new byte[] { 1 };
					if (ReferenceEquals(target, CSharp.Transforms.AddCheckedBlocks.ExplicitUncheckedAnnotation))
						return new byte[] { 2 };
					return null;
				case QueryUnsafeFacts:
					return target is Semantics.ResolveResult u ? new[] { UnsafeFacts(u) } : null;
				default:
					return null;
			}
		}

		/// <summary>The method of a resolve result, as <c>AstNode.GetSymbol</c> reads it.</summary>
		static TypeSystem.IMethod? MethodOf(object target)
		{
			return target switch {
				Semantics.MethodGroupResolveResult mgrr => mgrr.ChosenMethod,
				Semantics.ResolveResult rr => TypeSystem.TypeSystemExtensions.GetSymbol(rr) as TypeSystem.IMethod,
				_ => null
			};
		}

		static byte KnownCode(TypeSystem.IType type) => (byte)(type.GetDefinition()?.KnownTypeCode ?? TypeSystem.KnownTypeCode.None);

		static void WriteUtf8(MemoryStream s, string text)
		{
			var utf8 = System.Text.Encoding.UTF8.GetBytes(text);
			UVar(s, (ulong)utf8.Length);
			s.Write(utf8, 0, utf8.Length);
		}

		static string ReadUtf8(byte[] b, ref int pos)
		{
			int length = (int)ReadUVar(b, ref pos);
			string text = System.Text.Encoding.UTF8.GetString(b, pos, length);
			pos += length;
			return text;
		}

		/// <summary>The <c>UNSAFE_*</c> flags of <c>QUERY_UNSAFE_FACTS</c> for <paramref name="rr"/>.</summary>
		static byte UnsafeFacts(Semantics.ResolveResult rr)
		{
			int facts = 0;
			if (IsUnsafeType(rr.Type))
				facts |= 1;
			TypeSystem.IParameterizedMember? member = null;
			if ((rr as Semantics.MemberResolveResult)?.Member is TypeSystem.IParameterizedMember pm)
			{
				facts |= 2;
				member = pm;
			}
			else if (rr is Semantics.MethodGroupResolveResult mgrr)
			{
				facts |= 4;
				member = mgrr.ChosenMethod;
			}
			if (member != null)
			{
				if (member.Parameters.Any(p => IsUnsafeType(p.Type)))
					facts |= 8;
				if (IsUnsafeType(member.ReturnType))
					facts |= 16;
			}
			if (rr is Semantics.OperatorResolveResult orr && orr.Operands.FirstOrDefault()?.Type.Kind == TypeSystem.TypeKind.Pointer)
				facts |= 32;
			return (byte)facts;
		}

		static bool IsUnsafeType(TypeSystem.IType? type)
		{
			switch (type?.Kind)
			{
				case TypeSystem.TypeKind.Pointer:
				case TypeSystem.TypeKind.FunctionPointer:
					return true;
				case TypeSystem.TypeKind.ByReference:
				case TypeSystem.TypeKind.Array:
					return IsUnsafeType(((TypeSystem.Implementation.TypeWithElementType)type).ElementType);
				default:
					return false;
			}
		}

		static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, System.Reflection.ConstructorInfo> constructors = new();

		/// <summary>
		/// A node of <paramref name="type"/> without children, made with the constructor of the fewest
		/// parameters (empty strings and default values for them); the reader sets its scalars and
		/// fills its slots afterwards.
		/// </summary>
		static AstNode Create(Type type)
		{
			var ctor = constructors.GetOrAdd(type, t => t.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				.OrderBy(c => c.GetParameters().Length).First());
			var arguments = ctor.GetParameters()
				.Select(p => p.ParameterType == typeof(string) ? string.Empty : p.ParameterType.IsValueType ? Activator.CreateInstance(p.ParameterType) : null)
				.ToArray();
			var node = (AstNode)ctor.Invoke(arguments);
			foreach (var child in Children(node).ToList())
				child.Remove();
			return node;
		}

		/// <summary>A string as its length plus one (0 for null) and its UTF-16 code units.</summary>
		static void WriteString(MemoryStream s, string? value)
		{
			if (value == null)
			{
				s.WriteByte(0);
				return;
			}
			UVar(s, (ulong)value.Length + 1);
			WriteUtf16(s, value);
		}

		/// <summary>The UTF-16 code units of <paramref name="value"/>, little-endian.</summary>
		static void WriteUtf16(MemoryStream s, string value)
		{
			foreach (char c in value)
			{
				s.WriteByte(unchecked((byte)c));
				s.WriteByte(unchecked((byte)(c >> 8)));
			}
		}

		static string? ReadString(byte[] b, ref int pos)
		{
			int encoded = (int)ReadUVar(b, ref pos);
			if (encoded == 0)
				return null;
			int length = encoded - 1;
			var chars = new char[length];
			for (int i = 0; i < length; i++)
			{
				chars[i] = (char)(b[pos] | (b[pos + 1] << 8));
				pos += 2;
			}
			return new string(chars);
		}

		static void UVar(MemoryStream s, ulong v)
		{
			while (v >= 0x80)
			{
				s.WriteByte(unchecked((byte)(v | 0x80)));
				v >>= 7;
			}
			s.WriteByte(unchecked((byte)v));
		}

		static ulong ReadUVar(byte[] b, ref int pos)
		{
			ulong v = 0;
			int shift = 0;
			while (true)
			{
				byte x = b[pos++];
				v |= (ulong)(x & 0x7F) << shift;
				if ((x & 0x80) == 0)
					return v;
				shift += 7;
			}
		}

		static ulong ZigZag(long v) => unchecked((ulong)((v << 1) ^ (v >> 63)));

		static long UnZigZag(ulong v) => unchecked((long)(v >> 1) ^ -(long)(v & 1));
	}
}
