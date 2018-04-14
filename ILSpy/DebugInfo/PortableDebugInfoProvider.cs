using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.DebugInfo
{
	class PortableDebugInfoProvider : IDebugInfoProvider
	{
		string pdbFileName;
		MetadataReaderProvider provider;

		public PortableDebugInfoProvider(string pdbFileName, MetadataReaderProvider provider)
		{
			this.pdbFileName = pdbFileName;
			this.provider = provider;
		}

		public IList<Decompiler.Metadata.SequencePoint> GetSequencePoints(MethodDefinitionHandle method)
		{
			var metadata = provider.GetMetadataReader();
			var debugInfo = metadata.GetMethodDebugInformation(method);
			var sequencePoints = new List<Decompiler.Metadata.SequencePoint>();

			foreach (var point in debugInfo.GetSequencePoints()) {
				string documentFileName;

				if (!point.Document.IsNil) {
					var document = metadata.GetDocument(point.Document);
					documentFileName = metadata.GetString(document.Name);
				} else {
					documentFileName = "";
				}

				sequencePoints.Add(new Decompiler.Metadata.SequencePoint() {
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

		public IList<Variable> GetVariables(MethodDefinitionHandle method)
		{
			var metadata = provider.GetMetadataReader();
			var variables = new List<Variable>();

			foreach (var h in metadata.GetLocalScopes(method)) {
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables()) {
					var var = metadata.GetLocalVariable(v);
					variables.Add(new Variable { Name = metadata.GetString(var.Name) });
				}
			}

			return variables;
		}

		public bool TryGetName(MethodDefinitionHandle method, int index, out string name)
		{
			var metadata = provider.GetMetadataReader();
			name = null;

			foreach (var h in metadata.GetLocalScopes(method)) {
				var scope = metadata.GetLocalScope(h);
				foreach (var v in scope.GetLocalVariables()) {
					var var = metadata.GetLocalVariable(v);
					if (var.Index == index) {
						name = metadata.GetString(var.Name);
						return true;
					}
				}
			}
			return false;
		}
	}
}
