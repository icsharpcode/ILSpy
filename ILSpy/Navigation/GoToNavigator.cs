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
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Runs a go-to command and presents its outcome: one target is navigated to directly through
	/// the assembly tree (so the jump lands in the navigation history like any other selection),
	/// several targets open a <see cref="NavigationChooser"/>, and none shows a short notice.
	/// </summary>
	[Export]
	[Shared]
	public sealed class GoToNavigator
	{
		static readonly TimeSpan NoticeDuration = TimeSpan.FromSeconds(3);

		readonly AssemblyTreeModel assemblyTreeModel;
		readonly LanguageService languageService;
		Flyout? noticeFlyout;

		[ImportingConstructor]
		public GoToNavigator(AssemblyTreeModel assemblyTreeModel, LanguageService languageService)
		{
			this.assemblyTreeModel = assemblyTreeModel;
			this.languageService = languageService;
		}

		/// <summary>The most recently started go-to operation; completes once its outcome is presented.</summary>
		public Task LastOperation { get; private set; } = Task.CompletedTask;

		/// <summary>The chooser currently open, or null.</summary>
		public NavigationChooser? ActiveChooser { get; private set; }

		/// <summary>The text of the most recent notice.</summary>
		public string? LastNotice { get; private set; }

		/// <summary>Starts resolving <paramref name="kind"/> for <paramref name="entity"/> and presents the result.</summary>
		public void GoTo(GoToKind kind, IEntity entity, Control? anchor)
		{
			ArgumentNullException.ThrowIfNull(entity);
			LastOperation = GoToAsync(kind, entity, anchor);
		}

		async Task GoToAsync(GoToKind kind, IEntity entity, Control? anchor)
		{
			IReadOnlyList<IEntity> targets;
			if (kind == GoToKind.Declaration)
			{
				targets = new[] { entity };
			}
			else
			{
				var assemblyList = assemblyTreeModel.AssemblyList;
				if (assemblyList == null)
					return;
				// The inheritor search scans every module in scope; keep it off the UI thread.
				targets = await Task.Run(() => SymbolHierarchy.Find(kind, entity, assemblyList, CancellationToken.None));
			}

			switch (targets.Count)
			{
				case 0:
					ShowNotice(NothingFoundText(kind), anchor);
					break;
				case 1:
					NavigateTo(targets[0], anchor);
					break;
				default:
					ShowChoices(ChooserTitle(kind, entity),
						targets.Select(t => new NavigationChoice(Describe(t), () => NavigateTo(t, anchor))).ToList(),
						anchor);
					break;
			}
		}

		/// <summary>Selects the tree node of <paramref name="entity"/>, or shows a notice when it is not in the assembly list.</summary>
		public void NavigateTo(IEntity entity, Control? anchor)
		{
			var node = assemblyTreeModel.FindTreeNode(entity);
			if (node == null)
			{
				ShowNotice($"{Describe(entity)} is not in the assembly list", anchor);
				return;
			}
			assemblyTreeModel.SelectNode(node);
		}

		/// <summary>Opens a chooser over <paramref name="choices"/>, replacing any chooser already open.</summary>
		public NavigationChooser ShowChoices(string title, IReadOnlyList<NavigationChoice> choices, Control? anchor)
		{
			ActiveChooser?.Close();
			var chooser = new NavigationChooser(title, choices);
			chooser.Closed += (_, _) => {
				if (ReferenceEquals(ActiveChooser, chooser))
					ActiveChooser = null;
			};
			ActiveChooser = chooser;
			if (ResolveAnchor(anchor) is { } target)
				chooser.Show(target);
			return chooser;
		}

		/// <summary>Shows <paramref name="text"/> in a small non-modal flyout that closes by itself.</summary>
		public void ShowNotice(string text, Control? anchor)
		{
			LastNotice = text;
			noticeFlyout?.Hide();
			var target = ResolveAnchor(anchor);
			if (target == null)
				return;
			var flyout = new Flyout {
				Content = new TextBlock { Text = text, Margin = new Thickness(4) },
				Placement = PlacementMode.Center,
			};
			noticeFlyout = flyout;
			flyout.ShowAt(target);
			DispatcherTimer.RunOnce(() => {
				flyout.Hide();
				if (ReferenceEquals(noticeFlyout, flyout))
					noticeFlyout = null;
			}, NoticeDuration);
		}

		/// <summary>Display text for a navigation target: its signature plus the assembly it lives in.</summary>
		public string Describe(IEntity entity)
		{
			string text;
			try
			{
				text = languageService.CurrentLanguage.EntityToString(entity,
					ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedEntityNames);
			}
			catch (Exception)
			{
				text = entity.FullName;
			}
			var assemblyName = entity.ParentModule?.AssemblyName;
			return string.IsNullOrEmpty(assemblyName) ? text : $"{text}  [{assemblyName}]";
		}

		static string NothingFoundText(GoToKind kind) => kind switch {
			GoToKind.Implementation => "No implementations found",
			GoToKind.BaseSymbols => "No base symbols found",
			GoToKind.DerivedSymbols => "No derived symbols found",
			_ => "No declaration found",
		};

		static string ChooserTitle(GoToKind kind, IEntity entity) => kind switch {
			GoToKind.Implementation => $"Implementations of {entity.Name}",
			GoToKind.BaseSymbols => $"Base symbols of {entity.Name}",
			GoToKind.DerivedSymbols => $"Derived symbols of {entity.Name}",
			_ => entity.Name,
		};

		/// <summary>
		/// The control a popup is shown over: <paramref name="preferred"/> when it is part of a live
		/// visual tree, otherwise the application's main window.
		/// </summary>
		internal static Control? ResolveAnchor(Control? preferred)
		{
			if (preferred != null && TopLevel.GetTopLevel(preferred) != null)
				return preferred;
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } mainWindow })
				return mainWindow;
			var window = AppEnv.AppComposition.TryGetExport<Views.MainWindow>();
			return window is { IsVisible: true } ? window : null;
		}
	}
}
