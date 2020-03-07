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
	/// <summary>
	/// Visitor that generates debug information.
	/// 
	/// The intended usage is to create a new instance for each source file,
	/// and call syntaxTree.AcceptVisitor(debugInfoGenerator) to fill the internal debug info tables.
	/// This can happen concurrently for multiple source files.
	/// Then the main thread calls Generate() to write out the results into the PDB.
	/// </summary>
	class DebugInfoGenerator : DepthFirstAstVisitor
	{
		static readonly KeyComparer<ILVariable, int> ILVariableKeyComparer = new KeyComparer<ILVariable, int>(l => l.Index.Value, Comparer<int>.Default, EqualityComparer<int>.Default);

		IDecompilerTypeSystem typeSystem;
		readonly ImportScopeInfo globalImportScope = new ImportScopeInfo();
		ImportScopeInfo currentImportScope;
		List<ImportScopeInfo> importScopes = new List<ImportScopeInfo>();
		internal List<(MethodDefinitionHandle Method, ImportScopeInfo Import, int Offset, int Length, HashSet<ILVariable> Locals)> LocalScopes { get; } = new List<(MethodDefinitionHandle Method, ImportScopeInfo Import, int Offset, int Length, HashSet<ILVariable> Locals)>();
		List<ILFunction> functions = new List<ILFunction>();

		/// <summary>
		/// Gets all functions with bodies that were seen by the visitor so far.
		/// </summary>
		public IReadOnlyList<ILFunction> Functions {
			get => functions;
		}

		public DebugInfoGenerator(IDecompilerTypeSystem typeSystem)
		{
			this.typeSystem = typeSystem ?? throw new ArgumentNullException(nameof(typeSystem));
			this.currentImportScope = globalImportScope;
		}

		public void GenerateImportScopes(MetadataBuilder metadata, ImportScopeHandle globalImportScope)
		{
			foreach (var scope in importScopes) {
				var blob = EncodeImports(metadata, scope);
				scope.Handle = metadata.AddImportScope(scope.Parent == null ? globalImportScope : scope.Parent.Handle, blob);
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

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			var parentImportScope = currentImportScope;
			currentImportScope = new ImportScopeInfo(parentImportScope);
			importScopes.Add(currentImportScope);
			base.VisitNamespaceDeclaration(namespaceDeclaration);
			currentImportScope = parentImportScope;
		}

		public override void VisitUsingDeclaration(UsingDeclaration usingDeclaration)
		{
			currentImportScope.Imports.Add(usingDeclaration.Namespace);
		}

		public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			HandleMethod(methodDeclaration);
		}

		public override void VisitAccessor(Accessor accessor)
		{
			HandleMethod(accessor);
		}

		public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			HandleMethod(constructorDeclaration);
		}

		public override void VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration)
		{
			HandleMethod(destructorDeclaration);
		}

		public override void VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			HandleMethod(operatorDeclaration);
		}

		public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
		{
			HandleMethod(lambdaExpression);
		}

		public override void VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression)
		{
			HandleMethod(anonymousMethodExpression);
		}

		public override void VisitQuerySelectClause(QuerySelectClause querySelectClause)
		{
			HandleMethod(querySelectClause);
		}

		public override void VisitQueryWhereClause(QueryWhereClause queryWhereClause)
		{
			HandleMethod(queryWhereClause);
		}

		void HandleMethod(AstNode node)
		{
			// Look into method body, e.g. in order to find lambdas
			VisitChildren(node);

			var function = node.Annotation<ILFunction>();
			if (function == null || function.Method == null || function.Method.MetadataToken.IsNil)
				return;
			this.functions.Add(function);
			var method = function.MoveNextMethod ?? function.Method;
			MethodDefinitionHandle handle = (MethodDefinitionHandle)method.MetadataToken;
			var file = typeSystem.MainModule.PEFile;
			MethodDefinition md = file.Metadata.GetMethodDefinition(handle);
			if (md.HasBody()) {
				HandleMethodBody(function, file.Reader.GetMethodBody(md.RelativeVirtualAddress));
			}
		}

		void HandleMethodBody(ILFunction function, MethodBodyBlock methodBody)
		{
			var method = function.MoveNextMethod ?? function.Method;
			var localVariables = new HashSet<ILVariable>(ILVariableKeyComparer);

			if (!methodBody.LocalSignature.IsNil) {
#if DEBUG
				var types = typeSystem.MainModule.DecodeLocalSignature(methodBody.LocalSignature,
					new TypeSystem.GenericContext(method));
#endif

				foreach (var v in function.Variables) {
					if (v.Index != null && v.Kind.IsLocal()) {
#if DEBUG
						Debug.Assert(v.Index < types.Length && v.Type.Equals(types[v.Index.Value]));
#endif
						localVariables.Add(v);
					}
				}
			}

			LocalScopes.Add(((MethodDefinitionHandle)method.MetadataToken, currentImportScope,
				0, methodBody.GetCodeSize(), localVariables));
		}
	}
}
