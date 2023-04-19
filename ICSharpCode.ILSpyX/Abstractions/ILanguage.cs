// Copyright (c) 2022 Siegfried Pammer
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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Abstractions
{
	public interface ILanguage
	{
		bool ShowMember(IEntity member);
		string GetEntityName(PEFile module, System.Reflection.Metadata.EntityHandle handle, bool fullName, bool omitGenerics);
		string GetTooltip(IEntity entity);

		string TypeToString(IType type, bool includeNamespace);
		string MethodToString(IMethod method, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName);
		string FieldToString(IField field, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName);
		string PropertyToString(IProperty property, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName);
		string EventToString(IEvent @event, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName);
	}
}
