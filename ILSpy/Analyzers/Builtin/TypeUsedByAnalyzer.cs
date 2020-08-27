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
using System.ComponentModel.Composition;
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
			var scope = context.GetScopeOf((ITypeDefinition)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				foreach (var result in ScanType((ITypeDefinition)analyzedSymbol, type, context))
					yield return result;
			}
		}

		IEnumerable<IEntity> ScanType(ITypeDefinition analyzedEntity, ITypeDefinition type, AnalyzerContext context)
		{
			if (analyzedEntity.ParentModule.PEFile == type.ParentModule.PEFile
				&& analyzedEntity.MetadataToken == type.MetadataToken)
				yield break;

			var visitor = new TypeDefinitionUsedVisitor(analyzedEntity, false);

			foreach (var bt in type.DirectBaseTypes)
			{
				bt.AcceptVisitor(visitor);
			}

			if (visitor.Found)
				yield return type;

			foreach (var member in type.Members)
			{
				visitor.Found = false;
				VisitMember(visitor, member, context, scanBodies: true);
				if (visitor.Found)
					yield return member;
			}
		}

		void VisitMember(TypeDefinitionUsedVisitor visitor, IMember member, AnalyzerContext context, bool scanBodies = false)
		{
			member.DeclaringType.AcceptVisitor(visitor);
			switch (member)
			{
				case IField field:
					field.ReturnType.AcceptVisitor(visitor);
					break;
				case IMethod method:
					foreach (var p in method.Parameters)
					{
						p.Type.AcceptVisitor(visitor);
					}

					method.ReturnType.AcceptVisitor(visitor);

					foreach (var t in method.TypeArguments)
					{
						t.AcceptVisitor(visitor);
					}

					if (scanBodies && !visitor.Found)
						ScanMethodBody(visitor, method, context.GetMethodBody(method), context);

					break;
				case IProperty property:
					foreach (var p in property.Parameters)
					{
						p.Type.AcceptVisitor(visitor);
					}

					property.ReturnType.AcceptVisitor(visitor);

					if (scanBodies && !visitor.Found && property.CanGet)
						ScanMethodBody(visitor, property.Getter, context.GetMethodBody(property.Getter), context);

					if (scanBodies && !visitor.Found && property.CanSet)
						ScanMethodBody(visitor, property.Setter, context.GetMethodBody(property.Setter), context);

					break;
				case IEvent @event:
					@event.ReturnType.AcceptVisitor(visitor);

					if (scanBodies && !visitor.Found && @event.CanAdd)
						ScanMethodBody(visitor, @event.AddAccessor, context.GetMethodBody(@event.AddAccessor), context);

					if (scanBodies && !visitor.Found && @event.CanRemove)
						ScanMethodBody(visitor, @event.RemoveAccessor, context.GetMethodBody(@event.RemoveAccessor), context);

					if (scanBodies && !visitor.Found && @event.CanInvoke)
						ScanMethodBody(visitor, @event.InvokeAccessor, context.GetMethodBody(@event.InvokeAccessor), context);
					break;
			}
		}

		void ScanMethodBody(TypeDefinitionUsedVisitor visitor, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			if (methodBody == null)
				return;

			var module = (MetadataModule)method.ParentModule;
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer

			if (!methodBody.LocalSignature.IsNil)
			{
				foreach (var type in module.DecodeLocalSignature(methodBody.LocalSignature, genericContext))
				{
					type.AcceptVisitor(visitor);

					if (visitor.Found)
						return;
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
						var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (member.IsNil)
							continue;
						switch (member.Kind)
						{
							case HandleKind.TypeReference:
							case HandleKind.TypeSpecification:
							case HandleKind.TypeDefinition:
								module.ResolveType(member, genericContext).AcceptVisitor(visitor);
								if (visitor.Found)
									return;
								break;

							case HandleKind.FieldDefinition:
							case HandleKind.MethodDefinition:
							case HandleKind.MemberReference:
							case HandleKind.MethodSpecification:
								VisitMember(visitor, module.ResolveEntity(member, genericContext) as IMember, context);

								if (visitor.Found)
									return;
								break;

							case HandleKind.StandaloneSignature:
								var signature = module.DecodeMethodSignature((StandaloneSignatureHandle)member, genericContext);
								foreach (var type in signature.ParameterTypes)
								{
									type.AcceptVisitor(visitor);
								}

								signature.ReturnType.AcceptVisitor(visitor);

								if (visitor.Found)
									return;
								break;

							default:
								break;
						}
						break;
					default:
						blob.SkipOperand(opCode);
						break;
				}
			}
		}

		public bool Show(ISymbol symbol) => symbol is ITypeDefinition entity;
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
