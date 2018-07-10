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
	[Export(typeof(IAnalyzer<IField>))]
	class AssignedByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public AssignedByFieldAccessAnalyzer() : base(true) { }
	}

	/// <summary>
	/// Finds methods where this field is written.
	/// </summary>
	[Export(typeof(IAnalyzer<IField>))]
	class ReadByFieldAccessAnalyzer : FieldAccessAnalyzer
	{
		public ReadByFieldAccessAnalyzer() : base(false) { }
	}

	/// <summary>
	/// Finds methods where this field is read or written.
	/// </summary>
	class FieldAccessAnalyzer : IMethodBodyAnalyzer<IField>
	{
		readonly bool showWrites; // true: show writes; false: show read access

		public string Text => showWrites ? "Assigned By" : "Read By";

		public FieldAccessAnalyzer(bool showWrites)
		{
			this.showWrites = showWrites;
		}

		public bool Show(IField field)
		{
			return !showWrites || !field.IsConst;
		}

		public IEnumerable<IEntity> Analyze(IField analyzedField, IMethod method, MethodBodyBlock methodBody, AnalyzerContext context)
		{
			bool found = false;
			var blob = methodBody.GetILReader();

			while (!found && blob.RemainingBytes > 0) {
				var opCode = blob.DecodeOpCode();
				if (!CanBeReference(opCode)) {
					blob.SkipOperand(opCode);
					continue;
				}
				EntityHandle fieldHandle = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
				if (!fieldHandle.Kind.IsMemberKind())
					continue;
				var field = context.TypeSystem.ResolveAsField(fieldHandle);
				if (field == null)
					continue;

				found = field.MetadataToken == analyzedField.MetadataToken
					&& field.ParentAssembly.PEFile == analyzedField.ParentAssembly.PEFile;
			}

			if (found) {
				yield return method;
			}
		}

		bool CanBeReference(ILOpCode code)
		{
			switch (code) {
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
