﻿// Copyright (c) 2018 Siegfried Pammer
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

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>
	/// Shows entities that are used by a method.
	/// </summary>
	[ExportAnalyzer(Header = "Used By", Order = 20)]
	[PartCreationPolicy(CreationPolicy.Shared)]
	class MethodUsedByAnalyzer : IAnalyzer
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		public bool Show(ISymbol symbol) => symbol is IMethod method && !method.IsVirtual && method.ParentModule is not null;

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IMethod);

			var analyzedMethod = (IMethod)analyzedSymbol;
			var analyzedBaseMethod = (IMethod)InheritanceHelper.GetBaseMember(analyzedMethod);
			if (analyzedMethod.ParentModule?.MetadataFile == null)
				yield break;
			var mapping = context.Language
				.GetCodeMappingInfo(analyzedMethod.ParentModule.MetadataFile,
					analyzedMethod.DeclaringTypeDefinition!.MetadataToken);

			var parentMethod = mapping.GetParentMethod((MethodDefinitionHandle)analyzedMethod.MetadataToken);
			if (parentMethod != analyzedMethod.MetadataToken)
				yield return ((MetadataModule)analyzedMethod.ParentModule).GetDefinition(parentMethod);

			var scope = context.GetScopeOf(analyzedMethod);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				if (type.ParentModule?.MetadataFile == null)
					continue;
				var parentModule = (MetadataModule)type.ParentModule;
				mapping = null;
				var methods = type.GetMembers(m => m is IMethod, Options).OfType<IMethod>();
				foreach (var method in methods)
				{
					if (IsUsedInMethod(analyzedMethod, analyzedBaseMethod, method, context))
					{
						mapping ??= context.Language.GetCodeMappingInfo(parentModule.MetadataFile, type.MetadataToken);
						var parent = mapping.GetParentMethod((MethodDefinitionHandle)method.MetadataToken);
						yield return parentModule.GetDefinition(parent);
					}
				}

				foreach (var property in type.Properties)
				{
					if (property.CanGet && IsUsedInMethod(analyzedMethod, analyzedBaseMethod, property.Getter, context))
					{
						yield return property;
						continue;
					}
					if (property.CanSet && IsUsedInMethod(analyzedMethod, analyzedBaseMethod, property.Setter, context))
					{
						yield return property;
						continue;
					}
				}

				foreach (var @event in type.Events)
				{
					if (@event.CanAdd && IsUsedInMethod(analyzedMethod, analyzedBaseMethod, @event.AddAccessor, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanRemove && IsUsedInMethod(analyzedMethod, analyzedBaseMethod, @event.RemoveAccessor, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanInvoke && IsUsedInMethod(analyzedMethod, analyzedBaseMethod, @event.InvokeAccessor, context))
					{
						yield return @event;
						continue;
					}
				}
			}
		}

		bool IsUsedInMethod(IMethod analyzedEntity, IMethod analyzedBaseMethod, IMethod method, AnalyzerContext context)
		{
			return ScanMethodBody(analyzedEntity, method, analyzedBaseMethod, context.GetMethodBody(method));
		}

		static bool ScanMethodBody(IMethod analyzedMethod, IMethod method, IMethod analyzedBaseMethod, MethodBodyBlock? methodBody)
		{
			if (methodBody == null || method.ParentModule?.MetadataFile == null)
				return false;

			var mainModule = (MetadataModule)method.ParentModule;
			var blob = methodBody.GetILReader();

			var genericContext = new Decompiler.TypeSystem.GenericContext(); // type parameters don't matter for this analyzer

			while (blob.RemainingBytes > 0)
			{
				ILOpCode opCode;
				try
				{
					opCode = blob.DecodeOpCode();
					if (!IsSupportedOpCode(opCode))
					{
						ILParser.SkipOperand(ref blob, opCode);
						continue;
					}
				}
				catch (BadImageFormatException)
				{
					return false; // unexpected end of blob
				}
				var member = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!AnalyzerHelpers.IsPossibleReferenceTo(member, mainModule.MetadataFile, analyzedMethod))
				{
					if (analyzedBaseMethod == null || !AnalyzerHelpers.IsPossibleReferenceTo(member, mainModule.MetadataFile, analyzedBaseMethod))
					{
						continue;
					}
				}

				IMember? m;
				try
				{
					m = (mainModule.ResolveEntity(member, genericContext) as IMember)?.MemberDefinition;
				}
				catch (BadImageFormatException)
				{
					continue;
				}
				if (m == null)
					continue;

				if (opCode == ILOpCode.Callvirt && analyzedBaseMethod != null)
				{
					if (IsSameMember(analyzedBaseMethod, m))
					{
						return true;
					}
				}
				if (IsSameMember(analyzedMethod, m))
				{
					return true;
				}
			}

			return false;
		}

		static bool IsSupportedOpCode(ILOpCode opCode)
		{
			switch (opCode)
			{
				case ILOpCode.Call:
				case ILOpCode.Callvirt:
				case ILOpCode.Ldtoken:
				case ILOpCode.Ldftn:
				case ILOpCode.Ldvirtftn:
				case ILOpCode.Newobj:
					return true;
				default:
					return false;
			}
		}

		static bool IsSameMember(IMember analyzedMethod, IMember m)
		{
			return m.MetadataToken == analyzedMethod.MetadataToken
				&& m.ParentModule?.MetadataFile == analyzedMethod.ParentModule!.MetadataFile;
		}
	}
}
