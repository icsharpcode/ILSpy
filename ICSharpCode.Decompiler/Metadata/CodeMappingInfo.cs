using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.Decompiler.Metadata
{
	public class CodeMappingInfo
	{
		public PEFile Module { get; }
		public TypeDefinitionHandle TypeDefinition { get; }

		Dictionary<MethodDefinitionHandle, List<MethodDefinitionHandle>> parts;
		Dictionary<MethodDefinitionHandle, MethodDefinitionHandle> parents;

		public CodeMappingInfo(PEFile module, TypeDefinitionHandle type)
		{
			this.Module = module;
			this.TypeDefinition = type;
			this.parts = new Dictionary<MethodDefinitionHandle, List<MethodDefinitionHandle>>();
			this.parents = new Dictionary<MethodDefinitionHandle, MethodDefinitionHandle>();
		}

		public IEnumerable<MethodDefinitionHandle> GetMethodParts(MethodDefinitionHandle method)
		{
			if (parts.TryGetValue(method, out var p))
				return p;
			return new[] { method };
		}

		public MethodDefinitionHandle GetParentMethod(MethodDefinitionHandle method)
		{
			if (parents.TryGetValue(method, out var p))
				return p;
			return method;
		}

		public void AddMapping(MethodDefinitionHandle parent, MethodDefinitionHandle part)
		{
			//Debug.Print("Parent: " + MetadataTokens.GetRowNumber(parent) + " Part: " + MetadataTokens.GetRowNumber(part));
			if (parents.ContainsKey(part))
				return;
			parents.Add(part, parent);
			if (!parts.TryGetValue(parent, out var list)) {
				list = new List<MethodDefinitionHandle>();
				parts.Add(parent, list);
			}
			list.Add(part);
		}
	}
}
