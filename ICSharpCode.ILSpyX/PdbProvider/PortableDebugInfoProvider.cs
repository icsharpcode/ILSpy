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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

using static ICSharpCode.Decompiler.Metadata.MetadataFile;

#nullable enable

namespace ICSharpCode.ILSpyX.PdbProvider
{
	public class PortableDebugInfoProvider : IDebugInfoProvider
	{
		string? pdbFileName;
		string moduleFileName;
		readonly MetadataReaderProvider provider;
		MetadataReaderOptions options;
		bool hasError;

		public bool IsEmbedded => pdbFileName == null;

		public PortableDebugInfoProvider(string moduleFileName, MetadataReaderProvider provider,
			MetadataReaderOptions options = MetadataReaderOptions.Default,
			string? pdbFileName = null)
		{
			this.moduleFileName = moduleFileName ?? throw new ArgumentNullException(nameof(moduleFileName));
			this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
			this.options = options;
			this.pdbFileName = pdbFileName;
		}

		public string Description {
			get {
				if (pdbFileName == null)
				{
					if (hasError)
						return "Error while loading the PDB stream embedded in this assembly";
					return "Embedded in this assembly";
				}
				else
				{
					if (hasError)
						return $"Error while loading portable PDB: {pdbFileName}";
					return $"Loaded from portable PDB: {pdbFileName}";
				}
			}
		}

		public MetadataReader? GetMetadataReader()
		{
			try
			{
				hasError = false;
				return provider.GetMetadataReader();
			}
			catch (BadImageFormatException)
			{
				hasError = true;
				return null;
			}
			catch (IOException)
			{
				hasError = true;
				return null;
			}
		}

		public string SourceFileName => pdbFileName ?? moduleFileName;

		public IList<Decompiler.DebugInfo.SequencePoint> GetSequencePoints(MethodDefinitionHandle method)
		{
			var metadata = GetMetadataReader();
			if (metadata == null)
				return EmptyList<Decompiler.DebugInfo.SequencePoint>.Instance;
			var debugInfo = metadata.GetMethodDebugInformation(method);
			var sequencePoints = new List<Decompiler.DebugInfo.SequencePoint>();

			try
			{
				foreach (var point in debugInfo.GetSequencePoints())
				{
					string documentFileName;

					if (!point.Document.IsNil)
					{
						var document = metadata.GetDocument(point.Document);
						documentFileName = metadata.GetString(document.Name);
					}
					else
					{
						documentFileName = "";
					}

					sequencePoints.Add(new Decompiler.DebugInfo.SequencePoint() {
						Offset = point.Offset,
						StartLine = point.StartLine,
						StartColumn = point.StartColumn,
						EndLine = point.EndLine,
						EndColumn = point.EndColumn,
						DocumentUrl = documentFileName
					});
				}

				return sequencePoints;
			}
			catch (BadImageFormatException)
			{
				return EmptyList<Decompiler.DebugInfo.SequencePoint>.Instance;
			}
		}

		public IList<Variable> GetVariables(MethodDefinitionHandle method)
		{
			var metadata = GetMetadataReader();
			var variables = new List<Variable>();
			if (metadata == null)
				return variables;

			foreach (var h in metadata.GetLocalScopes(method))
			{
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables())
				{
					var var = metadata.GetLocalVariable(v);
					variables.Add(new Variable(var.Index, metadata.GetString(var.Name)));
				}
			}

			return variables;
		}

		public bool TryGetName(MethodDefinitionHandle method, int index, [NotNullWhen(true)] out string? name)
		{
			var metadata = GetMetadataReader();
			name = null;
			if (metadata == null)
				return false;

			foreach (var h in metadata.GetLocalScopes(method))
			{
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables())
				{
					var var = metadata.GetLocalVariable(v);
					if (var.Index == index)
					{
						name = metadata.GetString(var.Name);
						return true;
					}
				}
			}
			return false;
		}

		public bool TryGetExtraTypeInfo(MethodDefinitionHandle method, int index, out PdbExtraTypeInfo extraTypeInfo)
		{
			var metadata = GetMetadataReader();
			extraTypeInfo = default;

			if (metadata == null)
				return false;

			LocalVariableHandle localVariableHandle = default;
			foreach (var h in metadata.GetLocalScopes(method))
			{
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables())
				{
					var var = metadata.GetLocalVariable(v);
					if (var.Index == index)
					{
						localVariableHandle = v;
						break;
					}
				}

				if (!localVariableHandle.IsNil)
					break;
			}

			foreach (var h in metadata.CustomDebugInformation)
			{
				var cdi = metadata.GetCustomDebugInformation(h);
				if (cdi.Parent.IsNil || cdi.Parent.Kind != HandleKind.LocalVariable)
					continue;
				if (localVariableHandle != (LocalVariableHandle)cdi.Parent)
					continue;
				if (cdi.Value.IsNil || cdi.Kind.IsNil)
					continue;
				var kind = metadata.GetGuid(cdi.Kind);
				if (kind == KnownGuids.TupleElementNames && extraTypeInfo.TupleElementNames is null)
				{
					var reader = metadata.GetBlobReader(cdi.Value);
					var list = new List<string?>();
					while (reader.RemainingBytes > 0)
					{
						// Read a UTF8 null-terminated string
						int length = reader.IndexOf(0);
						string s = reader.ReadUTF8(length);
						// Skip null terminator
						reader.ReadByte();
						list.Add(string.IsNullOrWhiteSpace(s) ? null : s);
					}

					extraTypeInfo.TupleElementNames = list.ToArray();
				}
				else if (kind == KnownGuids.DynamicLocalVariables && extraTypeInfo.DynamicFlags is null)
				{
					var reader = metadata.GetBlobReader(cdi.Value);
					extraTypeInfo.DynamicFlags = new bool[reader.Length * 8];
					int j = 0;
					while (reader.RemainingBytes > 0)
					{
						int b = reader.ReadByte();
						for (int i = 1; i < 0x100; i <<= 1)
							extraTypeInfo.DynamicFlags[j++] = (b & i) != 0;
					}
				}

				if (extraTypeInfo.TupleElementNames != null && extraTypeInfo.DynamicFlags != null)
					break;
			}

			return extraTypeInfo.TupleElementNames != null || extraTypeInfo.DynamicFlags != null;
		}

		public MetadataFile ToMetadataFile()
		{
			var kind = IsEmbedded || Path.GetExtension(SourceFileName).Equals(".pdb", StringComparison.OrdinalIgnoreCase) ? MetadataFileKind.ProgramDebugDatabase : MetadataFileKind.Metadata;
			return new MetadataFile(kind, SourceFileName, provider, options, 0, IsEmbedded);
		}
	}
}
