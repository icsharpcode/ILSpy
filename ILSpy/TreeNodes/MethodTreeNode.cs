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
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	sealed class MethodTreeNode : ILSpyTreeNode
	{
		public IMethod MethodDefinition { get; }

		public MethodTreeNode(IMethod method)
		{
			MethodDefinition = method ?? throw new ArgumentNullException(nameof(method));
		}

		public override object Text => Language.EntityToString(MethodDefinition, ConversionFlags.None);

		public override object Icon {
			get {
				var baseImage = MethodDefinition.IsConstructor ? Images.Images.Constructor :
					MethodDefinition.IsOperator ? Images.Images.Operator :
					Images.Images.Method;
				return Images.Images.GetIcon(baseImage,
					Images.Images.GetOverlay(MethodDefinition.Accessibility),
					MethodDefinition.IsStatic,
					MethodDefinition.IsExtensionMethod);
			}
		}

		public override bool ShowExpander => false;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			=> language.DecompileMethod(MethodDefinition, output, options);

		// Stable identity for SessionSettings.ActiveTreeViewPath; matches the WPF host's format.
		public override string ToString()
			=> "Method " + new ICSharpCode.Decompiler.IL.ILAmbience {
				ConversionFlags = ConversionFlags.ShowTypeParameterList
					| ConversionFlags.PlaceReturnTypeAfterParameterList
					| ConversionFlags.ShowReturnType
					| ConversionFlags.ShowParameterList
					| ConversionFlags.ShowParameterModifiers,
			}.ConvertSymbol(MethodDefinition);
	}
}
