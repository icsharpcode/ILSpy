// Copyright (c) 2014 Daniel Grunwald
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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.IL
{
	public static partial class InstructionOutputExtensions
	{
		public static void Write(this ITextOutput output, OpCode opCode)
		{
			output.Write(originalOpCodeNames[(int)opCode]);
		}

		public static void Write(this ITextOutput output, StackType stackType)
		{
			output.Write(stackType.ToString().ToLowerInvariant());
		}

		public static void Write(this ITextOutput output, PrimitiveType primitiveType)
		{
			output.Write(primitiveType.ToString().ToLowerInvariant());
		}
		
		public static void WriteTo(this IType type, ITextOutput output, ILNameSyntax nameSyntax = ILNameSyntax.ShortTypeName)
		{
			output.WriteReference(type, type.ReflectionName);
		}
		
		public static void WriteTo(this IMember member, ITextOutput output)
		{
			if (member is IMethod method && method.IsConstructor)
				output.WriteReference(member, method.DeclaringType?.Name + "." + method.Name);
			else
				output.WriteReference(member, member.Name);
		}

		public static void WriteTo(this Interval interval, ITextOutput output, ILAstWritingOptions options)
		{
			if (!options.ShowILRanges)
				return;
			if (interval.IsEmpty)
				output.Write("[empty] ");
			else
				output.Write($"[{interval.Start:x4}..{interval.InclusiveEnd:x4}] ");
		}

		public static void WriteTo(this EntityHandle entity, PEFile module, ITextOutput output, Metadata.GenericContext genericContext, ILNameSyntax syntax = ILNameSyntax.Signature)
		{
			if (entity.IsNil) {
				output.Write("<nil>");
				return;
			}
			if (module == null)
				throw new ArgumentNullException(nameof(module));
			var metadata = module.Metadata;
			Action<ILNameSyntax> signature;
			MethodSignature<Action<ILNameSyntax>> methodSignature;
			string memberName;
			switch (entity.Kind) {
				case HandleKind.TypeDefinition: {
						var td = metadata.GetTypeDefinition((TypeDefinitionHandle)entity);
						output.WriteReference(module, entity, td.GetFullTypeName(metadata).ToILNameString());
						break;
					}
				case HandleKind.TypeReference: {
						var tr = metadata.GetTypeReference((TypeReferenceHandle)entity);
						EntityHandle resolutionScope;
						try {
							resolutionScope = tr.ResolutionScope;
						} catch (BadImageFormatException) {
							resolutionScope = default;
						}
						if (!resolutionScope.IsNil) {
							output.Write("[");
							var currentTypeRef = tr;
							while (currentTypeRef.ResolutionScope.Kind == HandleKind.TypeReference) {
								currentTypeRef = metadata.GetTypeReference((TypeReferenceHandle)currentTypeRef.ResolutionScope);
							}
							switch (currentTypeRef.ResolutionScope.Kind) {
								case HandleKind.ModuleDefinition:
									var modDef = metadata.GetModuleDefinition();
									output.Write(DisassemblerHelpers.Escape(metadata.GetString(modDef.Name)));
									break;
								case HandleKind.ModuleReference:
									break;
								case HandleKind.AssemblyReference:
									var asmRef = metadata.GetAssemblyReference((AssemblyReferenceHandle)currentTypeRef.ResolutionScope);
									output.Write(DisassemblerHelpers.Escape(metadata.GetString(asmRef.Name)));
									break;
							}
							output.Write("]");
						}
						output.WriteReference(module, entity, entity.GetFullTypeName(metadata).ToILNameString());
						break;
					}
				case HandleKind.TypeSpecification: {
						var ts = metadata.GetTypeSpecification((TypeSpecificationHandle)entity);
						signature = ts.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
						signature(syntax);
						break;
					}
				case HandleKind.FieldDefinition: {
						var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)entity);
						signature = fd.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), new Metadata.GenericContext(fd.GetDeclaringType(), module));
						signature(ILNameSyntax.SignatureNoNamedTypeParameters);
						output.Write(' ');
						((EntityHandle)fd.GetDeclaringType()).WriteTo(module, output, Metadata.GenericContext.Empty, ILNameSyntax.TypeName);
						output.Write("::");
						output.WriteReference(module, entity, DisassemblerHelpers.Escape(metadata.GetString(fd.Name)));
						break;
					}
				case HandleKind.MethodDefinition: {
						var md = metadata.GetMethodDefinition((MethodDefinitionHandle)entity);
						methodSignature = md.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), new Metadata.GenericContext((MethodDefinitionHandle)entity, module));
						WriteSignatureHeader(output, methodSignature);
						methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
						output.Write(' ');
						var declaringType = md.GetDeclaringType();
						if (!declaringType.IsNil) {
							((EntityHandle)declaringType).WriteTo(module, output, genericContext, ILNameSyntax.TypeName);
							output.Write("::");
						}
						bool isCompilerControlled = (md.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
						if (isCompilerControlled) {
							output.WriteReference(module, entity, DisassemblerHelpers.Escape(metadata.GetString(md.Name) + "$PST" + MetadataTokens.GetToken(entity).ToString("X8")));
						} else {
							output.WriteReference(module, entity, DisassemblerHelpers.Escape(metadata.GetString(md.Name)));
						}
						var genericParameters = md.GetGenericParameters();
						if (genericParameters.Count > 0) {
							output.Write('<');
							for (int i = 0; i < genericParameters.Count; i++) {
								if (i > 0)
									output.Write(", ");
								var gp = metadata.GetGenericParameter(genericParameters[i]);
								if ((gp.Attributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.ReferenceTypeConstraint) {
									output.Write("class ");
								} else if ((gp.Attributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.NotNullableValueTypeConstraint) {
									output.Write("valuetype ");
								}
								if ((gp.Attributes & GenericParameterAttributes.DefaultConstructorConstraint) == GenericParameterAttributes.DefaultConstructorConstraint) {
									output.Write(".ctor ");
								}
								var constraints = gp.GetConstraints();
								if (constraints.Count > 0) {
									output.Write('(');
									for (int j = 0; j < constraints.Count; j++) {
										if (j > 0)
											output.Write(", ");
										var constraint = metadata.GetGenericParameterConstraint(constraints[j]);
										constraint.Type.WriteTo(module, output, new Metadata.GenericContext((MethodDefinitionHandle)entity, module), ILNameSyntax.TypeName);
									}
									output.Write(") ");
								}
								if ((gp.Attributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant) {
									output.Write('-');
								} else if ((gp.Attributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant) {
									output.Write('+');
								}
								output.Write(DisassemblerHelpers.Escape(metadata.GetString(gp.Name)));
							}
							output.Write('>');
						}
						WriteParameterList(output, methodSignature);
						break;
					}
				case HandleKind.MemberReference:
					var mr = metadata.GetMemberReference((MemberReferenceHandle)entity);
					memberName = metadata.GetString(mr.Name);
					switch (mr.GetKind()) {
						case MemberReferenceKind.Method:
							methodSignature = mr.DecodeMethodSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
							WriteSignatureHeader(output, methodSignature);
							methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write(' ');
							WriteParent(output, module, metadata, mr.Parent, genericContext, syntax);
							output.Write("::");
							output.WriteReference(module, entity, DisassemblerHelpers.Escape(memberName));
							WriteParameterList(output, methodSignature);
							break;
						case MemberReferenceKind.Field:
							var fieldSignature = mr.DecodeFieldSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
							fieldSignature(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write(' ');
							WriteParent(output, module, metadata, mr.Parent, genericContext, syntax);
							output.Write("::");
							output.WriteReference(module, entity, DisassemblerHelpers.Escape(memberName));
							break;
					}
					break;
				case HandleKind.MethodSpecification:
					var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)entity);
					var substitution = ms.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
					switch (ms.Method.Kind) {
						case HandleKind.MethodDefinition:
							var methodDefinition = metadata.GetMethodDefinition((MethodDefinitionHandle)ms.Method);
							var methodName = metadata.GetString(methodDefinition.Name);
							methodSignature = methodDefinition.DecodeSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
							WriteSignatureHeader(output, methodSignature);
							methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write(' ');
							var declaringType = methodDefinition.GetDeclaringType();
							if (!declaringType.IsNil) {
								((EntityHandle)declaringType).WriteTo(module, output, genericContext, ILNameSyntax.TypeName);
								output.Write("::");
							}
							bool isCompilerControlled = (methodDefinition.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope;
							if (isCompilerControlled) {
								output.Write(DisassemblerHelpers.Escape(methodName + "$PST" + MetadataTokens.GetToken(ms.Method).ToString("X8")));
							} else {
								output.Write(DisassemblerHelpers.Escape(methodName));
							}
							WriteTypeParameterList(output, syntax, substitution);
							WriteParameterList(output, methodSignature);
							break;
						case HandleKind.MemberReference:
							var memberReference = metadata.GetMemberReference((MemberReferenceHandle)ms.Method);
							memberName = metadata.GetString(memberReference.Name);
							methodSignature = memberReference.DecodeMethodSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
							WriteSignatureHeader(output, methodSignature);
							methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
							output.Write(' ');
							WriteParent(output, module, metadata, memberReference.Parent, genericContext, syntax);
							output.Write("::");
							output.Write(DisassemblerHelpers.Escape(memberName));
							WriteTypeParameterList(output, syntax, substitution);
							WriteParameterList(output, methodSignature);
							break;
					}
					break;
				case HandleKind.StandaloneSignature:
					var standaloneSig = metadata.GetStandaloneSignature((StandaloneSignatureHandle)entity);
					switch (standaloneSig.GetKind()) {
						case StandaloneSignatureKind.Method:
							methodSignature = standaloneSig.DecodeMethodSignature(new DisassemblerSignatureTypeProvider(module, output), genericContext);
							WriteSignatureHeader(output, methodSignature);
							methodSignature.ReturnType(ILNameSyntax.SignatureNoNamedTypeParameters);
							WriteParameterList(output, methodSignature);
							break;
						case StandaloneSignatureKind.LocalVariables:
						default:
							output.Write($"@{MetadataTokens.GetToken(entity):X8} /* signature {standaloneSig.GetKind()} */");
							break;
					}
					break;
				default:
					output.Write($"@{MetadataTokens.GetToken(entity):X8}");
					break;
			}
		}

		static void WriteTypeParameterList(ITextOutput output, ILNameSyntax syntax, System.Collections.Immutable.ImmutableArray<Action<ILNameSyntax>> substitution)
		{
			output.Write('<');
			for (int i = 0; i < substitution.Length; i++) {
				if (i > 0)
					output.Write(", ");
				substitution[i](syntax);
			}
			output.Write('>');
		}

		static void WriteParameterList(ITextOutput output, MethodSignature<Action<ILNameSyntax>> methodSignature)
		{
			output.Write("(");
			for (int i = 0; i < methodSignature.ParameterTypes.Length; ++i) {
				if (i > 0)
					output.Write(", ");
				if (i == methodSignature.RequiredParameterCount)
					output.Write("..., ");
				methodSignature.ParameterTypes[i](ILNameSyntax.SignatureNoNamedTypeParameters);
			}
			output.Write(")");
		}

		static void WriteSignatureHeader(ITextOutput output, MethodSignature<Action<ILNameSyntax>> methodSignature)
		{
			if (methodSignature.Header.HasExplicitThis) {
				output.Write("instance explicit ");
			} else if (methodSignature.Header.IsInstance) {
				output.Write("instance ");
			}
			switch (methodSignature.Header.CallingConvention) {
				case SignatureCallingConvention.CDecl:
					output.Write("unmanaged cdecl ");
					break;
				case SignatureCallingConvention.StdCall:
					output.Write("unmanaged stdcall ");
					break;
				case SignatureCallingConvention.ThisCall:
					output.Write("unmanaged thiscall ");
					break;
				case SignatureCallingConvention.FastCall:
					output.Write("unmanaged fastcall ");
					break;
				case SignatureCallingConvention.VarArgs:
					output.Write("vararg ");
					break;
			}
		}

		static void WriteParent(ITextOutput output, PEFile module, MetadataReader metadata, EntityHandle parentHandle, Metadata.GenericContext genericContext, ILNameSyntax syntax)
		{
			switch (parentHandle.Kind) {
				case HandleKind.MethodDefinition:
					var methodDef = metadata.GetMethodDefinition((MethodDefinitionHandle)parentHandle);
					((EntityHandle)methodDef.GetDeclaringType()).WriteTo(module, output, genericContext, syntax);
					break;
				case HandleKind.ModuleReference:
					output.Write('[');
					var moduleRef = metadata.GetModuleReference((ModuleReferenceHandle)parentHandle);
					output.Write(metadata.GetString(moduleRef.Name));
					output.Write(']');
					break;
				case HandleKind.TypeDefinition:
				case HandleKind.TypeReference:
				case HandleKind.TypeSpecification:
					parentHandle.WriteTo(module, output, genericContext, syntax);
					break;
			}
		}

	}
}
