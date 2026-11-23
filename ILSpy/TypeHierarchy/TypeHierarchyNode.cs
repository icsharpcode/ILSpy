// Copyright (c) 2026 Piero Viano
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
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TypeHierarchy
{
	/// <summary>The role a <see cref="TypeHierarchyNode"/> plays in the hierarchy tree.</summary>
	public enum TypeHierarchyNodeKind
	{
		/// <summary>The type the hierarchy was opened for.</summary>
		Target,
		/// <summary>Groups the target's direct supertypes.</summary>
		BaseTypesGroup,
		/// <summary>Groups the target's direct subtypes and implementations.</summary>
		DerivedTypesGroup,
		/// <summary>A supertype; expands to its own supertypes.</summary>
		BaseType,
		/// <summary>A subtype or implementation; expands to its own subtypes.</summary>
		DerivedType,
		/// <summary>Stand-in child shown until the real children are loaded.</summary>
		Placeholder,
	}

	/// <summary>
	/// One row of the type-hierarchy tree. Children are produced on demand by a loader so that
	/// the (potentially slow) derived-type search only runs for the nodes the user expands. Until
	/// the loader has run, a placeholder child keeps the expander visible.
	/// </summary>
	public sealed partial class TypeHierarchyNode : ObservableObject
	{
		readonly Func<CancellationToken, Task<IReadOnlyList<TypeHierarchyNode>>>? loader;
		readonly CancellationToken cancellationToken;
		Task? loading;

		[ObservableProperty]
		private bool isExpanded;

		[ObservableProperty]
		private bool isLoading;

		internal TypeHierarchyNode(TypeHierarchyNodeKind kind, string text, IImage? icon, ITypeDefinition? type,
			Func<CancellationToken, Task<IReadOnlyList<TypeHierarchyNode>>>? loader, CancellationToken cancellationToken)
		{
			Kind = kind;
			Text = text;
			Icon = icon;
			Type = type;
			this.loader = loader;
			this.cancellationToken = cancellationToken;
			if (loader != null)
				Children.Add(CreatePlaceholder());
		}

		public TypeHierarchyNodeKind Kind { get; }

		public string Text { get; }

		public IImage? Icon { get; }

		/// <summary>The type this row stands for; null for group and placeholder rows.</summary>
		public ITypeDefinition? Type { get; }

		public ObservableCollection<TypeHierarchyNode> Children { get; } = new();

		/// <summary>True for rows that stand for a type and can therefore be navigated to.</summary>
		public bool IsNavigable => Type != null && Kind != TypeHierarchyNodeKind.BaseTypesGroup
			&& Kind != TypeHierarchyNodeKind.DerivedTypesGroup;

		partial void OnIsExpandedChanged(bool value)
		{
			if (value)
				LoadChildrenAsync().HandleExceptions();
		}

		/// <summary>
		/// Runs the loader once and replaces the placeholder with the loaded children. Later calls
		/// return the same task, so expanding a node repeatedly does not search again. A cancelled
		/// load leaves the placeholder in place.
		/// </summary>
		public Task LoadChildrenAsync() => loading ??= LoadCoreAsync();

		async Task LoadCoreAsync()
		{
			if (loader == null)
				return;
			IsLoading = true;
			try
			{
				IReadOnlyList<TypeHierarchyNode> loaded;
				try
				{
					loaded = await loader(cancellationToken).ConfigureAwait(true);
				}
				catch (OperationCanceledException)
				{
					return;
				}
				Children.Clear();
				foreach (var child in loaded)
					Children.Add(child);
			}
			finally
			{
				IsLoading = false;
			}
		}

		static TypeHierarchyNode CreatePlaceholder()
			=> new(TypeHierarchyNodeKind.Placeholder, "...", null, null, null, CancellationToken.None);

		public override string ToString() => Text;
	}
}
