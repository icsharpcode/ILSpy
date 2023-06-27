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

#nullable enable

using System.Diagnostics.CodeAnalysis;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Represents a property or indexer.
	/// </summary>
	public interface IProperty : IParameterizedMember
	{
		[MemberNotNullWhen(true, nameof(Getter))]
		bool CanGet { get; }
		[MemberNotNullWhen(true, nameof(Setter))]
		bool CanSet { get; }

		IMethod? Getter { get; }
		IMethod? Setter { get; }

		bool IsIndexer { get; }

		/// <summary>
		/// Gets whether the return type is 'ref readonly'.
		/// </summary>
		bool ReturnTypeIsRefReadOnly { get; }
	}
}
