// Copyright (c) 2026 Christoph Wille
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
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// The canonical metadata listing of rust/ilspy-decompiler/src/metadata/listing.rs, produced
/// from System.Reflection.Metadata. The format is defined there; keep the two in step.
/// </summary>
static class MetadataListing
{
	const int MaxNameDepth = 64;

	public static string Create(byte[] image)
	{
		using var pe = new PEReader(ImmutableArray.Create(image));
		MetadataReader reader;
		try
		{
			if (!pe.HasMetadata)
				return "NOT MANAGED\n";
			reader = pe.GetMetadataReader(MetadataReaderOptions.None);
		}
		catch (BadImageFormatException)
		{
			return "BAD IMAGE\n";
		}
		return new Lister(pe, reader).Write();
	}

	static string S(Func<object> f)
	{
		try
		{
			return Convert.ToString(f(), System.Globalization.CultureInfo.InvariantCulture)!;
		}
		catch (Exception)
		{
			return "!bad";
		}
	}

	static string Tok(EntityHandle h) => MetadataTokens.GetToken(h).ToString("x8");

	static string Tokens<T>(IEnumerable<T> handles, Func<T, EntityHandle> toEntity)
		=> "[" + string.Join(",", handles.Select(h => Tok(toEntity(h)))) + "]";

	static string Hex(byte[] bytes) => Convert.ToHexStringLower(bytes);

	static ulong Fnv1a(byte[] bytes)
	{
		ulong h = 0xcbf29ce484222325;
		foreach (byte b in bytes)
			h = (h ^ b) * 0x100000001b3;
		return h;
	}

	sealed class Lister(PEReader pe, MetadataReader r) : ISignatureTypeProvider<string, object?>
	{
		readonly StringBuilder sb = new();
		int depth;

		void Line(string s) => sb.Append(s).Append('\n');

		string Str(StringHandle h) => r.GetString(h);

		string Blob(BlobHandle h) => Hex(r.GetBlobBytes(h));

