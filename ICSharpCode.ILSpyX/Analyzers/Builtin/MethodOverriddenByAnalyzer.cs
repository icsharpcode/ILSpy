﻿// Copyright (c) 2018 Siegfried Pammer
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

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>
	/// Shows methods that override a method.
	/// </summary>
	[ExportAnalyzer(Header = "Overridden By", Order = 30)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class MethodOverriddenByAnalyzer : IAnalyzer
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IMethod);
			var scope = context.GetScopeOf((IEntity)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in AnalyzeType((IMethod)analyzedSymbol, type))
					yield return result;
			}
		}

		IEnumerable<IEntity> AnalyzeType(IMethod analyzedEntity, ITypeDefinition type)
		{
			if (analyzedEntity.DeclaringTypeDefinition?.ParentModule?.MetadataFile == null)
				yield break;
			var token = analyzedEntity.MetadataToken;
			var declaringTypeToken = analyzedEntity.DeclaringTypeDefinition.MetadataToken;
			var module = analyzedEntity.DeclaringTypeDefinition.ParentModule.MetadataFile;
			var allTypes = type.GetAllBaseTypeDefinitions();
			if (!allTypes.Any(t => t.MetadataToken == declaringTypeToken && t.ParentModule?.MetadataFile == module))
				yield break;

			foreach (var method in type.Methods)
			{
				if (!method.IsOverride)
					continue;
				var baseMembers = InheritanceHelper.GetBaseMembers(method, false);
				if (baseMembers.Any(p => p.MetadataToken == token && p.ParentModule?.MetadataFile == module))
				{
					yield return method;
				}
			}
		}

		public bool Show(ISymbol entity)
		{
			return entity is IMethod method && method.IsOverridable && method.DeclaringType.Kind != TypeKind.Interface;
		}
	}
}
