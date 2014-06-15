using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	enum PrimitiveType : byte
	{
		None = 0,
		I1 = MetadataType.SByte,
		I2 = MetadataType.Int16,
		I4 = MetadataType.Int32,
		I8 = MetadataType.Int64,
		R4 = MetadataType.Single,
		R8 = MetadataType.Double,
		U1 = MetadataType.Byte,
		U2 = MetadataType.UInt16,
		U4 = MetadataType.UInt32,
		U8 = MetadataType.UInt64,
		I = MetadataType.IntPtr,
		U = MetadataType.UIntPtr,
	}
}
