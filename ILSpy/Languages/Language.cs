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
using System.Diagnostics;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.Languages
{
	/// <summary>
	/// Output language for tree-node labels and decompiled views.
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

		/// <summary>
		/// Writes <paramref name="comment"/> as a single-line comment in this language's syntax.
		/// </summary>
		public virtual void WriteCommentLine(ITextOutput output, string comment)
		{
			output.WriteLine("// " + comment);
		}

		// Default Decompile* implementations write a stub comment so we always produce *something*
		// for symbols whose language doesn't have a meaningful decompilation. Real languages
		// (CSharpLanguage, eventually ILLanguage) override these.

		public virtual void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type));
		}

		public virtual void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(method.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(method.DeclaringTypeDefinition) + "." + method.Name);
		}

		public virtual void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(field.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(field.DeclaringTypeDefinition) + "." + field.Name);
		}

		public virtual void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(property.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(property.DeclaringTypeDefinition) + "." + property.Name);
		}

		public virtual void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(ev.DeclaringTypeDefinition != null);
			WriteCommentLine(output, TypeToString(ev.DeclaringTypeDefinition) + "." + ev.Name);
		}

		public virtual void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, nameSpace);
		}

		public override string ToString() => Name;
	}
}
