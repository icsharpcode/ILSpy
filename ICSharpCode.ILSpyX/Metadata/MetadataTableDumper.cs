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
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX.PdbProvider;

namespace ICSharpCode.ILSpyX.Metadata
{
	/// <summary>
	/// Implements the --dump-table mode: prints every row of a metadata table (RID, token,
	/// resolved names, heap offsets and coded indexes) as an aligned console table or as JSON.
	/// The columns of every table are spelled out explicitly (in ECMA-335 declaration order)
	/// so the output stays deterministic across runtime versions.
	/// </summary>
	public static class MetadataTableDumper
	{
		// The Cor tables plus the eight Portable-PDB debug tables, matching what the GUI's
		// metadata view shows. EnC tables are out of scope. A table that does not exist in the
		// file being dumped simply has no rows.
		static readonly TableIndex[] supportedTables = {
			TableIndex.Module, TableIndex.TypeRef, TableIndex.TypeDef, TableIndex.FieldPtr,
			TableIndex.Field, TableIndex.MethodPtr, TableIndex.MethodDef, TableIndex.ParamPtr,
			TableIndex.Param, TableIndex.InterfaceImpl, TableIndex.MemberRef, TableIndex.Constant,
			TableIndex.CustomAttribute, TableIndex.FieldMarshal, TableIndex.DeclSecurity,
			TableIndex.ClassLayout, TableIndex.FieldLayout, TableIndex.StandAloneSig,
			TableIndex.EventMap, TableIndex.EventPtr, TableIndex.Event, TableIndex.PropertyMap,
			TableIndex.PropertyPtr, TableIndex.Property, TableIndex.MethodSemantics,
			TableIndex.MethodImpl, TableIndex.ModuleRef, TableIndex.TypeSpec, TableIndex.ImplMap,
			TableIndex.FieldRva, TableIndex.Assembly, TableIndex.AssemblyRef, TableIndex.File,
			TableIndex.ExportedType, TableIndex.ManifestResource, TableIndex.NestedClass,
			TableIndex.GenericParam, TableIndex.MethodSpec, TableIndex.GenericParamConstraint,
			TableIndex.Document, TableIndex.MethodDebugInformation, TableIndex.LocalScope,
			TableIndex.LocalVariable, TableIndex.LocalConstant, TableIndex.ImportScope,
			TableIndex.StateMachineMethod, TableIndex.CustomDebugInformation,
		};

		public static string SupportedTableNames => string.Join(", ",
			supportedTables.Select(t => $"{t} (0x{(int)t:X2})"));

		public static bool TryParseTableName(string name, out TableIndex table)
		{
			// accept the ECMA-335 table number, decimal or 0x-prefixed hex, as an
			// alternative to the table name
			bool isNumber = name.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
				? int.TryParse(name.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int number)
				: int.TryParse(name, NumberStyles.None, CultureInfo.InvariantCulture, out number);
			if (isNumber)
			{
				table = (TableIndex)number;
				return supportedTables.Contains(table);
			}
			return Enum.TryParse(name, ignoreCase: true, out table)
				&& supportedTables.Contains(table);
		}

		public static int DumpTable(MetadataFile module, TextWriter output, TableIndex table, bool asJson)
		{
			// the rows of a debug table live in the PDB, which for a PE file is either embedded
			// in it or sits next to it; the provider owns the reader, so it outlives the dump
			using var debugInfo = TryLoadDebugMetadata(module, table);
			var source = debugInfo?.ToMetadataFile() ?? module;
			var rows = LoadRows(source.Metadata, table);
			if (asJson)
			{
				WriteJson(output, source.FileName, table, rows);
			}
			else
			{
				WriteConsoleTable(output, rows);
			}
			return 0;
		}

		/// <summary>
		/// True for the eight Portable-PDB debug tables, whose rows are stored in debug metadata
		/// rather than in the assembly.
		/// </summary>
		public static bool IsDebugTable(TableIndex table)
		{
			return table is TableIndex.Document or TableIndex.MethodDebugInformation
				or TableIndex.LocalScope or TableIndex.LocalVariable or TableIndex.LocalConstant
				or TableIndex.ImportScope or TableIndex.StateMachineMethod
				or TableIndex.CustomDebugInformation;
		}

