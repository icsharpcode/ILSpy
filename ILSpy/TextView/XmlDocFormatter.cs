// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows.Documents;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Xml;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Provides helper methods to create nicely formatted FlowDocuments from NRefactory XmlDoc.
	/// </summary>
	public static class XmlDocFormatter
	{
		public static FlowDocument CreateTooltip(IType type, bool useFullyQualifiedMemberNames = true)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			if (useFullyQualifiedMemberNames)
				ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedEntityNames;
			string header;
			if (type is ITypeDefinition)
				header = ambience.ConvertSymbol((ITypeDefinition)type);
			else
				header = ambience.ConvertType(type);

			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);

			ITypeDefinition entity = type.GetDefinition();/*
			if (entity != null) {
				var documentation = XmlDocumentationElement.Get(entity);
				if (documentation != null) {
					foreach (var child in documentation.Children) {
						b.AddDocumentationElement(child);
					}
				}
			}*/
			return b.CreateFlowDocument();
		}

		public static FlowDocument CreateTooltip(IEntity entity, bool useFullyQualifiedMemberNames = true)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			if (useFullyQualifiedMemberNames)
				ambience.ConversionFlags |= ConversionFlags.UseFullyQualifiedEntityNames;
			string header = ambience.ConvertSymbol(entity);
			var documentation = XmlDocumentationElement.Get(entity);

			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			if (documentation != null) {
				foreach (var child in documentation.Children) {
					b.AddDocumentationElement(child);
				}
			}
			return b.CreateFlowDocument();
		}

		public static FlowDocument CreateTooltip(ISymbol symbol)
		{
			var ambience = AmbienceService.GetCurrentAmbience();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			string header = ambience.ConvertSymbol(symbol);

			if (symbol is IParameter) {
				header = "parameter " + header;
			} else if (symbol is IVariable) {
				header = "local variable " + header;
			}

			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList;
			DocumentationUIBuilder b = new DocumentationUIBuilder(ambience);
			b.AddCodeBlock(header, keepLargeMargin: true);
			return b.CreateFlowDocument();
		}
	}
}
