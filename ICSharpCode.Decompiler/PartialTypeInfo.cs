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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	public class PartialTypeInfo
	{
		readonly HashSet<EntityHandle> declaredMembers = new();

		public PartialTypeInfo(ITypeDefinition declaringTypeDefinition)
		{
			DeclaringTypeDefinitionHandle = (TypeDefinitionHandle)declaringTypeDefinition.MetadataToken;
		}

		public PartialTypeInfo(TypeDefinitionHandle declaringTypeDefinitionHandle)
		{
			DeclaringTypeDefinitionHandle = declaringTypeDefinitionHandle;
		}

		public TypeDefinitionHandle DeclaringTypeDefinitionHandle { get; }

		/// <summary>
		/// The document this type is the code-behind of, as a project-relative path
		/// ("Views/MainWindow.xaml"), where there is one. The project decompiler names the type's
		/// C# file after it, so that the two sit next to each other the way the tooling expects.
		/// </summary>
		public string? CompanionFileName { get; set; }

		public void AddDeclaredMember(IMember member)
		{
			declaredMembers.Add(member.MetadataToken);
		}

		public void AddDeclaredMember(EntityHandle handle)
		{
			declaredMembers.Add(handle);
		}

		public bool IsDeclaredMember(IMember member)
		{
			return declaredMembers.Contains(member.MetadataToken);
		}

		public bool IsDeclaredMember(EntityHandle handle)
		{
			return declaredMembers.Contains(handle);
		}

		public void AddDeclaredMembers(PartialTypeInfo info)
		{
			foreach (var member in info.declaredMembers)
			{
				declaredMembers.Add(member);
			}
		}

		public string DebugOutput => string.Join(", ", declaredMembers.Select(m => MetadataTokens.GetToken(m).ToString("X")));
	}
}