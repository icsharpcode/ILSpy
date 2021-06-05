// Copyright (c) 2014 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// A type for the purpose of stack analysis.
	/// </summary>
	public enum StackType : byte
	{
		// Note: the numeric of these enum members is relevant for ILReader.MergeStacks:
		// when two branches meet where a stack slot has different types, the type after
		// the branch is the one with the higher numeric value.

		/// <summary>
		/// The stack type is unknown; for example a call returning an unknown type
		/// because an assembly reference isn't loaded.
		/// Can also occur with invalid IL.
		/// </summary>
		Unknown,
		/// <summary>32-bit integer</summary>
		/// <remarks>
		/// Used for C# <c>int</c>, <c>uint</c>,
		/// C# small integer types <c>byte</c>, <c>sbyte</c>, <c>short</c>, <c>ushort</c>,
		/// <c>bool</c> and <c>char</c>,
		/// and any enums with one of the above as underlying type.
		/// </remarks>
		I4,
		/// <summary>native-size integer, or unmanaged pointer</summary>
		/// <remarks>
		/// Used for C# <c>IntPtr</c>, <c>UIntPtr</c> and any native pointer types (<c>void*</c> etc.)
		/// Also used for IL function pointer types.
		/// </remarks>
		I,
		/// <summary>64-bit integer</summary>
		/// <remarks>
		/// Used for C# <c>long</c>, <c>ulong</c>,
		/// and any enums with one of the above as underlying type.
		/// </remarks>
		I8,
		/// <summary>32-bit floating point number</summary>
		/// <remarks>
		/// Used for C# <c>float</c>.
		/// </remarks>
		F4,
		/// <summary>64-bit floating point number</summary>
		/// <remarks>
		/// Used for C# <c>double</c>.
		/// </remarks>
		F8,
		/// <summary>Another stack type. Includes objects, value types, ...</summary>
		O,
		/// <summary>A managed pointer</summary>
		Ref,
		/// <summary>Represents the lack of a stack slot</summary>
		Void
	}
}
