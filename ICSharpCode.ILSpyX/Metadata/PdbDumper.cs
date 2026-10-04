// Copyright (c) 2026 Siegfried Pammer
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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

using ICSharpCode.ILSpyX.PdbProvider;

using Mono.Cecil;
using Mono.Cecil.Cil;

using KnownGuids = ICSharpCode.Decompiler.DebugInfo.KnownGuids;

namespace ICSharpCode.ILSpyX.Metadata
{
	/// <summary>
	/// Prints the debug information of an assembly's PDB: documents, per-method sequence points,
	/// the scope tree with its variables, constants and imports, and custom debug information.
	/// </summary>
	/// <remarks>
	/// Reading goes through Mono.Cecil, whose symbol readers cover both the Portable PDB and the
	/// Windows (native MSF) format in managed code, so the dump works the same on every OS. The
	/// raw table view of a Portable PDB is a different thing and belongs to
	/// <see cref="MetadataTableDumper"/>; a Windows PDB has no metadata tables at all.
	/// </remarks>
	public static class PdbDumper
	{
		/// <summary>
		/// Thrown when the assembly has no PDB next to it, or the given PDB cannot be read.
		/// </summary>
		public sealed class NoDebugSymbolsException : Exception
		{
			public NoDebugSymbolsException(string message, Exception? innerException = null)
				: base(message, innerException)
			{
			}
		}

		/// <summary>
		/// Writes the debug information of <paramref name="assemblyFileName"/> to <paramref name="output"/>.
		/// </summary>
		/// <param name="pdbFileName">
		/// The PDB to read. When null, the symbol file next to the assembly (or embedded in it) is used.
		/// </param>
		/// <exception cref="NoDebugSymbolsException">No readable debug symbols were found.</exception>
		public static void Dump(string assemblyFileName, string? pdbFileName, TextWriter output, bool asJson)
		{
			using var module = ReadModuleWithSymbols(assemblyFileName, pdbFileName);
			Dump(module, assemblyFileName, output, asJson);
		}

		/// <summary>
		/// Writes the debug information of the assembly image in <paramref name="assemblyStream"/> to <paramref name="output"/>.
		/// </summary>
		/// <param name="pdbFileName">
		/// The PDB to read. When null, only symbols embedded in the assembly image can be used.
		/// </param>
		/// <exception cref="NoDebugSymbolsException">No readable debug symbols were found.</exception>
		public static void Dump(Stream assemblyStream, string assemblyName, string? pdbFileName, TextWriter output, bool asJson)
		{
			using var module = ReadModuleWithSymbols(assemblyStream, assemblyName, pdbFileName);
			Dump(module, assemblyName, output, asJson);
		}

		static void Dump(ModuleDefinition module, string assemblyName, TextWriter output, bool asJson)
		{
			var methods = EnumerateTypes(module.Types)
				.SelectMany(t => t.Methods)
				.Where(m => m.DebugInformation != null && HasContent(m.DebugInformation))
				.ToList();
			var documents = CollectDocuments(methods);

			if (asJson)
				WriteJson(output, module, assemblyName, documents, methods);
			else
				WriteText(output, module, assemblyName, documents, methods);
		}

		static ModuleDefinition ReadModuleWithSymbols(string assemblyFileName, string? pdbFileName)
		{
			var parameters = new ReaderParameters { ReadSymbols = true, InMemory = true };
			ApplyExplicitSymbolFile(parameters, pdbFileName);
			ModuleDefinition module;
			try
			{
				module = ModuleDefinition.ReadModule(assemblyFileName, parameters);
			}
			catch (Exception ex) when (ex is FileNotFoundException or InvalidOperationException or BadImageFormatException)
			{
				throw new NoDebugSymbolsException(
					$"No debug symbols could be read for '{assemblyFileName}'.", ex);
			}
			if (module.SymbolReader == null)
			{
				module.Dispose();
				throw new NoDebugSymbolsException($"No debug symbols found for '{assemblyFileName}'.");
			}
			return module;
		}

