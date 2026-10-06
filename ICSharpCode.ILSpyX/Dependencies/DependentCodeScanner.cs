// Copyright (c) 2026 Piero Viano
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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpyX.Dependencies
{
	/// <summary>
	/// Finds the code of one module that depends on another assembly: the types, methods, fields,
	/// properties and events whose signatures, base types, interfaces, generic constraints, custom
	/// attributes (including those on parameters, and the types named in their arguments), locals
	/// or IL operands mention a type or member whose resolution scope is one of a given set of
	/// assembly references. Works on raw metadata, so an unresolvable target still counts as a
	/// dependency; only the underlying types of enums in attribute arguments are resolved.
	/// </summary>
	sealed class DependentCodeScanner : ISignatureTypeProvider<bool, object?>
	{
		readonly MetadataFile module;
		readonly MetadataReader metadata;
		readonly HashSet<AssemblyReferenceHandle> targetReferences;
		readonly List<IAssemblyReference> targetIdentities;
		readonly Dictionary<TypeReferenceHandle, bool> typeReferenceCache = new();
		readonly Dictionary<TypeSpecificationHandle, bool> typeSpecificationCache = new();

		DependentCodeScanner(MetadataFile module, IEnumerable<AssemblyReferenceHandle> targetReferences)
		{
			this.module = module;
			this.metadata = module.Metadata;
			this.targetReferences = new HashSet<AssemblyReferenceHandle>(targetReferences);
			this.targetIdentities = module.AssemblyReferences
				.Where(r => this.targetReferences.Contains(r.Handle))
				.ToList<IAssemblyReference>();
		}

		/// <summary>
		/// The assembly references of <paramref name="module"/> that denote <paramref name="target"/>
		/// (same name, culture and public key token).
		/// </summary>
		public static IReadOnlyList<AssemblyReferenceHandle> FindReferencesTo(MetadataFile module, MetadataFile target)
		{
			return module.AssemblyReferences
				.Where(r => r.IsReferenceTo(target.Metadata))
				.Select(r => r.Handle)
				.ToList();
		}

		/// <summary>
		/// Entities of <paramref name="typeSystem"/>'s main module (which must be built over
		/// <paramref name="module"/>) that depend on any of <paramref name="targetReferences"/>,
		/// each reported once, in metadata order (types, then members, then attribute owners).
		/// Accessors are reported as their owning property or event, parameters as their method.
		/// </summary>
		public static IEnumerable<ISymbol> FindDependentSymbols(MetadataFile module, DecompilerTypeSystem typeSystem,
			IReadOnlyCollection<AssemblyReferenceHandle> targetReferences, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(module);
			ArgumentNullException.ThrowIfNull(typeSystem);
			ArgumentNullException.ThrowIfNull(targetReferences);
			if (targetReferences.Count == 0)
				return Array.Empty<ISymbol>();
			var scanner = new DependentCodeScanner(module, targetReferences);
			return scanner.Scan(typeSystem.MainModule, typeSystem, cancellationToken).Distinct();
		}

		IEnumerable<ISymbol> Scan(MetadataModule mainModule, DecompilerTypeSystem typeSystem, CancellationToken ct)
		{
			foreach (var h in metadata.TypeDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				var td = metadata.GetTypeDefinition(h);
				bool found = UsesTarget(td.BaseType);
				foreach (var ih in td.GetInterfaceImplementations())
					found |= UsesTarget(metadata.GetInterfaceImplementation(ih).Interface);
				found |= UsesTargetInConstraints(td.GetGenericParameters());
				if (found && mainModule.GetDefinition(h) is { } type)
					yield return type;
			}

			foreach (var h in metadata.MethodDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				var md = metadata.GetMethodDefinition(h);
				bool found = Safe(() => AnyInSignature(md.DecodeSignature(this, null)))
					|| UsesTargetInConstraints(md.GetGenericParameters())
					|| UsesTargetInBody(md);
				if (found && mainModule.GetDefinition(h) is { } method)
					yield return method.AccessorOwner ?? method;
			}

			foreach (var h in metadata.FieldDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				var fd = metadata.GetFieldDefinition(h);
				if (Safe(() => fd.DecodeSignature(this, null)) && mainModule.GetDefinition(h) is { } field)
					yield return field;
			}

			foreach (var h in metadata.PropertyDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				var pd = metadata.GetPropertyDefinition(h);
				if (Safe(() => AnyInSignature(pd.DecodeSignature(this, null))) && mainModule.GetDefinition(h) is { } property)
					yield return property;
			}

			foreach (var h in metadata.EventDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				var ed = metadata.GetEventDefinition(h);
				if (UsesTarget(ed.Type) && mainModule.GetDefinition(h) is { } ev)
					yield return ev;
			}

			var argumentDecoder = new AttributeArgumentDecoder(this, mainModule);
			var referencedParameters = new HashSet<ParameterHandle>();
			foreach (var h in metadata.CustomAttributes)
			{
				ct.ThrowIfCancellationRequested();
				var attribute = metadata.GetCustomAttribute(h);
				if (!UsesTargetInAttributeConstructor(attribute.Constructor) && !UsesTargetInArguments(attribute, argumentDecoder))
					continue;
				if (attribute.Parent.Kind == HandleKind.Parameter)
				{
					referencedParameters.Add((ParameterHandle)attribute.Parent);
					continue;
				}
				var parent = AnalyzerHelpers.GetParentEntity(typeSystem, attribute);
				if (parent != null)
					yield return parent;
			}

			if (referencedParameters.Count == 0)
				yield break;
			foreach (var h in metadata.MethodDefinitions)
			{
				ct.ThrowIfCancellationRequested();
				if (metadata.GetMethodDefinition(h).GetParameters().Any(referencedParameters.Contains)
					&& mainModule.GetDefinition(h) is { } method)
				{
					yield return method.AccessorOwner ?? method;
				}
			}
		}

		/// <summary>
		/// True when the attribute constructor <paramref name="ctor"/> belongs to the target, or is a
		/// constructor of this module whose signature mentions the target (e.g. an enum parameter).
		/// </summary>
		bool UsesTargetInAttributeConstructor(EntityHandle ctor)
		{
			if (ctor.Kind != HandleKind.MethodDefinition)
				return UsesTarget(ctor);
			var md = metadata.GetMethodDefinition((MethodDefinitionHandle)ctor);
			return Safe(() => AnyInSignature(md.DecodeSignature(this, null)));
		}

		/// <summary>True when an argument of <paramref name="attribute"/> is or names a type of the target.</summary>
		static bool UsesTargetInArguments(CustomAttribute attribute, AttributeArgumentDecoder decoder)
		{
			return Safe(() => {
				var value = attribute.DecodeValue(decoder);
				return value.FixedArguments.Any(IsTargetArgument)
					|| value.NamedArguments.Any(a => IsTargetArgument(new(a.Type, a.Value)));
			});

			static bool IsTargetArgument(CustomAttributeTypedArgument<AttributeArgumentType> argument)
				=> argument.Type.UsesTarget
					|| argument.Value is AttributeArgumentType { UsesTarget: true }
					|| (argument.Value is ImmutableArray<CustomAttributeTypedArgument<AttributeArgumentType>> elements
						&& elements.Any(IsTargetArgument));
		}

		/// <summary>True when a parsed serialized type name names a type of the target, or is built from one.</summary>
		bool IsTargetTypeName(TypeName name)
		{
			if (name.IsArray || name.IsPointer || name.IsByRef)
				return IsTargetTypeName(name.GetElementType());
			if (name.IsConstructedGenericType)
				return IsTargetTypeName(name.GetGenericTypeDefinition()) || name.GetGenericArguments().Any(IsTargetTypeName);
			if (name.AssemblyName is not { } assemblyName)
				return false;
			var parsed = assemblyName.ToAssemblyName();
			return targetIdentities.Any(target =>
				string.Equals(target.Name, parsed.Name, StringComparison.OrdinalIgnoreCase)
				&& string.Equals(NormalizeCulture(target.Culture), NormalizeCulture(parsed.CultureName), StringComparison.OrdinalIgnoreCase)
				&& (target.PublicKeyToken ?? []).AsSpan().SequenceEqual(parsed.GetPublicKeyToken() ?? []));

			static string NormalizeCulture(string? culture)
				=> string.IsNullOrEmpty(culture) || culture == "neutral" ? string.Empty : culture;
		}

		static bool Safe(Func<bool> check)
		{
			try
			{
				return check();
			}
			catch (BadImageFormatException)
			{
				return false;
			}
			catch (EnumUnderlyingTypeResolveException)
			{
				return false;
			}
		}

		static bool AnyInSignature(MethodSignature<bool> signature)
			=> signature.ReturnType || signature.ParameterTypes.Any(p => p);

		bool UsesTargetInConstraints(GenericParameterHandleCollection parameters)
		{
			foreach (var h in parameters)
			{
				var gp = metadata.GetGenericParameter(h);
				foreach (var ch in gp.GetConstraints())
				{
					if (UsesTarget(metadata.GetGenericParameterConstraint(ch).Type))
						return true;
				}
			}
			return false;
		}

		bool UsesTargetInBody(MethodDefinition md)
		{
			if (md.RelativeVirtualAddress == 0)
				return false;
			MethodBodyBlock body;
			try
			{
				body = module.GetMethodBody(md.RelativeVirtualAddress);
			}
			catch (BadImageFormatException)
			{
				return false;
			}
			if (!body.LocalSignature.IsNil && UsesTarget(body.LocalSignature))
				return true;
			var reader = body.GetILReader();
			try
			{
				while (reader.RemainingBytes > 0)
				{
					var opCode = reader.DecodeOpCode();
					switch (opCode.GetOperandType())
					{
						case OperandType.Field:
						case OperandType.Method:
						case OperandType.Sig:
						case OperandType.Tok:
						case OperandType.Type:
							if (UsesTarget(MetadataTokenHelpers.EntityHandleOrNil(reader.ReadInt32())))
								return true;
							break;
						default:
							reader.SkipOperand(opCode);
							break;
					}
				}
			}
			catch (BadImageFormatException)
			{
				// A truncated or malformed body is scanned up to the point where it breaks.
			}
			return false;
		}

		/// <summary>
		/// True when <paramref name="handle"/> (a type or member reference, a type or method
		/// specification, or a standalone signature) mentions the target assembly.
		/// </summary>
		bool UsesTarget(EntityHandle handle)
		{
			if (handle.IsNil)
				return false;
			return Safe(() => {
				switch (handle.Kind)
				{
					case HandleKind.TypeReference:
						return IsTargetTypeReference((TypeReferenceHandle)handle);
					case HandleKind.TypeSpecification:
						return IsTargetTypeSpecification((TypeSpecificationHandle)handle);
					case HandleKind.MemberReference:
						var mr = metadata.GetMemberReference((MemberReferenceHandle)handle);
						if (UsesTarget(mr.Parent))
							return true;
						return mr.GetKind() switch {
							MemberReferenceKind.Method => AnyInSignature(mr.DecodeMethodSignature(this, null)),
							MemberReferenceKind.Field => mr.DecodeFieldSignature(this, null),
							_ => false,
						};
					case HandleKind.MethodSpecification:
						var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)handle);
						return UsesTarget(ms.Method) || ms.DecodeSignature(this, null).Any(t => t);
					case HandleKind.StandaloneSignature:
						var ss = metadata.GetStandaloneSignature((StandaloneSignatureHandle)handle);
						return ss.GetKind() switch {
							StandaloneSignatureKind.Method => AnyInSignature(ss.DecodeMethodSignature(this, null)),
							StandaloneSignatureKind.LocalVariables => ss.DecodeLocalSignature(this, null).Any(t => t),
							_ => false,
						};
					default:
						// Definitions in this module and module references are not the target.
						return false;
				}
			});
		}

		bool IsTargetTypeReference(TypeReferenceHandle handle)
		{
			if (typeReferenceCache.TryGetValue(handle, out bool cached))
				return cached;
			var scope = metadata.GetTypeReference(handle).ResolutionScope;
			bool result = scope.Kind switch {
				HandleKind.AssemblyReference => targetReferences.Contains((AssemblyReferenceHandle)scope),
				HandleKind.TypeReference => IsTargetTypeReference((TypeReferenceHandle)scope),
				_ => false,
			};
			typeReferenceCache[handle] = result;
			return result;
		}

		bool IsTargetTypeSpecification(TypeSpecificationHandle handle)
		{
			if (typeSpecificationCache.TryGetValue(handle, out bool cached))
				return cached;
			// Guards against a (malformed) self-referencing specification while it is decoded.
			typeSpecificationCache[handle] = false;
			bool result = metadata.GetTypeSpecification(handle).DecodeSignature(this, null);
			typeSpecificationCache[handle] = result;
			return result;
		}

		/// <summary>
		/// An attribute argument type as seen by <see cref="AttributeArgumentDecoder"/>: whether it
		/// mentions the target, the underlying type when it is an enum, and whether it is System.Type.
		/// </summary>
		readonly record struct AttributeArgumentType(bool UsesTarget, PrimitiveTypeCode EnumUnderlyingType = 0, bool IsSystemType = false);

		/// <summary>
		/// Decodes custom attribute blobs into <see cref="AttributeArgumentType"/>s. Target detection
		/// stays on raw metadata; enum underlying types, which the blob format requires, are resolved
		/// through the type system.
		/// </summary>
		sealed class AttributeArgumentDecoder(DependentCodeScanner scanner, MetadataModule mainModule)
			: ICustomAttributeTypeProvider<AttributeArgumentType>
		{
			public AttributeArgumentType GetPrimitiveType(PrimitiveTypeCode typeCode) => new(false);
			public AttributeArgumentType GetSystemType() => new(false, IsSystemType: true);
			public AttributeArgumentType GetSZArrayType(AttributeArgumentType elementType) => new(elementType.UsesTarget);
			public bool IsSystemType(AttributeArgumentType type) => type.IsSystemType;

			public AttributeArgumentType GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			{
				if (handle.IsEnum(reader, out PrimitiveTypeCode underlying))
					return new(false, underlying);
				return new(false, IsSystemType: ((EntityHandle)handle).IsKnownType(reader, KnownTypeCode.Type));
			}

			public AttributeArgumentType GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			{
				if (((EntityHandle)handle).IsKnownType(reader, KnownTypeCode.Type))
					return new(false, IsSystemType: true);
				return new(scanner.IsTargetTypeReference(handle), EnumUnderlyingTypeOf(mainModule.ResolveType(handle, default)));
			}

			public AttributeArgumentType GetTypeFromSerializedName(string name)
			{
				if (!TypeName.TryParse(name.AsSpan(), out var parsed))
					return new(false);
				IType? type;
				try
				{
					type = ReflectionHelper.ParseReflectionName(name, new SimpleTypeResolveContext(mainModule));
				}
				catch (ReflectionNameParseException)
				{
					type = null;
				}
				return new(scanner.IsTargetTypeName(parsed), EnumUnderlyingTypeOf(type));
			}

			public PrimitiveTypeCode GetUnderlyingEnumType(AttributeArgumentType type)
				=> type.EnumUnderlyingType != 0 ? type.EnumUnderlyingType : throw new EnumUnderlyingTypeResolveException();

			static PrimitiveTypeCode EnumUnderlyingTypeOf(IType? type)
				=> type?.GetDefinition()?.EnumUnderlyingType?.GetDefinition()?.KnownTypeCode.ToPrimitiveTypeCode() ?? 0;
		}

		#region ISignatureTypeProvider
		public bool GetArrayType(bool elementType, ArrayShape shape) => elementType;
		public bool GetByReferenceType(bool elementType) => elementType;
		public bool GetFunctionPointerType(MethodSignature<bool> signature) => AnyInSignature(signature);
		public bool GetGenericInstantiation(bool genericType, ImmutableArray<bool> typeArguments)
			=> genericType || typeArguments.Any(t => t);
		public bool GetGenericMethodParameter(object? genericContext, int index) => false;
		public bool GetGenericTypeParameter(object? genericContext, int index) => false;
		public bool GetModifiedType(bool modifier, bool unmodifiedType, bool isRequired) => modifier || unmodifiedType;
		public bool GetPinnedType(bool elementType) => elementType;
		public bool GetPointerType(bool elementType) => elementType;
		public bool GetPrimitiveType(PrimitiveTypeCode typeCode) => false;
		public bool GetSZArrayType(bool elementType) => elementType;
		public bool GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind) => false;
		public bool GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			=> IsTargetTypeReference(handle);
		public bool GetTypeFromSpecification(MetadataReader reader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
			=> IsTargetTypeSpecification(handle);
		#endregion
	}
}
