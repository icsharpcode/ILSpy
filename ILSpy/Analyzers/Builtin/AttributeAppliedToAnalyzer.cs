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
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using ICSharpCode.Decompiler;
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
			if (!IsBuiltinAttribute(attributeType, out var knownAttribute)) {
				return HandleCustomAttribute(attributeType, scope);
			} else {
				return HandleBuiltinAttribute(knownAttribute, scope).SelectMany(s => s);
			}
		}

		bool IsBuiltinAttribute(ITypeDefinition attributeType, out KnownAttribute knownAttribute)
		{
			knownAttribute = attributeType.IsBuiltinAttribute();
			switch (knownAttribute) {
				case KnownAttribute.Serializable:
				case KnownAttribute.ComImport:
				case KnownAttribute.StructLayout:
				case KnownAttribute.DllImport:
				case KnownAttribute.PreserveSig:
				case KnownAttribute.MethodImpl:
				case KnownAttribute.FieldOffset:
				case KnownAttribute.NonSerialized:
				case KnownAttribute.MarshalAs:
				case KnownAttribute.PermissionSet:
				case KnownAttribute.Optional:
				case KnownAttribute.In:
				case KnownAttribute.Out:
				case KnownAttribute.IndexerName:
					return true;
				default:
					return false;
			}
		}

		IEnumerable<IEnumerable<ISymbol>> HandleBuiltinAttribute(KnownAttribute attribute, AnalyzerScope scope)
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

			foreach (Decompiler.Metadata.PEFile module in scope.GetAllModules()) {
				var ts = new DecompilerTypeSystem(module, module.GetAssemblyResolver());

				switch (attribute) {
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

		IEnumerable<ISymbol> HandleCustomAttribute(ITypeDefinition attributeType, AnalyzerScope scope)
		{
			var genericContext = new GenericContext(); // type arguments do not matter for this analyzer.

			foreach (var module in scope.GetAllModules()) {
				var ts = new DecompilerTypeSystem(module, module.GetAssemblyResolver());
				var referencedParameters = new HashSet<ParameterHandle>();
				foreach (var h in module.Metadata.CustomAttributes) {
					var customAttribute = module.Metadata.GetCustomAttribute(h);
					var attributeCtor = ts.MainModule.ResolveMethod(customAttribute.Constructor, genericContext);
					if (attributeCtor.DeclaringTypeDefinition != null
						&& attributeCtor.ParentModule.PEFile == attributeType.ParentModule.PEFile
						&& attributeCtor.DeclaringTypeDefinition.MetadataToken == attributeType.MetadataToken) {
						if (customAttribute.Parent.Kind == HandleKind.Parameter) {
							referencedParameters.Add((ParameterHandle)customAttribute.Parent);
						} else {
							var parent = GetParentEntity(ts, customAttribute);
							if (parent != null)
								yield return parent;
						}
					}
				}
				if (referencedParameters.Count > 0) {
					foreach (var h in module.Metadata.MethodDefinitions) {
						var md = module.Metadata.GetMethodDefinition(h);
						foreach (var p in md.GetParameters()) {
							if (referencedParameters.Contains(p)) {
								var method = ts.MainModule.ResolveMethod(h, genericContext);
								if (method != null) {
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

		ISymbol GetParentEntity(DecompilerTypeSystem ts, CustomAttribute customAttribute)
		{
			var metadata = ts.MainModule.PEFile.Metadata;
			switch (customAttribute.Parent.Kind) {
				case HandleKind.MethodDefinition:
				case HandleKind.FieldDefinition:
				case HandleKind.PropertyDefinition:
				case HandleKind.EventDefinition:
				case HandleKind.TypeDefinition:
					return ts.MainModule.ResolveEntity(customAttribute.Parent);
				case HandleKind.AssemblyDefinition:
				case HandleKind.ModuleDefinition:
					return ts.MainModule;
				case HandleKind.GenericParameterConstraint:
					var gpc = metadata.GetGenericParameterConstraint((GenericParameterConstraintHandle)customAttribute.Parent);
					var gp = metadata.GetGenericParameter(gpc.Parameter);
					return ts.MainModule.ResolveEntity(gp.Parent);
				case HandleKind.GenericParameter:
					gp = metadata.GetGenericParameter((GenericParameterHandle)customAttribute.Parent);
					return ts.MainModule.ResolveEntity(gp.Parent);
				default:
					return null;
			}
		}

		public bool Show(ISymbol symbol)
		{
			return symbol is ITypeDefinition type && type.GetNonInterfaceBaseTypes()
				.Any(t => t.IsKnownType(KnownTypeCode.Attribute));
		}
	}
}
