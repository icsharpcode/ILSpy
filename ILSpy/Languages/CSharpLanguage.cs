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
using System.Composition;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ConversionFlags = ICSharpCode.Decompiler.Output.ConversionFlags;

namespace ILSpy.Languages
{
	[Export(typeof(Language))]
	[Shared]
	public sealed class CSharpLanguage : Language
	{
		public override string Name => "C#";

		public override string FileExtension => ".cs";

		static CSharpAmbience CreateAmbience() => new() {
			ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.PlaceReturnTypeAfterParameterList,
		};

		public override string TypeToString(IType type, ConversionFlags conversionFlags = ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.UseFullyQualifiedTypeNames)
		{
			ArgumentNullException.ThrowIfNull(type);
			var ambience = CreateAmbience();
			ambience.ConversionFlags |= conversionFlags;
			return type is ITypeDefinition def
				? ambience.ConvertSymbol(def)
				: ambience.ConvertType(type);
		}

		public override string EntityToString(IEntity entity, ConversionFlags conversionFlags)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var ambience = CreateAmbience();
			ambience.ConversionFlags |= conversionFlags
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			return ambience.ConvertSymbol(entity);
		}
	}
}
