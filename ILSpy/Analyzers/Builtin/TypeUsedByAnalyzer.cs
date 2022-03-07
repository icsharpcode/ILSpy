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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that use a type.
	/// </summary>
	[ExportAnalyzer(Header = "Used By", Order = 30)]
	class TypeUsedByAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is ITypeDefinition);
			var analyzedType = (ITypeDefinition)analyzedSymbol;
			var scope = context.GetScopeOf(analyzedType);
			context.SortResults = true;

			return scope.GetModulesInScope(context.CancellationToken)
				.AsParallel().AsOrdered()
				.SelectMany(AnalyzeModuleAndFilter);

			IEnumerable<ISymbol> AnalyzeModuleAndFilter(PEFile module)
			{
				return AnalyzeModule(analyzedType, scope, module)
					.Distinct()
					.Where(s => !analyzedType.Equals((s as IEntity)?.DeclaringTypeDefinition));
			}
		}

		static IEnumerable<ISymbol> AnalyzeModule(ITypeDefinition analyzedType, AnalyzerScope scope, PEFile module)
		{
			var metadata = module.Metadata;
			var typeSystem = scope.ConstructTypeSystem(module);
			var decoder = new FindTypeDecoder(typeSystem.MainModule, analyzedType);

			//// resolve type refs
			//int rowCount = metadata.GetTableRowCount(TableIndex.TypeRef);
			//BitSet typeReferences = new BitSet(rowCount);
			//for (int row = 0; row < rowCount; row++)
			//{
			//	var h = MetadataTokens.TypeReferenceHandle(row + 1);
			//	typeReferences[row] = decoder.GetTypeFromReference(metadata, h, 0);
			//}

			//// resolve type specs
			//rowCount = metadata.GetTableRowCount(TableIndex.TypeSpec);
			//BitSet typeSpecifications = new BitSet(rowCount);
			//for (int row = 0; row < rowCount; row++)
			//{
			//	var h = MetadataTokens.TypeSpecificationHandle(row + 1);
			//	typeSpecifications[row] = decoder.GetTypeFromSpecification(metadata, default, h, 0);
			//}

			foreach (ISymbol result in FindUsesInAttributes(typeSystem, metadata, decoder, analyzedType))
				yield return result;

			foreach (var h in metadata.TypeDefinitions)
			{
				var td = metadata.GetTypeDefinition(h);
				bool found = decoder.GetTypeFromEntity(metadata, td.GetBaseTypeOrNil());
				foreach (var ih in td.GetInterfaceImplementations())
				{
					var ii = metadata.GetInterfaceImplementation(ih);
					found |= decoder.GetTypeFromEntity(metadata, ii.Interface);
				}

				found |= FindUsesInGenericConstraints(metadata, td.GetGenericParameters(), decoder);

				if (found)
					yield return typeSystem.MainModule.GetDefinition(h);
			}

			foreach (var h in metadata.MethodDefinitions)
			{
				var md = metadata.GetMethodDefinition(h);
				var msig = md.DecodeSignature(decoder, default);
				bool found = FindTypeDecoder.AnyInMethodSignature(msig);
				found |= FindUsesInGenericConstraints(metadata, md.GetGenericParameters(), decoder);
				if (found || ScanMethodBody(analyzedType, module, md, decoder))
				{
					var method = typeSystem.MainModule.GetDefinition(h);
					yield return method?.AccessorOwner ?? method;
				}
			}

			foreach (var h in metadata.FieldDefinitions)
			{
				var fd = metadata.GetFieldDefinition(h);
				if (fd.DecodeSignature(decoder, default))
					yield return typeSystem.MainModule.GetDefinition(h);
			}

			foreach (var h in metadata.PropertyDefinitions)
			{
				var pd = metadata.GetPropertyDefinition(h);
				var psig = pd.DecodeSignature(decoder, default);
				if (FindTypeDecoder.AnyInMethodSignature(psig))
					yield return typeSystem.MainModule.GetDefinition(h);
			}

			foreach (var h in metadata.EventDefinitions)
			{
				var ed = metadata.GetEventDefinition(h);
				if (decoder.GetTypeFromEntity(metadata, ed.Type))
					yield return typeSystem.MainModule.GetDefinition(h);
			}
		}

		static bool FindUsesInGenericConstraints(MetadataReader metadata, GenericParameterHandleCollection collection, FindTypeDecoder decoder)
		{
			foreach (var h in collection)
			{
				var gp = metadata.GetGenericParameter(h);
				foreach (var hc in gp.GetConstraints())
				{
					var gc = metadata.GetGenericParameterConstraint(hc);
					if (decoder.GetTypeFromEntity(metadata, gc.Type))
						return true;
				}
			}
			return false;
		}

		static IEnumerable<ISymbol> FindUsesInAttributes(DecompilerTypeSystem typeSystem, MetadataReader metadata, FindTypeDecoder decoder, ITypeDefinition analyzedType)
		{
			var attrDecoder = new FindTypeInAttributeDecoder(typeSystem.MainModule, analyzedType);
			var referencedParameters = new HashSet<ParameterHandle>();

			foreach (var h in metadata.CustomAttributes)
			{
				var customAttribute = metadata.GetCustomAttribute(h);
				CustomAttributeValue<TokenSearchResult> value;
				try
				{
					value = customAttribute.DecodeValue(attrDecoder);
				}
				catch (EnumUnderlyingTypeResolveException)
				{
					continue;
				}
				if (AttributeAppliedToAnalyzer.IsCustomAttributeOfType(customAttribute.Constructor, metadata, decoder)
					|| AnalyzeCustomAttributeValue(value))
				{
					if (customAttribute.Parent.Kind == HandleKind.Parameter)
					{
						referencedParameters.Add((ParameterHandle)customAttribute.Parent);
					}
					else
					{
						var parent = AnalyzerHelpers.GetParentEntity(typeSystem, customAttribute);
						if (parent != null)
							yield return parent;
					}
				}
			}
			if (referencedParameters.Count > 0)
			{
				foreach (var h in metadata.MethodDefinitions)
				{
					var md = metadata.GetMethodDefinition(h);
					foreach (var p in md.GetParameters())
					{
						if (referencedParameters.Contains(p))
						{
							var method = typeSystem.MainModule.ResolveMethod(h, default);
							if (method != null)
							{
								if (method.IsAccessor)
									yield return method.AccessorOwner;
								else
									yield return method;
							}
							break;
						}
					}
				}
			}

		}

		private static bool AnalyzeCustomAttributeValue(CustomAttributeValue<TokenSearchResult> attribute)
		{
			foreach (var fa in attribute.FixedArguments)
			{
				if (CheckAttributeValue(fa.Value))
					return true;
			}

			foreach (var na in attribute.NamedArguments)
			{
				if (CheckAttributeValue(na.Value))
					return true;
			}
			return false;

			bool CheckAttributeValue(object value)
			{
				if (value is TokenSearchResult typeofType)
				{
					if ((typeofType & TokenSearchResult.Found) != 0)
						return true;
				}
				else if (value is ImmutableArray<CustomAttributeTypedArgument<IType>> arr)
				{
					foreach (var element in arr)
					{
						if (CheckAttributeValue(element.Value))
							return true;
					}
				}

				return false;
			}
		}

		static bool ScanMethodBody(ITypeDefinition analyzedType, PEFile module, in MethodDefinition md, FindTypeDecoder decoder)
		{
			if (!md.HasBody())
				return false;

			var methodBody = module.Reader.GetMethodBody(md.RelativeVirtualAddress);
			var metadata = module.Metadata;

			if (!methodBody.LocalSignature.IsNil)
			{
				try
				{
					var ss = metadata.GetStandaloneSignature(methodBody.LocalSignature);
					if (HandleStandaloneSignature(ss))
					{
						return true;
					}
				}
				catch (BadImageFormatException)
				{
					// Issue #2197: ignore invalid local signatures
				}
			}

			var blob = methodBody.GetILReader();

			while (blob.RemainingBytes > 0)
			{
				var opCode = blob.DecodeOpCode();
				switch (opCode.GetOperandType())
				{
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
					case OperandType.Type:
						if (HandleMember(MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32())))
							return true;
						break;
					default:
						blob.SkipOperand(opCode);
						break;
				}
			}

			return false;

			bool HandleMember(EntityHandle member)
			{
				if (member.IsNil)
					return false;
				switch (member.Kind)
				{
					case HandleKind.TypeReference:
						return decoder.GetTypeFromReference(metadata, (TypeReferenceHandle)member, 0);

					case HandleKind.TypeSpecification:
						return decoder.GetTypeFromSpecification(metadata, default, (TypeSpecificationHandle)member, 0);

					case HandleKind.TypeDefinition:
						return decoder.GetTypeFromDefinition(metadata, (TypeDefinitionHandle)member, 0);

					case HandleKind.FieldDefinition:
						var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)member);
						return HandleMember(fd.GetDeclaringType()) || fd.DecodeSignature(decoder, default);

					case HandleKind.MethodDefinition:
						var md = metadata.GetMethodDefinition((MethodDefinitionHandle)member);
						if (HandleMember(md.GetDeclaringType()))
							return true;
						var msig = md.DecodeSignature(decoder, default);
						if (msig.ReturnType)
							return true;
						foreach (var t in msig.ParameterTypes)
						{
							if (t)
								return true;
						}
						break;

					case HandleKind.MemberReference:
						var mr = metadata.GetMemberReference((MemberReferenceHandle)member);
						if (HandleMember(mr.Parent))
							return true;
						switch (mr.GetKind())
						{
							case MemberReferenceKind.Method:
								msig = mr.DecodeMethodSignature(decoder, default);
								if (msig.ReturnType)
									return true;
								foreach (var t in msig.ParameterTypes)
								{
									if (t)
										return true;
								}
								break;
							case MemberReferenceKind.Field:
								return mr.DecodeFieldSignature(decoder, default);
						}
						break;

					case HandleKind.MethodSpecification:
						var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)member);
						if (HandleMember(ms.Method))
							return true;
						var mssig = ms.DecodeSignature(decoder, default);
						foreach (var t in mssig)
						{
							if (t)
								return true;
						}
						break;

					case HandleKind.StandaloneSignature:
						var ss = metadata.GetStandaloneSignature((StandaloneSignatureHandle)member);
						return HandleStandaloneSignature(ss);
				}
				return false;
			}

			bool HandleStandaloneSignature(StandaloneSignature signature)
			{
				switch (signature.GetKind())
				{
					case StandaloneSignatureKind.Method:
						var msig = signature.DecodeMethodSignature(decoder, default);
						if (msig.ReturnType)
							return true;
						foreach (var t in msig.ParameterTypes)
						{
							if (t)
								return true;
						}
						break;
					case StandaloneSignatureKind.LocalVariables:
						var sig = signature.DecodeLocalSignature(decoder, default);
						foreach (var t in sig)
						{
							if (t)
								return true;
						}
						break;
				}
				return false;
			}
		}

		public bool Show(ISymbol symbol) => symbol is ITypeDefinition;
	}

	class TypeDefinitionUsedVisitor : TypeVisitor
	{
		public readonly ITypeDefinition TypeDefinition;

		public bool Found { get; set; }

		readonly bool topLevelOnly;

		public TypeDefinitionUsedVisitor(ITypeDefinition definition, bool topLevelOnly)
		{
			this.TypeDefinition = definition;
			this.topLevelOnly = topLevelOnly;
		}

		public override IType VisitTypeDefinition(ITypeDefinition type)
		{
			Found |= TypeDefinition.MetadataToken == type.MetadataToken
				&& TypeDefinition.ParentModule.PEFile == type.ParentModule.PEFile;
			return base.VisitTypeDefinition(type);
		}

		public override IType VisitParameterizedType(ParameterizedType type)
		{
			if (topLevelOnly)
				return type.GenericType.AcceptVisitor(this);
			return base.VisitParameterizedType(type);
		}
	}
}
