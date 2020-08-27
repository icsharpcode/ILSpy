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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Analyzers;

using ILOpCode = System.Reflection.Metadata.ILOpCode;

namespace ICSharpCode.ILSpy.Analyzers.Builtin
{
	/// <summary>
	/// Finds methods where this field is read.
	/// </summary>
	[ExportAnalyzer(Header = "Assigned By", Order = 20)]
	class AssignedByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public AssignedByFieldAccessAnalyzer() : base(true) { }
	}

	/// <summary>
	/// Finds methods where this field is written.
	/// </summary>
	[ExportAnalyzer(Header = "Read By", Order = 10)]
	class ReadByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public ReadByFieldAccessAnalyzer() : base(false) { }
	}

	/// <summary>
	/// Finds methods where this field is read or written.
	/// </summary>
	class FieldAccessAnalyzer : IAnalyzer
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		readonly bool showWrites; // true: show writes; false: show read access

		public FieldAccessAnalyzer(bool showWrites)
		{
			this.showWrites = showWrites;
		}

		public bool Show(ISymbol symbol)
		{
			return symbol is IField field && (!showWrites || !field.IsConst);
		}

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IField);
			var scope = context.GetScopeOf((IEntity)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				var mappingInfo = context.Language.GetCodeMappingInfo(type.ParentModule.PEFile, type.MetadataToken);
				var methods = type.GetMembers(m => m is IMethod, Options).OfType<IMethod>();
				foreach (var method in methods)
				{
					if (IsUsedInMethod((IField)analyzedSymbol, method, mappingInfo, context))
						yield return method;
				}

				foreach (var property in type.Properties)
				{
					if (property.CanGet && IsUsedInMethod((IField)analyzedSymbol, property.Getter, mappingInfo, context))
					{
						yield return property;
						continue;
					}
					if (property.CanSet && IsUsedInMethod((IField)analyzedSymbol, property.Setter, mappingInfo, context))
					{
						yield return property;
						continue;
					}
				}

				foreach (var @event in type.Events)
				{
					if (@event.CanAdd && IsUsedInMethod((IField)analyzedSymbol, @event.AddAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanRemove && IsUsedInMethod((IField)analyzedSymbol, @event.RemoveAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
					if (@event.CanInvoke && IsUsedInMethod((IField)analyzedSymbol, @event.InvokeAccessor, mappingInfo, context))
					{
						yield return @event;
						continue;
					}
				}
			}
		}

		bool IsUsedInMethod(IField analyzedField, IMethod method, CodeMappingInfo mappingInfo, AnalyzerContext context)
		{
			if (method.MetadataToken.IsNil)
				return false;
			var module = method.ParentModule.PEFile;
			foreach (var part in mappingInfo.GetMethodParts((MethodDefinitionHandle)method.MetadataToken))
			{
				var md = module.Metadata.GetMethodDefinition(part);
				if (!md.HasBody())
					continue;
				MethodBodyBlock body;
				try
				{
					body = module.Reader.GetMethodBody(md.RelativeVirtualAddress);
				}
				catch (BadImageFormatException)
				{
					return false;
				}
				if (ScanMethodBody(analyzedField, method, body))
					return true;
			}
			return false;
		}

		bool ScanMethodBody(IField analyzedField, IMethod method, MethodBodyBlock methodBody)
		{
			if (methodBody == null)
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
				EntityHandle fieldHandle = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!fieldHandle.Kind.IsMemberKind())
					continue;
				IField field;
				try
				{
					field = mainModule.ResolveEntity(fieldHandle, genericContext) as IField;
				}
				catch (BadImageFormatException)
				{
					continue;
				}
				if (field == null)
					continue;

				if (field.MetadataToken == analyzedField.MetadataToken
					&& field.ParentModule.PEFile == analyzedField.ParentModule.PEFile)
					return true;
			}

			return false;
		}

		bool CanBeReference(ILOpCode code)
		{
			switch (code)
			{
				case ILOpCode.Ldfld:
				case ILOpCode.Ldsfld:
					return !showWrites;
				case ILOpCode.Stfld:
				case ILOpCode.Stsfld:
					return showWrites;
				case ILOpCode.Ldflda:
				case ILOpCode.Ldsflda:
					return true; // always show address-loading
				default:
					return false;
			}
		}
	}
}
