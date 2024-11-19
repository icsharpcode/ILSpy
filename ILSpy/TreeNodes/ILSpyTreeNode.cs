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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Base class of all ILSpy tree nodes.
	/// </summary>
	public abstract class ILSpyTreeNode : SharpTreeNode, ITreeNode
	{
		protected ILSpyTreeNode()
		{
			MessageBus<SettingsChangedEventArgs>.Subscribers += (sender, e) => Settings_Changed(sender, e);
		}

		LanguageSettings LanguageSettings => SettingsService.SessionSettings.LanguageSettings;

		public Language Language => LanguageService.Language;

		protected static AssemblyTreeModel AssemblyTreeModel { get; } = App.ExportProvider.GetExportedValue<AssemblyTreeModel>();

		protected static ICollection<IResourceNodeFactory> ResourceNodeFactories { get; } = App.ExportProvider.GetExportedValues<IResourceNodeFactory>().ToArray();

		protected static SettingsService SettingsService { get; } = App.ExportProvider.GetExportedValue<SettingsService>();

		protected static LanguageService LanguageService { get; } = App.ExportProvider.GetExportedValue<LanguageService>();

		protected static DockWorkspace DockWorkspace { get; } = App.ExportProvider.GetExportedValue<DockWorkspace>();

		public virtual FilterResult Filter(LanguageSettings settings)
		{
			if (string.IsNullOrEmpty(settings.SearchTerm))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public abstract void Decompile(Language language, ITextOutput output, DecompilationOptions options);

		/// <summary>
		/// Used to implement special view logic for some items.
		/// This method is called on the main thread when only a single item is selected.
		/// If it returns false, normal decompilation is used to view the item.
		/// </summary>
		public virtual bool View(ViewModels.TabPageModel tabPage)
		{
			return false;
		}

		public override void ActivateItemSecondary(IPlatformRoutedEventArgs e)
		{
			var assemblyTreeModel = AssemblyTreeModel;

			assemblyTreeModel.SelectNode(this, inNewTabPage: true);

			App.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, assemblyTreeModel.RefreshDecompiledView);
		}

		/// <summary>
		/// Used to implement special save logic for some items.
		/// This method is called on the main thread when only a single item is selected.
		/// If it returns false, normal decompilation is used to save the item.
		/// </summary>
		public virtual bool Save(ViewModels.TabPageModel tabPage)
		{
			return false;
		}

		public override void OnChildrenChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnChildrenChanged(e);

			if (e.NewItems != null)
			{
				if (IsVisible)
				{
					foreach (ILSpyTreeNode node in e.NewItems)
						ApplyFilterToChild(node);
				}
			}
		}

		void ApplyFilterToChild(ILSpyTreeNode child)
		{
			FilterResult r = child.Filter(this.LanguageSettings);

			switch (r)
			{
				case FilterResult.Hidden:
					child.IsHidden = true;
					break;
				case FilterResult.Match:
					child.IsHidden = false;
					break;
				case FilterResult.Recurse:
				case FilterResult.MatchAndRecurse:
					child.EnsureChildrenFiltered();
					child.IsHidden = child.Children.All(c => c.IsHidden);
					break;
				default:
					throw new InvalidEnumArgumentException();
			}
		}

		protected virtual void Settings_Changed(object sender, PropertyChangedEventArgs e)
		{
			if (sender is not ILSpy.LanguageSettings)
				return;
			if (e.PropertyName is not (nameof(LanguageSettings.LanguageId) or nameof(LanguageSettings.LanguageVersionId)))
				return;

			RaisePropertyChanged(nameof(Text));
			if (IsVisible)
			{
				foreach (ILSpyTreeNode node in this.Children.OfType<ILSpyTreeNode>())
					ApplyFilterToChild(node);
			}
		}

		internal void EnsureChildrenFiltered()
		{
			EnsureLazyChildren();
			foreach (ILSpyTreeNode node in this.Children.OfType<ILSpyTreeNode>())
				ApplyFilterToChild(node);
		}

		protected string GetSuffixString(IMember member) => GetSuffixString(member.MetadataToken);

		protected string GetSuffixString(EntityHandle handle)
		{
			if (!SettingsService.DisplaySettings.ShowMetadataTokens)
				return string.Empty;

			int token = MetadataTokens.GetToken(handle);
			if (SettingsService.DisplaySettings.ShowMetadataTokensInBase10)
				return " @" + token;
			return " @" + token.ToString("x8");
		}

		public virtual bool IsPublicAPI {
			get { return true; }
		}

		public virtual bool IsAutoLoaded {
			get { return false; }
		}

		IEnumerable<ITreeNode> ITreeNode.Children => this.Children.OfType<ILSpyTreeNode>();
	}
}