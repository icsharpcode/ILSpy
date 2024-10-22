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
using System.Composition;
using System.Linq;
using System.Reflection;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TreeNodes;

using TomsToolbox.Wpf;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 10)]
	[NonShared]
	public sealed class DecompilerSettingsViewModel : ObservableObjectBase, IOptionPage
	{
		private static readonly PropertyInfo[] propertyInfos = typeof(Decompiler.DecompilerSettings).GetProperties()
			.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
			.ToArray();

		public string Title => Resources.Decompiler;

		private DecompilerSettingsGroupViewModel[] settings;
		public DecompilerSettingsGroupViewModel[] Settings {
			get => settings;
			set => SetProperty(ref settings, value);
		}

		private DecompilerSettings decompilerSettings;

		public void Load(SettingsSnapshot snapshot)
		{
			decompilerSettings = snapshot.GetSettings<DecompilerSettings>();
			LoadSettings();
		}

		private void LoadSettings()
		{
			this.Settings = propertyInfos
				.Select(p => new DecompilerSettingsItemViewModel(p, decompilerSettings))
				.OrderBy(item => item.Category, NaturalStringComparer.Instance)
				.GroupBy(p => p.Category)
				.Select(g => new DecompilerSettingsGroupViewModel(g.Key, g.OrderBy(i => i.Description).ToArray()))
				.ToArray();
		}

		public void LoadDefaults()
		{
			var defaults = new Decompiler.DecompilerSettings();

			foreach (var propertyInfo in propertyInfos)
			{
				propertyInfo.SetValue(decompilerSettings, propertyInfo.GetValue(defaults));
			}

			LoadSettings();
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

	public sealed class DecompilerSettingsItemViewModel(PropertyInfo property, DecompilerSettings decompilerSettings) : ObservableObjectBase
	{
		private bool isEnabled = property.GetValue(decompilerSettings) is true;

		public PropertyInfo Property => property;

		public bool IsEnabled {
			get => isEnabled;
			set {
				if (SetProperty(ref isEnabled, value))
				{
					property.SetValue(decompilerSettings, value);
				}
			}
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
