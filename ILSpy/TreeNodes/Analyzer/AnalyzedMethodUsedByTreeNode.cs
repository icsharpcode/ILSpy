// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	class AnalyzedMethodUsedByTreeNode : AnalyzerTreeNode
	{
		MethodDefinition analyzedMethod;
		ThreadingSupport threading;

		public AnalyzedMethodUsedByTreeNode(MethodDefinition analyzedMethod)
		{
			if (analyzedMethod == null)
				throw new ArgumentNullException("analyzedMethod");

			this.analyzedMethod = analyzedMethod;
			this.threading = new ThreadingSupport();
			this.LazyLoading = true;
		}

		public override object Text
		{
			get { return "Used By"; }
		}

		public override object Icon
		{
			get { return Images.Search; }
		}

		protected override void LoadChildren()
		{
			threading.LoadChildren(this, FetchChildren);
		}

		protected override void OnCollapsing()
		{
			if (threading.IsRunning) {
				this.LazyLoading = true;
				threading.Cancel();
				this.Children.Clear();
			}
		}

		IEnumerable<SharpTreeNode> FetchChildren(CancellationToken ct)
		{
			switch (DetermineAnalysisScope()) {
				case AnalysisScope.Type:
					return FindReferencesInDeclaringType(ct);
				case AnalysisScope.Assembly:
					return FindReferencesInAssembly(analyzedMethod.DeclaringType.Module.Assembly, ct);
				case AnalysisScope.Global:
				default:
					return FindReferencesGlobal(ct);
			}
		}

		IEnumerable<SharpTreeNode> FindReferencesGlobal(CancellationToken ct)
		{
			var assemblies = GetReferencingAssemblies(analyzedMethod.Module.Assembly, ct);
			// use parallelism only on the assembly level (avoid locks within Cecil)
			return assemblies.AsParallel().WithCancellation(ct).SelectMany((AssemblyDefinition asm) => FindReferencesInAssembly(asm, ct));
		}

		IEnumerable<SharpTreeNode> FindReferencesInAssembly(AssemblyDefinition asm, CancellationToken ct)
		{
			foreach (TypeDefinition type in TreeTraversal.PreOrder(asm.MainModule.Types, t => t.NestedTypes)) {
				ct.ThrowIfCancellationRequested();
				foreach (var result in FindReferencesInType(type)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<SharpTreeNode> FindReferencesInDeclaringType(CancellationToken ct)
		{
			foreach (TypeDefinition type in TreeTraversal.PreOrder(analyzedMethod.DeclaringType, t => t.NestedTypes)) {
				ct.ThrowIfCancellationRequested();
				foreach (var result in FindReferencesInType(type)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}

		IEnumerable<SharpTreeNode> FindReferencesInType(TypeDefinition type)
		{
			string name = analyzedMethod.Name;
			string declTypeName = analyzedMethod.DeclaringType.FullName;

			foreach (MethodDefinition method in type.Methods) {
				bool found = false;
				if (!method.HasBody)
					continue;
				foreach (Instruction instr in method.Body.Instructions) {
					MethodReference mr = instr.Operand as MethodReference;
					if (mr != null && mr.Name == name && mr.DeclaringType.FullName == declTypeName && mr.Resolve() == analyzedMethod) {
						found = true;
						break;
					}
				}
				if (found)
					yield return new AnalyzedMethodTreeNode(method);
			}
		}

		IEnumerable<AssemblyDefinition> GetReferencingAssemblies(AssemblyDefinition asm, CancellationToken ct)
		{
			yield return asm;

			string requiredAssemblyFullName = asm.FullName;

			IEnumerable<LoadedAssembly> assemblies = MainWindow.Instance.AssemblyList.GetAssemblies().Where(assy => assy.AssemblyDefinition != null);

			foreach (var assembly in assemblies) {
				ct.ThrowIfCancellationRequested();
				bool found = false;
				foreach (var reference in assembly.AssemblyDefinition.MainModule.AssemblyReferences) {
					if (requiredAssemblyFullName == reference.FullName) {
						found = true;
						break;
					}
				}
				if (found)
					yield return assembly.AssemblyDefinition;
			}
		}

		private AnalysisScope DetermineAnalysisScope()
		{
			// NOTE: This is *extremely* crude, and does not constrain the analysis to the true
			// accessibility domain of the member, but it is enough to significantly reduce the
			// search space. 

			// TODO: Nested classes are handled in a sub-optimal manner. we should walk the nesting hierarchy to determine visibility
			TypeAttributes visibility = analyzedMethod.DeclaringType.Attributes & TypeAttributes.VisibilityMask;
			MethodAttributes memberAccess = analyzedMethod.Attributes & MethodAttributes.MemberAccessMask;

			if (memberAccess == MethodAttributes.Private)
				return AnalysisScope.Type;

			if (memberAccess == MethodAttributes.Assembly || memberAccess == MethodAttributes.FamANDAssem)
				return AnalysisScope.Assembly;

			if (visibility == TypeAttributes.NotPublic)
				return AnalysisScope.Assembly;

			return AnalysisScope.Global;
		}

		private enum AnalysisScope
		{
			Type,
			Assembly,
			Global
		}
	}
}
