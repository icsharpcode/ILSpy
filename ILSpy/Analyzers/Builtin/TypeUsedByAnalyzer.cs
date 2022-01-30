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
				VisitMember(visitor, member, context, scanBodies: true);
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

		void VisitMember(TypeDefinitionUsedVisitor visitor, IMember member, AnalyzerContext context, bool scanBodies = false)
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

					if (scanBodies && !visitor.Found)
						ScanMethodBody(visitor, method, context.GetMethodBody(method), context);

					break;
				case IProperty property:
					foreach (var p in property.Parameters)
					{
						p.Type.AcceptVisitor(visitor);
					}

					if (!visitor.Found)
						ScanAttributes(visitor, property.GetAttributes());

					property.ReturnType.AcceptVisitor(visitor);

					if (scanBodies && !visitor.Found && property.CanGet)
					{
						if (!visitor.Found)
							ScanAttributes(visitor, property.Getter.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, property.Getter.GetReturnTypeAttributes());

						ScanMethodBody(visitor, property.Getter, context.GetMethodBody(property.Getter), context);
					}

					if (scanBodies && !visitor.Found && property.CanSet)
					{
						if (!visitor.Found)
							ScanAttributes(visitor, property.Setter.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, property.Setter.GetReturnTypeAttributes());

						ScanMethodBody(visitor, property.Setter, context.GetMethodBody(property.Setter), context);
					}

					break;
				case IEvent @event:
					@event.ReturnType.AcceptVisitor(visitor);

					if (scanBodies && !visitor.Found && @event.CanAdd)
					{
						if (!visitor.Found)
							ScanAttributes(visitor, @event.AddAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.AddAccessor.GetReturnTypeAttributes());

						ScanMethodBody(visitor, @event.AddAccessor, context.GetMethodBody(@event.AddAccessor), context);
					}

					if (scanBodies && !visitor.Found && @event.CanRemove)
					{
						if (!visitor.Found)
							ScanAttributes(visitor, @event.RemoveAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.RemoveAccessor.GetReturnTypeAttributes());

						ScanMethodBody(visitor, @event.RemoveAccessor, context.GetMethodBody(@event.RemoveAccessor), context);
					}

					if (scanBodies && !visitor.Found && @event.CanInvoke)
					{
						if (!visitor.Found)
							ScanAttributes(visitor, @event.InvokeAccessor.GetAttributes());
						if (!visitor.Found)
							ScanAttributes(visitor, @event.InvokeAccessor.GetReturnTypeAttributes());

						ScanMethodBody(visitor, @event.InvokeAccessor, context.GetMethodBody(@event.InvokeAccessor), context);
					}

					break;
			}
		}

		void ScanMethodBody(TypeDefinitionUsedVisitor visitor, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			if (methodBody == null)
				return;

			var module = (MetadataModule)method.ParentModule;
			var metadata = module.PEFile.Metadata;
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer
			var decoder = new FindTypeDecoder(module, visitor.TypeDefinition);

			if (!methodBody.LocalSignature.IsNil)
			{
				try
				{
					var ss = metadata.GetStandaloneSignature(methodBody.LocalSignature);
					visitor.Found |= HandleStandaloneSignature(ss);
				}
				catch (BadImageFormatException)
				{
					// Issue #2197: ignore invalid local signatures
				}
			}

			var blob = methodBody.GetILReader();

			while (!visitor.Found && blob.RemainingBytes > 0)
			{
				var opCode = blob.DecodeOpCode();
				switch (opCode.GetOperandType())
				{
					case OperandType.Field:
					case OperandType.Method:
					case OperandType.Sig:
					case OperandType.Tok:
					case OperandType.Type:
						HandleMember(MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32()));
						if (visitor.Found)
							return;
						break;
					default:
						blob.SkipOperand(opCode);
						break;
				}
			}

			void HandleMember(EntityHandle member)
			{
				if (member.IsNil)
					return;
				switch (member.Kind)
				{
					case HandleKind.TypeReference:
					case HandleKind.TypeSpecification:
					case HandleKind.TypeDefinition:
						module.ResolveType(member, genericContext).AcceptVisitor(visitor);
						break;

					case HandleKind.FieldDefinition:
						var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)member);
						HandleMember(fd.GetDeclaringType());
						visitor.Found |= fd.DecodeSignature(decoder, default);
						break;

					case HandleKind.MethodDefinition:
						var md = metadata.GetMethodDefinition((MethodDefinitionHandle)member);
						HandleMember(md.GetDeclaringType());
						var msig = md.DecodeSignature(decoder, default);
						visitor.Found |= msig.ReturnType;
						foreach (var t in msig.ParameterTypes)
						{
							visitor.Found |= t;
						}
						break;

					case HandleKind.MemberReference:
						var mr = metadata.GetMemberReference((MemberReferenceHandle)member);
						HandleMember(mr.Parent);
						switch (mr.GetKind())
						{
							case MemberReferenceKind.Method:
								msig = mr.DecodeMethodSignature(decoder, default);
								visitor.Found |= msig.ReturnType;
								foreach (var t in msig.ParameterTypes)
								{
									visitor.Found |= t;
								}
								break;
							case MemberReferenceKind.Field:
								visitor.Found |= mr.DecodeFieldSignature(decoder, default);
								break;
						}
						break;

					case HandleKind.MethodSpecification:
						var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)member);
						HandleMember(ms.Method);
						var mssig = ms.DecodeSignature(decoder, default);
						foreach (var t in mssig)
						{
							visitor.Found |= t;
						}
						break;

					case HandleKind.StandaloneSignature:
						var ss = metadata.GetStandaloneSignature((StandaloneSignatureHandle)member);
						visitor.Found |= HandleStandaloneSignature(ss);
						break;
				}
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
						return false;
					case StandaloneSignatureKind.LocalVariables:
						var sig = signature.DecodeLocalSignature(decoder, default);
						foreach (var t in sig)
						{
							if (t)
								return true;
						}
						return false;
					default:
						return false;
				}
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
