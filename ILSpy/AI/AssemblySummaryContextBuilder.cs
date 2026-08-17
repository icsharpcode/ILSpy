// Copyright (c) 2026 Masroor
using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.AI
{
	public static class AssemblySummaryContextBuilder
	{
		public static string Build(LoadedAssembly assembly)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			MetadataFile? metadataFile = assembly.GetMetadataFileOrNull();
			ICompilation? compilation = assembly.GetTypeSystemOrNull();
			if (metadataFile is null || compilation is null)
				throw new InvalidOperationException("The selected assembly has no usable metadata.");

			var module = compilation.MainModule;
			var metadata = metadataFile.Metadata;
			var builder = new StringBuilder();
			builder.AppendLine("# Assembly Summary Context");
			builder.AppendLine();
			builder.Append("- **Assembly:** ").AppendLine(module.AssemblyName ?? assembly.ShortName);
			string version = metadata.IsAssembly ? metadata.GetAssemblyDefinition().Version?.ToString() ?? "unknown" : "unknown";
			builder.Append("- **Version:** ").AppendLine(version);
			builder.Append("- **Target Framework:** ").AppendLine(metadataFile.DetectTargetFrameworkId() ?? "unknown");
			builder.AppendLine();

			var topLevelTypes = module.TopLevelTypeDefinitions.ToArray();
			var publicTypes = module.TypeDefinitions
				.Where(type => type.Accessibility == Accessibility.Public)
				.ToArray();
			builder.AppendLine("## Top-level namespaces");
			foreach (string name in topLevelTypes.Select(type => type.Namespace).Where(name => !string.IsNullOrEmpty(name)).Distinct(StringComparer.Ordinal).OrderBy(name => name, StringComparer.Ordinal))
				builder.Append("- ").AppendLine(name);
			builder.AppendLine();
			builder.Append("- **Public types:** ").AppendLine(publicTypes.Length.ToString());

			builder.AppendLine("## Assembly attributes");
			foreach (string attribute in module.GetAssemblyAttributes().Select(attribute => attribute.AttributeType.FullName).Distinct(StringComparer.Ordinal).Take(20))
				builder.Append("- ").AppendLine(attribute);
			builder.AppendLine();

			builder.AppendLine("## Entry point");
			int entryPointToken = metadataFile.CorHeader?.EntryPointTokenOrRelativeVirtualAddress ?? 0;
			var metadataModule = module as MetadataModule;
			EntityHandle entryPoint = MetadataTokenHelpers.EntityHandleOrNil(entryPointToken);
			if (metadataModule is not null && entryPoint.Kind == HandleKind.MethodDefinition)
				builder.AppendLine("- " + (metadataModule.GetDefinition((MethodDefinitionHandle)entryPoint)?.FullName ?? "unknown"));
			else
				builder.AppendLine("- none");
			builder.AppendLine();

			builder.AppendLine("## Largest public types");
			foreach (ITypeDefinition type in publicTypes.OrderByDescending(type => type.Members.Count).ThenBy(type => type.FullName, StringComparer.Ordinal).Take(10))
			{
				string baseType = type.DirectBaseTypes.FirstOrDefault(candidate => candidate.Kind != TypeKind.Interface)?.FullName ?? "object";
				builder.Append("- ").Append(type.FullName).Append(" : ").Append(baseType)
					.Append(" (members: ").Append(type.Members.Count).AppendLine(")");
			}
			return builder.ToString();
		}
	}
}