		public string Write()
		{
			Line($"METADATA {r.MetadataVersion}");
			var cor = pe.PEHeaders.CorHeader!;
			Line($"CLI flags={(uint)cor.Flags:x8} entry={cor.EntryPointTokenOrRelativeVirtualAddress:x8}");
			Line($"HEAP strings={r.GetHeapSize(HeapIndex.String)} blob={r.GetHeapSize(HeapIndex.Blob)} guid={r.GetHeapSize(HeapIndex.Guid)} us={r.GetHeapSize(HeapIndex.UserString)}");
			for (int i = 0; i <= (int)TableIndex.GenericParamConstraint; i++)
			{
				var t = (TableIndex)i;
				Line($"TABLE {i:x2} rows={r.GetTableRowCount(t)} size={r.GetTableRowSize(t)}");
			}
			if (r.GetTableRowCount(TableIndex.Module) > 0)
			{
				var m = r.GetModuleDefinition();
				Line($"MODULE gen={S(() => m.Generation)} name={S(() => Str(m.Name))} mvid={S(() => r.GetGuid(m.Mvid))} encid={S(() => r.GetGuid(m.GenerationId))} encbase={S(() => r.GetGuid(m.BaseGenerationId))}");
			}
			foreach (var h in Rows(TableIndex.TypeRef, MetadataTokens.TypeReferenceHandle))
			{
				var t = r.GetTypeReference(h);
				Line($"TYPEREF {Tok(h)} scope={S(() => Tok(t.ResolutionScope))} name={S(() => Str(t.Name))} ns={S(() => Str(t.Namespace))}");
			}
			foreach (var h in Rows(TableIndex.TypeDef, MetadataTokens.TypeDefinitionHandle))
			{
				var t = r.GetTypeDefinition(h);
				Line($"TYPEDEF {Tok(h)} flags={S(() => ((uint)t.Attributes).ToString("x8"))} name={S(() => Str(t.Name))} ns={S(() => Str(t.Namespace))} extends={S(() => Tok(t.BaseType))} decl={S(() => Tok(t.GetDeclaringType()))} layout={S(() => { var l = t.GetLayout(); return $"{l.PackingSize},{l.Size}"; })} fields={S(() => Tokens(t.GetFields(), x => x))} methods={S(() => Tokens(t.GetMethods(), x => x))} events={S(() => Tokens(t.GetEvents(), x => x))} props={S(() => Tokens(t.GetProperties(), x => x))} ifaces={S(() => Tokens(t.GetInterfaceImplementations(), x => x))}");
			}
			foreach (var h in Rows(TableIndex.Field, MetadataTokens.FieldDefinitionHandle))
			{
				var f = r.GetFieldDefinition(h);
				Line($"FIELD {Tok(h)} flags={S(() => ((ushort)f.Attributes).ToString("x4"))} name={S(() => Str(f.Name))} sig={S(() => f.DecodeSignature(this, null))} const={Tok(f.GetDefaultValue())} rva={S(() => f.GetRelativeVirtualAddress())} offset={S(() => f.GetOffset())} marshal={S(() => Blob(f.GetMarshallingDescriptor()))}");
			}
			foreach (var h in Rows(TableIndex.MethodDef, MetadataTokens.MethodDefinitionHandle))
			{
				var m = r.GetMethodDefinition(h);
				Line($"METHOD {Tok(h)} rva={S(() => m.RelativeVirtualAddress)} impl={S(() => ((ushort)m.ImplAttributes).ToString("x4"))} flags={S(() => ((ushort)m.Attributes).ToString("x4"))} name={S(() => Str(m.Name))} sig={S(() => Method(m.DecodeSignature(this, null)))} params={S(() => Tokens(m.GetParameters(), x => x))} import={S(() => { var i = m.GetImport(); return $"{(ushort)i.Attributes:x4},{Str(i.Name)},{Tok(i.Module)}"; })}");
				if (m.RelativeVirtualAddress != 0)
					Body(h, m.RelativeVirtualAddress);
			}
			foreach (var h in Rows(TableIndex.Param, MetadataTokens.ParameterHandle))
			{
				var p = r.GetParameter(h);
				Line($"PARAM {Tok(h)} flags={S(() => ((ushort)p.Attributes).ToString("x4"))} seq={S(() => p.SequenceNumber)} name={S(() => Str(p.Name))} const={Tok(p.GetDefaultValue())} marshal={S(() => Blob(p.GetMarshallingDescriptor()))}");
			}
			foreach (var h in Rows(TableIndex.InterfaceImpl, MetadataTokens.InterfaceImplementationHandle))
			{
				var i = r.GetInterfaceImplementation(h);
				Line($"IFACE {Tok(h)} iface={S(() => Tok(i.Interface))}");
			}
			foreach (var h in Rows(TableIndex.MemberRef, MetadataTokens.MemberReferenceHandle))
			{
				var m = r.GetMemberReference(h);
				Line($"MEMBERREF {Tok(h)} parent={S(() => Tok(m.Parent))} name={S(() => Str(m.Name))} sig={S(() => m.GetKind() == MemberReferenceKind.Field ? "field " + m.DecodeFieldSignature(this, null) : Method(m.DecodeMethodSignature(this, null)))}");
			}
			foreach (var h in Rows(TableIndex.Constant, MetadataTokens.ConstantHandle))
			{
				var c = r.GetConstant(h);
				Line($"CONSTANT {Tok(h)} type={S(() => ((byte)c.TypeCode).ToString("x2"))} parent={S(() => Tok(c.Parent))} value={S(() => Blob(c.Value))}");
			}
			foreach (var h in Rows(TableIndex.CustomAttribute, MetadataTokens.CustomAttributeHandle))
			{
				var c = r.GetCustomAttribute(h);
				Line($"CA {Tok(h)} parent={S(() => Tok(c.Parent))} ctor={S(() => Tok(c.Constructor))} value={S(() => Blob(c.Value))}");
			}
			foreach (var h in Rows(TableIndex.DeclSecurity, MetadataTokens.DeclarativeSecurityAttributeHandle))
			{
				var d = r.GetDeclarativeSecurityAttribute(h);
				Line($"DECLSEC {Tok(h)} action={S(() => (ushort)d.Action)} parent={S(() => Tok(d.Parent))} perms={S(() => Blob(d.PermissionSet))}");
			}
			foreach (var h in Rows(TableIndex.StandAloneSig, MetadataTokens.StandaloneSignatureHandle))
			{
				var s = r.GetStandaloneSignature(h);
				Line($"STANDALONESIG {Tok(h)} sig={S(() => s.GetKind() == StandaloneSignatureKind.Method ? "method " + Method(s.DecodeMethodSignature(this, null)) : "locals(" + string.Join(",", s.DecodeLocalSignature(this, null)) + ")")}");
			}
			foreach (var h in Rows(TableIndex.Event, MetadataTokens.EventDefinitionHandle))
			{
				var e = r.GetEventDefinition(h);
				var a = e.GetAccessors();
				Line($"EVENT {Tok(h)} flags={S(() => ((ushort)e.Attributes).ToString("x4"))} name={S(() => Str(e.Name))} type={S(() => Tok(e.Type))} add={Tok(a.Adder)} remove={Tok(a.Remover)} raise={Tok(a.Raiser)} other={Tokens(a.Others, x => x)}");
			}
			foreach (var h in Rows(TableIndex.Property, MetadataTokens.PropertyDefinitionHandle))
			{
				var p = r.GetPropertyDefinition(h);
				var a = p.GetAccessors();
				Line($"PROPERTY {Tok(h)} flags={S(() => ((ushort)p.Attributes).ToString("x4"))} name={S(() => Str(p.Name))} sig={S(() => Method(p.DecodeSignature(this, null)))} const={Tok(p.GetDefaultValue())} get={Tok(a.Getter)} set={Tok(a.Setter)} other={Tokens(a.Others, x => x)}");
			}
			foreach (var h in Rows(TableIndex.MethodImpl, MetadataTokens.MethodImplementationHandle))
			{
				var m = r.GetMethodImplementation(h);
				Line($"METHODIMPL {Tok(h)} class={S(() => Tok(m.Type))} body={S(() => Tok(m.MethodBody))} decl={S(() => Tok(m.MethodDeclaration))}");
			}
			foreach (var h in Rows(TableIndex.ModuleRef, MetadataTokens.ModuleReferenceHandle))
			{
				var m = r.GetModuleReference(h);
				Line($"MODULEREF {Tok(h)} name={S(() => Str(m.Name))}");
			}
			foreach (var h in Rows(TableIndex.TypeSpec, MetadataTokens.TypeSpecificationHandle))
			{
				var t = r.GetTypeSpecification(h);
				Line($"TYPESPEC {Tok(h)} sig={S(() => t.DecodeSignature(this, null))}");
			}
			if (r.GetTableRowCount(TableIndex.Assembly) > 0)
			{
				var a = r.GetAssemblyDefinition();
				Line($"ASSEMBLY hash={S(() => ((uint)a.HashAlgorithm).ToString("x8"))} version={S(() => a.Version)} flags={S(() => ((uint)a.Flags).ToString("x8"))} key={S(() => Blob(a.PublicKey))} name={S(() => Str(a.Name))} culture={S(() => Str(a.Culture))}");
			}
			foreach (var h in Rows(TableIndex.AssemblyRef, MetadataTokens.AssemblyReferenceHandle))
			{
				var a = r.GetAssemblyReference(h);
				Line($"ASSEMBLYREF {Tok(h)} version={S(() => a.Version)} flags={S(() => ((uint)a.Flags).ToString("x8"))} key={S(() => Blob(a.PublicKeyOrToken))} name={S(() => Str(a.Name))} culture={S(() => Str(a.Culture))} hash={S(() => Blob(a.HashValue))}");
			}
			foreach (var h in Rows(TableIndex.File, MetadataTokens.AssemblyFileHandle))
			{
				var f = r.GetAssemblyFile(h);
				Line($"FILE {Tok(h)} meta={S(() => f.ContainsMetadata ? 1 : 0)} name={S(() => Str(f.Name))} hash={S(() => Blob(f.HashValue))}");
			}
			foreach (var h in Rows(TableIndex.ExportedType, MetadataTokens.ExportedTypeHandle))
			{
				var e = r.GetExportedType(h);
				Line($"EXPORTEDTYPE {Tok(h)} flags={S(() => ((uint)e.Attributes).ToString("x8"))} typedef={S(() => e.GetTypeDefinitionId().ToString("x8"))} name={S(() => Str(e.Name))} ns={S(() => Str(e.Namespace))} impl={S(() => Tok(e.Implementation))}");
			}
			foreach (var h in Rows(TableIndex.ManifestResource, MetadataTokens.ManifestResourceHandle))
			{
				var m = r.GetManifestResource(h);
				Line($"RESOURCE {Tok(h)} offset={S(() => m.Offset)} flags={S(() => ((uint)m.Attributes).ToString("x8"))} name={S(() => Str(m.Name))} impl={S(() => Tok(m.Implementation))}");
			}
			foreach (var h in Rows(TableIndex.GenericParam, MetadataTokens.GenericParameterHandle))
			{
				var g = r.GetGenericParameter(h);
				Line($"GENERICPARAM {Tok(h)} owner={S(() => Tok(g.Parent))} number={S(() => g.Index)} flags={S(() => ((ushort)g.Attributes).ToString("x4"))} name={S(() => Str(g.Name))} constraints={Tokens(g.GetConstraints(), x => x)}");
			}
			foreach (var h in Rows(TableIndex.MethodSpec, MetadataTokens.MethodSpecificationHandle))
			{
				var m = r.GetMethodSpecification(h);
				Line($"METHODSPEC {Tok(h)} method={S(() => Tok(m.Method))} args={S(() => string.Join(",", m.DecodeSignature(this, null)))}");
			}
			foreach (var h in Rows(TableIndex.GenericParamConstraint, MetadataTokens.GenericParameterConstraintHandle))
			{
				var c = r.GetGenericParameterConstraint(h);
				Line($"CONSTRAINT {Tok(h)} owner={S(() => Tok(c.Parameter))} type={S(() => Tok(c.Type))}");
			}
			return sb.ToString();
		}

