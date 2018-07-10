// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Determines the accessibility domain of a member for where-used analysis.
	/// </summary>
	class ScopedWhereUsedAnalyzer<T> where T : IEntity
	{
		readonly Language language;
		readonly IAnalyzer<T> analyzer;
		readonly IAssembly assemblyScope;
		readonly T analyzedEntity;
		ITypeDefinition typeScope;

		readonly Accessibility memberAccessibility = Accessibility.Public;
		Accessibility typeAccessibility = Accessibility.Public;

		public ScopedWhereUsedAnalyzer(Language language, T analyzedEntity, IAnalyzer<T> analyzer)
		{
			this.language = language ?? throw new ArgumentNullException(nameof(language));
			this.analyzer = analyzer ?? throw new ArgumentNullException(nameof(analyzer));
			this.analyzedEntity = analyzedEntity;
			this.assemblyScope = analyzedEntity.ParentAssembly;
			if (analyzedEntity is ITypeDefinition type) {
				this.typeScope = type;
				this.typeAccessibility = type.Accessibility;
			} else {
				this.typeScope = analyzedEntity.DeclaringTypeDefinition;
				this.typeAccessibility = this.typeScope.Accessibility;
				this.memberAccessibility = analyzedEntity.Accessibility;
			}
		}

		public IEnumerable<IEntity> PerformAnalysis(CancellationToken ct)
		{
			if (memberAccessibility == Accessibility.Private) {
				return FindReferencesInTypeScope(ct);
			}

			DetermineTypeAccessibility();

			if (typeAccessibility == Accessibility.Private && typeScope.DeclaringType != null) {
				return FindReferencesInEnclosingTypeScope(ct);
			}

			if (memberAccessibility == Accessibility.Internal ||
				memberAccessibility == Accessibility.ProtectedOrInternal ||
				typeAccessibility == Accessibility.Internal ||
				typeAccessibility == Accessibility.ProtectedAndInternal)
				return FindReferencesInAssemblyAndFriends(ct);

			return FindReferencesGlobal(ct);
		}
		
		void DetermineTypeAccessibility()
		{
			while (typeScope.DeclaringType != null) {
				Accessibility accessibility = typeScope.Accessibility;
				if ((int)typeAccessibility > (int)accessibility) {
					typeAccessibility = accessibility;
					if (typeAccessibility == Accessibility.Private)
						return;
				}
				typeScope = typeScope.DeclaringTypeDefinition;
			}

			if ((int)typeAccessibility > (int)Accessibility.Internal) {
				typeAccessibility = Accessibility.Internal;
			}
		}

		IEnumerable<IEntity> FindReferencesInAssemblyAndFriends(CancellationToken ct)
		{
			var assemblies = GetAssemblyAndAnyFriends(assemblyScope.PEFile, ct);
			return assemblies.AsParallel().WithCancellation(ct).SelectMany(a => FindReferencesInAssembly(a, ct));
		}

		IEnumerable<IEntity> FindReferencesGlobal(CancellationToken ct)
		{
			var assemblies = GetReferencingAssemblies(assemblyScope.PEFile, ct);
			return assemblies.AsParallel().WithCancellation(ct).SelectMany(asm => FindReferencesInAssembly(asm, ct));
		}

		IEnumerable<IEntity> FindReferencesInAssembly(PEFile module, CancellationToken ct)
		{
			var ts = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
			var context = new AnalyzerContext(ts) { CancellationToken = ct, Language = language };
			foreach (var type in ts.MainAssembly.TypeDefinitions) {
				ct.ThrowIfCancellationRequested();
				if (type.MetadataToken.IsNil) continue;
				context.CodeMappingInfo = language.GetCodeMappingInfo(module, type.MetadataToken);
				foreach (var result in RunAnalysisOn(module, type, context)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<IEntity> FindReferencesInTypeScope(CancellationToken ct)
		{
			var ts = new DecompilerTypeSystem(assemblyScope.PEFile, assemblyScope.PEFile.GetAssemblyResolver());
			var context = new AnalyzerContext(ts) { CancellationToken = ct, Language = language };
			var types = TreeTraversal.PreOrder(typeScope,
				t => t.GetNestedTypes(options: GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions)
				.Select(td => td.GetDefinition()));
			foreach (var type in types) {
				ct.ThrowIfCancellationRequested();
				if (type.MetadataToken.IsNil) continue;
				context.CodeMappingInfo = language.GetCodeMappingInfo(assemblyScope.PEFile, type.MetadataToken);
				foreach (var result in RunAnalysisOn(assemblyScope.PEFile, type, context)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<IEntity> FindReferencesInEnclosingTypeScope(CancellationToken ct)
		{
			var ts = new DecompilerTypeSystem(assemblyScope.PEFile, assemblyScope.PEFile.GetAssemblyResolver());
			var context = new AnalyzerContext(ts) { CancellationToken = ct, Language = language };
			var types = TreeTraversal.PreOrder(typeScope.DeclaringTypeDefinition,
				t => t.GetNestedTypes(options: GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions)
				.Select(td => td.GetDefinition()));
			foreach (var type in types) {
				ct.ThrowIfCancellationRequested();
				if (type.MetadataToken.IsNil) continue;
				context.CodeMappingInfo = language.GetCodeMappingInfo(assemblyScope.PEFile, type.MetadataToken);
				foreach (var result in RunAnalysisOn(assemblyScope.PEFile, type, context)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<PEFile> GetReferencingAssemblies(PEFile asm, CancellationToken ct)
		{
			yield return asm;

			IEnumerable<LoadedAssembly> assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();
			foreach (var assembly in assemblies) {
				ct.ThrowIfCancellationRequested();
				bool found = false;
				var module = assembly.GetPEFileOrNull();
				if (module == null || !module.IsAssembly)
					continue;
				var resolver = assembly.GetAssemblyResolver();
				foreach (var reference in module.AssemblyReferences) {
					using (LoadedAssembly.DisableAssemblyLoad()) {
						if (resolver.Resolve(reference) == asm) {
							found = true;
							break;
						}
					}
				}
				if (found && AssemblyReferencesScopeType(module.Metadata, typeScope.Name, typeScope.Namespace))
					yield return module;
			}
		}

		IEnumerable<PEFile> GetAssemblyAndAnyFriends(PEFile asm, CancellationToken ct)
		{
			yield return asm;

			var typeProvider = MetadataExtensions.MinimalAttributeTypeProvider;
			var attributes = asm.Metadata.CustomAttributes.Select(h => asm.Metadata.GetCustomAttribute(h))
				.Where(ca => ca.GetAttributeType(asm.Metadata).GetFullTypeName(asm.Metadata).ToString() == "System.Runtime.CompilerServices.InternalsVisibleToAttribute");
			var friendAssemblies = new HashSet<string>();
			foreach (var attribute in attributes) {
				string assemblyName = attribute.DecodeValue(typeProvider).FixedArguments[0].Value as string;
				assemblyName = assemblyName.Split(',')[0]; // strip off any public key info
				friendAssemblies.Add(assemblyName);
			}

			if (friendAssemblies.Count > 0) {
				IEnumerable<LoadedAssembly> assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies();

				foreach (var assembly in assemblies) {
					ct.ThrowIfCancellationRequested();
					if (friendAssemblies.Contains(assembly.ShortName)) {
						var module = assembly.GetPEFileOrNull();
						if (module == null)
							continue;
						if (AssemblyReferencesScopeType(module.Metadata, typeScope.Name, typeScope.Namespace))
							yield return module;
					}
				}
			}
		}

		bool AssemblyReferencesScopeType(MetadataReader metadata, string typeScopeName, string typeScopeNamespace)
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

		IEnumerable<IEntity> RunAnalysisOn(PEFile module, ITypeDefinition type, AnalyzerContext context)
		{
			if (analyzer is IMethodBodyAnalyzer<T> methodAnalyzer) {
				var reader = module.Reader;
				foreach (var method in type.GetMethods(options: GetMemberOptions.IgnoreInheritedMembers)) {
					if (!method.HasBody || method.MetadataToken.IsNil) continue;
					var md = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
					var body = reader.GetMethodBody(md.RelativeVirtualAddress);
					foreach (var result in methodAnalyzer.Analyze(analyzedEntity, method, body, context))
						yield return GetParentEntity(context, result);
				}
				foreach (var method in type.GetAccessors(options: GetMemberOptions.IgnoreInheritedMembers)) {
					if (!method.HasBody || method.MetadataToken.IsNil) continue;
					var md = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
					var body = reader.GetMethodBody(md.RelativeVirtualAddress);
					foreach (var result in methodAnalyzer.Analyze(analyzedEntity, method, body, context))
						yield return GetParentEntity(context, result);
				}
			}
			if (analyzer is ITypeDefinitionAnalyzer<T> typeDefinitionAnalyzer) {
				foreach (var result in typeDefinitionAnalyzer.Analyze(analyzedEntity, type, context))
					yield return GetParentEntity(context, result);
			}
		}

		private IEntity GetParentEntity(AnalyzerContext context, IEntity entity)
		{
			if (entity.MetadataToken.Kind == HandleKind.MethodDefinition && !entity.MetadataToken.IsNil) {
				var parentHandle = context.CodeMappingInfo.GetParentMethod((MethodDefinitionHandle)entity.MetadataToken);
				if (entity.MetadataToken == parentHandle)
					return entity;
				var method = context.TypeSystem.ResolveAsMethod(parentHandle);
				if (method != null) {
					return method.AccessorOwner ?? method;
				}
			}
			return entity;
		}
	}
}
