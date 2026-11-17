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
using System.Linq;
using System.Reflection;
using System.Text;

using ICSharpCode.Decompiler.CSharp.Syntax;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The shape of the C# syntax tree as the round trip of seam 2 sends it: every concrete node
	/// class (a kind), its child slots in child order and its scalar properties, read from the
	/// generated slot metadata. The engine's <c>csharp/syntax_schema.rs</c> is generated from it.
	/// </summary>
	internal static class SyntaxSchema
	{
		internal sealed class Slot
		{
			public Slot(CSharpSlotInfo info) => Info = info;
			public CSharpSlotInfo Info { get; }
		}

		internal sealed class Scalar
		{
			public Scalar(PropertyInfo property) => Property = property;
			public PropertyInfo Property { get; }
		}

		internal sealed class Kind
		{
			public Kind(int index, Type type, IReadOnlyList<Slot> slots, IReadOnlyList<Scalar> scalars)
			{
				Index = index;
				Type = type;
				Slots = slots;
				Scalars = scalars;
			}

			public int Index { get; }
			public Type Type { get; }
			public IReadOnlyList<Slot> Slots { get; }
			public IReadOnlyList<Scalar> Scalars { get; }

			/// <summary>The index of the per-node slot <paramref name="info"/> in <see cref="Slots"/>.</summary>
			public int SlotIndex(CSharpSlotInfo info)
			{
				for (int i = 0; i < Slots.Count; i++)
				{
					if (Slots[i].Info == info)
						return i;
				}
				throw new InvalidOperationException($"{Type.Name} has no slot {info.Name}");
			}
		}

		static readonly Lazy<(IReadOnlyList<Kind> Kinds, Dictionary<Type, Kind> ByType)> schema = new(Build);

		/// <summary>The kinds, ordered by class name.</summary>
		public static IReadOnlyList<Kind> Kinds => schema.Value.Kinds;

		public static Kind Of(Type type) => schema.Value.ByType[type];

		static (IReadOnlyList<Kind>, Dictionary<Type, Kind>) Build()
		{
			var types = typeof(AstNode).Assembly.GetTypes()
				.Where(t => typeof(AstNode).IsAssignableFrom(t) && !t.IsAbstract && !t.IsGenericTypeDefinition
					&& !t.IsNested && t.Namespace == typeof(AstNode).Namespace)
				.OrderBy(t => t.Name, StringComparer.Ordinal)
				.ToList();
			var kinds = new List<Kind>();
			var byType = new Dictionary<Type, Kind>();
			foreach (var type in types)
			{
				// The generated slot fields, base class first, each class in declaration order.
				var hierarchy = new List<Type>();
				for (var t = type; t != null && t != typeof(object); t = t.BaseType)
					hierarchy.Insert(0, t);
				var slots = new List<Slot>();
				var slotProperties = new HashSet<string>();
				foreach (var t in hierarchy)
				{
					foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
						.Where(f => typeof(CSharpSlotInfo).IsAssignableFrom(f.FieldType) && f.Name.EndsWith("Slot", StringComparison.Ordinal))
						.OrderBy(f => f.MetadataToken))
					{
						var info = (CSharpSlotInfo)field.GetValue(null)!;
						slots.RemoveAll(s => s.Info.Name == info.Name);
						slots.Add(new Slot(info));
						slotProperties.Add(info.Name);
						if (info.Name.EndsWith("Token", StringComparison.Ordinal))
							slotProperties.Add(info.Name.Substring(0, info.Name.Length - "Token".Length));
					}
				}
				var scalars = new List<Scalar>();
				foreach (var t in hierarchy)
				{
					foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
						.Where(p => ((p.CanWrite && p.SetMethod!.IsPublic) || BackingField(p) != null) && p.GetIndexParameters().Length == 0
							&& !slotProperties.Contains(p.Name)
							&& !typeof(AstNode).IsAssignableFrom(p.PropertyType)
							&& !p.PropertyType.Name.StartsWith("AstNodeCollection", StringComparison.Ordinal)
							&& IsState(p))
						.OrderBy(p => p.MetadataToken))
					{
						scalars.RemoveAll(s => s.Property.Name == p.Name);
						scalars.Add(new Scalar(p));
					}
				}
				var kind = new Kind(kinds.Count, type, slots, scalars);
				kinds.Add(kind);
				byType.Add(type, kind);
			}
			return (kinds, byType);
		}

		/// <summary>The compiler's backing field of an auto-property, null for another property.</summary>
		internal static FieldInfo? BackingField(PropertyInfo p)
			=> p.DeclaringType!.GetField($"<{p.Name}>k__BackingField", BindingFlags.NonPublic | BindingFlags.Instance);

		/// <summary>Sets the scalar <paramref name="p"/> of <paramref name="node"/>, through the backing field of a get-only one.</summary>
		internal static void SetScalar(PropertyInfo p, object node, object? value)
		{
			if (p.CanWrite && p.SetMethod!.IsPublic)
				p.SetValue(node, value);
			else
				BackingField(p)!.SetValue(node, value);
		}

		/// <summary>
		/// Whether a public property is state of its own (settable, or get-only and set by a
		/// constructor): an auto-property, the literal value of
		/// <see cref="PrimitiveExpression"/>, an identifier's name or a primitive type's keyword. Other settable properties are views of slots or of other
		/// properties (the <c>Name</c> of a field declaration, for one).
		/// </summary>
		internal static bool IsState(PropertyInfo p)
			=> BackingField(p) != null
				|| (p.DeclaringType == typeof(PrimitiveExpression) && p.Name == nameof(PrimitiveExpression.Value))
				|| (p.DeclaringType == typeof(Identifier) && p.Name == nameof(Identifier.Name))
				|| (p.DeclaringType == typeof(PrimitiveType) && p.Name == nameof(PrimitiveType.Keyword));

		static string SnakeCase(string name)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < name.Length; i++)
			{
				char c = name[i];
				if (char.IsUpper(c) && i > 0 && (char.IsLower(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1]))))
					sb.Append('_');
				sb.Append(char.ToLowerInvariant(c));
			}
			return sb.ToString();
		}

		static string TypeName(Type t)
		{
			var underlying = Nullable.GetUnderlyingType(t);
			return underlying != null ? TypeName(underlying) + "?" : t.Name;
		}

		/// <summary>The engine's <c>csharp/syntax_schema.rs</c>.</summary>
		public static string RustSource()
		{
			var sb = new StringBuilder();
			sb.Append("//! The shape of the C# syntax tree the round trip of seam 2 sends: every node kind with its\n");
			sb.Append("//! child slots (in child order) and scalar properties. Generated from the node classes by\n");
			sb.Append("//! `SyntaxSchemaTests` (`Interop/SyntaxSchema.cs`); do not edit.\n\n");
			sb.Append("pub struct SlotSchema {\n    pub name: &'static str,\n    pub kind: &'static str,\n    pub child: &'static str,\n    pub collection: bool,\n    pub optional: bool,\n}\n\n");
			sb.Append("pub struct ScalarSchema {\n    pub name: &'static str,\n    pub ty: &'static str,\n}\n\n");
			sb.Append("pub struct KindSchema {\n    pub name: &'static str,\n    pub slots: &'static [SlotSchema],\n    pub scalars: &'static [ScalarSchema],\n}\n\n");
			sb.Append("pub const KINDS: &[KindSchema] = &[\n");
			foreach (var k in Kinds)
			{
				sb.Append($"    KindSchema {{\n        name: \"{k.Type.Name}\",\n        slots: &[\n");
				foreach (var s in k.Slots)
				{
					var info = s.Info;
					sb.Append($"            SlotSchema {{ name: \"{info.Name}\", kind: \"{info.Kind?.Name ?? info.Name}\", child: \"{info.ChildType.Name}\", collection: {(info.IsCollection ? "true" : "false")}, optional: {(info.IsOptional ? "true" : "false")} }},\n");
				}
				sb.Append("        ],\n        scalars: &[\n");
				foreach (var s in k.Scalars)
					sb.Append($"            ScalarSchema {{ name: \"{s.Property.Name}\", ty: \"{TypeName(s.Property.PropertyType)}\" }},\n");
				sb.Append("        ],\n    },\n");
			}
			sb.Append("];\n\n");
			sb.Append("/// The kind index of each node class.\npub mod kind {\n");
			foreach (var k in Kinds)
				sb.Append($"    pub const {SnakeCase(k.Type.Name).ToUpperInvariant()}: u16 = {k.Index};\n");
			sb.Append("}\n");
			foreach (var k in Kinds.Where(k => k.Slots.Count > 0 || k.Scalars.Count > 0))
			{
				sb.Append($"\n/// The slot and scalar indices of `{k.Type.Name}`.\npub mod {SnakeCase(k.Type.Name)} {{\n");
				if (k.Slots.Count > 0)
				{
					sb.Append("    pub mod slot {\n");
					for (int i = 0; i < k.Slots.Count; i++)
						sb.Append($"        pub const {SnakeCase(k.Slots[i].Info.Name).ToUpperInvariant()}: usize = {i};\n");
					sb.Append("    }\n");
				}
				if (k.Scalars.Count > 0)
				{
					sb.Append("    pub mod scalar {\n");
					for (int i = 0; i < k.Scalars.Count; i++)
						sb.Append($"        pub const {SnakeCase(k.Scalars[i].Property.Name).ToUpperInvariant()}: usize = {i};\n");
					sb.Append("    }\n");
				}
				sb.Append("}\n");
			}
			return sb.ToString();
		}
	}
}