		IEnumerable<T> Rows<T>(TableIndex table, Func<int, T> handle)
		{
			for (int row = 1; row <= r.GetTableRowCount(table); row++)
				yield return handle(row);
		}

		void Body(MethodDefinitionHandle h, int rva)
		{
			MethodBodyBlock body;
			try
			{
				body = pe.GetMethodBody(rva);
			}
			catch (Exception)
			{
				Line($"BODY {Tok(h)} !bad");
				return;
			}
			byte[] il = body.GetILBytes() ?? [];
			Line($"BODY {Tok(h)} maxstack={body.MaxStack} locals={Tok(body.LocalSignature)} init={(body.LocalVariablesInitialized ? 1 : 0)} size={body.Size} il={il.Length}:{Fnv1a(il):x16}");
			foreach (var c in body.ExceptionRegions)
				Line($"EH {Tok(h)} kind={(int)c.Kind} try={c.TryOffset},{c.TryLength} handler={c.HandlerOffset},{c.HandlerLength} catch={MetadataTokens.GetToken(c.CatchType):x8} filter={c.FilterOffset}");
		}

		string Method(MethodSignature<string> sig)
		{
			var parameters = new List<string>();
			for (int i = 0; i < sig.ParameterTypes.Length; i++)
			{
				if (i == sig.RequiredParameterCount)
					parameters.Add("...");
				parameters.Add(sig.ParameterTypes[i]);
			}
			return $"hdr={sig.Header.RawValue:x2} gen={sig.GenericParameterCount} {sig.ReturnType}({string.Join(",", parameters)})";
		}

