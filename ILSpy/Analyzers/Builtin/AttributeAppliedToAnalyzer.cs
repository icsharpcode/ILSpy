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

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	[ExportAnalyzer(Header = "Applied To", Order = 10)]
	class AttributeAppliedToAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			if (!(analyzedSymbol is ITypeDefinition attributeType))
				return Empty<ISymbol>.Array;

			var scope = context.GetScopeOf(attributeType);
			// TODO: DeclSecurity attributes are not supported.
			if (!IsBuiltinAttribute(attributeType, out var knownAttribute))
			{
				return HandleCustomAttribute(attributeType, scope, context.CancellationToken).Distinct();
			}
			else
			{
				return HandleBuiltinAttribute(knownAttribute, scope, context.CancellationToken).SelectMany(s => s);
			}
		}

		bool IsBuiltinAttribute(ITypeDefinition attributeType, out KnownAttribute knownAttribute)
		{
			knownAttribute = attributeType.IsBuiltinAttribute();
			return !knownAttribute.IsCustomAttribute();
		}

		IEnumerable<IEnumerable<ISymbol>> HandleBuiltinAttribute(KnownAttribute attribute, AnalyzerScope scope, CancellationToken ct)
		{
			IEnumerable<ISymbol> ScanTypes(DecompilerTypeSystem ts)
			{
				return ts.MainModule.TypeDefinitions
					.Where(t => t.HasAttribute(attribute));
			}

			IEnumerable<ISymbol> ScanMethods(DecompilerTypeSystem ts)
			{
				return ts.MainModule.TypeDefinitions
					.SelectMany(t => t.Members.OfType<IMethod>())
					.Where(m => m.HasAttribute(attribute))
					.Select(m => m.AccessorOwner ?? m);
			}

			IEnumerable<ISymbol> ScanFields(DecompilerTypeSystem ts)
			{
				return ts.MainModule.TypeDefinitions
					.SelectMany(t => t.Fields)
					.Where(f => f.HasAttribute(attribute));
			}

			IEnumerable<ISymbol> ScanProperties(DecompilerTypeSystem ts)
			{
				return ts.MainModule.TypeDefinitions
					.SelectMany(t => t.Properties)
					.Where(p => p.HasAttribute(attribute));
			}

			IEnumerable<ISymbol> ScanParameters(DecompilerTypeSystem ts)
			{
				return ts.MainModule.TypeDefinitions
					.SelectMany(t => t.Members.OfType<IMethod>())
					.Where(m => m.Parameters.Any(p => p.HasAttribute(attribute)))
					.Select(m => m.AccessorOwner ?? m);
			}

			foreach (Decompiler.Metadata.PEFile module in scope.GetModulesInScope(ct))
			{
				var ts = scope.ConstructTypeSystem(module);
				ct.ThrowIfCancellationRequested();

				switch (attribute)
				{
					case KnownAttribute.Serializable:
					case KnownAttribute.ComImport:
					case KnownAttribute.StructLayout:
						yield return ScanTypes(ts);
						break;
					case KnownAttribute.DllImport:
					case KnownAttribute.PreserveSig:
					case KnownAttribute.MethodImpl:
						yield return ScanMethods(ts);
						break;
					case KnownAttribute.FieldOffset:
					case KnownAttribute.NonSerialized:
						yield return ScanFields(ts);
						break;
					case KnownAttribute.MarshalAs:
						yield return ScanFields(ts);
						yield return ScanParameters(ts);
						goto case KnownAttribute.Out;
					case KnownAttribute.Optional:
					case KnownAttribute.In:
					case KnownAttribute.Out:
						yield return ScanParameters(ts);
						break;
					case KnownAttribute.IndexerName:
						yield return ScanProperties(ts);
						break;
				}
			}
		}

		IEnumerable<ISymbol> HandleCustomAttribute(ITypeDefinition attributeType, AnalyzerScope scope, CancellationToken ct)
		{
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type arguments do not matter for this analyzer.

			foreach (var module in scope.GetModulesInScope(ct))
			{
				var ts = scope.ConstructTypeSystem(module);
				ct.ThrowIfCancellationRequested();
				var decoder = new FindTypeDecoder(ts.MainModule, attributeType);

				var referencedParameters = new HashSet<ParameterHandle>();
				foreach (var h in module.Metadata.CustomAttributes)
				{
					var customAttribute = module.Metadata.GetCustomAttribute(h);
					if (IsCustomAttributeOfType(customAttribute.Constructor, module.Metadata, decoder))
					{
						if (customAttribute.Parent.Kind == HandleKind.Parameter)
						{
							referencedParameters.Add((ParameterHandle)customAttribute.Parent);
						}
						else
						{
							var parent = AnalyzerHelpers.GetParentEntity(ts, customAttribute);
							if (parent != null)
								yield return parent;
						}
					}
				}
				if (referencedParameters.Count > 0)
				{
					foreach (var h in module.Metadata.MethodDefinitions)
					{
						var md = module.Metadata.GetMethodDefinition(h);
						foreach (var p in md.GetParameters())
						{
							if (referencedParameters.Contains(p))
							{
								var method = ts.MainModule.ResolveMethod(h, genericContext);
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
		}

		internal static bool IsCustomAttributeOfType(EntityHandle customAttributeCtor, MetadataReader metadata, FindTypeDecoder decoder)
		{
			var declaringAttributeType = customAttributeCtor.GetDeclaringType(metadata);
			return decoder.GetTypeFromEntity(metadata, declaringAttributeType);
		}

		public bool Show(ISymbol symbol)
		{
			return symbol is ITypeDefinition type && type.GetNonInterfaceBaseTypes()
				.Any(t => t.IsKnownType(KnownTypeCode.Attribute));
		}
	}
}
