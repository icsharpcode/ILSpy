// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;

namespace ICSharpCode.ILSpy.Metadata.Filters
{
	/// <summary>
	/// Static description of how a <see cref="FlagsAttribute"/> enum's bits decompose into
	/// mutually-exclusive sub-ranges (mutex groups carved out by <c>*Mask</c> fields) and
	/// independent single-bit flags. The filter UI renders from this shape; the matcher
	/// applies it bitwise.
	/// </summary>
	public sealed record FlagsSchema(
		Type EnumType,
		IReadOnlyList<MutexGroup> MutexGroups,
		IReadOnlyList<IndependentFlag> IndependentFlags);

	/// <summary>
	/// One sub-range of an enum carved out by a <c>*Mask</c> field. A row's value carries
	/// exactly one of <see cref="Values"/>, isolated by ANDing with <see cref="Mask"/>.
	/// </summary>
	public sealed record MutexGroup(
		string Name,
		uint Mask,
		IReadOnlyList<MutexValue> Values);

	public sealed record MutexValue(string Label, uint Value);

	/// <summary>
	/// One single-bit flag whose presence is independent of any mutex group. Filtering on
	/// these is tri-state (don't care / required / excluded).
	/// </summary>
	public sealed record IndependentFlag(string Name, uint Bit);
}
