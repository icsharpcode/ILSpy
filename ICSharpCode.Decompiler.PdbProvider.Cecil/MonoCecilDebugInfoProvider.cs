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
