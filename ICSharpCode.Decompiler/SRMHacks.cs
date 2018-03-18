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

		public static ImmutableArray<MethodImplementationHandle> GetMethodImplementations(this MethodDefinitionHandle handle, MetadataReader reader)
		{
			var resultBuilder = ImmutableArray.CreateBuilder<MethodImplementationHandle>();
			var typeDefinition = reader.GetTypeDefinition(reader.GetMethodDefinition(handle).GetDeclaringType());

			foreach (var methodImplementationHandle in typeDefinition.GetMethodImplementations()) {
				var methodImplementation = reader.GetMethodImplementation(methodImplementationHandle);
				if (methodImplementation.MethodBody == handle) {
					resultBuilder.Add(methodImplementationHandle);
				}
			}

			return resultBuilder.ToImmutable();
		}

		/*
		internal static unsafe ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)> GetAccessors(PEFile module, uint encodedTag)
		{
			var reader = module.GetMetadataReader();
			byte* startPointer = reader.MetadataPointer;
			int offset = reader.GetTableMetadataOffset(TableIndex.MethodSemantics);

			var methodDefRefSize = reader.GetReferenceSize(TableIndex.MethodDef);
			(int startRow, int endRow) = reader.BinarySearchRange(TableIndex.MethodSemantics, 2 + methodDefRefSize, encodedTag, reader.IsSmallReference(TableIndex.MethodSemantics));
			if (startRow == -1)
				return ImmutableArray<(MethodSemanticsAttributes Kind, MethodDefinition Method)>.Empty;
			var methods = new(MethodSemanticsAttributes Kind, MethodDefinition Method)[endRow - startRow + 1];
			int rowSize = reader.GetTableRowSize(TableIndex.MethodSemantics);
			for (int row = startRow; row <= endRow; row++) {
				int rowOffset = row * rowSize;
				byte* ptr = startPointer + offset + rowOffset;
				var kind = (MethodSemanticsAttributes)(*(ushort*)ptr);
				uint rowNo = methodDefRefSize == 2 ? *(ushort*)(ptr + 2) : *(uint*)(ptr + 2);
				var handle = MetadataTokens.MethodDefinitionHandle((int)rowNo);
				methods[row - startRow] = (kind, new MethodDefinition(module, handle));
			}
			return methods.ToImmutableArray();
		}
		*/
	}
}