		static ModuleDefinition ReadModuleWithSymbols(Stream assemblyStream, string assemblyName, string? pdbFileName)
		{
			var parameters = new ReaderParameters { ReadSymbols = true, InMemory = true };
			ApplyExplicitSymbolFile(parameters, pdbFileName);
			ModuleDefinition module;
			try
			{
				assemblyStream.Position = 0;
				module = ModuleDefinition.ReadModule(assemblyStream, parameters);
			}
			catch (Exception ex) when (ex is FileNotFoundException or InvalidOperationException or BadImageFormatException)
			{
				throw new NoDebugSymbolsException(
					$"No debug symbols could be read for '{assemblyName}'.", ex);
			}
			if (module.SymbolReader == null)
			{
				module.Dispose();
				throw new NoDebugSymbolsException($"No debug symbols found for '{assemblyName}'.");
			}
			return module;
		}

		static void ApplyExplicitSymbolFile(ReaderParameters parameters, string? pdbFileName)
		{
			if (pdbFileName == null)
				return;

			// SymbolStream is only honoured together with an explicit provider, and the
			// provider must match the format of the file the user named
			parameters.SymbolStream = File.OpenRead(pdbFileName);
			parameters.SymbolReaderProvider = IsWindowsPdb(pdbFileName)
				? new Mono.Cecil.Pdb.NativePdbReaderProvider()
				: new Mono.Cecil.Cil.PortablePdbReaderProvider();
		}

		static bool IsWindowsPdb(string pdbFileName)
		{
			using var stream = File.OpenRead(pdbFileName);
			return DebugInfoUtils.IsWindowsPdb(stream);
		}

		static IEnumerable<TypeDefinition> EnumerateTypes(IEnumerable<TypeDefinition> types)
		{
			foreach (var type in types)
			{
				yield return type;
				foreach (var nested in EnumerateTypes(type.NestedTypes))
					yield return nested;
			}
		}

		static bool HasContent(MethodDebugInformation debugInfo)
		{
			return debugInfo.HasSequencePoints
				|| debugInfo.Scope != null
				|| debugInfo.StateMachineKickOffMethod != null
				|| debugInfo.HasCustomDebugInformations;
		}

		/// <summary>
		/// Collects the documents the methods refer to. Neither symbol reader exposes the
		/// document list itself, so the sequence points are the only way to reach them.
		/// </summary>
		static List<Document> CollectDocuments(List<MethodDefinition> methods)
		{
			var documents = new Dictionary<string, Document>(StringComparer.Ordinal);
			foreach (var method in methods)
			{
				if (!method.DebugInformation.HasSequencePoints)
					continue;
				foreach (var point in method.DebugInformation.SequencePoints)
				{
					if (point.Document != null)
						documents[point.Document.Url] = point.Document;
				}
			}
			return documents.Values.OrderBy(d => d.Url, StringComparer.Ordinal).ToList();
		}

		static string FormatToken(MetadataToken token) => "0x" + token.ToUInt32().ToString("X8", CultureInfo.InvariantCulture);

		static string FormatOffset(int offset) => "IL_" + offset.ToString("X4", CultureInfo.InvariantCulture);

		/// <summary>
		/// Reading <see cref="InstructionOffset.Offset"/> of an end-of-method offset throws, so
		/// that case gets its own text.
		/// </summary>
		static string FormatOffset(InstructionOffset offset)
		{
			return offset.IsEndOfMethod ? "end" : FormatOffset(offset.Offset);
		}

		static string FormatSpan(SequencePoint point)
		{
			return point.IsHidden
				? "hidden"
				: $"({point.StartLine},{point.StartColumn})-({point.EndLine},{point.EndColumn})";
		}

		static string FormatHash(byte[]? hash)
		{
			return hash == null ? "" : string.Concat(hash.Select(b => b.ToString("x2", CultureInfo.InvariantCulture)));
		}

		static string FormatConstantValue(object? value)
		{
			return value switch {
				null => "null",
				string s => "\"" + s + "\"",
				IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
				_ => value.ToString() ?? "",
			};
		}

		static string FormatImport(ImportTarget target)
		{
			var text = new StringBuilder(target.Kind.ToString());
			if (target.Alias != null)
				text.Append(" ").Append(target.Alias).Append(" =");
			if (target.Namespace != null)
				text.Append(" ").Append(target.Namespace);
			if (target.Type != null)
				text.Append(" ").Append(target.Type.FullName);
			if (target.AssemblyReference != null)
				text.Append(" [").Append(target.AssemblyReference.Name).Append("]");
			return text.ToString();
		}

