// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ICSharpCode.NRefactory.Utils;
using ICSharpCode.TreeView;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	class AnalyzedFieldAccessNode : AnalyzerTreeNode
	{
		readonly bool showWrites; // true: show writes; false: show read access
		readonly FieldDefinition analyzedField;
		readonly ThreadingSupport threading;

		public AnalyzedFieldAccessNode(FieldDefinition analyzedField, bool showWrites)
		{
			if (analyzedField == null)
				throw new ArgumentNullException("analyzedField");

			this.analyzedField = analyzedField;
			this.showWrites = showWrites;
			this.threading = new ThreadingSupport();
			this.LazyLoading = true;
		}

		public override object Text
		{
			get { return showWrites ? "Assigned By" : "Read By"; }
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
					return FindReferencesInAssembly(analyzedField.DeclaringType.Module.Assembly, ct);
				case AnalysisScope.Global:
				default:
					return FindReferencesGlobal(ct);
			}
		}

		IEnumerable<SharpTreeNode> FindReferencesGlobal(CancellationToken ct)
		{
			var assemblies = GetReferencingAssemblies(analyzedField.Module.Assembly, ct);
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
			foreach (TypeDefinition type in TreeTraversal.PreOrder(analyzedField.DeclaringType, t => t.NestedTypes)) {
				ct.ThrowIfCancellationRequested();
				foreach (var result in FindReferencesInType(type)) {
					ct.ThrowIfCancellationRequested();
					yield return result;
				}
			}
		}


		IEnumerable<SharpTreeNode> FindReferencesInType(TypeDefinition type)
		{
			string name = analyzedField.Name;
			string declTypeName = analyzedField.DeclaringType.FullName;

			foreach (MethodDefinition method in type.Methods) {
				bool found = false;
				if (!method.HasBody)
					continue;
				foreach (Instruction instr in method.Body.Instructions) {
					if (CanBeReference(instr.OpCode.Code)) {
						FieldReference fr = instr.Operand as FieldReference;
						if (fr != null && fr.Name == name && fr.DeclaringType.FullName == declTypeName && fr.Resolve() == analyzedField) {
							found = true;
							break;
						}
					}
				}
				if (found)
					yield return new AnalyzedMethodTreeNode(method);
			}
		}

		bool CanBeReference(Code code)
		{
			switch (code) {
				case Code.Ldfld:
				case Code.Ldsfld:
					return !showWrites;
				case Code.Stfld:
				case Code.Stsfld:
					return showWrites;
				case Code.Ldflda:
				case Code.Ldsflda:
					return true; // always show address-loading
				default:
					return false;
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
			TypeAttributes visibility = analyzedField.DeclaringType.Attributes & TypeAttributes.VisibilityMask;
			FieldAttributes memberAccess = analyzedField.Attributes & FieldAttributes.FieldAccessMask;

			if (memberAccess == FieldAttributes.Private)
				return AnalysisScope.Type;

			if (memberAccess == FieldAttributes.Assembly || memberAccess == FieldAttributes.FamANDAssem)
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
