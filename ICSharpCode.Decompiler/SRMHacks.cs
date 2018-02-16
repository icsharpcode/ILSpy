using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler
{
	public static partial class SRMExtensions
	{
		public static unsafe MethodSemanticsAttributes GetMethodSemanticsAttributes(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			int rowCount = reader.GetTableRowCount(TableIndex.MethodSemantics);
			var small = reader.IsSmallReference(TableIndex.MethodDef);

			int methodRowNo = reader.GetRowNumber(handle);
			for (int row = rowCount - 1; row >= 0; row--) {
				byte* ptr = startPointer + offset + rowSize * row;
				uint rowNo = small ? *(ushort*)(ptr + 2) : *(uint*)(ptr + 2);
				if (methodRowNo == rowNo) {
					return (MethodSemanticsAttributes)(*(ushort*)ptr);
				}
			}
			return 0;
		}

		public static unsafe ImmutableArray<MethodImplementationHandle> GetMethodImplementations(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodImpl);
			int rowSize = reader.GetTableRowSize(TableIndex.MethodImpl);
			int rowCount = reader.GetTableRowCount(TableIndex.MethodImpl);
			var methodDefSize = reader.GetReferenceSize(TableIndex.MethodDef);
			var typeDefSize = reader.GetReferenceSize(TableIndex.TypeDef);

			var containingTypeRow = reader.GetRowNumber(reader.GetMethodDefinition(handle).GetDeclaringType());
			var methodDefRow = reader.GetRowNumber(handle);

			var list = new List<MethodImplementationHandle>();

			// TODO : if sorted -> binary search?
			for (int row = 0; row < reader.GetTableRowCount(TableIndex.MethodImpl); row++) {
				byte* ptr = startPointer + offset + rowSize * row;
				uint currentTypeRow = typeDefSize == 2 ? *(ushort*)ptr : *(uint*)ptr;
				if (currentTypeRow != containingTypeRow) continue;
				uint currentMethodRowCoded = methodDefSize == 2 ? *(ushort*)(ptr + typeDefSize) : *(uint*)(ptr + typeDefSize);
				if ((currentMethodRowCoded >> 1) != methodDefRow) continue;
				list.Add(MetadataTokens.MethodImplementationHandle(row + 1));
			}

			return list.ToImmutableArray();
		}
	}
}
