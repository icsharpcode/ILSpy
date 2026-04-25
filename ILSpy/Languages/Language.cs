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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.Languages
{
	/// <summary>
	/// Output language for tree-node labels and (eventually) decompiled views.
	/// Subclasses override formatting for their target syntax. Implementations must be
	/// thread-safe.
	/// </summary>
	public abstract class Language
	{
		public abstract string Name { get; }

		public abstract string FileExtension { get; }

		/// <summary>
		/// Pretty-prints a type for tree nodes / search results.
		/// </summary>
		public virtual string TypeToString(IType type, ConversionFlags conversionFlags = ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames)
		{
			ArgumentNullException.ThrowIfNull(type);
			var ambience = new ILAmbience { ConversionFlags = conversionFlags };
			return ambience.ConvertType(type);
		}

		/// <summary>
		/// Pretty-prints an entity (method/field/property/event signature) for tree nodes.
		/// </summary>
		public virtual string EntityToString(IEntity entity, ConversionFlags conversionFlags)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var ambience = new ILAmbience {
				ConversionFlags = ConversionFlags.ShowTypeParameterList
					| ConversionFlags.PlaceReturnTypeAfterParameterList
					| ConversionFlags.ShowReturnType
					| ConversionFlags.ShowParameterList
					| ConversionFlags.ShowParameterModifiers
					| conversionFlags
			};
			return ambience.ConvertSymbol(entity);
		}

		public override string ToString() => Name;
	}
}
