// Copyright (c) 2026 Franck
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

using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Redecomp.Core.Recovery;

/// <summary>Finds, for each top-level type of an assembly, the source file that declared it.</summary>
public static class TypeDocumentMap
{
	// Kind GUID of the "TypeDefinitionDocuments" custom debug information, which the compiler emits
	// for types that have no method body to carry sequence points (enums, interfaces, ...).
	static readonly Guid TypeDefinitionDocumentsKind = new("932E74BC-DBA9-4478-8D46-0F32A7BAB3D3");

	/// <summary>Returns the original source path of every top-level type that the PDB can place.</summary>
	public static IReadOnlyDictionary<TypeDefinitionHandle, string> Build(MetadataReader module, MetadataReader pdb)
	{
		var votes = new Dictionary<TypeDefinitionHandle, Dictionary<string, int>>();

		foreach (var methodHandle in module.MethodDefinitions)
		{
			var info = pdb.GetMethodDebugInformation(methodHandle.ToDebugInformationHandle());
			if (info.SequencePointsBlob.IsNil)
				continue;
			foreach (var point in info.GetSequencePoints())
			{
				if (point.IsHidden || point.Document.IsNil)
					continue;
				var type = TopLevel(module, module.GetMethodDefinition(methodHandle).GetDeclaringType());
				Vote(type, pdb.GetString(pdb.GetDocument(point.Document).Name));
				break;
			}
		}

		foreach (var typeHandle in module.TypeDefinitions)
		{
			foreach (var cdiHandle in pdb.GetCustomDebugInformation(typeHandle))
			{
				var cdi = pdb.GetCustomDebugInformation(cdiHandle);
				if (pdb.GetGuid(cdi.Kind) != TypeDefinitionDocumentsKind)
					continue;
				var blob = pdb.GetBlobReader(cdi.Value);
				while (blob.RemainingBytes > 0)
				{
					var document = MetadataTokens.DocumentHandle(blob.ReadCompressedInteger());
					Vote(TopLevel(module, typeHandle), pdb.GetString(pdb.GetDocument(document).Name));
				}
			}
		}

		// A type split across files (partial classes) goes to the file holding most of its methods.
		return votes.ToDictionary(v => v.Key, v => v.Value.MaxBy(d => d.Value).Key);

		void Vote(TypeDefinitionHandle type, string document)
		{
			if (!votes.TryGetValue(type, out var counts))
				votes[type] = counts = new Dictionary<string, int>();
			counts[document] = counts.GetValueOrDefault(document) + 1;
		}
	}

	static TypeDefinitionHandle TopLevel(MetadataReader module, TypeDefinitionHandle type)
	{
		while (true)
		{
			var declaring = module.GetTypeDefinition(type).GetDeclaringType();
			if (declaring.IsNil)
				return type;
			type = declaring;
		}
	}
}
