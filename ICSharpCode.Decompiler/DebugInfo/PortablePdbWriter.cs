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
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.DebugInfo
{
	public class PortablePdbWriter
	{
		static readonly FileVersionInfo decompilerVersion = FileVersionInfo.GetVersionInfo(typeof(CSharpDecompiler).Assembly.Location);

		public static bool HasCodeViewDebugDirectoryEntry(PEFile file)
		{
			return file.Reader.ReadDebugDirectory().Any(entry => entry.Type == DebugDirectoryEntryType.CodeView);
		}

		private static bool IncludeTypeWhenGeneratingPdb(PEFile module, TypeDefinitionHandle type, DecompilerSettings settings)
		{
			var metadata = module.Metadata;
			var typeDef = metadata.GetTypeDefinition(type);
			string name = metadata.GetString(typeDef.Name);
			string ns = metadata.GetString(typeDef.Namespace);
			if (name == "<Module>" || CSharpDecompiler.MemberIsHidden(module, type, settings))
				return false;
			if (ns == "XamlGeneratedNamespace" && name == "GeneratedInternalTypeHelper")
				return false;
			if (!typeDef.IsNested && RemoveEmbeddedAttributes.attributeNames.Contains(ns + "." + name))
				return false;
			return true;
		}

		public static void WritePdb(
			PEFile file,
			CSharpDecompiler decompiler,
			DecompilerSettings settings,
			Stream targetStream,
			bool noLogo = false,
			BlobContentId? pdbId = null,
			IProgress<DecompilationProgress> progress = null)
		{
			MetadataBuilder metadata = new MetadataBuilder();
			MetadataReader reader = file.Metadata;
			var entrypointHandle = MetadataTokens.MethodDefinitionHandle(file.Reader.PEHeaders.CorHeader.EntryPointTokenOrRelativeVirtualAddress);

			var sequencePointBlobs = new Dictionary<MethodDefinitionHandle, (DocumentHandle Document, BlobHandle SequencePoints)>();
			var emptyList = new List<SequencePoint>();
			var localScopes = new List<(MethodDefinitionHandle Method, ImportScopeInfo Import, int Offset, int Length, HashSet<ILVariable> Locals)>();
			var stateMachineMethods = new List<(MethodDefinitionHandle MoveNextMethod, MethodDefinitionHandle KickoffMethod)>();
			var customDebugInfo = new List<(EntityHandle Parent, GuidHandle Guid, BlobHandle Blob)>();
			var customMethodDebugInfo = new List<(MethodDefinitionHandle Parent, GuidHandle Guid, BlobHandle Blob)>();
			var globalImportScope = metadata.AddImportScope(default, default);

			string BuildFileNameFromTypeName(TypeDefinitionHandle handle)
			{
				var typeName = handle.GetFullTypeName(reader).TopLevelTypeName;
				string ns = settings.UseNestedDirectoriesForNamespaces
					? WholeProjectDecompiler.CleanUpPath(typeName.Namespace)
					: WholeProjectDecompiler.CleanUpDirectoryName(typeName.Namespace);
				return Path.Combine(ns, WholeProjectDecompiler.CleanUpFileName(typeName.Name) + ".cs");
			}

			var sourceFiles = reader.GetTopLevelTypeDefinitions().Where(t => IncludeTypeWhenGeneratingPdb(file, t, settings)).GroupBy(BuildFileNameFromTypeName).ToList();
			DecompilationProgress currentProgress = new() {
				TotalUnits = sourceFiles.Count,
				UnitsCompleted = 0,
				Title = "Generating portable PDB..."
			};

			foreach (var sourceFile in sourceFiles)
			{
				// Generate syntax tree
				var syntaxTree = decompiler.DecompileTypes(sourceFile);

				if (progress != null)
				{
					currentProgress.UnitsCompleted++;
					progress.Report(currentProgress);
				}

				if (!syntaxTree.HasChildren)
					continue;

				// Generate source and checksum
				if (!noLogo)
					syntaxTree.InsertChildAfter(null, new Comment(" PDB and source generated by ICSharpCode.Decompiler " + decompilerVersion.FileVersion), Roles.Comment);
				var sourceText = SyntaxTreeToString(syntaxTree, settings);

				// Generate sequence points for the syntax tree
				var sequencePoints = decompiler.CreateSequencePoints(syntaxTree);

				// Generate other debug information
				var debugInfoGen = new DebugInfoGenerator(decompiler.TypeSystem);
				syntaxTree.AcceptVisitor(debugInfoGen);

				lock (metadata)
				{
					var sourceBlob = WriteSourceToBlob(metadata, sourceText, out var sourceCheckSum);
					var name = metadata.GetOrAddDocumentName(sourceFile.Key);

					// Create Document(Handle)
					var document = metadata.AddDocument(name,
						hashAlgorithm: metadata.GetOrAddGuid(KnownGuids.HashAlgorithmSHA256),
						hash: metadata.GetOrAddBlob(sourceCheckSum),
						language: metadata.GetOrAddGuid(KnownGuids.CSharpLanguageGuid));

					// Add embedded source to the PDB
					customDebugInfo.Add((document,
						metadata.GetOrAddGuid(KnownGuids.EmbeddedSource),
						sourceBlob));

					debugInfoGen.GenerateImportScopes(metadata, globalImportScope);

					localScopes.AddRange(debugInfoGen.LocalScopes);

					foreach (var function in debugInfoGen.Functions)
					{
						var method = function.MoveNextMethod ?? function.Method;
						var methodHandle = (MethodDefinitionHandle)method.MetadataToken;
						sequencePoints.TryGetValue(function, out var points);
						ProcessMethod(methodHandle, document, points, syntaxTree);
						if (function.MoveNextMethod != null)
						{
							stateMachineMethods.Add((
								(MethodDefinitionHandle)function.MoveNextMethod.MetadataToken,
								(MethodDefinitionHandle)function.Method.MetadataToken
							));
							customDebugInfo.Add((
								function.MoveNextMethod.MetadataToken,
								metadata.GetOrAddGuid(KnownGuids.StateMachineHoistedLocalScopes),
								metadata.GetOrAddBlob(BuildStateMachineHoistedLocalScopes(function))
							));
						}
						if (function.IsAsync)
						{
							customMethodDebugInfo.Add((methodHandle,
								metadata.GetOrAddGuid(KnownGuids.MethodSteppingInformation),
								metadata.GetOrAddBlob(function.AsyncDebugInfo.BuildBlob(methodHandle))));
						}
					}
				}
			}

			foreach (var method in reader.MethodDefinitions)
			{
				var md = reader.GetMethodDefinition(method);

				if (sequencePointBlobs.TryGetValue(method, out var info))
				{
					metadata.AddMethodDebugInformation(info.Document, info.SequencePoints);
				}
				else
				{
					metadata.AddMethodDebugInformation(default, default);
				}
			}

			localScopes.Sort((x, y) => {
				if (x.Method != y.Method)
				{
					return MetadataTokens.GetRowNumber(x.Method) - MetadataTokens.GetRowNumber(y.Method);
				}
				if (x.Offset != y.Offset)
				{
					return x.Offset - y.Offset;
				}
				return y.Length - x.Length;
			});
			foreach (var localScope in localScopes)
			{
				int nextRow = metadata.GetRowCount(TableIndex.LocalVariable) + 1;
				var firstLocalVariable = MetadataTokens.LocalVariableHandle(nextRow);

				foreach (var local in localScope.Locals.OrderBy(l => l.Index))
				{
					var localVarName = local.Name != null ? metadata.GetOrAddString(local.Name) : default;
					metadata.AddLocalVariable(LocalVariableAttributes.None, local.Index.Value, localVarName);
				}

				metadata.AddLocalScope(localScope.Method, localScope.Import.Handle, firstLocalVariable,
					default, localScope.Offset, localScope.Length);
			}

			stateMachineMethods.SortBy(row => MetadataTokens.GetRowNumber(row.MoveNextMethod));
			foreach (var row in stateMachineMethods)
			{
				metadata.AddStateMachineMethod(row.MoveNextMethod, row.KickoffMethod);
			}
			customMethodDebugInfo.SortBy(row => MetadataTokens.GetRowNumber(row.Parent));
			foreach (var row in customMethodDebugInfo)
			{
				metadata.AddCustomDebugInformation(row.Parent, row.Guid, row.Blob);
			}
			customDebugInfo.SortBy(row => MetadataTokens.GetRowNumber(row.Parent));
			foreach (var row in customDebugInfo)
			{
				metadata.AddCustomDebugInformation(row.Parent, row.Guid, row.Blob);
			}

			if (pdbId == null)
			{
				var debugDir = file.Reader.ReadDebugDirectory().FirstOrDefault(dir => dir.Type == DebugDirectoryEntryType.CodeView);
				var portable = file.Reader.ReadCodeViewDebugDirectoryData(debugDir);
				pdbId = new BlobContentId(portable.Guid, debugDir.Stamp);
			}

			PortablePdbBuilder serializer = new PortablePdbBuilder(metadata, GetRowCounts(reader), entrypointHandle, blobs => pdbId.Value);
			BlobBuilder blobBuilder = new BlobBuilder();
			serializer.Serialize(blobBuilder);
			blobBuilder.WriteContentTo(targetStream);

			void ProcessMethod(MethodDefinitionHandle method, DocumentHandle document,
				List<SequencePoint> sequencePoints, SyntaxTree syntaxTree)
			{
				var methodDef = reader.GetMethodDefinition(method);
				int localSignatureRowId;
				MethodBodyBlock methodBody;
				if (methodDef.RelativeVirtualAddress != 0)
				{
					methodBody = file.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
					localSignatureRowId = methodBody.LocalSignature.IsNil ? 0 : MetadataTokens.GetRowNumber(methodBody.LocalSignature);
				}
				else
				{
					methodBody = null;
					localSignatureRowId = 0;
				}

				// Check if sequence points were already processed - ILFunction gets defined in C# twice:
				// This may happen if a compiler-generated function gets transformed into a lambda expression,
				// but its method definition is not removed from the syntax tree.
				if (!sequencePointBlobs.ContainsKey(method))
				{
					if (sequencePoints?.Count > 0)
						sequencePointBlobs.Add(method, (document, EncodeSequencePoints(metadata, localSignatureRowId, sequencePoints)));
					else
						sequencePointBlobs.Add(method, (default, default));
				}
				else
				{
					Debug.Assert(false, "Duplicate sequence point definition detected: " + MetadataTokens.GetToken(method).ToString("X8"));
				}
			}
		}

		static BlobBuilder BuildStateMachineHoistedLocalScopes(ILFunction function)
		{
			var builder = new BlobBuilder();
			foreach (var variable in function.Variables.Where(v => v.StateMachineField != null).OrderBy(v => MetadataTokens.GetRowNumber(v.StateMachineField.MetadataToken)))
			{
				builder.WriteUInt32(0);
				builder.WriteUInt32((uint)function.CodeSize);
			}
			return builder;
		}

		static BlobHandle WriteSourceToBlob(MetadataBuilder metadata, string sourceText, out byte[] sourceCheckSum)
		{
			var builder = new BlobBuilder();
			using (var memory = new MemoryStream())
			{
				var deflate = new DeflateStream(memory, CompressionLevel.Optimal, leaveOpen: true);
				byte[] bytes = Encoding.UTF8.GetBytes(sourceText);
				deflate.Write(bytes, 0, bytes.Length);
				deflate.Close();
				byte[] buffer = memory.ToArray();
				builder.WriteInt32(bytes.Length); // compressed
				builder.WriteBytes(buffer);
				using (var hasher = SHA256.Create())
				{
					sourceCheckSum = hasher.ComputeHash(bytes);
				}
			}

			return metadata.GetOrAddBlob(builder);
		}

		static BlobHandle EncodeSequencePoints(MetadataBuilder metadata, int localSignatureRowId, List<SequencePoint> sequencePoints)
		{
			if (sequencePoints.Count == 0)
				return default;
			var writer = new BlobBuilder();
			// header:
			writer.WriteCompressedInteger(localSignatureRowId);

			int previousOffset = -1;
			int previousStartLine = -1;
			int previousStartColumn = -1;

			for (int i = 0; i < sequencePoints.Count; i++)
			{
				var sequencePoint = sequencePoints[i];
				// delta IL offset:
				if (i > 0)
					writer.WriteCompressedInteger(sequencePoint.Offset - previousOffset);
				else
					writer.WriteCompressedInteger(sequencePoint.Offset);
				previousOffset = sequencePoint.Offset;

				if (sequencePoint.IsHidden)
				{
					writer.WriteInt16(0);
					continue;
				}

				int lineDelta = sequencePoint.EndLine - sequencePoint.StartLine;
				int columnDelta = sequencePoint.EndColumn - sequencePoint.StartColumn;

				writer.WriteCompressedInteger(lineDelta);

				if (lineDelta == 0)
				{
					writer.WriteCompressedInteger(columnDelta);
				}
				else
				{
					writer.WriteCompressedSignedInteger(columnDelta);
				}

				if (previousStartLine < 0)
				{
					writer.WriteCompressedInteger(sequencePoint.StartLine);
					writer.WriteCompressedInteger(sequencePoint.StartColumn);
				}
				else
				{
					writer.WriteCompressedSignedInteger(sequencePoint.StartLine - previousStartLine);
					writer.WriteCompressedSignedInteger(sequencePoint.StartColumn - previousStartColumn);
				}

				previousStartLine = sequencePoint.StartLine;
				previousStartColumn = sequencePoint.StartColumn;
			}

			return metadata.GetOrAddBlob(writer);
		}

		static ImmutableArray<int> GetRowCounts(MetadataReader reader)
		{
			var builder = ImmutableArray.CreateBuilder<int>(MetadataTokens.TableCount);
			for (int i = 0; i < MetadataTokens.TableCount; i++)
			{
				builder.Add(reader.GetTableRowCount((TableIndex)i));
			}

			return builder.MoveToImmutable();
		}

		static string SyntaxTreeToString(SyntaxTree syntaxTree, DecompilerSettings settings)
		{
			StringWriter w = new StringWriter();
			TokenWriter tokenWriter = new TextWriterTokenWriter(w);
			tokenWriter = TokenWriter.WrapInWriterThatSetsLocationsInAST(tokenWriter);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
			return w.ToString();
		}
	}
}
