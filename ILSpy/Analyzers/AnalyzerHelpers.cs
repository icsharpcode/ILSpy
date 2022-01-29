// Copyright (c) 2018 Siegfried Pammer
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

using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Analyzers
{
	using ICSharpCode.Decompiler.TypeSystem;

	internal static class AnalyzerHelpers
	{
		public static bool IsPossibleReferenceTo(EntityHandle member, PEFile module, IMethod analyzedMethod)
		{
			if (member.IsNil)
				return false;
			MetadataReader metadata = module.Metadata;
			switch (member.Kind)
			{
				case HandleKind.MethodDefinition:
					return member == analyzedMethod.MetadataToken
						&& module == analyzedMethod.ParentModule.PEFile;
				case HandleKind.MemberReference:
					var mr = metadata.GetMemberReference((MemberReferenceHandle)member);
					if (mr.GetKind() != MemberReferenceKind.Method)
						return false;
					return metadata.StringComparer.Equals(mr.Name, analyzedMethod.Name);
				case HandleKind.MethodSpecification:
					var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)member);
					return IsPossibleReferenceTo(ms.Method, module, analyzedMethod);
				default:
					return false;
			}
		}
	}
}
