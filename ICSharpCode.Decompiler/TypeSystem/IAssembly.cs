// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	/// <summary>
	/// Interface used to help with construction of the type system.
	/// </summary>
	/// <remarks>
	/// The type system is an immutable cyclic data structure:
	/// the compilation (ICompilation) has references to all modules,
	/// and each module has a reference back to the compilation.
	/// 
	/// Module references are used to solve this cyclic dependency:
	/// The compilation constructor accepts module references,
	/// and only the IModuleReference.Resolve() function can observe a
	/// partially-constructed compilation; but not any user code.
	/// </remarks>
	public interface IModuleReference
	{
		/// <summary>
		/// Resolves this metadata module.
		/// </summary>
		IModule Resolve(ITypeResolveContext context);
	}

	/// <summary>
	/// Represents a metadata module.
	/// </summary>
	public interface IModule : ISymbol, ICompilationProvider
	{
		/// <summary>
		/// Gets the underlying metadata file. May return null, if the IAssembly was not created from a PE file.
		/// </summary>
		PEFile PEFile { get; }

		/// <summary>
		/// Gets whether this assembly is the main assembly of the compilation.
		/// </summary>
		bool IsMainModule { get; }

		/// <summary>
		/// Gets the assembly name (short name).
		/// </summary>
		string AssemblyName { get; }

		/// <summary>
		/// Gets the full assembly name (including public key token etc.)
		/// </summary>
		string FullAssemblyName { get; }

		/// <summary>
		/// Gets the list of all assembly attributes in the project.
		/// </summary>
		IEnumerable<IAttribute> GetAssemblyAttributes();

		/// <summary>
		/// Gets the list of all module attributes in the project.
		/// </summary>
		IEnumerable<IAttribute> GetModuleAttributes();

		/// <summary>
		/// Gets whether the internals of this assembly are visible in the specified assembly.
		/// </summary>
		bool InternalsVisibleTo(IModule module);

		/// <summary>
		/// Gets the root namespace for this module.
		/// </summary>
		/// <remarks>
		/// This always is the namespace without a name - it's unrelated to the 'root namespace' project setting.
		/// It contains only subnamespaces and types defined in this module -- use ICompilation.RootNamespace
		/// to get the combined view of all referenced assemblies.
		/// </remarks>
		INamespace RootNamespace { get; }

		/// <summary>
		/// Gets the type definition for a top-level type.
		/// </summary>
		/// <remarks>This method uses ordinal name comparison, not the compilation's name comparer.</remarks>
		ITypeDefinition GetTypeDefinition(TopLevelTypeName topLevelTypeName);

		/// <summary>
		/// Gets all non-nested types in the assembly.
		/// </summary>
		IEnumerable<ITypeDefinition> TopLevelTypeDefinitions { get; }

		/// <summary>
		/// Gets all types in the assembly, including nested types.
		/// </summary>
		IEnumerable<ITypeDefinition> TypeDefinitions { get; }
	}
}
