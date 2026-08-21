// Copyright (c) 2026 Dr. Masroor Ehsan
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
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.AI.Decompiler
{
	/// <summary>
	/// Same-module caller discovery for AI context building. Copied verbatim (semantics preserved:
	/// ordering, limits, exception handling) from ILSpyX's internal
	/// <c>MethodUsedByAnalyzer.FindCallers</c>/<c>ScanMethodBody</c> and
	/// <c>AnalyzerHelpers.IsPossibleReferenceTo</c> when the AI context builder moved into this
	/// assembly: those helpers are internal to ILSpyX and this module must not reference it.
	/// Behavioral changes here must be mirrored consciously, not accidentally.
	/// </summary>
	internal static class SameModuleCallerDiscovery
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		/// <summary>
		/// Finds same-module callers using the same reference rules as the "Used By" analyzer.
		/// </summary>
		internal static IEnumerable<IMember> FindCallers(IMethod analyzedMethod, MetadataModule module)
		{
			if (analyzedMethod.ParentModule?.MetadataFile != module.MetadataFile)
				yield break;

			var analyzedBaseMethod = (IMethod?)InheritanceHelper.GetBaseMember(analyzedMethod);
			foreach (ITypeDefinition type in GetTypes(module))
			{
				foreach (var method in type.GetMembers(m => m is IMethod, Options).OfType<IMethod>())
				{
					if (method.MetadataToken != analyzedMethod.MetadataToken
						&& ScanMethodBody(analyzedMethod, method, analyzedBaseMethod, GetMethodBody(method, module)))
						yield return method;
				}

				foreach (var property in type.Properties)
				{
					if ((property.CanGet && ScanMethodBody(analyzedMethod, property.Getter, analyzedBaseMethod, GetMethodBody(property.Getter, module)))
						|| (property.CanSet && ScanMethodBody(analyzedMethod, property.Setter, analyzedBaseMethod, GetMethodBody(property.Setter, module))))
						yield return property;
				}

				foreach (var @event in type.Events)
				{
					if ((@event.CanAdd && ScanMethodBody(analyzedMethod, @event.AddAccessor, analyzedBaseMethod, GetMethodBody(@event.AddAccessor, module)))
						|| (@event.CanRemove && ScanMethodBody(analyzedMethod, @event.RemoveAccessor, analyzedBaseMethod, GetMethodBody(@event.RemoveAccessor, module)))
						|| (@event.CanInvoke && ScanMethodBody(analyzedMethod, @event.InvokeAccessor, analyzedBaseMethod, GetMethodBody(@event.InvokeAccessor, module))))
						yield return @event;
				}
			}
		}

		static IEnumerable<ITypeDefinition> GetTypes(MetadataModule module)
		{
			var pending = new Stack<ITypeDefinition>(module.TypeDefinitions.Reverse());
			while (pending.Count > 0)
			{
				ITypeDefinition type = pending.Pop();
				yield return type;
				foreach (ITypeDefinition nestedType in type.NestedTypes.Reverse())
					pending.Push(nestedType);
			}
		}

		static MethodBodyBlock? GetMethodBody(IMethod method, MetadataModule module)
		{
			if (!method.HasBody || method.MetadataToken.IsNil || method.ParentModule?.MetadataFile != module.MetadataFile)
				return null;
			try
			{
				var definition = module.MetadataFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
				return definition.RelativeVirtualAddress == 0 ? null : module.MetadataFile.GetMethodBody(definition.RelativeVirtualAddress);
			}
			catch (Exception exception) when (exception is BadImageFormatException or ArgumentException or InvalidOperationException)
			{
				return null;
			}
		}

		static bool ScanMethodBody(IMethod analyzedMethod, IMethod method, IMethod? analyzedBaseMethod, MethodBodyBlock? methodBody)
		{
			if (methodBody == null || method.ParentModule?.MetadataFile == null)
				return false;

			var mainModule = (MetadataModule)method.ParentModule;
			var blob = methodBody.GetILReader();

			var genericContext = new GenericContext(); // type parameters don't matter for caller discovery

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
				if (!IsPossibleReferenceTo(member, mainModule.MetadataFile, analyzedMethod))
				{
					if (analyzedBaseMethod == null || !IsPossibleReferenceTo(member, mainModule.MetadataFile, analyzedBaseMethod))
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

		/// <summary>
		/// Cheap metadata-only pre-filter copied from ILSpyX's AnalyzerHelpers: checks whether the
		/// handle could reference the analyzed method without resolving entities.
		/// </summary>
		static bool IsPossibleReferenceTo(EntityHandle member, MetadataFile module, IMethod analyzedMethod)
		{
			if (member.IsNil)
				return false;
			MetadataReader metadata = module.Metadata;
			switch (member.Kind)
			{
				case HandleKind.MethodDefinition:
					return member == analyzedMethod.MetadataToken
						&& module == analyzedMethod.ParentModule?.MetadataFile;
				case HandleKind.MemberReference:
					var mr = metadata.GetMemberReference((MemberReferenceHandle)member);
					if (mr.GetKind() != MemberReferenceKind.Method)
						return false;
					return metadata.StringComparer.Equals(mr.Name, analyzedMethod.Name);
				case HandleKind.MethodSpecification:
					var ms = metadata.GetMethodSpecification((MethodSpecificationHandle)member);
					return IsPossibleReferenceTo(ms.Method, module, analyzedMethod);
				default:
					return false;
			}
		}
	}
}
