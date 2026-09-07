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
using System.Composition;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using ILOpCode = System.Reflection.Metadata.ILOpCode;

namespace ICSharpCode.ILSpyX.Analyzers.Builtin
{
	/// <summary>
	/// Finds methods where this field is written.
	/// </summary>
	[ExportAnalyzer(Header = "Assigned By", Order = 20)]
	[Shared]
	class AssignedByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public AssignedByFieldAccessAnalyzer() : base(FieldAccessKind.Write) { }
	}

	/// <summary>
	/// Finds methods where this field is read.
	/// </summary>
	[ExportAnalyzer(Header = "Read By", Order = 10)]
	[Shared]
	class ReadByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public ReadByFieldAccessAnalyzer() : base(FieldAccessKind.Read) { }
	}

	/// <summary>
	/// Finds methods that load this field's address.
	/// </summary>
	[ExportAnalyzer(Header = "Address Taken By", Order = 30)]
	[Shared]
	class AddressTakenByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public AddressTakenByFieldAccessAnalyzer() : base(FieldAccessKind.AddressOf) { }
	}

	enum FieldAccessKind
	{
		Read,
		Write,
		AddressOf
	}

	/// <summary>
	/// Finds methods that access this field in one particular way.
	/// </summary>
	class FieldAccessAnalyzer : IAnalyzer
	{
		const GetMemberOptions Options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

		readonly FieldAccessKind kind;

		public FieldAccessAnalyzer(FieldAccessKind kind)
		{
			this.kind = kind;
		}

		public bool Show(ISymbol? symbol)
		{
			// A constant is inlined at every use: there is nothing to assign to and no address
			// to take.
			return symbol is IField field && (kind == FieldAccessKind.Read || !field.IsConst);
		}

		public IEnumerable<ISymbol> Analyze(ISymbol analyzedSymbol, AnalyzerContext context)
		{
			Debug.Assert(analyzedSymbol is IField);
			var scope = context.GetScopeOf((IEntity)analyzedSymbol);
			foreach (var type in scope.GetTypesInScope(context.CancellationToken))
			{
				if (type.ParentModule?.MetadataFile == null)
					continue;
				var mappingInfo = context.Language.GetCodeMappingInfo(type.ParentModule.MetadataFile, type.MetadataToken);
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
			if (method.MetadataToken.IsNil || method.ParentModule?.MetadataFile == null)
				return false;
			var module = method.ParentModule.MetadataFile;
			foreach (var part in mappingInfo.GetMethodParts((MethodDefinitionHandle)method.MetadataToken))
			{
				var md = module.Metadata.GetMethodDefinition(part);
				if (!md.HasBody())
					continue;
				MethodBodyBlock body;
				try
				{
					body = module.GetMethodBody(md.RelativeVirtualAddress);
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
				IField? field;
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
					&& field.ParentModule?.MetadataFile == analyzedField.ParentModule!.MetadataFile)
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
					return kind == FieldAccessKind.Read;
				case ILOpCode.Stfld:
				case ILOpCode.Stsfld:
					return kind == FieldAccessKind.Write;
				case ILOpCode.Ldflda:
				case ILOpCode.Ldsflda:
					// An address load says only that something needed a reference to the field.
					// What happens through that reference is decided by the consumer - calling a
					// method on a value-type field reads it, passing it as a ref argument may
					// write it - and the IL scan here does not look at the consumer, so it is
					// neither a read nor a write.
					return kind == FieldAccessKind.AddressOf;
				default:
					return false;
			}
		}
	}
}
