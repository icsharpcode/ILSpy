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
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpyX.Analyzers
{
	using ICSharpCode.Decompiler.TypeSystem;

	public class AnalyzerScope
	{
		readonly ITypeDefinition typeScope;
		readonly AssemblyListSnapshot assemblyListSnapshot;

		/// <summary>
		/// Returns whether this scope is local, i.e., AnalyzedSymbol is only reachable
		/// from the current module or containing type.
		/// </summary>
		public bool IsLocal { get; }

		public AssemblyList AssemblyList { get; }

		public ISymbol AnalyzedSymbol { get; }

		public ITypeDefinition TypeScope => typeScope;

		Accessibility effectiveAccessibility;

		public AnalyzerScope(AssemblyList assemblyList, IEntity entity)
		{
			AssemblyList = assemblyList;
			assemblyListSnapshot = assemblyList.GetSnapshot();
			AnalyzedSymbol = entity;
			DetermineEffectiveAccessibility(entity, out typeScope, out effectiveAccessibility);
			IsLocal = effectiveAccessibility.LessThanOrEqual(Accessibility.Private);
		}

		public IEnumerable<MetadataFile> GetModulesInScope(CancellationToken ct)
		{
			if (IsLocal)
				return new[] { TypeScope.ParentModule!.MetadataFile! };

			if (effectiveAccessibility.LessThanOrEqual(Accessibility.Internal))
				return GetModuleAndAnyFriends(TypeScope, ct);

			return GetReferencingModules(TypeScope.ParentModule!.MetadataFile!, ct);
		}

		public IEnumerable<MetadataFile> GetAllModules()
		{
			return assemblyListSnapshot.GetAllAssembliesAsync().GetAwaiter().GetResult()
				.Select(asm => asm.GetMetadataFileOrNull())
				.Where(x => x != null && !x.IsMetadataOnly)!;
		}

		public DecompilerTypeSystem ConstructTypeSystem(MetadataFile module)
		{
			return new DecompilerTypeSystem(module, module.GetAssemblyResolver(assemblyListSnapshot, loadOnDemand: false));
		}

		public IEnumerable<ITypeDefinition> GetTypesInScope(CancellationToken ct)
		{
			if (IsLocal)
			{
				foreach (var type in TreeTraversal.PreOrder(typeScope, t => t.NestedTypes))
				{
					yield return type;
				}
			}
			else
			{
				foreach (var module in GetModulesInScope(ct))
				{
					var typeSystem = ConstructTypeSystem(module);
					foreach (var type in typeSystem.MainModule.TypeDefinitions)
					{
						yield return type;
					}
				}
			}
		}

		static void DetermineEffectiveAccessibility(IEntity input, out ITypeDefinition typeScope, out Accessibility accessibility)
		{
			if (input is ITypeDefinition td)
			{
				accessibility = Accessibility.Public;
				typeScope = td;
			}
			else
			{
				accessibility = input.Accessibility;
				typeScope = input.DeclaringTypeDefinition!;
			}
			// Once we reach a private entity, we leave the loop with typeScope set to the class that
			// contains the private entity = the scope that needs to be searched.
			// Otherwise (if we don't find a private entity) we return the top-level class.
			var prevTypeScope = typeScope;
			while (typeScope != null && !accessibility.LessThanOrEqual(Accessibility.Private))
			{
				accessibility = accessibility.Intersect(typeScope.Accessibility);
				prevTypeScope = typeScope;
				typeScope = prevTypeScope.DeclaringTypeDefinition!;
			}
			if (typeScope == null)
			{
				typeScope = prevTypeScope;
			}
		}

		#region Find modules
		IEnumerable<MetadataFile> GetReferencingModules(MetadataFile self, CancellationToken ct)
		{
			yield return self;

			string reflectionTypeScopeName = typeScope.Name;
			if (typeScope.TypeParameterCount > 0)
				reflectionTypeScopeName += "`" + typeScope.TypeParameterCount;

			var toWalkFiles = new Stack<MetadataFile>();
			var checkedFiles = new HashSet<MetadataFile>();

			toWalkFiles.Push(self);
			checkedFiles.Add(self);
			IList<LoadedAssembly> assemblies = assemblyListSnapshot.GetAllAssembliesAsync().GetAwaiter().GetResult();

			do
			{
				MetadataFile curFile = toWalkFiles.Pop();
				foreach (var assembly in assemblies)
				{
					ct.ThrowIfCancellationRequested();
					bool found = false;
					var module = assembly.GetMetadataFileOrNull();
					if (module == null || !module.IsAssembly)
						continue;
					if (checkedFiles.Contains(module))
						continue;
					var resolver = assembly.GetAssemblyResolver(assemblyListSnapshot, loadOnDemand: false);
					foreach (var reference in module.AssemblyReferences)
					{
						if (resolver.Resolve(reference) == curFile)
						{
							found = true;
							break;
						}
					}
					if (found && checkedFiles.Add(module))
					{
						if (ModuleReferencesScopeType(module.Metadata, reflectionTypeScopeName, typeScope.Namespace))
							yield return module;
						if (ModuleForwardsScopeType(module.Metadata, reflectionTypeScopeName, typeScope.Namespace))
							toWalkFiles.Push(module);
					}
				}
			} while (toWalkFiles.Count > 0);
		}

		IEnumerable<MetadataFile> GetModuleAndAnyFriends(ITypeDefinition typeScope, CancellationToken ct)
		{
			var self = typeScope.ParentModule!.MetadataFile!;

			yield return self;

			var typeProvider = MetadataExtensions.MinimalAttributeTypeProvider;
			var attributes = self.Metadata.CustomAttributes.Select(h => self.Metadata.GetCustomAttribute(h))
				.Where(ca => ca.GetAttributeType(self.Metadata).GetFullTypeName(self.Metadata).ToString() == "System.Runtime.CompilerServices.InternalsVisibleToAttribute");
			var friendAssemblies = new HashSet<string>();
			foreach (var attribute in attributes)
			{
				string? assemblyName = attribute.DecodeValue(typeProvider).FixedArguments[0].Value as string;
				assemblyName = assemblyName?.Split(',')[0]; // strip off any public key info
				if (assemblyName != null)
					friendAssemblies.Add(assemblyName);
			}

			if (friendAssemblies.Count > 0)
			{
				IEnumerable<LoadedAssembly> assemblies = assemblyListSnapshot.GetAllAssembliesAsync()
					.GetAwaiter().GetResult();

				foreach (var assembly in assemblies)
				{
					ct.ThrowIfCancellationRequested();
					if (friendAssemblies.Contains(assembly.ShortName))
					{
						var module = assembly.GetMetadataFileOrNull();
						if (module == null || module.IsMetadataOnly)
							continue;
						if (ModuleReferencesScopeType(module.Metadata, typeScope.Name, typeScope.Namespace))
							yield return module;
					}
				}
			}
		}

		bool ModuleReferencesScopeType(MetadataReader metadata, string typeScopeName, string typeScopeNamespace)
		{
			bool hasRef = false;
			foreach (var h in metadata.TypeReferences)
			{
				var typeRef = metadata.GetTypeReference(h);
				if (metadata.StringComparer.Equals(typeRef.Name, typeScopeName)
					&& metadata.StringComparer.Equals(typeRef.Namespace, typeScopeNamespace))
				{
					hasRef = true;
					break;
				}
			}
			return hasRef;
		}

		bool ModuleForwardsScopeType(MetadataReader metadata, string typeScopeName, string typeScopeNamespace)
		{
			bool hasForward = false;
			foreach (var h in metadata.ExportedTypes)
			{
				var exportedType = metadata.GetExportedType(h);
				if (exportedType.IsForwarder
					&& metadata.StringComparer.Equals(exportedType.Name, typeScopeName)
					&& metadata.StringComparer.Equals(exportedType.Namespace, typeScopeNamespace))
				{
					hasForward = true;
					break;
				}
			}
			return hasForward;
		}
		#endregion
	}
}
