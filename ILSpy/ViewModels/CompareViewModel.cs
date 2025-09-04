// Copyright (c) 2025 Siegfried Pammer
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

#nullable enable

namespace ICSharpCode.ILSpy.ViewModels
{
	using ICSharpCode.Decompiler.TypeSystem;

	using TomsToolbox.Wpf;

	class CompareViewModel : ObservableObject
	{
		private readonly TabPageModel tabPage;
		private readonly AssemblyTreeModel assemblyTreeModel;
		private LoadedAssembly leftAssembly;
		private LoadedAssembly rightAssembly;
		private ComparisonEntryTreeNode root;
		private bool updating = false;
		private bool showIdentical;

		public CompareViewModel(TabPageModel tabPage, AssemblyTreeModel assemblyTreeModel, LoadedAssembly left, LoadedAssembly right)
		{
			this.tabPage = tabPage;
			this.assemblyTreeModel = assemblyTreeModel;
			leftAssembly = left;
			rightAssembly = right;

			var leftTree = CreateEntityTree(left.GetTypeSystemOrNull()!);
			var rightTree = CreateEntityTree(right.GetTypeSystemOrNull()!);

			this.root = new ComparisonEntryTreeNode(MergeTrees(leftTree.Item2, rightTree.Item2), this);

			this.SwapAssembliesCommand = new DelegateCommand(OnSwapAssemblies);
			this.ExpandAllCommand = new DelegateCommand(OnExpandAll);
			this.CopyToClipboardAsJSONCommand = new DelegateCommand(OnCopyToClipboardAsJSON);

			this.PropertyChanged += CompareViewModel_PropertyChanged;
		}

		private void CompareViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(LeftAssembly):
				case nameof(RightAssembly):
					if (updating)
						break;
					updating = true;
					var view = tabPage.Content;
					tabPage.ShowTextView(t => t.RunWithCancellation(token => Task.Run(DoCompare, token), $"Comparing {LeftAssembly.Text} - {RightAssembly.Text}").Then(_ => {
						tabPage.Title = $"Compare {LeftAssembly.Text} - {RightAssembly.Text}";
						tabPage.SupportsLanguageSwitching = false;
						tabPage.FrozenContent = true;
						tabPage.Content = view;
						updating = false;
					}).HandleExceptions());

