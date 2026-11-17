// Copyright (c) 2026 Christoph Wille
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

using System.IO;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The oracle answer of the engine's <c>RenameVisualBasicAnonymousTypes</c>
	/// (<c>csharp/transforms/rename_visual_basic_anonymous_types.rs</c>, which documents the kind).
	/// </summary>
	partial class SyntaxRoundTrip
	{
		const byte QueryVisualBasicAnonymousEntity = 210;

		/// <summary>The answer to a query of <c>RenameVisualBasicAnonymousTypes</c>; null where it does not apply.</summary>
		byte[]? AnswerRenameVisualBasicAnonymousTypes(byte[] query)
		{
			int pos = 1;
			int id = (int)ReadUVar(query, ref pos);
			if (id < 0 || id >= symbols.Count || symbols[id] is not IEntity entity)
				return null;
			var declaringType = entity as ITypeDefinition ?? entity.DeclaringTypeDefinition;
			byte flags = 0;
			if (entity is ITypeDefinition)
				flags |= 1;
			if (declaringType != null && declaringType.IsAnonymousTypeDeclaredAsNamedType())
				flags |= 2;
			var s = new MemoryStream();
			s.WriteByte(flags);
			WriteUtf16(s, entity.Name);
			return s.ToArray();
		}
	}
}
