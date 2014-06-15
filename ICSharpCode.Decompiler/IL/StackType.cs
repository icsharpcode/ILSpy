using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A type for the purpose of stack analysis.
	/// </summary>
	enum StackType
	{
		Unknown,
		/// <summary>32-bit integer</summary>
		I4,
		/// <summary>64-bit integer</summary>
		I8,
		/// <summary>native-size integer</summary>
		I,
		/// <summary>Floating point number</summary>
		F,
		/// <summary>Another stack type. Includes objects, value types, function pointers, ...</summary>
		O,
		/// <summary>A managed pointer</summary>
		Ref,
		/// <summary>Represents the lack of a stack slot</summary>
		Void
	}
}
