using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.ILSpy
{
	public class CodeMappingInfo
	{
		public Decompiler.Metadata.PEFile Module { get; }
		public Language Language { get; }
		public TypeDefinitionHandle TypeDefinition { get; }

		Dictionary<MethodDefinitionHandle, List<MethodDefinitionHandle>> parts;
		Dictionary<MethodDefinitionHandle, MethodDefinitionHandle> parents;

		public CodeMappingInfo(Language language, Decompiler.Metadata.PEFile module, TypeDefinitionHandle type)
		{
			this.Language = language;
			this.Module = module;
			this.TypeDefinition = type;
			this.parts = new Dictionary<MethodDefinitionHandle, List<MethodDefinitionHandle>>();
			this.parents = new Dictionary<MethodDefinitionHandle, MethodDefinitionHandle>();
		}

		public bool ShowMember(EntityHandle entity)
		{
			throw null;
			//return Language.ShowMember(new Decompiler.Metadata.Entity(Module, entity));
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
			Debug.Print("Parent: " + MetadataTokens.GetRowNumber(parent) + " Part: " + MetadataTokens.GetRowNumber(part));
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
