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
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Shows methods that instantiate a type.
	/// </summary>
	[ExportAnalyzer(Header = "Instantiated By", Order = 20)]
	class TypeInstantiatedByAnalyzer : IAnalyzer
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is ITypeDefinition);
			var scope = context.GetScopeOf((ITypeDefinition)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				var mappingInfo = context.Language.GetCodeMappingInfo(type.ParentModule.PEFile, type.MetadataToken);
				var methods = type.GetMembers(m => m is IMethod, Options).OfType<IMethod>();
				foreach (var method in methods)
				{
					if (IsUsedInMethod((ITypeDefinition)analyzedSymbol, method, mappingInfo, context))
						yield return method;
				}

				foreach (var property in type.Properties)
				{
					if (property.CanGet && IsUsedInMethod((ITypeDefinition)analyzedSymbol, property.Getter, mappingInfo, context))
					{
						yield return property;
						continue;
					}
					if (property.CanSet && IsUsedInMethod((ITypeDefinition)analyzedSymbol, property.Setter, mappingInfo, context))
					{
						yield return property;
						continue;
					}
				}

				foreach (var @event in type.Events)
				{
					if (@event.CanAdd && IsUsedInMethod((ITypeDefinition)analyzedSymbol, @event.AddAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanRemove && IsUsedInMethod((ITypeDefinition)analyzedSymbol, @event.RemoveAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanInvoke && IsUsedInMethod((ITypeDefinition)analyzedSymbol, @event.InvokeAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
				}
			}
		}

		bool IsUsedInMethod(ITypeDefinition analyzedEntity, IMethod method, CodeMappingInfo mappingInfo, AnalyzerContext context)
		{
			return ScanMethodBody(analyzedEntity, method, context.GetMethodBody(method));
		}

		bool ScanMethodBody(ITypeDefinition analyzedEntity, IMethod method, MethodBodyBlock methodBody)
		{
			if (methodBody == null)
				return false;
			var blob = methodBody.GetILReader();
			var module = (MetadataModule)method.ParentModule;
			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer

			while (blob.RemainingBytes > 0)
			{
				ILOpCode opCode;
				try
				{
					opCode = blob.DecodeOpCode();
					if (!CanBeReference(opCode))
					{
						blob.SkipOperand(opCode);
						continue;
					}
				}
				catch (BadImageFormatException)
				{
					return false;
				}
				EntityHandle methodHandle = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!methodHandle.Kind.IsMemberKind())
					continue;
				IMethod ctor;
				try
				{
					ctor = module.ResolveMethod(methodHandle, genericContext);
				}
				catch (BadImageFormatException)
				{
					continue;
				}
				if (ctor == null || !ctor.IsConstructor)
					continue;

				if (ctor.DeclaringTypeDefinition?.MetadataToken == analyzedEntity.MetadataToken
					&& ctor.ParentModule.PEFile == analyzedEntity.ParentModule.PEFile)
					return true;
			}

			return false;
		}

		bool CanBeReference(ILOpCode opCode)
		{
			return opCode == ILOpCode.Newobj || opCode == ILOpCode.Initobj;
		}

		public bool Show(ISymbol symbol) => symbol is ITypeDefinition entity && !entity.IsAbstract && !entity.IsStatic;
	}
}