		int Deeper(int d) => d >= MaxNameDepth ? throw new BadImageFormatException("type reference cycle") : d + 1;

		string Qualified(StringHandle ns, StringHandle name)
		{
			string n = Str(ns);
			return n.Length == 0 ? Str(name) : n + "." + Str(name);
		}

		string DefName(TypeDefinitionHandle h, int d)
		{
			var t = r.GetTypeDefinition(h);
			string name = Qualified(t.Namespace, t.Name);
			var outer = t.GetDeclaringType();
			return outer.IsNil ? name : DefName(outer, Deeper(d)) + "/" + name;
		}

		string RefName(TypeReferenceHandle h, int d)
		{
			var t = r.GetTypeReference(h);
			string name = Qualified(t.Namespace, t.Name);
			var scope = t.ResolutionScope;
			return scope.Kind == HandleKind.TypeReference && !scope.IsNil
				? RefName((TypeReferenceHandle)scope, Deeper(d)) + "/" + name
				: name;
		}

		static string Prefix(byte rawTypeKind) => rawTypeKind == 0x11 ? "valuetype " : "";

		public string GetPrimitiveType(PrimitiveTypeCode typeCode) => "System." + typeCode;

		public string GetTypeFromDefinition(MetadataReader reader, TypeDefinitionHandle handle, byte rawTypeKind)
			=> Prefix(rawTypeKind) + DefName(handle, depth);

		public string GetTypeFromReference(MetadataReader reader, TypeReferenceHandle handle, byte rawTypeKind)
			=> Prefix(rawTypeKind) + RefName(handle, depth);

		public string GetTypeFromSpecification(MetadataReader reader, object? genericContext, TypeSpecificationHandle handle, byte rawTypeKind)
		{
			int saved = depth;
			depth = Deeper(depth);
			try
			{
				return Prefix(rawTypeKind) + reader.GetTypeSpecification(handle).DecodeSignature(this, genericContext);
			}
			finally
			{
				depth = saved;
			}
		}

		public string GetSZArrayType(string elementType) => elementType + "[]";

		public string GetArrayType(string elementType, ArrayShape shape)
			=> $"{elementType}[{shape.Rank};{string.Join(",", shape.Sizes)};{string.Join(",", shape.LowerBounds)}]";

		public string GetByReferenceType(string elementType) => elementType + "&";

		public string GetPointerType(string elementType) => elementType + "*";

		public string GetPinnedType(string elementType) => elementType + " pinned";

		public string GetGenericInstantiation(string genericType, ImmutableArray<string> typeArguments)
			=> genericType + "<" + string.Join(",", typeArguments) + ">";

		public string GetGenericTypeParameter(object? genericContext, int index) => "!" + index;

		public string GetGenericMethodParameter(object? genericContext, int index) => "!!" + index;

		public string GetModifiedType(string modifier, string unmodifiedType, bool isRequired)
			=> $"{unmodifiedType} {(isRequired ? "modreq" : "modopt")}({modifier})";

		public string GetFunctionPointerType(MethodSignature<string> signature) => "method " + Method(signature);
	}
}
