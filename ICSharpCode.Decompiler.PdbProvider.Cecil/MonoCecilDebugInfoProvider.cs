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

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

using Mono.Cecil;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.PdbProvider.Cecil
{
	public class MonoCecilDebugInfoProvider : IDebugInfoProvider
	{
		ModuleDefinition module;

		public MonoCecilDebugInfoProvider(ModuleDefinition module)
		{
			this.module = module;
		}

		public IList<SequencePoint> GetSequencePoints(SRM.MethodDefinitionHandle handle)
		{
			var method = this.module.LookupToken(MetadataTokens.GetToken(handle)) as Mono.Cecil.MethodDefinition;
			if (method?.DebugInformation == null || !method.DebugInformation.HasSequencePoints)
				return EmptyList<SequencePoint>.Instance;
			return method.DebugInformation.SequencePoints.Select(point => new Metadata.SequencePoint {
				Offset = point.Offset,
				StartLine = point.StartLine,
				StartColumn = point.StartColumn,
				EndLine = point.EndLine,
				EndColumn = point.EndColumn,
				DocumentUrl = point.Document.Url
			}).ToList();
		}

		public IList<Variable> GetVariables(SRM.MethodDefinitionHandle handle)
		{
			var method = this.module.LookupToken(MetadataTokens.GetToken(handle)) as Mono.Cecil.MethodDefinition;
			if (method?.DebugInformation == null)
				return EmptyList<Variable>.Instance;
			return method.DebugInformation.GetScopes()
				.SelectMany(s => s.Variables)
				.Select(v => new Variable { Name = v.Name }).ToList();
		}

		public bool TryGetName(SRM.MethodDefinitionHandle handle, int index, out string name)
		{
			var method = this.module.LookupToken(MetadataTokens.GetToken(handle)) as Mono.Cecil.MethodDefinition;
			name = null;
			if (method?.DebugInformation == null || !method.HasBody)
				return false;
			var variable = method.Body.Variables.FirstOrDefault(v => v.Index == index);
			if (variable == null)
				return false;
			return method.DebugInformation.TryGetName(variable, out name);
		}
	}
}
