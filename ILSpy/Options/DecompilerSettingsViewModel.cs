// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel;
using System.Composition;
using System.Linq;
using System.Reflection;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.TreeNodes;

// `Decompiler` short-form alias keeps the reflection-walk over `Decompiler.DecompilerSettings`
// terse and disambiguates from the ILSpyX `DecompilerSettings` wrapper imported just below.
using Decompiler = ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Reflection-driven viewmodel for the Decompiler-settings panel. Walks every
	/// non-[Browsable(false)] property on <see cref="Decompiler.DecompilerSettings"/>,
	/// groups them by their [Category] attribute, and surfaces each as an
	/// <see cref="DecompilerSettingsItemViewModel"/> with a localised description.
	/// </summary>
	// No [Shared]: each Options tab open gets a fresh viewmodel instance bound to a fresh
	// snapshot. In System.Composition the absence of [Shared] gives per-call instantiation.
	[ExportOptionPage(Order = 10)]
	public sealed partial class DecompilerSettingsViewModel : ObservableObject, IOptionPage
	{
		static readonly PropertyInfo[] propertyInfos = typeof(Decompiler.DecompilerSettings).GetProperties()
			.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
			.ToArray();

		public string Title => Resources.Decompiler;

		/// <summary>The groups currently shown: all of <see cref="allGroups"/>, or the subset
		/// that survives the Options search text (see <see cref="ApplySearch"/>).</summary>
		[ObservableProperty]
		DecompilerSettingsGroupViewModel[] settings = System.Array.Empty<DecompilerSettingsGroupViewModel>();

		DecompilerSettingsGroupViewModel[] allGroups = System.Array.Empty<DecompilerSettingsGroupViewModel>();

		string searchText = string.Empty;

		DecompilerSettings decompilerSettings = null!;

		public IEnumerable<string> SearchableText
			=> allGroups.SelectMany(g => g.Settings.Select(s => s.Description).Prepend(g.Category));

		public void Load(SettingsService settings)
		{
			decompilerSettings = settings.DecompilerSettings;
			LoadSettings();
		}

		void LoadSettings()
		{
			allGroups = propertyInfos
				.Select(p => new DecompilerSettingsItemViewModel(p, decompilerSettings))
				.OrderBy(item => item.Category, NaturalStringComparer.Instance)
				.GroupBy(p => p.Category)
				.Select(g => new DecompilerSettingsGroupViewModel(g.Key, g.OrderBy(i => i.Description).ToArray()))
				.ToArray();
			ApplySearch(searchText);
		}

		/// <summary>
		/// Narrows <see cref="Settings"/> to the groups with a hit: a matching category keeps all
		/// of its items, otherwise only the matching items are kept. The group's tri-state
		/// checkbox then bulk-toggles just the items shown.
		/// </summary>
		public void ApplySearch(string text)
		{
			searchText = text;
			// Narrowed groups are throwaway wrappers around the same items; detach them so the
			// items don't keep every wrapper from every keystroke alive through PropertyChanged.
			foreach (var g in Settings.Except(allGroups))
				g.Detach();
			if (text.Length == 0)
			{
				Settings = allGroups;
				return;
			}
			Settings = allGroups
				.Select(g => OptionsPageModel.Contains(g.Category, text)
					? g
					: new DecompilerSettingsGroupViewModel(g.Category,
						g.Settings.Where(s => OptionsPageModel.Contains(s.Description, text)).ToArray()))
				.Where(g => g.Settings.Length > 0)
				.ToArray();
		}

		public void LoadDefaults()
		{
			var defaults = new Decompiler.DecompilerSettings();
			foreach (var p in propertyInfos)
				p.SetValue(decompilerSettings, p.GetValue(defaults));
			LoadSettings();
		}
	}

	/// <summary>One Category section in the Decompiler panel. Surfaces a tri-state header
	/// checkbox (all/none/some) that bulk-toggles every item under it.</summary>
	public sealed partial class DecompilerSettingsGroupViewModel : ObservableObject
	{
		[ObservableProperty]
		bool? areAllItemsChecked;

		public DecompilerSettingsGroupViewModel(string category, DecompilerSettingsItemViewModel[] settings)
		{
			Settings = settings;
			Category = category;
			areAllItemsChecked = GetAreAllItemsChecked(Settings);
			foreach (var s in settings)
				s.PropertyChanged += ItemPropertyChanged;
		}

		public string Category { get; }

		public DecompilerSettingsItemViewModel[] Settings { get; }

		public void Detach()
		{
			foreach (var s in Settings)
				s.PropertyChanged -= ItemPropertyChanged;
		}

		partial void OnAreAllItemsCheckedChanged(bool? value)
		{
			if (!value.HasValue)
				return;
			foreach (var s in Settings)
				s.IsEnabled = value.Value;
		}

		void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DecompilerSettingsItemViewModel.IsEnabled))
				AreAllItemsChecked = GetAreAllItemsChecked(Settings);
		}

		static bool? GetAreAllItemsChecked(ICollection<DecompilerSettingsItemViewModel> settings)
		{
			var enabled = settings.Count(i => i.IsEnabled);
			if (enabled == settings.Count)
				return true;
			if (enabled == 0)
				return false;
			return null;
		}
	}

	/// <summary>One reflection-discovered boolean setting in the Decompiler panel.</summary>
	public sealed partial class DecompilerSettingsItemViewModel : ObservableObject
	{
		readonly PropertyInfo property;
		readonly DecompilerSettings settings;

		public DecompilerSettingsItemViewModel(PropertyInfo property, DecompilerSettings settings)
		{
			this.property = property;
			this.settings = settings;
			isEnabled = property.GetValue(settings) is true;
			Description = GetResourceString(property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name);
			Category = GetResourceString(property.GetCustomAttribute<CategoryAttribute>()?.Category ?? Resources.Other);
		}

		[ObservableProperty]
		bool isEnabled;

		/// <summary>The <see cref="Decompiler.DecompilerSettings"/> property this item
		/// reflects. Exposed for tests that need to look up an item by name.</summary>
		public PropertyInfo Property => property;

		public string Description { get; }

		public string Category { get; }

		partial void OnIsEnabledChanged(bool value)
		{
			property.SetValue(settings, value);
		}

		static string GetResourceString(string key)
		{
			var str = !string.IsNullOrEmpty(key) ? Resources.ResourceManager.GetString(key) : null;
			return string.IsNullOrEmpty(key) || string.IsNullOrEmpty(str) ? key : str;
		}
	}

}
