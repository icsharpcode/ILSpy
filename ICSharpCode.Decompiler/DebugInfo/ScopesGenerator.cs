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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.DebugInfo
{
	class ScopesGenerator : DepthFirstAstVisitor<ImportScopeInfo, ImportScopeInfo>
	{
		static readonly KeyComparer<ILVariable, int> ILVariableKeyComparer = new KeyComparer<ILVariable, int>(l => l.Index.Value, Comparer<int>.Default, EqualityComparer<int>.Default);

		IDecompilerTypeSystem typeSystem;
		ImportScopeInfo currentImportScope;
		List<ImportScopeInfo> importScopes = new List<ImportScopeInfo>();
		List<(MethodDefinitionHandle Method, ImportScopeInfo Import, int Offset, int Length, HashSet<ILVariable> Locals)> localScopes = new List<(MethodDefinitionHandle Method, ImportScopeInfo Import, int Offset, int Length, HashSet<ILVariable> Locals)>();

		private ScopesGenerator()
		{
		}

		public static void Generate(IDecompilerTypeSystem typeSystem, MetadataBuilder metadata, ImportScopeHandle globalImportScope, SyntaxTree syntaxTree)
		{
			var generator = new ScopesGenerator();
			generator.typeSystem = typeSystem;
			syntaxTree.AcceptVisitor(generator, new ImportScopeInfo());
			foreach (var scope in generator.importScopes) {
				var blob = EncodeImports(metadata, scope);
				scope.Handle = metadata.AddImportScope(scope.Parent == null ? globalImportScope : scope.Parent.Handle, blob);
			}

			foreach (var localScope in generator.localScopes) {
				int nextRow = metadata.GetRowCount(TableIndex.LocalVariable) + 1;
				var firstLocalVariable = MetadataTokens.LocalVariableHandle(nextRow);
				
				foreach (var local in localScope.Locals.OrderBy(l => l.Index)) {
					var name = local.Name != null ? metadata.GetOrAddString(local.Name) : default;
					metadata.AddLocalVariable(LocalVariableAttributes.None, local.Index.Value, name);
				}

				metadata.AddLocalScope(localScope.Method, localScope.Import.Handle, firstLocalVariable,
					default, localScope.Offset, localScope.Length);
			}
		}

		static BlobHandle EncodeImports(MetadataBuilder metadata, ImportScopeInfo scope)
		{
			var writer = new BlobBuilder();

			foreach (var import in scope.Imports) {
				writer.WriteByte((byte)ImportDefinitionKind.ImportNamespace);
				writer.WriteCompressedInteger(MetadataTokens.GetHeapOffset(metadata.GetOrAddBlobUTF8(import)));
			}

			return metadata.GetOrAddBlob(writer);
		}

		public override ImportScopeInfo VisitSyntaxTree(SyntaxTree unit, ImportScopeInfo data)
		{
			importScopes.Add(data);
			currentImportScope = data;
			base.VisitSyntaxTree(unit, data);
			currentImportScope = data;
			return data;
		}

		public override ImportScopeInfo VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, ImportScopeInfo data)
		{
			ImportScopeInfo scope = new ImportScopeInfo(data);
			importScopes.Add(scope);
			currentImportScope = scope;
			base.VisitNamespaceDeclaration(namespaceDeclaration, scope);
			currentImportScope = data;
			return data;
		}

		public override ImportScopeInfo VisitUsingDeclaration(UsingDeclaration usingDeclaration, ImportScopeInfo data)
		{
			data.Imports.Add(usingDeclaration.Namespace);
			return data;
		}

		public override ImportScopeInfo VisitMethodDeclaration(MethodDeclaration methodDeclaration, ImportScopeInfo data)
		{
			HandleMethod(methodDeclaration, data);
			return data;
		}

		public override ImportScopeInfo VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, ImportScopeInfo data)
		{
			var symbol = propertyDeclaration.GetSymbol();
			if (symbol is IProperty property && !property.MetadataToken.IsNil) {
				if (property.CanGet && !property.Getter.MetadataToken.IsNil)
					data.MethodDefinitions.Add((MethodDefinitionHandle)property.Getter.MetadataToken);
				if (property.CanSet && !property.Setter.MetadataToken.IsNil)
					data.MethodDefinitions.Add((MethodDefinitionHandle)property.Setter.MetadataToken);
			}
			return data;
		}

		public override ImportScopeInfo VisitCustomEventDeclaration(CustomEventDeclaration eventDeclaration, ImportScopeInfo data)
		{
			HandleEvent(eventDeclaration, data);
			return data;
		}

		void HandleEvent(AstNode node, ImportScopeInfo data)
		{
			var symbol = node.GetSymbol();
			if (symbol is IEvent @event && !@event.MetadataToken.IsNil) {
				if (@event.CanAdd && !@event.AddAccessor.MetadataToken.IsNil)
					data.MethodDefinitions.Add((MethodDefinitionHandle)@event.AddAccessor.MetadataToken);
				if (@event.CanRemove && !@event.RemoveAccessor.MetadataToken.IsNil)
					data.MethodDefinitions.Add((MethodDefinitionHandle)@event.RemoveAccessor.MetadataToken);
				if (@event.CanInvoke && !@event.InvokeAccessor.MetadataToken.IsNil)
					data.MethodDefinitions.Add((MethodDefinitionHandle)@event.InvokeAccessor.MetadataToken);
			}
		}

		public override ImportScopeInfo VisitEventDeclaration(EventDeclaration eventDeclaration, ImportScopeInfo data)
		{
			HandleEvent(eventDeclaration, data);
			return data;
		}

		public override ImportScopeInfo VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, ImportScopeInfo data)
		{
			HandleMethod(constructorDeclaration, data);
			return data;
		}

		public override ImportScopeInfo VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, ImportScopeInfo data)
		{
			HandleMethod(destructorDeclaration, data);
			return data;
		}

		void HandleMethod(AstNode node, ImportScopeInfo data)
		{
			var symbol = node.GetSymbol();
			var function = node.Annotations.OfType<ILFunction>().FirstOrDefault();
			if (function != null && symbol is IMethod method && !method.MetadataToken.IsNil) {
				MethodDefinitionHandle handle = (MethodDefinitionHandle)method.MetadataToken;
				data.MethodDefinitions.Add(handle);
				var file = typeSystem.MainModule.PEFile;
				MethodDefinition md = file.Metadata.GetMethodDefinition(handle);
				if (md.HasBody()) {
					HandleMethodBody(function, file.Reader.GetMethodBody(md.RelativeVirtualAddress));
				}
			}
		}

		void HandleMethodBody(ILFunction function, MethodBodyBlock methodBody)
		{
			var localVariables = new HashSet<ILVariable>(ILVariableKeyComparer);

			if (!methodBody.LocalSignature.IsNil) {
				var types = typeSystem.MainModule.DecodeLocalSignature(methodBody.LocalSignature,
					new TypeSystem.GenericContext(function.Method));

				foreach (var v in function.Variables) {
					if (v.Index != null && v.Kind.IsLocal()) {
						Debug.Assert(v.Index < types.Length && v.Type.Equals(types[v.Index.Value]));
						localVariables.Add(v);
					}
				}
			}

			localScopes.Add(((MethodDefinitionHandle)function.Method.MetadataToken, currentImportScope,
				0, methodBody.GetCodeSize(), localVariables));
		}
	}
}
