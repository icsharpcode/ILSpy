using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public enum Nullability : byte
	{
		Oblivious = 0,
		NotNullable = 1,
		Nullable = 2
	}
}
