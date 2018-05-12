using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public sealed class TupleType
	{
		/// <summary>
		/// Gets the underlying <c>System.ValueType</c> type.
		/// </summary>
		public ParameterizedType UnderlyingType { get; }
		
		public ImmutableArray<IField> TupleElements { get; }
	}
}
