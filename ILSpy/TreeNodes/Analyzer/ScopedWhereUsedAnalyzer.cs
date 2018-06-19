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
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	/// <summary>
	/// Determines the accessibility domain of a member for where-used analysis.
	/// </summary>
	class ScopedWhereUsedAnalyzer<T>
	{
		readonly Language language;
		readonly Decompiler.Metadata.PEFile assemblyScope;
		readonly bool provideTypeSystem;
		TypeDefinitionHandle typeScopeHandle;
		TypeDefinition typeScope;

		readonly Accessibility memberAccessibility = Accessibility.Public;
		Accessibility typeAccessibility = Accessibility.Public;
		readonly Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction;

		public ScopedWhereUsedAnalyzer(Language language, Decompiler.Metadata.PEFile module, TypeDefinitionHandle type, bool provideTypeSystem, Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction)
		{
			this.language = language;
			this.typeScopeHandle = type;
			this.assemblyScope = module;
			this.typeScope = module.Metadata.GetTypeDefinition(typeScopeHandle);
			this.provideTypeSystem = provideTypeSystem;
			this.typeAnalysisFunction = typeAnalysisFunction;
		}

		public ScopedWhereUsedAnalyzer(Language language, Decompiler.Metadata.PEFile module, MethodDefinitionHandle method, bool provideTypeSystem, Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction)
			: this(language, module, method.GetDeclaringType(module.Metadata), provideTypeSystem, typeAnalysisFunction)
		{
			this.memberAccessibility = GetMethodAccessibility(module.Metadata, method);
		}

		public ScopedWhereUsedAnalyzer(Language language, Decompiler.Metadata.PEFile module, PropertyDefinitionHandle property, bool provideTypeSystem, Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction)
			: this(language, module, property.GetDeclaringType(module.Metadata), provideTypeSystem, typeAnalysisFunction)
		{
			var pd = module.Metadata.GetPropertyDefinition(property);
			var accessors = pd.GetAccessors();
			Accessibility getterAccessibility = (accessors.Getter.IsNil) ? Accessibility.Private : GetMethodAccessibility(module.Metadata, accessors.Getter);
			Accessibility setterAccessibility = (accessors.Setter.IsNil) ? Accessibility.Private : GetMethodAccessibility(module.Metadata, accessors.Setter);
			this.memberAccessibility = (Accessibility)Math.Max((int)getterAccessibility, (int)setterAccessibility);
		}

		public ScopedWhereUsedAnalyzer(Language language, Decompiler.Metadata.PEFile module, EventDefinitionHandle eventDef, bool provideTypeSystem, Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction)
			: this(language, module, eventDef.GetDeclaringType(module.Metadata), provideTypeSystem, typeAnalysisFunction)
		{
			// we only have to check the accessibility of the the get method
			// [CLS Rule 30: The accessibility of an event and of its accessors shall be identical.]
			var ed = module.Metadata.GetEventDefinition(eventDef);
			this.memberAccessibility = GetMethodAccessibility(module.Metadata, ed.GetAccessors().Adder);
		}

		public ScopedWhereUsedAnalyzer(Language language, Decompiler.Metadata.PEFile module, FieldDefinitionHandle field, bool provideTypeSystem, Func<Decompiler.Metadata.PEFile, TypeDefinitionHandle, CodeMappingInfo, IDecompilerTypeSystem, IEnumerable<T>> typeAnalysisFunction)
			: this(language, module, field.GetDeclaringType(module.Metadata), provideTypeSystem, typeAnalysisFunction)
		{
			var fd = module.Metadata.GetFieldDefinition(field);
			switch (fd.Attributes & FieldAttributes.FieldAccessMask) {
				case FieldAttributes.Private:
				default:
					memberAccessibility = Accessibility.Private;
					break;
				case FieldAttributes.FamANDAssem:
					memberAccessibility = Accessibility.FamilyAndInternal;
					break;
				case FieldAttributes.Assembly:
					memberAccessibility = Accessibility.Internal;
					break;
				case FieldAttributes.Family:
					memberAccessibility = Accessibility.Family;
					break;
				case FieldAttributes.FamORAssem:
					memberAccessibility = Accessibility.FamilyOrInternal;
					break;
				case FieldAttributes.Public:
					memberAccessibility = Accessibility.Public;
					break;
			}
		}

		Accessibility GetMethodAccessibility(MetadataReader metadata, MethodDefinitionHandle method)
		{
			Accessibility accessibility;
			var methodInfo = metadata.GetMethodDefinition(method);
			switch (methodInfo.Attributes & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Private:
				default:
					accessibility = Accessibility.Private;
					break;
				case MethodAttributes.FamANDAssem:
					accessibility = Accessibility.FamilyAndInternal;
					break;
				case MethodAttributes.Family:
					accessibility = Accessibility.Family;
					break;
				case MethodAttributes.Assembly:
					accessibility = Accessibility.Internal;
					break;
				case MethodAttributes.FamORAssem:
					accessibility = Accessibility.FamilyOrInternal;
					break;
				case MethodAttributes.Public:
					accessibility = Accessibility.Public;
					break;
			}
			return accessibility;
		}

		public IEnumerable<T> PerformAnalysis(CancellationToken ct)
		{
			if (memberAccessibility == Accessibility.Private) {
				return FindReferencesInTypeScope(ct);
			}

			DetermineTypeAccessibility();

			if (typeAccessibility == Accessibility.Private) {
				return FindReferencesInEnclosingTypeScope(ct);
			}

			if (memberAccessibility == Accessibility.Internal ||
				memberAccessibility == Accessibility.FamilyAndInternal ||
				typeAccessibility == Accessibility.Internal ||
				typeAccessibility == Accessibility.FamilyAndInternal)
				return FindReferencesInAssemblyAndFriends(ct);

			return FindReferencesGlobal(ct);
		}
		
		void DetermineTypeAccessibility()
		{
			while (!typeScope.GetDeclaringType().IsNil) {
				Accessibility accessibility = GetNestedTypeAccessibility(typeScope);
				if ((int)typeAccessibility > (int)accessibility) {
					typeAccessibility = accessibility;
					if (typeAccessibility == Accessibility.Private)
						return;
				}
				typeScopeHandle = typeScope.GetDeclaringType();
				typeScope = assemblyScope.Metadata.GetTypeDefinition(typeScopeHandle);
			}

			if ((typeScope.Attributes & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic &&
				((int)typeAccessibility > (int)Accessibility.Internal)) {
				typeAccessibility = Accessibility.Internal;
			}
		}

		static Accessibility GetNestedTypeAccessibility(TypeDefinition type)
		{
			Accessibility result;
			switch (type.Attributes & TypeAttributes.VisibilityMask) {
				case TypeAttributes.NestedPublic:
					result = Accessibility.Public;
					break;
				case TypeAttributes.NestedPrivate:
					result = Accessibility.Private;
					break;
				case TypeAttributes.NestedFamily:
					result = Accessibility.Family;
					break;
				case TypeAttributes.NestedAssembly:
					result = Accessibility.Internal;
					break;
				case TypeAttributes.NestedFamANDAssem:
					result = Accessibility.FamilyAndInternal;
					break;
				case TypeAttributes.NestedFamORAssem:
					result = Accessibility.FamilyOrInternal;
					break;
				default:
					throw new InvalidOperationException();
			}
			return result;
		}

		/// <summary>
		/// The effective accessibility of a member
		/// </summary>
		enum Accessibility
		{
			Private,
			FamilyAndInternal,
			Internal,
			Family,
			FamilyOrInternal,
			Public
		}

		IEnumerable<T> FindReferencesInAssemblyAndFriends(CancellationToken ct)
		{
			var assemblies = GetAssemblyAndAnyFriends(assemblyScope, ct);

			// use parallelism only on the assembly level (avoid locks within Cecil)
			return assemblies.AsParallel().WithCancellation(ct).SelectMany(a => FindReferencesInAssembly(a, ct));
		}

		IEnumerable<T> FindReferencesGlobal(CancellationToken ct)
		{
			var assemblies = GetReferencingAssemblies(assemblyScope, ct);

			// use parallelism only on the assembly level (avoid locks within Cecil)
			return assemblies.AsParallel().WithCancellation(ct).SelectMany(asm => FindReferencesInAssembly(asm, ct));
		}

		IEnumerable<T> FindReferencesInAssembly(Decompiler.Metadata.PEFile module, CancellationToken ct)
		{
			IDecompilerTypeSystem ts = provideTypeSystem ? new DecompilerTypeSystem(module) : null;
			var metadata = module.Metadata;
			foreach (var type in TreeTraversal.PreOrder(metadata.TypeDefinitions, t => metadata.GetTypeDefinition(t).GetNestedTypes())) {
				ct.ThrowIfCancellationRequested();
				var codeMappingInfo = language.GetCodeMappingInfo(assemblyScope, type);
				foreach (var result in typeAnalysisFunction(module, type, codeMappingInfo, ts)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<T> FindReferencesInTypeScope(CancellationToken ct)
		{
			IDecompilerTypeSystem ts = provideTypeSystem ? new DecompilerTypeSystem(assemblyScope) : null;
			foreach (var type in TreeTraversal.PreOrder(typeScopeHandle, t => assemblyScope.Metadata.GetTypeDefinition(t).GetNestedTypes())) {
				ct.ThrowIfCancellationRequested();
				var codeMappingInfo = language.GetCodeMappingInfo(assemblyScope, type);
				foreach (var result in typeAnalysisFunction(assemblyScope, type, codeMappingInfo, ts)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<T> FindReferencesInEnclosingTypeScope(CancellationToken ct)
		{
			IDecompilerTypeSystem ts = provideTypeSystem ? new DecompilerTypeSystem(assemblyScope) : null;
			var codeMappingInfo = language.GetCodeMappingInfo(assemblyScope, typeScope.GetDeclaringType());
			foreach (var type in TreeTraversal.PreOrder(typeScope.GetDeclaringType(), t => assemblyScope.Metadata.GetTypeDefinition(t).GetNestedTypes())) {
				ct.ThrowIfCancellationRequested();
				foreach (var result in typeAnalysisFunction(assemblyScope, type, codeMappingInfo, ts)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<Decompiler.Metadata.PEFile> GetReferencingAssemblies(Decompiler.Metadata.PEFile asm, CancellationToken ct)
		{
			yield return asm;

			string typeScopeNamespace = asm.Metadata.GetString(typeScope.Namespace);
			string typeScopeName = asm.Metadata.GetString(typeScope.Name);

			IEnumerable<LoadedAssembly> assemblies = MainWindow.Instance.CurrentAssemblyList.GetAssemblies().Where(assy => assy.GetPEFileOrNull()?.IsAssembly == true);

			foreach (var assembly in assemblies) {
				ct.ThrowIfCancellationRequested();
				bool found = false;
				var module = assembly.GetPEFileOrNull();
				if (module == null)
					continue;
				var resolver = assembly.GetAssemblyResolver();
				var metadata = module.Metadata;
				foreach (var reference in module.AssemblyReferences) {
					using (LoadedAssembly.DisableAssemblyLoad()) {
						if (resolver.Resolve(reference) == asm) {
							found = true;
							break;
						}
					}
				}
				if (found && AssemblyReferencesScopeType(metadata, typeScopeName, typeScopeNamespace))
					yield return module;
			}
		}

		IEnumerable<Decompiler.Metadata.PEFile> GetAssemblyAndAnyFriends(Decompiler.Metadata.PEFile asm, CancellationToken ct)
		{
			yield return asm;
			var metadata = asm.Metadata;

			string typeScopeNamespace = metadata.GetString(typeScope.Namespace);
			string typeScopeName = metadata.GetString(typeScope.Name);

			var typeProvider = Decompiler.Metadata.MetadataExtensions.MinimalAttributeTypeProvider;
			var attributes = metadata.CustomAttributes.Select(h => metadata.GetCustomAttribute(h)).Where(ca => ca.GetAttributeType(metadata).GetFullTypeName(metadata).ToString() == "System.Runtime.CompilerServices.InternalsVisibleToAttribute");
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
						if (AssemblyReferencesScopeType(module.Metadata, typeScopeName, typeScopeNamespace))
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
	}
}
