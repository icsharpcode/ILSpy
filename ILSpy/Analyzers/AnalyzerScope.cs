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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Analyzers
{
	public class AnalyzerScope
	{
		readonly ITypeDefinition typeScope;

		/// <summary>
		/// Returns whether this scope is local, i.e., AnalyzedSymbol is only reachable
		/// from the current module or containing type.
		/// </summary>
		public bool IsLocal { get; }

		public AssemblyList AssemblyList { get; }
		public ISymbol AnalyzedSymbol { get; }

		public ITypeDefinition TypeScope => typeScope;

		Accessibility memberAccessibility, typeAccessibility;

		public AnalyzerScope(AssemblyList assemblyList, IEntity entity)
		{
			AssemblyList = assemblyList;
			AnalyzedSymbol = entity;
			if (entity is ITypeDefinition type) {
				typeScope = type;
				memberAccessibility = Accessibility.None;
			} else {
				typeScope = entity.DeclaringTypeDefinition;
				memberAccessibility = entity.Accessibility;
			}
			typeAccessibility = DetermineTypeAccessibility(ref typeScope);
			IsLocal = memberAccessibility == Accessibility.Private || typeAccessibility == Accessibility.Private;
		}

		public IEnumerable<PEFile> GetModulesInScope(CancellationToken ct)
		{
			if (IsLocal)
				return new[] { TypeScope.ParentModule.PEFile };

			if (memberAccessibility == Accessibility.Internal ||
				memberAccessibility == Accessibility.ProtectedOrInternal ||
				typeAccessibility == Accessibility.Internal ||
				typeAccessibility == Accessibility.ProtectedAndInternal)
				return GetModuleAndAnyFriends(TypeScope, ct);

			return GetReferencingModules(TypeScope.ParentModule.PEFile, ct);
		}

		public IEnumerable<PEFile> GetAllModules()
		{
			foreach (var module in AssemblyList.GetAssemblies()) {
				var file = module.GetPEFileOrNull();
				if (file == null) continue;
				yield return file;
			}
		}

		public IEnumerable<ITypeDefinition> GetTypesInScope(CancellationToken ct)
		{
			if (IsLocal) {
				var typeSystem = new DecompilerTypeSystem(TypeScope.ParentModule.PEFile, TypeScope.ParentModule.PEFile.GetAssemblyResolver());
				ITypeDefinition scope = typeScope;
				if (memberAccessibility != Accessibility.Private && typeScope.DeclaringTypeDefinition != null) {
					scope = typeScope.DeclaringTypeDefinition;
				}
				foreach (var type in TreeTraversal.PreOrder(scope, t => t.NestedTypes)) {
					yield return type;
				}
			} else {
				foreach (var module in GetModulesInScope(ct)) {
					var typeSystem = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
					foreach (var type in typeSystem.MainModule.TypeDefinitions) {
						yield return type;
					}
				}
			}
		}

		Accessibility DetermineTypeAccessibility(ref ITypeDefinition typeScope)
		{
			var typeAccessibility = typeScope.Accessibility;
			while (typeScope.DeclaringType != null) {
				Accessibility accessibility = typeScope.Accessibility;
				if ((int)typeAccessibility > (int)accessibility) {
					typeAccessibility = accessibility;
					if (typeAccessibility == Accessibility.Private)
						break;
				}
				typeScope = typeScope.DeclaringTypeDefinition;
			}

			if ((int)typeAccessibility > (int)Accessibility.Internal) {
				typeAccessibility = Accessibility.Internal;
			}
			return typeAccessibility;
		}

		#region Find modules
		IEnumerable<PEFile> GetReferencingModules(PEFile self, CancellationToken ct)
		{
			yield return self;

			string reflectionTypeScopeName = typeScope.Name;
			if (typeScope.TypeParameterCount > 0)
				reflectionTypeScopeName += "`" + typeScope.TypeParameterCount;

			var toWalkFiles = new Stack<PEFile>();
			var checkedFiles = new HashSet<PEFile>();

			toWalkFiles.Push(self);
			checkedFiles.Add(self);

			do {
				PEFile curFile = toWalkFiles.Pop();
				foreach (var assembly in AssemblyList.GetAssemblies()) {
					ct.ThrowIfCancellationRequested();
					bool found = false;
					var module = assembly.GetPEFileOrNull();
					if (module == null || !module.IsAssembly)
						continue;
					if (checkedFiles.Contains(module))
						continue;
					var resolver = assembly.GetAssemblyResolver();
					foreach (var reference in module.AssemblyReferences) {
						using (LoadedAssembly.DisableAssemblyLoad()) {
							if (resolver.Resolve(reference) == curFile) {
								found = true;
								break;
							}
						}
					}
					if (found && checkedFiles.Add(module)) {
						if (ModuleReferencesScopeType(module.Metadata, reflectionTypeScopeName, typeScope.Namespace))
							yield return module;
						if (ModuleForwardsScopeType(module.Metadata, reflectionTypeScopeName, typeScope.Namespace))
							toWalkFiles.Push(module);
					}
				}
			} while (toWalkFiles.Count > 0);
		}

		IEnumerable<PEFile> GetModuleAndAnyFriends(ITypeDefinition typeScope, CancellationToken ct)
		{
			var self = typeScope.ParentModule.PEFile;

			yield return self;

			var typeProvider = MetadataExtensions.MinimalAttributeTypeProvider;
			var attributes = self.Metadata.CustomAttributes.Select(h => self.Metadata.GetCustomAttribute(h))
				.Where(ca => ca.GetAttributeType(self.Metadata).GetFullTypeName(self.Metadata).ToString() == "System.Runtime.CompilerServices.InternalsVisibleToAttribute");
			var friendAssemblies = new HashSet<string>();
			foreach (var attribute in attributes) {
				string assemblyName = attribute.DecodeValue(typeProvider).FixedArguments[0].Value as string;
				assemblyName = assemblyName.Split(',')[0]; // strip off any public key info
				friendAssemblies.Add(assemblyName);
			}

			if (friendAssemblies.Count > 0) {
				IEnumerable<LoadedAssembly> assemblies = AssemblyList.GetAssemblies();

				foreach (var assembly in assemblies) {
					ct.ThrowIfCancellationRequested();
					if (friendAssemblies.Contains(assembly.ShortName)) {
						var module = assembly.GetPEFileOrNull();
						if (module == null)
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
			foreach (var h in metadata.TypeReferences) {
				var typeRef = metadata.GetTypeReference(h);
				if (metadata.StringComparer.Equals(typeRef.Name, typeScopeName) && metadata.StringComparer.Equals(typeRef.Namespace, typeScopeNamespace)) {
					hasRef = true;
					break;
				}
			}
			return hasRef;
		}

		bool ModuleForwardsScopeType(MetadataReader metadata, string typeScopeName, string typeScopeNamespace)
		{
			bool hasForward = false;
			foreach (var h in metadata.ExportedTypes) {
				var exportedType = metadata.GetExportedType(h);
				if (exportedType.IsForwarder && metadata.StringComparer.Equals(exportedType.Name, typeScopeName) && metadata.StringComparer.Equals(exportedType.Namespace, typeScopeNamespace)) {
					hasForward = true;
					break;
				}
			}
			return hasForward;
		}
		#endregion
	}
}
