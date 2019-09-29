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

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents some well-known types.
	/// </summary>
	public enum KnownTypeCode
	{
		// Note: DefaultResolvedTypeDefinition uses (KnownTypeCode)-1 as special value for "not yet calculated".
		// The order of type codes at the beginning must correspond to those in System.TypeCode.
		
		/// <summary>
		/// Not one of the known types.
		/// </summary>
		None,
		/// <summary><c>object</c> (System.Object)</summary>
		Object,
		/// <summary><c>System.DBNull</c></summary>
		DBNull,
		/// <summary><c>bool</c> (System.Boolean)</summary>
		Boolean,
		/// <summary><c>char</c> (System.Char)</summary>
		Char,
		/// <summary><c>sbyte</c> (System.SByte)</summary>
		SByte,
		/// <summary><c>byte</c> (System.Byte)</summary>
		Byte,
		/// <summary><c>short</c> (System.Int16)</summary>
		Int16,
		/// <summary><c>ushort</c> (System.UInt16)</summary>
		UInt16,
		/// <summary><c>int</c> (System.Int32)</summary>
		Int32,
		/// <summary><c>uint</c> (System.UInt32)</summary>
		UInt32,
		/// <summary><c>long</c> (System.Int64)</summary>
		Int64,
		/// <summary><c>ulong</c> (System.UInt64)</summary>
		UInt64,
		/// <summary><c>float</c> (System.Single)</summary>
		Single,
		/// <summary><c>double</c> (System.Double)</summary>
		Double,
		/// <summary><c>decimal</c> (System.Decimal)</summary>
		Decimal,
		/// <summary><c>System.DateTime</c></summary>
		DateTime,
		/// <summary><c>string</c> (System.String)</summary>
		String = 18,
		
		// String was the last element from System.TypeCode, now our additional known types start
		
		/// <summary><c>void</c> (System.Void)</summary>
		Void,
		/// <summary><c>System.Type</c></summary>
		Type,
		/// <summary><c>System.Array</c></summary>
		Array,
		/// <summary><c>System.Attribute</c></summary>
		Attribute,
		/// <summary><c>System.ValueType</c></summary>
		ValueType,
		/// <summary><c>System.Enum</c></summary>
		Enum,
		/// <summary><c>System.Delegate</c></summary>
		Delegate,
		/// <summary><c>System.MulticastDelegate</c></summary>
		MulticastDelegate,
		/// <summary><c>System.Exception</c></summary>
		Exception,
		/// <summary><c>System.IntPtr</c></summary>
		IntPtr,
		/// <summary><c>System.UIntPtr</c></summary>
		UIntPtr,
		/// <summary><c>System.Collections.IEnumerable</c></summary>
		IEnumerable,
		/// <summary><c>System.Collections.IEnumerator</c></summary>
		IEnumerator,
		/// <summary><c>System.Collections.Generic.IEnumerable{T}</c></summary>
		IEnumerableOfT,
		/// <summary><c>System.Collections.Generic.IEnumerator{T}</c></summary>
		IEnumeratorOfT,
		/// <summary><c>System.Collections.Generic.ICollection</c></summary>
		ICollection,
		/// <summary><c>System.Collections.Generic.ICollection{T}</c></summary>
		ICollectionOfT,
		/// <summary><c>System.Collections.Generic.IList</c></summary>
		IList,
		/// <summary><c>System.Collections.Generic.IList{T}</c></summary>
		IListOfT,
		/// <summary><c>System.Collections.Generic.IReadOnlyCollection{T}</c></summary>
		IReadOnlyCollectionOfT,
		/// <summary><c>System.Collections.Generic.IReadOnlyList{T}</c></summary>
		IReadOnlyListOfT,
		/// <summary><c>System.Threading.Tasks.Task</c></summary>
		Task,
		/// <summary><c>System.Threading.Tasks.Task{T}</c></summary>
		TaskOfT,
		/// <summary><c>System.Threading.Tasks.ValueTask</c></summary>
		ValueTask,
		/// <summary><c>System.Threading.Tasks.ValueTask{T}</c></summary>
		ValueTaskOfT,
		/// <summary><c>System.Nullable{T}</c></summary>
		NullableOfT,
		/// <summary><c>System.IDisposable</c></summary>
		IDisposable,
		/// <summary><c>System.IAsyncDisposable</c></summary>
		IAsyncDisposable,
		/// <summary><c>System.Runtime.CompilerServices.INotifyCompletion</c></summary>
		INotifyCompletion,
		/// <summary><c>System.Runtime.CompilerServices.ICriticalNotifyCompletion</c></summary>
		ICriticalNotifyCompletion,
		/// <summary><c>System.TypedReference</c></summary>
		TypedReference,
		/// <summary><c>System.IFormattable</c></summary>
		IFormattable,
		/// <summary><c>System.FormattableString</c></summary>
		FormattableString,
		/// <summary><c>System.Span{T}</c></summary>
		SpanOfT,
		/// <summary><c>System.ReadOnlySpan{T}</c></summary>
		ReadOnlySpanOfT,
		/// <summary><c>System.Memory{T}</c></summary>
		MemoryOfT,
		/// <summary><c>System.Runtime.CompilerServices.Unsafe</c></summary>
		Unsafe,
		/// <summary><c>System.Collections.Generic.IAsyncEnumerable{T}</c></summary>
		IAsyncEnumerableOfT,
		/// <summary><c>System.Collections.Generic.IAsyncEnumerator{T}</c></summary>
		IAsyncEnumeratorOfT,
	}

	/// <summary>
	/// Contains well-known type references.
	/// </summary>
	[Serializable]
	public sealed class KnownTypeReference : ITypeReference
	{
		internal const int KnownTypeCodeCount = (int)KnownTypeCode.IAsyncEnumeratorOfT + 1;

		static readonly KnownTypeReference[] knownTypeReferences = new KnownTypeReference[KnownTypeCodeCount] {
			null, // None
			new KnownTypeReference(KnownTypeCode.Object,   TypeKind.Class, "System", "Object", baseType: KnownTypeCode.None),
			new KnownTypeReference(KnownTypeCode.DBNull,   TypeKind.Class, "System", "DBNull"),
			new KnownTypeReference(KnownTypeCode.Boolean,  TypeKind.Struct, "System", "Boolean"),
			new KnownTypeReference(KnownTypeCode.Char,     TypeKind.Struct, "System", "Char"),
			new KnownTypeReference(KnownTypeCode.SByte,    TypeKind.Struct, "System", "SByte"),
			new KnownTypeReference(KnownTypeCode.Byte,     TypeKind.Struct, "System", "Byte"),
			new KnownTypeReference(KnownTypeCode.Int16,    TypeKind.Struct, "System", "Int16"),
			new KnownTypeReference(KnownTypeCode.UInt16,   TypeKind.Struct, "System", "UInt16"),
			new KnownTypeReference(KnownTypeCode.Int32,    TypeKind.Struct, "System", "Int32"),
			new KnownTypeReference(KnownTypeCode.UInt32,   TypeKind.Struct, "System", "UInt32"),
			new KnownTypeReference(KnownTypeCode.Int64,    TypeKind.Struct, "System", "Int64"),
			new KnownTypeReference(KnownTypeCode.UInt64,   TypeKind.Struct, "System", "UInt64"),
			new KnownTypeReference(KnownTypeCode.Single,   TypeKind.Struct, "System", "Single"),
			new KnownTypeReference(KnownTypeCode.Double,   TypeKind.Struct, "System", "Double"),
			new KnownTypeReference(KnownTypeCode.Decimal,  TypeKind.Struct, "System", "Decimal"),
			new KnownTypeReference(KnownTypeCode.DateTime, TypeKind.Struct, "System", "DateTime"),
			null,
			new KnownTypeReference(KnownTypeCode.String,    TypeKind.Class, "System", "String"),
			new KnownTypeReference(KnownTypeCode.Void,      TypeKind.Void,  "System", "Void", baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Type,      TypeKind.Class, "System", "Type"),
			new KnownTypeReference(KnownTypeCode.Array,     TypeKind.Class, "System", "Array"),
			new KnownTypeReference(KnownTypeCode.Attribute, TypeKind.Class, "System", "Attribute"),
			new KnownTypeReference(KnownTypeCode.ValueType, TypeKind.Class, "System", "ValueType"),
			new KnownTypeReference(KnownTypeCode.Enum,      TypeKind.Class, "System", "Enum", baseType: KnownTypeCode.ValueType),
			new KnownTypeReference(KnownTypeCode.Delegate,  TypeKind.Class, "System", "Delegate"),
			new KnownTypeReference(KnownTypeCode.MulticastDelegate, TypeKind.Class, "System", "MulticastDelegate", baseType: KnownTypeCode.Delegate),
			new KnownTypeReference(KnownTypeCode.Exception, TypeKind.Class, "System", "Exception"),
			new KnownTypeReference(KnownTypeCode.IntPtr,    TypeKind.Struct, "System", "IntPtr"),
			new KnownTypeReference(KnownTypeCode.UIntPtr,   TypeKind.Struct, "System", "UIntPtr"),
			new KnownTypeReference(KnownTypeCode.IEnumerable,    TypeKind.Interface, "System.Collections", "IEnumerable"),
			new KnownTypeReference(KnownTypeCode.IEnumerator,    TypeKind.Interface, "System.Collections", "IEnumerator"),
			new KnownTypeReference(KnownTypeCode.IEnumerableOfT, TypeKind.Interface, "System.Collections.Generic", "IEnumerable", 1),
			new KnownTypeReference(KnownTypeCode.IEnumeratorOfT, TypeKind.Interface, "System.Collections.Generic", "IEnumerator", 1),
			new KnownTypeReference(KnownTypeCode.ICollection,    TypeKind.Interface, "System.Collections", "ICollection"),
			new KnownTypeReference(KnownTypeCode.ICollectionOfT, TypeKind.Interface, "System.Collections.Generic", "ICollection", 1),
			new KnownTypeReference(KnownTypeCode.IList,          TypeKind.Interface, "System.Collections", "IList"),
			new KnownTypeReference(KnownTypeCode.IListOfT,       TypeKind.Interface, "System.Collections.Generic", "IList", 1),

			new KnownTypeReference(KnownTypeCode.IReadOnlyCollectionOfT, TypeKind.Interface, "System.Collections.Generic", "IReadOnlyCollection", 1),
			new KnownTypeReference(KnownTypeCode.IReadOnlyListOfT, TypeKind.Interface, "System.Collections.Generic", "IReadOnlyList", 1),
			new KnownTypeReference(KnownTypeCode.Task,         TypeKind.Class, "System.Threading.Tasks", "Task"),
			new KnownTypeReference(KnownTypeCode.TaskOfT,      TypeKind.Class, "System.Threading.Tasks", "Task", 1, baseType: KnownTypeCode.Task),
			new KnownTypeReference(KnownTypeCode.ValueTask,    TypeKind.Struct, "System.Threading.Tasks", "ValueTask"),
			new KnownTypeReference(KnownTypeCode.ValueTaskOfT, TypeKind.Struct, "System.Threading.Tasks", "ValueTask", 1),
			new KnownTypeReference(KnownTypeCode.NullableOfT, TypeKind.Struct, "System", "Nullable", 1),
			new KnownTypeReference(KnownTypeCode.IDisposable, TypeKind.Interface, "System", "IDisposable"),
			new KnownTypeReference(KnownTypeCode.IAsyncDisposable, TypeKind.Interface, "System", "IAsyncDisposable"),
			new KnownTypeReference(KnownTypeCode.INotifyCompletion, TypeKind.Interface, "System.Runtime.CompilerServices", "INotifyCompletion"),
			new KnownTypeReference(KnownTypeCode.ICriticalNotifyCompletion, TypeKind.Interface, "System.Runtime.CompilerServices", "ICriticalNotifyCompletion"),

			new KnownTypeReference(KnownTypeCode.TypedReference, TypeKind.Struct, "System", "TypedReference"),
			new KnownTypeReference(KnownTypeCode.IFormattable, TypeKind.Interface, "System", "IFormattable"),
			new KnownTypeReference(KnownTypeCode.FormattableString, TypeKind.Class, "System", "FormattableString", baseType: KnownTypeCode.IFormattable),
			new KnownTypeReference(KnownTypeCode.SpanOfT, TypeKind.Struct, "System", "Span", 1),
			new KnownTypeReference(KnownTypeCode.ReadOnlySpanOfT, TypeKind.Struct, "System", "ReadOnlySpan", 1),
			new KnownTypeReference(KnownTypeCode.MemoryOfT, TypeKind.Struct, "System", "Memory", 1),
			new KnownTypeReference(KnownTypeCode.Unsafe, TypeKind.Class, "System.Runtime.CompilerServices", "Unsafe", 0),
			new KnownTypeReference(KnownTypeCode.IAsyncEnumerableOfT, TypeKind.Interface, "System.Collections.Generic", "IAsyncEnumerable", 1),
			new KnownTypeReference(KnownTypeCode.IAsyncEnumeratorOfT, TypeKind.Interface, "System.Collections.Generic", "IAsyncEnumerator", 1),
		};
		
		/// <summary>
		/// Gets the known type reference for the specified type code.
		/// Returns null for KnownTypeCode.None.
		/// </summary>
		public static KnownTypeReference Get(KnownTypeCode typeCode)
		{
			return knownTypeReferences[(int)typeCode];
		}
		
		readonly KnownTypeCode knownTypeCode;
		readonly string namespaceName;
		readonly string name;
		readonly int typeParameterCount;
		internal readonly KnownTypeCode baseType;
		internal readonly TypeKind typeKind;

		private KnownTypeReference(KnownTypeCode knownTypeCode, TypeKind typeKind, string namespaceName, string name, int typeParameterCount = 0, KnownTypeCode baseType = KnownTypeCode.Object)
		{
			if (typeKind == TypeKind.Struct && baseType == KnownTypeCode.Object)
				baseType = KnownTypeCode.ValueType;
			this.knownTypeCode = knownTypeCode;
			this.namespaceName = namespaceName;
			this.name = name;
			this.typeParameterCount = typeParameterCount;
			this.typeKind = typeKind;
			this.baseType = baseType;
		}
		
		public KnownTypeCode KnownTypeCode {
			get { return knownTypeCode; }
		}
		
		public string Namespace {
			get { return namespaceName; }
		}
		
		public string Name {
			get { return name; }
		}
		
		public int TypeParameterCount {
			get { return typeParameterCount; }
		}

		public TopLevelTypeName TypeName => new TopLevelTypeName(namespaceName, name, typeParameterCount);

		public IType Resolve(ITypeResolveContext context)
		{
			return context.Compilation.FindType(knownTypeCode);
		}
		
		public override string ToString()
		{
			return GetCSharpNameByTypeCode(knownTypeCode) ?? (this.Namespace + "." + this.Name);
		}
		
		/// <summary>
		/// Gets the C# primitive type name from the known type code.
		/// Returns null if there is no primitive name for the specified type.
		/// </summary>
		public static string GetCSharpNameByTypeCode(KnownTypeCode knownTypeCode)
		{
			switch (knownTypeCode) {
				case KnownTypeCode.Object:
					return "object";
				case KnownTypeCode.Boolean:
					return "bool";
				case KnownTypeCode.Char:
					return "char";
				case KnownTypeCode.SByte:
					return "sbyte";
				case KnownTypeCode.Byte:
					return "byte";
				case KnownTypeCode.Int16:
					return "short";
				case KnownTypeCode.UInt16:
					return "ushort";
				case KnownTypeCode.Int32:
					return "int";
				case KnownTypeCode.UInt32:
					return "uint";
				case KnownTypeCode.Int64:
					return "long";
				case KnownTypeCode.UInt64:
					return "ulong";
				case KnownTypeCode.Single:
					return "float";
				case KnownTypeCode.Double:
					return "double";
				case KnownTypeCode.Decimal:
					return "decimal";
				case KnownTypeCode.String:
					return "string";
				case KnownTypeCode.Void:
					return "void";
				default:
					return null;
			}
		}
	}
}
