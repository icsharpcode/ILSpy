// Copyright (c) 2018 Daniel Grunwald
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Used as context object for metadata TS entities;
	/// should be turned into IAssembly implementation when the TS refactoring is complete.
	/// </summary>
	[DebuggerDisplay("<MetadataAssembly: {AssemblyName}>")]
	public class MetadataAssembly : IAssembly
	{
		public ICompilation Compilation { get; }
		internal readonly Metadata.PEFile PEFile;
		internal readonly MetadataReader metadata;
		readonly TypeSystemOptions options;
		internal readonly TypeProvider TypeProvider;

		readonly MetadataNamespace rootNamespace;
		readonly MetadataTypeDefinition[] typeDefs;
		readonly MetadataField[] fieldDefs;

		internal MetadataAssembly(ICompilation compilation, Metadata.PEFile peFile, TypeSystemOptions options)
		{
			this.Compilation = compilation;
			this.PEFile = peFile;
			this.metadata = peFile.Metadata;
			this.options = options;
			this.TypeProvider = new TypeProvider(this);

			// assembly metadata
			if (metadata.IsAssembly) {
				var asmdef = metadata.GetAssemblyDefinition();
				this.AssemblyName = metadata.GetString(asmdef.Name);
				this.FullAssemblyName = metadata.GetFullAssemblyName();
			} else {
				var moddef = metadata.GetModuleDefinition();
				this.AssemblyName = metadata.GetString(moddef.Name);
				this.FullAssemblyName = this.AssemblyName;
			}
			// create arrays for resolved entities, indexed by row index
			this.typeDefs = new MetadataTypeDefinition[metadata.TypeDefinitions.Count + 1];
			this.fieldDefs = new MetadataField[metadata.FieldDefinitions.Count + 1];
		}

		internal string GetString(StringHandle name)
		{
			return metadata.GetString(name);
		}

		public TypeSystemOptions TypeSystemOptions => options;

		#region IAssembly interface
		public bool IsMainAssembly => this == Compilation.MainAssembly;

		public string AssemblyName { get; }
		public string FullAssemblyName { get; }

		public INamespace RootNamespace => rootNamespace;

		public IEnumerable<ITypeDefinition> TopLevelTypeDefinitions => TypeDefinitions.Where(td => td.DeclaringTypeDefinition == null);
		
		public ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName)
		{
			var typeDefHandle = PEFile.GetTypeDefinition(topLevelTypeName);
			return GetDefinition(typeDefHandle);
		}
		#endregion

		#region InternalsVisibleTo
		public bool InternalsVisibleTo(IAssembly assembly)
		{
			if (this == assembly)
				return true;
			foreach (string shortName in GetInternalsVisibleTo()) {
				if (assembly.AssemblyName == shortName)
					return true;
			}
			return false;
		}

		string[] internalsVisibleTo;

		string[] GetInternalsVisibleTo()
		{
			var result = LazyInit.VolatileRead(ref this.internalsVisibleTo);
			if (result != null) {
				return result;
			}
			if (metadata.IsAssembly) {
				var list = new List<string>();
				foreach (var attrHandle in metadata.GetAssemblyDefinition().GetCustomAttributes()) {
					var attr = metadata.GetCustomAttribute(attrHandle);
					if (attr.IsKnownAttribute(metadata, KnownAttribute.InternalsVisibleTo)) {
						var attrValue = attr.DecodeValue(this.TypeProvider);
						if (attrValue.FixedArguments.Length == 1) {
							if (attrValue.FixedArguments[0].Value is string s) {
								list.Add(s);
							}
						}
					}
				}
				result = list.ToArray();
			} else {
				result = Empty<string>.Array;
			}
			return LazyInit.GetOrSet(ref this.internalsVisibleTo, result);
		}
		#endregion

		#region GetDefinition
		/// <summary>
		/// Gets all types in the assembly, including nested types.
		/// </summary>
		public IEnumerable<ITypeDefinition> TypeDefinitions {
			get {
				for (int row = 1; row < typeDefs.Length; row++) {
					var typeDef = LazyInit.VolatileRead(ref typeDefs[row]);
					if (typeDef != null) {
						yield return typeDef;
					} else {
						typeDef = new MetadataTypeDefinition(this, MetadataTokens.TypeDefinitionHandle(row));
						yield return LazyInit.GetOrSet(ref typeDefs[row], typeDef);
					}
				}
			}
		}

		public ITypeDefinition GetDefinition(TypeDefinitionHandle handle)
		{
			int row = MetadataTokens.GetRowNumber(handle);
			if (row >= typeDefs.Length)
				return null;
			var typeDef = LazyInit.VolatileRead(ref typeDefs[row]);
			if (typeDef != null || handle.IsNil)
				return typeDef;
			typeDef = new MetadataTypeDefinition(this, handle);
			return LazyInit.GetOrSet(ref typeDefs[row], typeDef);
		}

		public IField GetDefinition(FieldDefinitionHandle handle)
		{
			int row = MetadataTokens.GetRowNumber(handle);
			if (row >= fieldDefs.Length)
				return null;
			var field = LazyInit.VolatileRead(ref fieldDefs[row]);
			if (field != null || handle.IsNil)
				return field;
			field = new MetadataField(this, handle);
			return LazyInit.GetOrSet(ref fieldDefs[row], field);
		}

		public IMethod GetDefinition(MethodDefinitionHandle handle)
		{
			throw new NotImplementedException();
		}

		public IProperty GetDefinition(PropertyDefinitionHandle handle)
		{
			throw new NotImplementedException();
		}

		public IEvent GetDefinition(EventDefinitionHandle handle)
		{
			throw new NotImplementedException();
		}
		#endregion

		public IMethod ResolveMethod(EntityHandle methodRefDefSpec, GenericContext context = default)
		{
			throw new NotImplementedException();
		}

		public IType ResolveType(EntityHandle typeRefDefSpec, GenericContext context = default, CustomAttributeHandleCollection? typeAttributes = null)
		{
			return MetadataTypeReference.Resolve(typeRefDefSpec, metadata, TypeProvider, context, options, typeAttributes);
		}

		#region Module / Assembly attributes
		IAttribute[] assemblyAttributes;
		IAttribute[] moduleAttributes;

		/// <summary>
		/// Gets the list of all assembly attributes in the project.
		/// </summary>
		public IReadOnlyList<IAttribute> AssemblyAttributes {
			get {
				var attrs = LazyInit.VolatileRead(ref this.assemblyAttributes);
				if (attrs != null)
					return attrs;
				var b = new AttributeListBuilder(this);
				if (metadata.IsAssembly) {
					var assembly = metadata.GetAssemblyDefinition();
					b.Add(metadata.GetCustomAttributes(Handle.AssemblyDefinition));
					b.AddSecurityAttributes(assembly.GetDeclarativeSecurityAttributes());

					// AssemblyVersionAttribute
					if (assembly.Version != null) {
						b.Add(KnownAttribute.AssemblyVersion, KnownTypeCode.String, assembly.Version.ToString());
					}
				}
				return LazyInit.GetOrSet(ref this.assemblyAttributes, b.Build());
			}
		}

		/// <summary>
		/// Gets the list of all module attributes in the project.
		/// </summary>
		public IReadOnlyList<IAttribute> ModuleAttributes {
			get {
				var attrs = LazyInit.VolatileRead(ref this.moduleAttributes);
				if (attrs != null)
					return attrs;
				var b = new AttributeListBuilder(this);
				b.Add(metadata.GetCustomAttributes(Handle.ModuleDefinition));
				return LazyInit.GetOrSet(ref this.moduleAttributes, b.Build());
			}
		}
		#endregion

		#region Attribute Helpers
		/// <summary>
		/// Cache for parameterless known attribute types.
		/// </summary>
		readonly IType[] knownAttributeTypes = new IType[KnownAttributes.Count];

		internal IType GetAttributeType(KnownAttribute attr)
		{
			var ty = LazyInit.VolatileRead(ref knownAttributeTypes[(int)attr]);
			if (ty != null)
				return ty;
			ty = Compilation.FindType(attr.GetTypeName());
			return LazyInit.GetOrSet(ref knownAttributeTypes[(int)attr], ty);
		}

		/// <summary>
		/// Cache for parameterless known attributes.
		/// </summary>
		readonly IAttribute[] knownAttributes = new IAttribute[KnownAttributes.Count];

		/// <summary>
		/// Construct a builtin attribute.
		/// </summary>
		internal IAttribute MakeAttribute(KnownAttribute type)
		{
			var attr = LazyInit.VolatileRead(ref knownAttributes[(int)type]);
			if (attr != null)
				return attr;
			attr = new DefaultAttribute(GetAttributeType(type));
			return LazyInit.GetOrSet(ref knownAttributes[(int)type], attr);
		}
		#endregion

		#region Visibility Filter
		internal bool IsVisible(FieldAttributes attributes)
		{
			return true;
		}
		#endregion
	}
}