		/// <summary>
		/// Finds the debug metadata belonging to <paramref name="module"/> when a debug table is
		/// asked for and the module itself carries no such rows. Null when the module already is
		/// (or has) the right metadata, or when no Portable PDB could be found.
		/// </summary>
		static PortableDebugInfoProvider? TryLoadDebugMetadata(MetadataFile module, TableIndex table)
		{
			if (!IsDebugTable(table) || module.Metadata.GetTableRowCount(table) > 0)
				return null;
			if (module is not PEFile peFile)
				return null;
			return DebugInfoUtils.LoadSymbols(peFile) as PortableDebugInfoProvider;
		}

		/// <summary>
		/// A row is an ordered list of (column, value) pairs; values are either int (RID)
		/// or already-formatted strings, so the console and JSON writers stay in sync.
		/// </summary>
		static List<List<(string Column, object Value)>> LoadRows(MetadataReader metadata, TableIndex table)
		{
			var rows = new List<List<(string, object)>>();
			int rid = 0;

			switch (table)
			{
				case TableIndex.Module:
					if (metadata.GetTableRowCount(TableIndex.Module) > 0)
					{
						var module = metadata.GetModuleDefinition();
						rows.Add(Row(metadata, 1, MetadataTokens.EntityHandle(TableIndex.Module, 1),
							("Generation", module.Generation),
							("Name", module.Name),
							("Mvid", module.Mvid),
							("GenerationId", module.GenerationId),
							("BaseGenerationId", module.BaseGenerationId)));
					}
					break;
				case TableIndex.TypeRef:
					foreach (var h in metadata.TypeReferences)
					{
						var typeRef = metadata.GetTypeReference(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("ResolutionScope", typeRef.ResolutionScope),
							("Name", typeRef.Name),
							("Namespace", typeRef.Namespace)));
					}
					break;
				case TableIndex.TypeDef:
					var listColumns = metadata.GetTypeDefListColumns().ToDictionary(r => r.Type, r => (r.FieldList, r.MethodList));
					foreach (var h in metadata.TypeDefinitions)
					{
						var typeDef = metadata.GetTypeDefinition(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", typeDef.Attributes),
							("Name", typeDef.Name),
							("Namespace", typeDef.Namespace),
							("BaseType", typeDef.BaseType),
							("FieldList", FormatRid(listColumns[h].FieldList)),
							("MethodList", FormatRid(listColumns[h].MethodList))));
					}
					break;
				case TableIndex.Field:
					foreach (var h in metadata.FieldDefinitions)
					{
						var field = metadata.GetFieldDefinition(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", field.Attributes),
							("Name", field.Name),
							("Signature", field.Signature)));
					}
					break;
				case TableIndex.MethodDef:
					var paramLists = metadata.GetMethodDefParamLists().ToDictionary(r => r.Method, r => r.ParamList);
					foreach (var h in metadata.MethodDefinitions)
					{
						var method = metadata.GetMethodDefinition(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("RVA", FormatHex(method.RelativeVirtualAddress)),
							("ImplAttributes", method.ImplAttributes),
							("Attributes", method.Attributes),
							("Name", method.Name),
							("Signature", method.Signature),
							("ParamList", FormatRid(paramLists[h]))));
					}
					break;
				case TableIndex.Param:
					foreach (int r in RidRange(metadata, table))
					{
						var param = metadata.GetParameter(MetadataTokens.ParameterHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("Attributes", param.Attributes),
							("SequenceNumber", param.SequenceNumber),
							("Name", param.Name)));
					}
					break;
				case TableIndex.InterfaceImpl:
					foreach (var (cls, iface) in metadata.GetInterfaceImplRows())
						rows.Add(Row(metadata, ++rid, table, ("Class", cls), ("Interface", iface)));
					break;
				case TableIndex.MemberRef:
					foreach (var h in metadata.MemberReferences)
					{
						var memberRef = metadata.GetMemberReference(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Parent", memberRef.Parent),
							("Name", memberRef.Name),
							("Signature", memberRef.Signature)));
					}
					break;
				case TableIndex.Constant:
					foreach (int r in RidRange(metadata, table))
					{
						var constant = metadata.GetConstant(MetadataTokens.ConstantHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("TypeCode", constant.TypeCode),
							("Parent", constant.Parent),
							("Value", constant.Value)));
					}
					break;
				case TableIndex.CustomAttribute:
					foreach (var h in metadata.CustomAttributes)
					{
						var attribute = metadata.GetCustomAttribute(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Parent", attribute.Parent),
							("Constructor", attribute.Constructor),
							("Value", attribute.Value)));
					}
					break;
				case TableIndex.FieldMarshal:
					foreach (var (parent, nativeType) in metadata.GetFieldMarshals())
						rows.Add(Row(metadata, ++rid, table, ("Parent", parent), ("NativeType", nativeType)));
					break;
				case TableIndex.DeclSecurity:
					foreach (var h in metadata.DeclarativeSecurityAttributes)
					{
						var declSecurity = metadata.GetDeclarativeSecurityAttribute(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Action", declSecurity.Action),
							("Parent", declSecurity.Parent),
							("PermissionSet", declSecurity.PermissionSet)));
					}
					break;
				case TableIndex.ClassLayout:
					foreach (var (packingSize, classSize, parent) in metadata.GetClassLayouts())
						rows.Add(Row(metadata, ++rid, table, ("PackingSize", packingSize), ("ClassSize", classSize), ("Parent", parent)));
					break;
				case TableIndex.FieldLayout:
					foreach (var (offset, field) in metadata.GetFieldLayoutRows())
						rows.Add(Row(metadata, ++rid, table, ("Offset", offset), ("Field", field)));
					break;
				case TableIndex.StandAloneSig:
					foreach (int r in RidRange(metadata, table))
					{
						var signature = metadata.GetStandaloneSignature(MetadataTokens.StandaloneSignatureHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("Signature", signature.Signature)));
					}
					break;
				case TableIndex.EventMap:
					foreach (var (parent, eventList) in metadata.GetEventMaps())
						rows.Add(Row(metadata, ++rid, table, ("Parent", parent), ("EventList", eventList)));
					break;
				case TableIndex.Event:
					foreach (var h in metadata.EventDefinitions)
					{
						var eventDef = metadata.GetEventDefinition(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", eventDef.Attributes),
							("Name", eventDef.Name),
							("Type", eventDef.Type)));
					}
					break;
				case TableIndex.PropertyMap:
					foreach (var (parent, propertyList) in metadata.GetPropertyMaps())
						rows.Add(Row(metadata, ++rid, table, ("Parent", parent), ("PropertyList", propertyList)));
					break;
				case TableIndex.Property:
					foreach (var h in metadata.PropertyDefinitions)
					{
						var property = metadata.GetPropertyDefinition(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", property.Attributes),
							("Name", property.Name),
							("Signature", property.Signature)));
					}
					break;
				case TableIndex.MethodSemantics:
					foreach (var (_, semantics, method, association) in metadata.GetMethodSemantics())
						rows.Add(Row(metadata, ++rid, table, ("Semantics", semantics), ("Method", method), ("Association", association)));
					break;
				case TableIndex.MethodImpl:
					foreach (int r in RidRange(metadata, table))
					{
						var methodImpl = metadata.GetMethodImplementation(MetadataTokens.MethodImplementationHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("Class", methodImpl.Type),
							("MethodBody", methodImpl.MethodBody),
							("MethodDeclaration", methodImpl.MethodDeclaration)));
					}
					break;
				case TableIndex.ModuleRef:
					foreach (var h in metadata.GetModuleReferences())
					{
						var moduleRef = metadata.GetModuleReference(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Name", moduleRef.Name)));
					}
					break;
				case TableIndex.TypeSpec:
					foreach (var h in metadata.GetTypeSpecifications())
					{
						var typeSpec = metadata.GetTypeSpecification(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Signature", typeSpec.Signature)));
					}
					break;
				case TableIndex.ImplMap:
					foreach (var (mappingFlags, memberForwarded, importName, importScope) in metadata.GetImplMaps())
						rows.Add(Row(metadata, ++rid, table, ("MappingFlags", mappingFlags), ("MemberForwarded", memberForwarded), ("ImportName", importName), ("ImportScope", importScope)));
					break;
				case TableIndex.FieldRva:
					foreach (var (rva, field) in metadata.GetFieldRvas())
						rows.Add(Row(metadata, ++rid, table, ("RVA", FormatHex(rva)), ("Field", field)));
					break;
				case TableIndex.Assembly:
					if (metadata.IsAssembly)
					{
						var assembly = metadata.GetAssemblyDefinition();
						rows.Add(Row(metadata, 1, MetadataTokens.EntityHandle(TableIndex.Assembly, 1),
							("HashAlgorithm", assembly.HashAlgorithm),
							("Version", assembly.Version),
							("Flags", assembly.Flags),
							("PublicKey", assembly.PublicKey),
							("Name", assembly.Name),
							("Culture", assembly.Culture)));
					}
					break;
				case TableIndex.AssemblyRef:
					foreach (var h in metadata.AssemblyReferences)
					{
						var assemblyRef = metadata.GetAssemblyReference(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Version", assemblyRef.Version),
							("Flags", assemblyRef.Flags),
							("PublicKeyOrToken", assemblyRef.PublicKeyOrToken),
							("Name", assemblyRef.Name),
							("Culture", assemblyRef.Culture),
							("HashValue", assemblyRef.HashValue)));
					}
					break;
				case TableIndex.File:
					foreach (var h in metadata.AssemblyFiles)
					{
						var file = metadata.GetAssemblyFile(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("ContainsMetadata", file.ContainsMetadata),
							("Name", file.Name),
							("HashValue", file.HashValue)));
					}
					break;
				case TableIndex.ExportedType:
					foreach (var h in metadata.ExportedTypes)
					{
						var exportedType = metadata.GetExportedType(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", exportedType.Attributes),
							("Name", exportedType.Name),
							("Namespace", exportedType.Namespace),
							("Implementation", exportedType.Implementation),
							("IsForwarder", exportedType.IsForwarder)));
					}
					break;
				case TableIndex.ManifestResource:
					foreach (var h in metadata.ManifestResources)
					{
						var resource = metadata.GetManifestResource(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Offset", resource.Offset),
							("Attributes", resource.Attributes),
							("Name", resource.Name),
							("Implementation", resource.Implementation)));
					}
					break;
				case TableIndex.NestedClass:
					foreach (var (nested, enclosing) in metadata.GetNestedClasses())
						rows.Add(Row(metadata, ++rid, table, ("NestedClass", nested), ("EnclosingClass", enclosing)));
					break;
				case TableIndex.GenericParam:
					foreach (int r in RidRange(metadata, table))
					{
						var genericParam = metadata.GetGenericParameter(MetadataTokens.GenericParameterHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("Number", genericParam.Index),
							("Attributes", genericParam.Attributes),
							("Owner", genericParam.Parent),
							("Name", genericParam.Name)));
					}
					break;
				case TableIndex.MethodSpec:
					foreach (var h in metadata.GetMethodSpecifications())
					{
						var methodSpec = metadata.GetMethodSpecification(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Method", methodSpec.Method),
							("Instantiation", methodSpec.Signature)));
					}
					break;
				case TableIndex.GenericParamConstraint:
					foreach (int r in RidRange(metadata, table))
					{
						var constraint = metadata.GetGenericParameterConstraint(MetadataTokens.GenericParameterConstraintHandle(r));
						rows.Add(Row(metadata, r, MetadataTokens.EntityHandle(table, r),
							("Owner", (EntityHandle)constraint.Parameter),
							("Constraint", constraint.Type)));
					}
					break;
				case TableIndex.Document:
					foreach (var h in metadata.Documents)
					{
						var document = metadata.GetDocument(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Name", document.Name),
							("HashAlgorithm", document.HashAlgorithm),
							("Hash", document.Hash),
							("Language", document.Language)));
					}
					break;
				case TableIndex.MethodDebugInformation:
					foreach (var h in metadata.MethodDebugInformation)
					{
						var debugInfo = metadata.GetMethodDebugInformation(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Document", debugInfo.Document),
							("SequencePoints", debugInfo.SequencePointsBlob),
							("LocalSignature", (EntityHandle)debugInfo.LocalSignature)));
					}
					break;
				case TableIndex.LocalScope:
					foreach (var h in metadata.LocalScopes)
					{
						var scope = metadata.GetLocalScope(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Method", scope.Method),
							("ImportScope", scope.ImportScope),
							("VariableList", scope.GetLocalVariables().FirstOrDefault()),
							("ConstantList", scope.GetLocalConstants().FirstOrDefault()),
							("StartOffset", scope.StartOffset),
							("Length", scope.Length)));
					}
					break;
				case TableIndex.LocalVariable:
					foreach (var h in metadata.LocalVariables)
					{
						var localVariable = metadata.GetLocalVariable(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Attributes", localVariable.Attributes),
							("Index", localVariable.Index),
							("Name", localVariable.Name)));
					}
					break;
				case TableIndex.LocalConstant:
					foreach (var h in metadata.LocalConstants)
					{
						var localConstant = metadata.GetLocalConstant(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Name", localConstant.Name),
							("Signature", localConstant.Signature)));
					}
					break;
				case TableIndex.ImportScope:
					foreach (var h in metadata.ImportScopes)
					{
						var scope = metadata.GetImportScope(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Parent", scope.Parent),
							("Imports", scope.ImportsBlob)));
					}
					break;
				case TableIndex.StateMachineMethod:
					foreach (var (moveNext, kickoff) in metadata.GetStateMachineMethods())
					{
						rows.Add(Row(metadata, ++rid, table,
							("MoveNextMethod", moveNext),
							("KickoffMethod", kickoff)));
					}
					break;
				case TableIndex.CustomDebugInformation:
					foreach (var h in metadata.CustomDebugInformation)
					{
						var debugInfo = metadata.GetCustomDebugInformation(h);
						rows.Add(Row(metadata, MetadataTokens.GetRowNumber(h), h,
							("Parent", debugInfo.Parent),
							("Kind", debugInfo.Kind),
							("KindName", GetCustomDebugInformationKindName(metadata, debugInfo.Kind)),
							("Value", debugInfo.Value)));
					}
					break;
				case TableIndex.EventPtr:
				case TableIndex.FieldPtr:
				case TableIndex.MethodPtr:
				case TableIndex.ParamPtr:
				case TableIndex.PropertyPtr:
					foreach (var target in metadata.GetPtrRows(table))
						rows.Add(Row(metadata, ++rid, table, ("Target", target)));
					break;
				default:
					throw new InvalidOperationException($"Table {table} is listed as supported but has no loader.");
			}
			return rows;
		}

		static IEnumerable<int> RidRange(MetadataReader metadata, TableIndex table)
		{
			return Enumerable.Range(1, metadata.GetTableRowCount(table));
		}

		static List<(string, object)> Row(MetadataReader metadata, int rid, EntityHandle handle, params (string Column, object Value)[] columns)
		{
			return BuildRow(metadata, rid, MetadataTokens.GetToken(handle), columns);
		}

		static List<(string, object)> Row(MetadataReader metadata, int rid, TableIndex table, params (string Column, object Value)[] columns)
		{
			// tables without an SRM handle type get their token synthesized from table id and RID
			return BuildRow(metadata, rid, ((int)table << 24) | rid, columns);
		}

		static List<(string, object)> BuildRow(MetadataReader metadata, int rid, int token, (string Column, object Value)[] columns)
		{
			var row = new List<(string, object)> {
				("RID", rid),
				("Token", FormatHex(token)),
			};
			foreach (var (column, value) in columns)
			{
				row.Add((column, FormatValue(metadata, value)));
			}
			return row;
		}

		static string FormatValue(MetadataReader metadata, object value)
		{
			switch (value)
			{
				case null:
					return "";
				case string s:
					return s;
				case StringHandle sh:
					return sh.IsNil ? "" : metadata.GetString(sh);
				case BlobHandle bh:
					return bh.IsNil ? "nil" : FormatHex(MetadataTokens.GetHeapOffset(bh));
				case GuidHandle gh:
					return gh.IsNil ? "nil" : metadata.GetGuid(gh).ToString();
				case EntityHandle eh:
					return eh.IsNil ? "nil" : FormatHex(MetadataTokens.GetToken(eh));
				// The concrete handle cases below are not dead code: pattern matching tests the
				// runtime type of the boxed value and ignores SRM's user-defined conversions, so
				// a boxed MethodDefinitionHandle does not match the EntityHandle case above.
				case MethodDefinitionHandle mdh:
					return FormatValue(metadata, (EntityHandle)mdh);
				case FieldDefinitionHandle fdh:
					return FormatValue(metadata, (EntityHandle)fdh);
				case TypeDefinitionHandle tdh:
					return FormatValue(metadata, (EntityHandle)tdh);
				case EventDefinitionHandle edh:
					return FormatValue(metadata, (EntityHandle)edh);
				case PropertyDefinitionHandle pdh:
					return FormatValue(metadata, (EntityHandle)pdh);
				case ModuleReferenceHandle mrh:
					return FormatValue(metadata, (EntityHandle)mrh);
				case StandaloneSignatureHandle ssh:
					return FormatValue(metadata, (EntityHandle)ssh);
				case DocumentHandle dh:
					return FormatValue(metadata, (EntityHandle)dh);
				case MethodDebugInformationHandle mdih:
					return FormatValue(metadata, (EntityHandle)mdih);
				case LocalScopeHandle lsh:
					return FormatValue(metadata, (EntityHandle)lsh);
				case LocalVariableHandle lvh:
					return FormatValue(metadata, (EntityHandle)lvh);
				case LocalConstantHandle lch:
					return FormatValue(metadata, (EntityHandle)lch);
				case ImportScopeHandle ish:
					return FormatValue(metadata, (EntityHandle)ish);
				case CustomDebugInformationHandle cdih:
					return FormatValue(metadata, (EntityHandle)cdih);
				// the document name is a blob in a dedicated encoding; SRM decodes it to the
				// source path, which is what the row is about
				case DocumentNameBlobHandle dnbh:
					return dnbh.IsNil ? "" : metadata.GetString(dnbh);
				case Enum e:
					return e.ToString();
				case bool b:
					return b ? "true" : "false";
				case Version v:
					return v.ToString();
				default:
					// Reject unlisted handle types instead of letting Convert.ToString render
					// them as their type name; cast handles to EntityHandle at the call site.
					if (value.GetType().Namespace == "System.Reflection.Metadata")
						throw new InvalidOperationException($"Unhandled metadata handle type {value.GetType().Name}.");
					return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "";
			}
		}

		static string GetCustomDebugInformationKindName(MetadataReader metadata, GuidHandle kind)
		{
			if (kind.IsNil)
				return "";
			return KnownGuids.GetCustomDebugInformationKindName(metadata.GetGuid(kind)) ?? "Unknown";
		}

		static string FormatHex(int value)
		{
			return "0x" + value.ToString("X8", CultureInfo.InvariantCulture);
		}

		static string FormatRid(int value)
		{
			return value.ToString(CultureInfo.InvariantCulture);
		}

		static void WriteConsoleTable(TextWriter output, List<List<(string Column, object Value)>> rows)
		{
			if (rows.Count == 0)
			{
				output.WriteLine("0 rows");
				return;
			}
			var columns = rows[0].Select(c => c.Column).ToArray();
			int[] widths = columns.Select(c => c.Length).ToArray();
			foreach (var row in rows)
			{
				for (int i = 0; i < row.Count; i++)
				{
					widths[i] = Math.Max(widths[i], Convert.ToString(row[i].Value, CultureInfo.InvariantCulture)!.Length);
				}
			}
			output.WriteLine(string.Join("  ", columns.Select((c, i) => c.PadRight(widths[i]))).TrimEnd());
			output.WriteLine(string.Join("  ", widths.Select(w => new string('-', w))));
			foreach (var row in rows)
			{
				output.WriteLine(string.Join("  ", row.Select((c, i) => Convert.ToString(c.Value, CultureInfo.InvariantCulture)!.PadRight(widths[i]))).TrimEnd());
			}
		}

		static void WriteJson(TextWriter output, string assemblyFileName, TableIndex table, List<List<(string Column, object Value)>> rows)
		{
			using var stream = new MemoryStream();
			using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true }))
			{
				writer.WriteStartObject();
				writer.WriteString("assembly", assemblyFileName);
				writer.WriteString("table", table.ToString());
				writer.WriteNumber("rowCount", rows.Count);
				writer.WriteStartArray("rows");
				foreach (var row in rows)
				{
					writer.WriteStartObject();
					foreach (var (column, value) in row)
					{
						if (value is int i)
							writer.WriteNumber(column, i);
						else
							writer.WriteString(column, (string)value);
					}
					writer.WriteEndObject();
				}
				writer.WriteEndArray();
				writer.WriteEndObject();
			}
			output.WriteLine(System.Text.Encoding.UTF8.GetString(stream.ToArray()));
		}
	}
}