					Task<bool> DoCompare()
					{
						var leftTree = CreateEntityTree(leftAssembly.GetTypeSystemOrNull()!);
						var rightTree = CreateEntityTree(rightAssembly.GetTypeSystemOrNull()!);

						this.RootEntry = new ComparisonEntryTreeNode(MergeTrees(leftTree.Item2, rightTree.Item2), this);
						return Task.FromResult(true);
					}
					break;
				case nameof(ShowIdentical):
					this.RootEntry.EnsureChildrenFiltered();
					break;
			}
		}

		public LoadedAssembly LeftAssembly {
			get => leftAssembly;
			set {
				if (leftAssembly != value)
				{
					leftAssembly = value;
					OnPropertyChanged();
				}
			}
		}

		public LoadedAssembly RightAssembly {
			get => rightAssembly;
			set {
				if (rightAssembly != value)
				{
					rightAssembly = value;
					OnPropertyChanged();
				}
			}
		}

		public ComparisonEntryTreeNode RootEntry {
			get => root;
			set {
				if (root != value)
				{
					root = value;
					OnPropertyChanged();
				}
			}
		}

		public bool ShowIdentical {
			get => showIdentical;
			set {
				if (showIdentical != value)
				{
					showIdentical = value;
					OnPropertyChanged();
				}
			}
		}

		public ICommand SwapAssembliesCommand { get; set; }
		public ICommand ExpandAllCommand { get; set; }
		public ICommand CopyToClipboardAsJSONCommand { get; set; }

		void OnSwapAssemblies()
		{
			var left = this.leftAssembly;
			this.leftAssembly = this.rightAssembly;
			this.rightAssembly = left;
			OnPropertyChanged(nameof(LeftAssembly));
			OnPropertyChanged(nameof(RightAssembly));
		}

		void OnExpandAll()
		{
			foreach (var node in RootEntry.DescendantsAndSelf())
			{
				node.IsExpanded = true;
			}
		}

		void OnCopyToClipboardAsJSON()
		{
			var options = new JsonSerializerOptions {
				WriteIndented = true
			};

			var jsonEntry = ConvertToJson(this.root.Entry);
			var json = JsonSerializer.Serialize(jsonEntry, options);
			Clipboard.SetText(json);
		}

		private object ConvertToJson(Entry entry)
		{
			List<Entry> changedTypes = new();
			List<Entry> addedTypes = new();
			List<Entry> removedTypes = new();

			foreach (var item in TreeTraversal.PreOrder(entry, entry => entry.Children))
			{
				if (item.Entity is ITypeDefinition)
				{
					switch (item.RecursiveKind)
					{
						case DiffKind.Add:
							addedTypes.Add(item);
							break;
						case DiffKind.Remove:
							removedTypes.Add(item);
							break;
						case DiffKind.Update:
							changedTypes.Add(item);
							break;
					}
				}
			}
			var result = new {
				left = LeftAssembly.FileName.Replace('\\', '/'),
				right = RightAssembly.FileName.Replace('\\', '/'),
				changedTypes = changedTypes.SelectArray(t => new { typeName = ((ITypeDefinition)t.Entity).FullName, changes = GetChanges(t.Children) }),
				addedTypes = addedTypes.SelectArray(t => new { typeName = ((ITypeDefinition)t.Entity).FullName, changes = GetChanges(t.Children) }),
				removedTypes = removedTypes.SelectArray(t => new { typeName = ((ITypeDefinition)t.Entity).FullName, changes = GetChanges(t.Children) })
			};
			return result;

			string? GetEntityText(ISymbol? symbol) => symbol switch {
				ITypeDefinition t => this.assemblyTreeModel.CurrentLanguage.TypeToString(t, includeNamespace: true),
				IMethod m => this.assemblyTreeModel.CurrentLanguage.MethodToString(m, false, false, false),
				IField f => this.assemblyTreeModel.CurrentLanguage.FieldToString(f, false, false, false),
				IProperty p => this.assemblyTreeModel.CurrentLanguage.PropertyToString(p, false, false, false),
				IEvent e => this.assemblyTreeModel.CurrentLanguage.EventToString(e, false, false, false),
				INamespace n => n.FullName,
				IModule m => m.FullAssemblyName,
				_ => null,
			};

			IEnumerable<object> GetChanges(List<Entry>? entries)
			{
				if (entries == null)
					yield break;
				foreach (var item in entries)
				{
					if (item.Kind is not (DiffKind.Add or DiffKind.Remove or DiffKind.Update))
						continue;
					yield return new {
						name = GetEntityText(item.Entity),
						operation = item.Kind switch {
							DiffKind.Add => "added",
							DiffKind.Remove => "removed",
							DiffKind.Update => "changed",
							_ => throw new InvalidOperationException($"Unexpected DiffKind: {item.Kind}"),
						}
					};
				}
			}
		}

		Entry MergeTrees(Entry a, Entry b)
		{
			var m = new Entry() {
				Entity = a.Entity,
				OtherEntity = b.Entity,
				Signature = a.Signature,
			};

			if (a.Children?.Count > 0 && b.Children?.Count > 0)
			{
				var diff = CalculateDiff(a.Children, b.Children);
				m.Children ??= new();

				foreach (var (left, right) in diff)
				{
					if (left != null && right != null)
						m.Children.Add(MergeTrees(left, right));
					else if (left != null)
						m.Children.Add(left);
					else if (right != null)
						m.Children.Add(right);
					else
						Debug.Fail("wh00t?");
					m.Children[^1].Parent = m;
				}
			}
			else if (a.Children?.Count > 0)
			{
				m.Children ??= new();
				foreach (var child in a.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}
			else if (b.Children?.Count > 0)
			{
				m.Children ??= new();
				foreach (var child in b.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}

			return m;
		}

		(List<Entry>, Entry) CreateEntityTree(ICompilation typeSystem)
		{
			var module = (MetadataModule)typeSystem.MainModule!;
			var metadata = module.MetadataFile.Metadata;
			var ambience = new CSharpAmbience();
			ambience.ConversionFlags = ICSharpCode.Decompiler.Output.ConversionFlags.All & ~ICSharpCode.Decompiler.Output.ConversionFlags.ShowDeclaringType;

			List<Entry> results = new();
			Dictionary<TypeDefinitionHandle, Entry> typeEntries = new();
			Dictionary<string, Entry> namespaceEntries = new(StringComparer.Ordinal);

			Entry root = new Entry { Entity = module, Signature = module.FullAssemblyName };

			// typeEntries need a different signature: must include list of base types

			Entry? TryCreateEntry(IEntity entity)
			{
				if (entity.EffectiveAccessibility() != Accessibility.Public)
					return null;

				Entry? parent = null;

				if (entity.DeclaringTypeDefinition != null
					&& !typeEntries.TryGetValue((TypeDefinitionHandle)entity.DeclaringTypeDefinition.MetadataToken, out parent))
				{
					return null;
				}

				var entry = new Entry {
					Signature = ambience.ConvertSymbol(entity),
					Entity = entity,
					Parent = parent,
				};

				if (parent != null)
				{
					parent.Children ??= new();
					parent.Children.Add(entry);
				}

				return entry;
			}

			foreach (var typeDefHandle in metadata.TypeDefinitions)
			{
				var typeDef = module.GetDefinition(typeDefHandle);

				if (typeDef.EffectiveAccessibility() != Accessibility.Public)
					continue;

				var entry = typeEntries[typeDefHandle] = new Entry {
					Signature = ambience.ConvertSymbol(typeDef),
					Entity = typeDef
				};

				if (typeDef.DeclaringType == null)
				{
					if (!namespaceEntries.TryGetValue(typeDef.Namespace, out var nsEntry))
					{
						namespaceEntries[typeDef.Namespace] = nsEntry = new Entry { Parent = root, Signature = typeDef.Namespace, Entity = ResolveNamespace(typeDef.Namespace, typeDef.ParentModule!)! };
						root.Children ??= new();
						root.Children.Add(nsEntry);
					}

					entry.Parent = nsEntry;
					nsEntry.Children ??= new();
					nsEntry.Children.Add(entry);
				}
			}

			foreach (var fieldHandle in metadata.FieldDefinitions)
			{
				var fieldDef = module.GetDefinition(fieldHandle);
				var entry = TryCreateEntry(fieldDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var eventHandle in metadata.EventDefinitions)
			{
				var eventDef = module.GetDefinition(eventHandle);
				var entry = TryCreateEntry(eventDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var propertyHandle in metadata.PropertyDefinitions)
			{
				var propertyDef = module.GetDefinition(propertyHandle);
				var entry = TryCreateEntry(propertyDef);

				if (entry != null)
					results.Add(entry);
			}

			foreach (var methodHandle in metadata.MethodDefinitions)
			{
				var methodDef = module.GetDefinition(methodHandle);

				if (methodDef.AccessorOwner != null)
					continue;

				var entry = TryCreateEntry(methodDef);

				if (entry != null)
					results.Add(entry);
			}

			return (results, root);

			INamespace? ResolveNamespace(string namespaceName, IModule module)
			{
				INamespace current = module.RootNamespace;
				string[] parts = namespaceName.Split('.');

				for (int i = 0; i < parts.Length; i++)
				{
					if (i == 0 && string.IsNullOrEmpty(parts[i]))
					{
						continue;
					}
					var next = current.GetChildNamespace(parts[i]);
					if (next != null)
						current = next;
					else
						return null;
				}

				return current;
			}
		}

		List<(Entry? Left, Entry? Right)> CalculateDiff(List<Entry> left, List<Entry> right)
		{
			Dictionary<string, List<Entry>> leftMap = new();
			Dictionary<string, List<Entry>> rightMap = new();

			foreach (var item in left)
			{
				string key = item.Signature;
				if (leftMap.ContainsKey(key))
					leftMap[key].Add(item);
				else
					leftMap[key] = [item];
			}

			foreach (var item in right)
			{
				string key = item.Signature;
				if (rightMap.ContainsKey(key))
					rightMap[key].Add(item);
				else
					rightMap[key] = [item];
			}

			List<(Entry? Left, Entry? Right)> results = new();

			foreach (var (key, items) in leftMap)
			{
				if (rightMap.TryGetValue(key, out var rightEntries))
				{
					foreach (var item in items)
					{
						var other = rightEntries.Find(_ => EntryComparer.Instance.Equals(_, item));
						results.Add((item, other));
						if (other == null)
						{
							SetKind(item, DiffKind.Remove);
						}
					}
				}
				else
				{
					foreach (var item in items)
					{
						SetKind(item, DiffKind.Remove);
						results.Add((item, null));
					}
				}
			}

			foreach (var (key, items) in rightMap)
			{
				if (leftMap.TryGetValue(key, out var leftEntries))
				{
					foreach (var item in items)
					{
						if (!leftEntries.Any(_ => EntryComparer.Instance.Equals(_, item)))
						{
							results.Add((null, item));
							SetKind(item, DiffKind.Add);
						}
					}
				}
				else
				{
					foreach (var item in items)
					{
						SetKind(item, DiffKind.Add);
						results.Add((null, item));
					}
				}
			}

			return results;

			static void SetKind(Entry item, DiffKind kind)
			{
				if (item.Children?.Count > 0)
				{
					foreach (var child in item.Children)
					{
						SetKind(child, kind);
					}
				}
				else
				{
					item.Kind = kind;
				}
			}
		}

		internal static void Show(TabPageModel tabPage, LoadedAssembly left, LoadedAssembly right, AssemblyTreeModel assemblyTreeModel)
		{
			tabPage.ShowTextView(t => t.RunWithCancellation(token => Task.Run(DoCompare, token), $"Comparing {left.Text} - {right.Text}").Then(vm => {
				tabPage.Title = $"Compare {left.Text} - {right.Text}";
				tabPage.SupportsLanguageSwitching = false;
				tabPage.FrozenContent = true;
				var compareView = new CompareView();
				compareView.DataContext = vm;
				tabPage.Content = compareView;
			}));

			CompareViewModel DoCompare()
			{
				return new CompareViewModel(tabPage, assemblyTreeModel, left, right);
			}
		}
	}

	[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
	public class Entry
	{
		private DiffKind? kind;

		public DiffKind RecursiveKind {
			get {
				if (kind != null)
					return kind.Value;
				if (Children == null)
					return DiffKind.None;

				int addCount = 0, removeCount = 0, updateCount = 0;

				foreach (var item in Children)
				{
					switch (item.RecursiveKind)
					{
						case DiffKind.Add:
							addCount++;
							break;
						case DiffKind.Remove:
							removeCount++;
							break;
						case DiffKind.Update:
							updateCount++;
							break;
					}
				}

				if (addCount == Children.Count)
					return DiffKind.Add;
				if (removeCount == Children.Count)
					return DiffKind.Remove;
				if (addCount > 0 || removeCount > 0 || updateCount > 0)
					return DiffKind.Update;
				return DiffKind.None;
			}
		}

		public DiffKind Kind {
			get => this.kind ?? DiffKind.None;
			set => this.kind = value;
		}
		public required string Signature { get; init; }
		public required ISymbol Entity { get; init; }
		public ISymbol? OtherEntity { get; init; }

		public Entry? Parent { get; set; }
		public List<Entry>? Children { get; set; }

		private string GetDebuggerDisplay()
		{
			return $"Entry{Kind}{Entity.ToString() ?? Signature}";
		}
	}

	public class EntryComparer : IEqualityComparer<Entry>
	{
		public static EntryComparer Instance = new();

		public bool Equals(Entry? x, Entry? y)
		{
			return x?.Signature == y?.Signature;
		}

		public int GetHashCode([DisallowNull] Entry obj)
		{
			return obj.Signature.GetHashCode();
		}
	}

	public enum DiffKind
	{
		None = ' ',
		Add = '+',
		Remove = '-',
		Update = '~'
	}

	class ComparisonEntryTreeNode : ILSpyTreeNode
	{
		private readonly Entry entry;
		private readonly CompareViewModel compareViewModel;

		internal Entry Entry => entry;

		public ComparisonEntryTreeNode(Entry entry, CompareViewModel compareViewModel)
		{
			this.entry = entry;
			this.LazyLoading = entry.Children != null;
			this.compareViewModel = compareViewModel;
		}

		protected override void LoadChildren()
		{
			if (entry.Children == null)
				return;

			foreach (var item in entry.Children.OrderBy(e => (-(int)e.RecursiveKind, e.Entity.SymbolKind, e.Signature)))
			{
				this.Children.Add(new ComparisonEntryTreeNode(item, compareViewModel));
			}
		}

		public override object Text {
			get {
				string? entityText = GetEntityText(entry.Entity);
				string? otherText = GetEntityText(entry.OtherEntity);

				return entityText + (otherText != null && entityText != otherText ? " -> " + otherText : "");

				string? GetEntityText(ISymbol? symbol) => symbol switch {
					ITypeDefinition t => this.Language.TypeToString(t, includeNamespace: false) + GetSuffixString(t.MetadataToken),
					IMethod m => this.Language.MethodToString(m, false, false, false) + GetSuffixString(m.MetadataToken),
					IField f => this.Language.FieldToString(f, false, false, false) + GetSuffixString(f.MetadataToken),
					IProperty p => this.Language.PropertyToString(p, false, false, false) + GetSuffixString(p.MetadataToken),
					IEvent e => this.Language.EventToString(e, false, false, false) + GetSuffixString(e.MetadataToken),
					INamespace n => n.FullName,
					IModule m => m.FullAssemblyName,
					_ => null,
				};
			}
		}

		public override object Icon {
			get {
				switch (entry.Entity)
				{
					case ITypeDefinition t:
						return TypeTreeNode.GetIcon(t);
					case IMethod m:
						return MethodTreeNode.GetIcon(m);
					case IField f:
						return FieldTreeNode.GetIcon(f);
					case IProperty p:
						return PropertyTreeNode.GetIcon(p);
					case IEvent e:
						return EventTreeNode.GetIcon(e);
					case INamespace n:
						return Images.Namespace;
					case IModule m:
						return Images.Assembly;
					default:
						throw new NotSupportedException();
				}
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
		}

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			var node = AssemblyTreeModel.FindTreeNode(entry.Entity);
			AssemblyTreeModel.SelectNode(node);
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			return compareViewModel.ShowIdentical || entry.RecursiveKind != DiffKind.None ? FilterResult.Match : FilterResult.Hidden;
		}

		public Brush Background {
			get {
				switch (entry.RecursiveKind)
				{
					case DiffKind.Add:
						return Brushes.LightGreen;
					case DiffKind.Remove:
						return Brushes.LightPink;
					case DiffKind.Update:
						return Brushes.LightBlue;
				}

				return Brushes.Transparent;
			}
		}
	}
}
