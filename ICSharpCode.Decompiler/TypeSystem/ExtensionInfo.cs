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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.TypeSystem.Implementation;

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

			foreach (var nestedType in extensionContainer.NestedTypes)
			{
				if (TryEncodingV1(nestedType))
				{
					continue;
				}

				TryEncodingV2(nestedType);
			}

			bool TryEncodingV1(ITypeDefinition extGroup)
			{
				if (!(extGroup is { Kind: TypeKind.Class, IsSealed: true }
					&& extGroup.Name.StartsWith("<>E__", System.StringComparison.Ordinal)))
				{
					return false;
				}

				TypeDefinition td = metadata.GetTypeDefinition((TypeDefinitionHandle)extGroup.MetadataToken);
				IMethod? marker = null;
				bool hasMultipleMarkers = false;
				List<IMethod> extensionMethods = [];
				ITypeParameter[] extensionGroupTypeParameters = extGroup.TypeParameters.ToArray();

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
					return false;

				CollectImplementationMethods(extGroup, marker, extensionMethods, extensionGroupTypeParameters);
				return true;
			}

			bool TryEncodingV2(ITypeDefinition extensionGroupsContainer)
			{
				// there exists one nested type per extension target type
				if (!(extensionGroupsContainer is { Kind: TypeKind.Class, IsSealed: true }
					&& extensionGroupsContainer.Name.StartsWith("<G>$", StringComparison.Ordinal)))
				{
					return false;
				}

				// if there are multiple extension-blocks with the same target type,
				// but different names for the extension parameter,
				// there is a separate markerType, so there are multiple marker types per
				// target type
				foreach (var markerType in extensionGroupsContainer.NestedTypes)
				{
					if (!(markerType.Name.StartsWith("<M>$", StringComparison.Ordinal) && markerType.IsStatic))
						continue;
					var marker = markerType.Methods.SingleOrDefault(m => m.Name == "<Extension>$" && m.IsStatic && m.Parameters.Count == 1);
					if (marker == null)
						continue;

					TypeDefinition td = metadata.GetTypeDefinition((TypeDefinitionHandle)extensionGroupsContainer.MetadataToken);
					List<IMethod> extensionMethods = [];
					ITypeParameter[] extensionGroupTypeParameters = new ITypeParameter[extensionGroupsContainer.TypeParameterCount];

					// For easier access to accessors we use SRM
					foreach (var h in td.GetMethods())
					{
						var method = module.GetDefinition(h);

						if (method.SymbolKind is SymbolKind.Constructor)
							continue;

						var attribute = method.GetAttribute(KnownAttribute.ExtensionMarker);
						if (attribute == null)
							continue;

						if (attribute.FixedArguments[0].Value?.ToString() != markerType.Name)
							continue;

						extensionMethods.Add(method);
					}

					CollectImplementationMethods(extensionGroupsContainer, marker, extensionMethods, extensionGroupTypeParameters);
				}

				return true;
			}

			void CollectImplementationMethods(ITypeDefinition extGroup, IMethod marker, List<IMethod> extensionMethods, ITypeParameter[] extensionGroupTypeParameters)
			{
				List<(IMethod extension, IMethod implementation)> implementations = [];

				string[] typeParameterNames = new string[extGroup.TypeParameterCount];

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

					IMethod? foundImpl = null;

					foreach (var impl in extensionContainer.Methods)
					{
						if (!IsMatchingImplementation(impl))
							continue;
						Debug.Assert(foundImpl == null, "Multiple matching implementations found");
						foundImpl = impl;
					}

					Debug.Assert(foundImpl != null, "No matching implementation found");

					implementations.Add((extension, foundImpl));
				}

				foreach (var (extension, implementation) in implementations)
				{
					for (int i = 0; i < extensionGroupTypeParameters.Length; i++)
					{
						if (typeParameterNames[i] == null)
						{
							typeParameterNames[i] = implementation.TypeParameters[i].Name;
						}
						else if (typeParameterNames[i] != implementation.TypeParameters[i].Name)
						{
							// TODO: Handle name conflicts properly
							typeParameterNames[i] = $"T{i + 1}";
						}
					}
				}

				for (int i = 0; i < extensionGroupTypeParameters.Length; i++)
				{
					var originalTypeParameter = extGroup.TypeParameters[i];
					if (extensionGroupTypeParameters[i] == null)
					{
						extensionGroupTypeParameters[i] = new DefaultTypeParameter(
							extGroup, i, typeParameterNames[i],
							VarianceModifier.Invariant,
							attributes: originalTypeParameter.GetAttributes().ToArray(),
							originalTypeParameter.HasValueTypeConstraint,
							originalTypeParameter.HasReferenceTypeConstraint,
							originalTypeParameter.HasDefaultConstructorConstraint,
							originalTypeParameter.TypeConstraints.Select(c => c.Type).ToArray(),
							originalTypeParameter.NullabilityConstraint
						);
					}
				}

				foreach (var (extension, implementation) in implementations)
				{
					var info = new ExtensionMemberInfo(marker, extension, implementation, extensionGroupTypeParameters);
					this.extensionMemberMap[extension] = info;
					this.implementationMemberMap[implementation] = info;
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

		public IEnumerable<IGrouping<(IMethod Marker, ITypeParameter[] TypeParameters), ExtensionMemberInfo>> GetGroups()
		{
			return this.extensionMemberMap.Values.GroupBy(x => (x.ExtensionMarkerMethod, x.ExtensionGroupingTypeParameters));
		}

		public bool IsExtensionGroupingType(ITypeDefinition type)
		{
			return this.extensionMemberMap.Values.Any(x => x.ExtensionGroupingType.Equals(type));
		}
	}

	public readonly struct ExtensionMemberInfo(IMethod marker, IMethod extension, IMethod implementation, ITypeParameter[] extensionGroupingTypeParameters)
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
		/// This is the array of type parameters for the extension declaration.
		/// </summary>
		public ITypeParameter[] ExtensionGroupingTypeParameters => extensionGroupingTypeParameters;

		/// <summary>
		/// This class holds the type parameters for the extension declaration with full fidelity of C# constraints.
		/// </summary>
		public ITypeDefinition ExtensionMarkerType => ExtensionMarkerMethod.DeclaringTypeDefinition!;
	}
}