		static string DescribeCustomDebugInformation(CustomDebugInformation info)
		{
			return info switch {
				// Cecil hands every kind it does not model back as raw bytes; name the ones
				// Roslyn documents so the dump is readable
				BinaryCustomDebugInformation binary =>
					$"{KnownGuids.GetCustomDebugInformationKindName(info.Identifier) ?? info.Identifier.ToString()} ({binary.Data?.Length ?? 0} bytes)",
				StateMachineScopeDebugInformation scopes =>
					$"{info.Kind} " + string.Join(", ", scopes.Scopes.Select(s => $"{FormatOffset(s.Start)}..{FormatOffset(s.End)}")),
				AsyncMethodBodyDebugInformation async =>
					$"{info.Kind} catch handler {FormatOffset(async.CatchHandler)}, {async.Resumes.Count} resume point(s)",
				EmbeddedSourceDebugInformation embedded => $"{info.Kind} ({embedded.Content?.Length ?? 0} bytes, compressed={embedded.Compress})",
				SourceLinkDebugInformation sourceLink => $"{info.Kind} {sourceLink.Content}",
				_ => info.Kind.ToString(),
			};
		}

		static void WriteText(TextWriter output, ModuleDefinition module, string assemblyFileName,
			List<Document> documents, List<MethodDefinition> methods)
		{
			output.WriteLine($"Assembly: {assemblyFileName}");
			output.WriteLine($"Symbols:  {module.SymbolReader!.GetType().Name}");
			output.WriteLine();

			if (module.HasCustomDebugInformations)
			{
				// SourceLink and the compilation options are recorded against the module, not
				// against any single method
				output.WriteLine("Module custom debug information");
				foreach (var info in module.CustomDebugInformations)
				{
					output.WriteLine($"  {DescribeCustomDebugInformation(info)}");
				}
				output.WriteLine();
			}

			output.WriteLine($"Documents ({documents.Count})");
			foreach (var document in documents)
			{
				output.WriteLine($"  {document.Url}");
				output.WriteLine($"    language={document.Language} hashAlgorithm={document.HashAlgorithm} hash={FormatHash(document.Hash)}");
				if (document.EmbeddedSource is { Length: > 0 } embedded)
					output.WriteLine($"    embeddedSource={embedded.Length} bytes");
			}
			output.WriteLine();

			output.WriteLine($"Methods ({methods.Count})");
			foreach (var method in methods)
			{
				var debugInfo = method.DebugInformation;
				output.WriteLine($"  {FormatToken(method.MetadataToken)} {method.FullName}");
				if (debugInfo.StateMachineKickOffMethod != null)
					output.WriteLine($"    kickoff: {FormatToken(debugInfo.StateMachineKickOffMethod.MetadataToken)} {debugInfo.StateMachineKickOffMethod.FullName}");
				if (debugInfo.HasSequencePoints)
				{
					output.WriteLine("    Sequence points:");
					foreach (var point in debugInfo.SequencePoints)
					{
						output.WriteLine($"      {FormatOffset(point.Offset)}  {FormatSpan(point)}  {point.Document?.Url}");
					}
				}
				if (debugInfo.Scope != null)
					WriteScopeText(output, debugInfo.Scope, "    ");
				foreach (var info in debugInfo.CustomDebugInformations)
				{
					output.WriteLine($"    CDI: {DescribeCustomDebugInformation(info)}");
				}
			}
		}

		static void WriteScopeText(TextWriter output, ScopeDebugInformation scope, string indent)
		{
			output.WriteLine($"{indent}Scope {FormatOffset(scope.Start)}..{FormatOffset(scope.End)}");
			foreach (var variable in scope.Variables)
			{
				output.WriteLine($"{indent}  [{variable.Index}] {variable.Name}{(variable.IsDebuggerHidden ? " (hidden)" : "")}");
			}
			foreach (var constant in scope.Constants)
			{
				output.WriteLine($"{indent}  const {constant.Name} : {constant.ConstantType?.FullName} = {FormatConstantValue(constant.Value)}");
			}
			for (var import = scope.Import; import != null; import = import.Parent)
			{
				foreach (var target in import.Targets)
				{
					output.WriteLine($"{indent}  import {FormatImport(target)}");
				}
			}
			foreach (var nested in scope.Scopes)
			{
				WriteScopeText(output, nested, indent + "  ");
			}
		}

