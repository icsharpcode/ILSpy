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
using System.Reflection.Metadata;

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
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in ScanType(analyzedType, type, context))
					yield return result;
			}
		}

		IEnumerable<IEntity> ScanType(ITypeDefinition analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			if (analyzedEntity.ParentModule.PEFile == type.ParentModule.PEFile
				&& analyzedEntity.MetadataToken == type.MetadataToken)
				yield break;

			var visitor = new TypeDefinitionUsedVisitor(analyzedEntity, topLevelOnly: false);

			foreach (var bt in type.DirectBaseTypes)
			{
				bt.AcceptVisitor(visitor);
			}

			if (visitor.Found || ScanAttributes(visitor, type.GetAttributes()))
				yield return type;

			foreach (var member in type.Members)
			{
				visitor.Found = false;
				VisitMember(visitor, member, context);
				if (visitor.Found)
					yield return member;
			}
		}

		bool ScanAttributes(TypeDefinitionUsedVisitor visitor, IEnumerable<IAttribute> attributes)
		{
			foreach (var attribute in attributes)
			{
				foreach (var fa in attribute.FixedArguments)
				{
					CheckAttributeValue(fa.Value);
					if (visitor.Found)
						return true;
				}

				foreach (var na in attribute.NamedArguments)
				{
					CheckAttributeValue(na.Value);
					if (visitor.Found)
						return true;
				}
			}
			return false;

			void CheckAttributeValue(object value)
			{
				if (value is IType typeofType)
				{
					typeofType.AcceptVisitor(visitor);
				}
				else if (value is ImmutableArray<CustomAttributeTypedArgument<IType>> arr)
				{
					foreach (var element in arr)
					{
						CheckAttributeValue(element.Value);
					}
				}
			}
		}

		void VisitMember(TypeDefinitionUsedVisitor visitor, IMember member, AnalyzerContext context)
		{
			member.DeclaringType.AcceptVisitor(visitor);
			switch (member)
			{
				case IField field:
					field.ReturnType.AcceptVisitor(visitor);

					if (!visitor.Found)
						ScanAttributes(visitor, field.GetAttributes());
					break;
				case IMethod method:
					foreach (var p in method.Parameters)
					{
						p.Type.AcceptVisitor(visitor);
						if (!visitor.Found)
							ScanAttributes(visitor, p.GetAttributes());
					}

					if (!visitor.Found)
						ScanAttributes(visitor, method.GetAttributes());

					method.ReturnType.AcceptVisitor(visitor);

					if (!visitor.Found)
						ScanAttributes(visitor, method.GetReturnTypeAttributes());

					foreach (var t in method.TypeArguments)
					{
						t.AcceptVisitor(visitor);
					}

					foreach (var t in method.TypeParameters)
					{
						t.AcceptVisitor(visitor);

						if (!visitor.Found)
							ScanAttributes(visitor, t.GetAttributes());
					}
					if (!visitor.Found)
						visitor.Found |= ScanMethodBody(visitor.TypeDefinition, method, context.GetMethodBody(method));
					break;
				case IProperty property:
					foreach (var p in property.Parameters)
					{
						p.Type.AcceptVisitor(visitor);
					}

					if (!visitor.Found)
						ScanAttributes(visitor, property.GetAttributes());

					property.ReturnType.AcceptVisitor(visitor);

					if (!visitor.Found && property.CanGet)
					{
						ScanAttributes(visitor, property.Getter.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, property.Getter.GetReturnTypeAttributes());
						if (!visitor.Found)
							visitor.Found |= ScanMethodBody(visitor.TypeDefinition, property.Getter, context.GetMethodBody(property.Getter));
					}

					if (!visitor.Found && property.CanSet)
					{
						ScanAttributes(visitor, property.Setter.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, property.Setter.GetReturnTypeAttributes());
						if (!visitor.Found)
							visitor.Found |= ScanMethodBody(visitor.TypeDefinition, property.Setter, context.GetMethodBody(property.Setter));
					}

					break;
				case IEvent @event:
					@event.ReturnType.AcceptVisitor(visitor);

					if (!visitor.Found && @event.CanAdd)
					{
						ScanAttributes(visitor, @event.AddAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.AddAccessor.GetReturnTypeAttributes());
						if (!visitor.Found)
							visitor.Found |= ScanMethodBody(visitor.TypeDefinition, @event.AddAccessor, context.GetMethodBody(@event.AddAccessor));
					}

					if (!visitor.Found && @event.CanRemove)
					{
						ScanAttributes(visitor, @event.RemoveAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.RemoveAccessor.GetReturnTypeAttributes());
						if (!visitor.Found)
							visitor.Found |= ScanMethodBody(visitor.TypeDefinition, @event.RemoveAccessor, context.GetMethodBody(@event.RemoveAccessor));
					}

					if (!visitor.Found && @event.CanInvoke)
					{
						ScanAttributes(visitor, @event.InvokeAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.InvokeAccessor.GetReturnTypeAttributes());
						if (!visitor.Found)
							visitor.Found |= ScanMethodBody(visitor.TypeDefinition, @event.InvokeAccessor, context.GetMethodBody(@event.InvokeAccessor));
					}

					break;
			}
		}

		bool ScanMethodBody(ITypeDefinition analyzedType, IMethod method, MethodBodyBlock methodBody)
		{
			if (methodBody == null)
				return false;

			var module = (MetadataModule)method.ParentModule;
			var metadata = module.PEFile.Metadata;
			var decoder = new FindTypeDecoder(module, analyzedType);

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
