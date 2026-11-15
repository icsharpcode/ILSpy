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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

namespace Redecomp.Core.Verification;

/// <summary>Method-by-method comparison of the IL of two assemblies.</summary>
/// <param name="Total">Methods with a body in the original assembly.</param>
/// <param name="Identical">Methods whose IL is the same in both assemblies.</param>
/// <param name="Differing">Signatures of the methods whose IL differs.</param>
/// <param name="Missing">Signatures of the original methods absent from the rebuilt assembly.</param>
public sealed record IlComparison(int Total, int Identical, IReadOnlyList<string> Differing, IReadOnlyList<string> Missing)
{
	public bool IsIdentical => Identical == Total;

	public override string ToString() => Total == 0 ? "n/a" : $"{100.0 * Identical / Total:0}% ({Identical}/{Total})";
}

/// <summary>
/// Compares the IL code of every method of two assemblies, as the ILSpy disassembler prints it with
/// member references resolved to names, so that metadata token numbering does not matter.
/// Custom attributes are not compared; see <see cref="Normalize"/>.
/// </summary>
public static class IlComparer
{
	public static IlComparison Compare(string originalAssembly, string rebuiltAssembly)
	{
		var original = Disassemble(originalAssembly);
		var rebuilt = Disassemble(rebuiltAssembly);
		var differing = new List<string>();
		var missing = new List<string>();
		int identical = 0;
		foreach (var (signature, body) in original)
		{
			if (!rebuilt.TryGetValue(signature, out var rebuiltBody))
				missing.Add(signature);
			else if (rebuiltBody == body)
				identical++;
			else
				differing.Add(signature);
		}
		return new IlComparison(original.Count, identical, differing, missing);
	}

	/// <summary>Disassembly of every method with a body, keyed by declaring type and method header.</summary>
	static SortedDictionary<string, string> Disassemble(string assemblyPath)
	{
		using var file = new PEFile(assemblyPath);
		var metadata = file.Metadata;
		var methods = new SortedDictionary<string, string>(StringComparer.Ordinal);
		foreach (var handle in metadata.MethodDefinitions)
		{
			var method = metadata.GetMethodDefinition(handle);
			if (method.RelativeVirtualAddress == 0)
				continue;
			string signature = TypeName(metadata, method.GetDeclaringType()) + " " + Normalize(Render(d => d.DisassembleMethodHeader(file, handle)));
			string body = Normalize(Render(d => d.DisassembleMethod(file, handle)));
			// Overloads always differ in their header; the suffix only guards against identical renderings.
			string key = signature;
			for (int i = 2; methods.ContainsKey(key); i++)
				key = signature + " #" + i;
			methods.Add(key, body);
		}
		return methods;
	}

	static string Render(Action<ReflectionDisassembler> disassemble)
	{
		using var writer = new StringWriter();
		var disassembler = new ReflectionDisassembler(new PlainTextOutput(writer), CancellationToken.None) {
			DetectControlStructure = false,
		};
		disassemble(disassembler);
		return writer.ToString();
	}

	/// <summary>
	/// Keeps what determines execution: the signature, the locals and the instructions. Custom
	/// attribute blocks are dropped, because the compiler moves annotations such as
	/// NullableContextAttribute between a type and its members depending on the other members of
	/// the type; the relative virtual address only depends on where the method lands in the file.
	/// </summary>
	static string Normalize(string disassembly)
	{
		var kept = new List<string>();
		bool inAttributeBlob = false;
		foreach (string raw in disassembly.Split('\n'))
		{
			string line = raw.Trim();
			if (inAttributeBlob)
			{
				inAttributeBlob = line != ")";
				continue;
			}
			if (line.StartsWith(".custom ", StringComparison.Ordinal))
			{
				inAttributeBlob = line.EndsWith('(');
				continue;
			}
			// A ".param [n]" line without a default value only introduces attributes of that parameter.
			if (line.StartsWith(".param [", StringComparison.Ordinal) && !line.Contains('='))
				continue;
			if (line.Length > 0 && !line.Contains("RVA 0x", StringComparison.Ordinal))
				kept.Add(line);
		}
		return string.Join("\n", kept);
	}

	static string TypeName(MetadataReader metadata, TypeDefinitionHandle handle)
	{
		var type = metadata.GetTypeDefinition(handle);
		string name = metadata.GetString(type.Name);
		var declaring = type.GetDeclaringType();
		if (!declaring.IsNil)
			return TypeName(metadata, declaring) + "/" + name;
		string ns = metadata.GetString(type.Namespace);
		return ns.Length == 0 ? name : ns + "." + name;
	}
}