		static void WriteJson(TextWriter output, ModuleDefinition module, string assemblyFileName,
			List<Document> documents, List<MethodDefinition> methods)
		{
			using var stream = new MemoryStream();
			using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
			{
				writer.WriteStartObject();
				writer.WriteString("assembly", assemblyFileName);
				writer.WriteString("symbolReader", module.SymbolReader!.GetType().Name);

				writer.WriteStartArray("moduleCustomDebugInformation");
				foreach (var info in module.CustomDebugInformations)
				{
					writer.WriteStartObject();
					writer.WriteString("kind", info.Kind.ToString());
					writer.WriteString("identifier", info.Identifier.ToString());
					writer.WriteString("description", DescribeCustomDebugInformation(info));
					writer.WriteEndObject();
				}
				writer.WriteEndArray();

				writer.WriteStartArray("documents");
				foreach (var document in documents)
				{
					writer.WriteStartObject();
					writer.WriteString("url", document.Url);
					writer.WriteString("language", document.Language.ToString());
					writer.WriteString("hashAlgorithm", document.HashAlgorithm.ToString());
					writer.WriteString("hash", FormatHash(document.Hash));
					writer.WriteNumber("embeddedSourceLength", document.EmbeddedSource?.Length ?? 0);
					writer.WriteEndObject();
				}
				writer.WriteEndArray();

				writer.WriteStartArray("methods");
				foreach (var method in methods)
				{
					var debugInfo = method.DebugInformation;
					writer.WriteStartObject();
					writer.WriteString("token", FormatToken(method.MetadataToken));
					writer.WriteString("name", method.Name);
					writer.WriteString("fullName", method.FullName);
					if (debugInfo.StateMachineKickOffMethod != null)
						writer.WriteString("kickoffMethod", debugInfo.StateMachineKickOffMethod.FullName);

					writer.WriteStartArray("sequencePoints");
					foreach (var point in debugInfo.SequencePoints)
					{
						writer.WriteStartObject();
						writer.WriteNumber("offset", point.Offset);
						writer.WriteBoolean("hidden", point.IsHidden);
						writer.WriteNumber("startLine", point.StartLine);
						writer.WriteNumber("startColumn", point.StartColumn);
						writer.WriteNumber("endLine", point.EndLine);
						writer.WriteNumber("endColumn", point.EndColumn);
						writer.WriteString("document", point.Document?.Url);
						writer.WriteEndObject();
					}
					writer.WriteEndArray();

					if (debugInfo.Scope != null)
					{
						writer.WritePropertyName("scope");
						WriteScopeJson(writer, debugInfo.Scope);
					}

					writer.WriteStartArray("customDebugInformation");
					foreach (var info in debugInfo.CustomDebugInformations)
					{
						writer.WriteStartObject();
						writer.WriteString("kind", info.Kind.ToString());
						writer.WriteString("identifier", info.Identifier.ToString());
						writer.WriteString("description", DescribeCustomDebugInformation(info));
						writer.WriteEndObject();
					}
					writer.WriteEndArray();

					writer.WriteEndObject();
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			output.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));
		}

		static void WriteScopeJson(Utf8JsonWriter writer, ScopeDebugInformation scope)
		{
			writer.WriteStartObject();
			writer.WriteString("start", FormatOffset(scope.Start));
			writer.WriteString("end", FormatOffset(scope.End));

			writer.WriteStartArray("variables");
			foreach (var variable in scope.Variables)
			{
				writer.WriteStartObject();
				writer.WriteNumber("index", variable.Index);
				writer.WriteString("name", variable.Name);
				writer.WriteBoolean("debuggerHidden", variable.IsDebuggerHidden);
				writer.WriteEndObject();
			}
			writer.WriteEndArray();

			writer.WriteStartArray("constants");
			foreach (var constant in scope.Constants)
			{
				writer.WriteStartObject();
				writer.WriteString("name", constant.Name);
				writer.WriteString("type", constant.ConstantType?.FullName);
				writer.WriteString("value", FormatConstantValue(constant.Value));
				writer.WriteEndObject();
			}
			writer.WriteEndArray();

			writer.WriteStartArray("imports");
			for (var import = scope.Import; import != null; import = import.Parent)
			{
				foreach (var target in import.Targets)
				{
					writer.WriteStringValue(FormatImport(target));
				}
			}
			writer.WriteEndArray();

			writer.WriteStartArray("scopes");
			foreach (var nested in scope.Scopes)
			{
				WriteScopeJson(writer, nested);
			}
			writer.WriteEndArray();

			writer.WriteEndObject();
		}
	}
}
