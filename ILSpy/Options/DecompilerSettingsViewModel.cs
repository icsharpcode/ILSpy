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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpyX.Settings;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 10)]
	[PartCreationPolicy(CreationPolicy.NonShared)]
	public sealed class DecompilerSettingsViewModel : ObservableObjectBase, IOptionPage
	{
		private DecompilerSettingsGroupViewModel[] settings;

		public string Title => Resources.Decompiler;

		public DecompilerSettingsGroupViewModel[] Settings {
			get => settings;
			set => SetProperty(ref settings, value);
		}

		public void Load(ILSpySettings spySettings)
		{
			Load(ISettingsProvider.LoadDecompilerSettings(spySettings));
		}

		private void Load(DecompilerSettings decompilerSettings)
		{
			this.Settings = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
				.Select(p => new DecompilerSettingsItemViewModel(p) { IsEnabled = p.GetValue(decompilerSettings) is true })
				.OrderBy(item => item.Category, NaturalStringComparer.Instance)
				.GroupBy(p => p.Category)
				.Select(g => new DecompilerSettingsGroupViewModel(g.Key, g.OrderBy(i => i.Description).ToArray()))
				.ToArray();
		}

		public void Save(XElement root)
		{
			var newSettings = ToDecompilerSettings();
			ISettingsProvider.SaveDecompilerSettings(root, newSettings);

			SettingsService.Instance.DecompilerSettings = newSettings;
			SettingsService.Instance.AssemblyListManager.ApplyWinRTProjections = newSettings.ApplyWindowsRuntimeProjections;
			SettingsService.Instance.AssemblyListManager.UseDebugSymbols = newSettings.UseDebugSymbols;
		}

		public void LoadDefaults()
		{
			Load(new DecompilerSettings());
		}

		private DecompilerSettings ToDecompilerSettings()
		{
			var newSettings = new DecompilerSettings();

			foreach (var item in Settings.SelectMany(group => group.Settings))
			{
				item.Property.SetValue(newSettings, item.IsEnabled);
			}

			return newSettings;
		}
	}

	public sealed class DecompilerSettingsGroupViewModel : ObservableObjectBase
	{
		private bool? areAllItemsChecked;

		public DecompilerSettingsGroupViewModel(string category, DecompilerSettingsItemViewModel[] settings)
		{
			Settings = settings;
			Category = category;

			areAllItemsChecked = GetAreAllItemsChecked(Settings);

			foreach (DecompilerSettingsItemViewModel viewModel in settings)
			{
				viewModel.PropertyChanged += Item_PropertyChanged;
			}
		}

		public bool? AreAllItemsChecked {
			get => areAllItemsChecked;
			set {
				SetProperty(ref areAllItemsChecked, value);

				if (!value.HasValue)
					return;

				foreach (var setting in Settings)
				{
					setting.IsEnabled = value.Value;
				}
			}
		}

		public string Category { get; }

		public DecompilerSettingsItemViewModel[] Settings { get; }

		private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DecompilerSettingsItemViewModel.IsEnabled))
			{
				AreAllItemsChecked = GetAreAllItemsChecked(Settings);
			}
		}

		private static bool? GetAreAllItemsChecked(ICollection<DecompilerSettingsItemViewModel> settings)
		{
			var numberOfEnabledItems = settings.Count(item => item.IsEnabled);

			if (numberOfEnabledItems == settings.Count)
				return true;

			if (numberOfEnabledItems == 0)
				return false;

			return null;
		}
	}

	public sealed class DecompilerSettingsItemViewModel(PropertyInfo property) : ObservableObjectBase
	{
		private bool isEnabled;

		public PropertyInfo Property { get; } = property;

		public bool IsEnabled {
			get => isEnabled;
			set => SetProperty(ref isEnabled, value);
		}

		public string Description { get; set; } = GetResourceString(property.GetCustomAttribute<DescriptionAttribute>()?.Description ?? property.Name);

		public string Category { get; set; } = GetResourceString(property.GetCustomAttribute<CategoryAttribute>()?.Category ?? Resources.Other);

		private static string GetResourceString(string key)
		{
			var str = !string.IsNullOrEmpty(key) ? Resources.ResourceManager.GetString(key) : null;
			return string.IsNullOrEmpty(key) || string.IsNullOrEmpty(str) ? key : str;
		}
	}
}
