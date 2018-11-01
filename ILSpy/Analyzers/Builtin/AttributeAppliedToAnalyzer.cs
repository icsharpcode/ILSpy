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

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	[ExportAnalyzer(Header = "Applied To", Order = 10)]
	class AttributeAppliedToAnalyzer : IAnalyzer
	{
		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			if (!(analyzedSymbol is ITypeDefinition attributeType))
				return Array.Empty<ISymbol>();

			var scope = context.GetScopeOf(attributeType);
			// TODO: The IndexerNameAttribute needs special support, because it is not available on IL level. Do we want to support it?
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
					return true;
				default:
					return false;
			}
		}

		IEnumerable<IEnumerable<ISymbol>> HandleBuiltinAttribute(KnownAttribute attributeType, AnalyzerScope scope)
		{
			// For built-in attributes (i.e. metadata flags) we have to perform the same checks
			// as implemented in the GetAttributes methods of MetadataField, MetadataMethod, MetadataParameter, etc.
			foreach (Decompiler.Metadata.PEFile module in scope.GetAllModules()) {
				var ts = new DecompilerTypeSystem(module, module.GetAssemblyResolver());

				switch (attributeType) {
					case KnownAttribute.Serializable:
						yield return ScanDefinitions(ts, TypeAttributes.Serializable);
						break;
					case KnownAttribute.ComImport:
						yield return ScanDefinitions(ts, TypeAttributes.Import);
						break;
					case KnownAttribute.StructLayout:
						yield return ScanDefinitions(ts, matcher: HasStructLayout);
						break;
					case KnownAttribute.DllImport:
						yield return ScanDefinitions(ts, matcher: IsPinvokeImpl);
						break;
					case KnownAttribute.PreserveSig:
						yield return ScanDefinitions(ts, matcher: (mod, m) => !IsPinvokeImpl(mod, m) && (m.ImplAttributes & ~MethodImplAttributes.CodeTypeMask) == MethodImplAttributes.PreserveSig);
						break;
					case KnownAttribute.MethodImpl:
						yield return ScanDefinitions(ts, matcher: HasMethodImplOptions);
						break;
					case KnownAttribute.FieldOffset:
						yield return ScanDefinitions(ts, matcher: (mod, f) => f.GetOffset() != -1);
						break;
					case KnownAttribute.NonSerialized:
						yield return ScanDefinitions(ts, FieldAttributes.NotSerialized);
						break;
					case KnownAttribute.MarshalAs:
						yield return ScanDefinitions(ts, matcher: (mod, f) => !f.GetMarshallingDescriptor().IsNil);
						yield return ScanParameters(ts, matcher: (mod, _, p) => !p.GetMarshallingDescriptor().IsNil);
						break;
					case KnownAttribute.Optional:
						yield return ScanParameters(ts, matcher: (mod, m, p) => (p.Attributes & ParameterAttributes.Optional) != 0 && !CheckTSParamFlag(ts.MainModule, m, tsp => tsp.HasConstantValueInSignature));
						break;
					case KnownAttribute.In:
						yield return ScanParameters(ts, matcher: (mod, m, p) => (p.Attributes & ParameterAttributes.In) == ParameterAttributes.In && CheckTSParamFlag(ts.MainModule, m, tsp => tsp.HasAttribute(KnownAttribute.In)));
						break;
					case KnownAttribute.Out:
						yield return ScanParameters(ts, matcher: (mod, m, p) => (p.Attributes & ParameterAttributes.Out) == ParameterAttributes.Out && CheckTSParamFlag(ts.MainModule, m, tsp => tsp.HasAttribute(KnownAttribute.Out)));
						break;
				}
			}
		}

		private bool CheckTSParamFlag(MetadataModule module, MethodDefinitionHandle h, Func<IParameter, bool> matcher)
		{
			var m = module.GetDefinition(h);
			return m.Parameters.Any(matcher);
		}

		private bool HasMethodImplOptions(Decompiler.Metadata.PEFile module, MethodDefinition m)
		{
			var implOptions = m.ImplAttributes & ~MethodImplAttributes.CodeTypeMask;
			if (IsPinvokeImpl(module, m)) {
				implOptions &= ~MethodImplAttributes.PreserveSig;
			}
			if (implOptions == MethodImplAttributes.PreserveSig)
				return false;
			return implOptions != 0;
		}

		private bool IsPinvokeImpl(Decompiler.Metadata.PEFile module, MethodDefinition m)
		{
			var info = m.GetImport();
			return (m.Attributes & MethodAttributes.PinvokeImpl) == MethodAttributes.PinvokeImpl && !info.Module.IsNil;
		}

		private bool HasStructLayout(Decompiler.Metadata.PEFile module, TypeDefinition t)
		{
			LayoutKind layoutKind = LayoutKind.Auto;
			switch (t.Attributes & TypeAttributes.LayoutMask) {
				case TypeAttributes.SequentialLayout:
					layoutKind = LayoutKind.Sequential;
					break;
				case TypeAttributes.ExplicitLayout:
					layoutKind = LayoutKind.Explicit;
					break;
			}
			CharSet charSet = CharSet.None;
			switch (t.Attributes & TypeAttributes.StringFormatMask) {
				case TypeAttributes.AnsiClass:
					charSet = CharSet.Ansi;
					break;
				case TypeAttributes.AutoClass:
					charSet = CharSet.Auto;
					break;
				case TypeAttributes.UnicodeClass:
					charSet = CharSet.Unicode;
					break;
			}
			var defaultLayoutKind = t.IsValueType(module.Metadata) && !t.IsEnum(module.Metadata) ? LayoutKind.Sequential : LayoutKind.Auto;
			var layout = t.GetLayout();
			return layoutKind != defaultLayoutKind || charSet != CharSet.Ansi || layout.PackingSize > 0 || layout.Size > 0;
		}

		IEnumerable<ISymbol> ScanDefinitions(DecompilerTypeSystem ts, TypeAttributes attribute = 0, Func<Decompiler.Metadata.PEFile, TypeDefinition, bool> matcher = null)
		{
			var module = ts.MainModule.PEFile;
			foreach (var h in module.Metadata.TypeDefinitions) {
				var t = module.Metadata.GetTypeDefinition(h);
				if (matcher != null) {
					if (!matcher(module, t)) continue;
				} else {
					if ((t.Attributes & attribute) == 0) continue;
				}
				yield return ts.MainModule.GetDefinition(h);
			}
		}

		IEnumerable<ISymbol> ScanDefinitions(DecompilerTypeSystem ts, MethodAttributes attribute = 0, Func<Decompiler.Metadata.PEFile, MethodDefinition, bool> matcher = null)
		{
			var module = ts.MainModule.PEFile;
			foreach (var h in module.Metadata.MethodDefinitions) {
				var m = module.Metadata.GetMethodDefinition(h);
				if (matcher != null) {
					if (!matcher(module, m)) continue;
				} else {
					if ((m.Attributes & attribute) == 0) continue;
				}
				yield return ts.MainModule.GetDefinition(h);
			}
		}

		IEnumerable<ISymbol> ScanDefinitions(DecompilerTypeSystem ts, FieldAttributes attribute = 0, Func<Decompiler.Metadata.PEFile, FieldDefinition, bool> matcher = null)
		{
			var module = ts.MainModule.PEFile;
			foreach (var h in module.Metadata.FieldDefinitions) {
				var f = module.Metadata.GetFieldDefinition(h);
				if (matcher != null) {
					if (!matcher(module, f)) continue;
				} else {
					if ((f.Attributes & attribute) == 0) continue;
				}
				yield return ts.MainModule.GetDefinition(h);
			}
		}

		IEnumerable<ISymbol> ScanDefinitions(DecompilerTypeSystem ts, PropertyAttributes attribute = 0, Func<Decompiler.Metadata.PEFile, PropertyDefinition, bool> matcher = null)
		{
			var module = ts.MainModule.PEFile;
			foreach (var h in module.Metadata.PropertyDefinitions) {
				var p = module.Metadata.GetPropertyDefinition(h);
				if (matcher != null) {
					if (!matcher(module, p)) continue;
				} else {
					if ((p.Attributes & attribute) == 0) continue;
				}
				yield return ts.MainModule.GetDefinition(h);
			}
		}

		IEnumerable<ISymbol> ScanDefinitions(DecompilerTypeSystem ts, EventAttributes attribute)
		{
			var module = ts.MainModule.PEFile;
			foreach (var h in module.Metadata.EventDefinitions) {
				var e = module.Metadata.GetEventDefinition(h);
				if ((e.Attributes & attribute) == 0) continue;
				yield return ts.MainModule.GetDefinition(h);
			}
		}

		IEnumerable<ISymbol> ScanParameters(DecompilerTypeSystem ts, ParameterAttributes attribute = 0, Func<Decompiler.Metadata.PEFile, MethodDefinitionHandle, Parameter, bool> matcher = null)
		{
			var module = ts.MainModule.PEFile;
			var genericContext = new GenericContext();
			foreach (var h in module.Metadata.MethodDefinitions) {
				var m = module.Metadata.GetMethodDefinition(h);
				foreach (var ph in m.GetParameters()) {
					var p = module.Metadata.GetParameter(ph);
					if (matcher != null) {
						if (!matcher(module, h, p)) continue;
					} else {
						if ((p.Attributes & attribute) == 0) continue;
					}
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
