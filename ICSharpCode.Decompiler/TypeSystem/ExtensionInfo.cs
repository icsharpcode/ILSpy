// Copyright (c) 2025 Daniel Grunwald
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

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem
{
	public class ExtensionInfo
	{
		readonly Dictionary<IMember, ExtensionMemberInfo> extensionMemberMap;
		readonly Dictionary<IMember, ExtensionMemberInfo> implementationMemberMap;

		public ExtensionInfo(MetadataModule module, ITypeDefinition extensionContainer)
		{
			this.extensionMemberMap = new();
			this.implementationMemberMap = new();

			var metadata = module.MetadataFile.Metadata;

			foreach (var extGroup in extensionContainer.NestedTypes)
			{
				if (!(extGroup is { Kind: TypeKind.Class, IsSealed: true }
					&& extGroup.Name.StartsWith("<>E__", System.StringComparison.Ordinal)))
				{
					continue;
				}

				TypeDefinition td = metadata.GetTypeDefinition((TypeDefinitionHandle)extGroup.MetadataToken);
				IMethod? marker = null;
				bool hasMultipleMarkers = false;
				List<IMethod> extensionMethods = [];

				// For easier access to accessors we use SRM
				foreach (var h in td.GetMethods())
				{
					var method = module.GetDefinition(h);

					if (method.SymbolKind is SymbolKind.Constructor)
						continue;
					if (method is { Name: "<Extension>$", IsStatic: true, Parameters.Count: 1 })
					{
						if (marker == null)
							marker = method;
						else
							hasMultipleMarkers = true;
						continue;
					}

					extensionMethods.Add(method);
				}

				if (marker == null || hasMultipleMarkers)
					continue;

				foreach (var extension in extensionMethods)
				{
					int expectedTypeParameterCount = extension.TypeParameters.Count + extGroup.TypeParameterCount;
					bool hasInstance = !extension.IsStatic;
					int parameterOffset = hasInstance ? 1 : 0;
					int expectedParameterCount = extension.Parameters.Count + parameterOffset;
					TypeParameterSubstitution subst = new TypeParameterSubstitution([], [.. extGroup.TypeArguments, .. extension.TypeArguments]);

					bool IsMatchingImplementation(IMethod impl)
					{
						if (!impl.IsStatic)
							return false;
						if (extension.Name != impl.Name)
							return false;
						if (expectedTypeParameterCount != impl.TypeParameters.Count)
							return false;
						if (expectedParameterCount != impl.Parameters.Count)
							return false;
						if (hasInstance)
						{
							IType ti = impl.Parameters[0].Type.AcceptVisitor(subst);
							IType tm = marker.Parameters.Single().Type;
							if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(ti, tm))
								return false;
						}
						for (int i = 0; i < extension.Parameters.Count; i++)
						{
							IType ti = impl.Parameters[i + parameterOffset].Type.AcceptVisitor(subst);
							IType tm = extension.Parameters[i].Type;
							if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(ti, tm))
								return false;
						}
						return NormalizeTypeVisitor.TypeErasure.EquivalentTypes(
							impl.ReturnType.AcceptVisitor(subst),
							extension.ReturnType
						);
					}

					foreach (var impl in extensionContainer.Methods)
					{
						if (!IsMatchingImplementation(impl))
							continue;
						var emi = new ExtensionMemberInfo(marker, extension, impl);
						extensionMemberMap[extension] = emi;
						implementationMemberMap[impl] = emi;
					}
				}
			}

		}

		public ExtensionMemberInfo? InfoOfExtensionMember(IMethod method)
		{
			return this.extensionMemberMap.TryGetValue(method, out var value) ? value : null;
		}

		public ExtensionMemberInfo? InfoOfImplementationMember(IMethod method)
		{
			return this.implementationMemberMap.TryGetValue(method, out var value) ? value : null;
		}

		public IEnumerable<IGrouping<IMethod, ExtensionMemberInfo>> GetGroups()
		{
			return this.extensionMemberMap.Values.GroupBy(x => x.ExtensionMarkerMethod);
		}
	}

	public readonly struct ExtensionMemberInfo(IMethod marker, IMethod extension, IMethod implementation)
	{
		/// <summary>
		/// Metadata-only method called '&lt;Extension&gt;$'. Has the C# signature for the extension declaration.
		/// 
		/// <code>extension(ReceiverType name) {} -> void &lt;Extension&gt;$(ReceiverType name) {}</code>
		/// </summary>
		public readonly IMethod ExtensionMarkerMethod = marker;
		/// <summary>
		/// Metadata-only method with a signature as declared in C# within the extension declaration.
		/// This could also be an accessor of an extension property.
		/// </summary>
		public readonly IMethod ExtensionMember = extension;
		/// <summary>
		/// The actual implementation method in the outer class. The signature is a concatenation
		/// of the extension marker and the extension member's signatures.
		/// </summary>
		public readonly IMethod ImplementationMethod = implementation;

		/// <summary>
		/// This is the enclosing static class.
		/// </summary>
		public ITypeDefinition ExtensionContainer => ImplementationMethod.DeclaringTypeDefinition!;

		/// <summary>
		/// This is the compiler-generated class containing the extension members. Has type parameters
		/// from the extension declaration with minimal constraints.
		/// </summary>
		public ITypeDefinition ExtensionGroupingType => ExtensionMember.DeclaringTypeDefinition!;

		/// <summary>
		/// This class holds the type parameters for the extension declaration with full fidelity of C# constraints.
		/// </summary>
		public ITypeDefinition ExtensionMarkerType => ExtensionMarkerMethod.DeclaringTypeDefinition!;
	}
}
