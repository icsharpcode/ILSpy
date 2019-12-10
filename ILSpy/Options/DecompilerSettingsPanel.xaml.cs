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

using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml.Linq;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for DecompilerSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = nameof(Properties.Resources.Decompiler), Order = 10)]
	internal partial class DecompilerSettingsPanel : UserControl, IOptionPage
	{
		public DecompilerSettingsPanel()
		{
			InitializeComponent();
		}

		static Decompiler.DecompilerSettings currentDecompilerSettings;

		public static Decompiler.DecompilerSettings CurrentDecompilerSettings {
			get {
				return currentDecompilerSettings ?? (currentDecompilerSettings = LoadDecompilerSettings(ILSpySettings.Load()));
			}
		}

		public static Decompiler.DecompilerSettings LoadDecompilerSettings(ILSpySettings settings)
		{
			XElement e = settings["DecompilerSettings"];
			var newSettings = new Decompiler.DecompilerSettings();
			var properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);
			foreach (var p in properties) {
				var value = (bool?)e.Attribute(p.Name);
				if (value.HasValue)
					p.SetValue(newSettings, value.Value);
			}
			return newSettings;
		}

		public void Load(ILSpySettings settings)
		{
			this.DataContext = new DecompilerSettings(LoadDecompilerSettings(settings));
		}

		public void Save(XElement root)
		{
			XElement section = new XElement("DecompilerSettings");
			var newSettings = ((DecompilerSettings)this.DataContext).ToDecompilerSettings();
			var properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);
			foreach (var p in properties) {
				section.SetAttributeValue(p.Name, p.GetValue(newSettings));
			}
			XElement existingElement = root.Element("DecompilerSettings");
			if (existingElement != null)
				existingElement.ReplaceWith(section);
			else
				root.Add(section);

			currentDecompilerSettings = newSettings;
		}

		private void OnGroupChecked(object sender, RoutedEventArgs e)
		{
			CheckGroup((CollectionViewGroup)((CheckBox)sender).DataContext, true);
		}
		private void OnGroupUnchecked(object sender, RoutedEventArgs e)
		{
			CheckGroup((CollectionViewGroup)((CheckBox)sender).DataContext, false);
		}

		void CheckGroup(CollectionViewGroup group, bool value)
		{
			foreach (var item in group.Items) {
				switch (item) {
					case CollectionViewGroup subGroup:
						CheckGroup(subGroup, value);
						break;
					case CSharpDecompilerSetting setting:
						setting.IsEnabled = value;
						break;
				}
			}
		}

		bool IsGroupChecked(CollectionViewGroup group)
		{
			bool value = true;
			foreach (var item in group.Items) {
				switch (item) {
					case CollectionViewGroup subGroup:
						value = value && IsGroupChecked(subGroup);
						break;
					case CSharpDecompilerSetting setting:
						value = value && setting.IsEnabled;
						break;
				}
			}
			return value;
		}

		private void OnGroupLoaded(object sender, RoutedEventArgs e)
		{
			CheckBox checkBox = (CheckBox)sender;
			checkBox.IsChecked = IsGroupChecked((CollectionViewGroup)checkBox.DataContext);
		}

		public void LoadDefaults()
		{
			currentDecompilerSettings = new Decompiler.DecompilerSettings();
			this.DataContext = new DecompilerSettings(currentDecompilerSettings);
		}
	}

	public class DecompilerSettings : INotifyPropertyChanged
	{
		public CSharpDecompilerSetting[] Settings { get; set; }

		public DecompilerSettings(Decompiler.DecompilerSettings settings)
		{
			Settings = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
				.Select(p => new CSharpDecompilerSetting(p) { IsEnabled = (bool)p.GetValue(settings) })
				.OrderBy(item => item.Category)
				.ThenBy(item => item.Description)
				.ToArray();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public Decompiler.DecompilerSettings ToDecompilerSettings()
		{
			var settings = new Decompiler.DecompilerSettings();
			foreach (var item in Settings) {
				item.Property.SetValue(settings, item.IsEnabled);
			}
			return settings;
		}
	}

	public class CSharpDecompilerSetting : INotifyPropertyChanged
	{
		bool isEnabled;

		public CSharpDecompilerSetting(PropertyInfo p)
		{
			this.Property = p;
			this.Category =  GetResourceString(p.GetCustomAttribute<CategoryAttribute>()?.Category ?? Resources.Other);
			this.Description =  GetResourceString(p.GetCustomAttribute<DescriptionAttribute>()?.Description ?? p.Name);
		}

		public PropertyInfo Property { get; }

		public bool IsEnabled {
			get => isEnabled;
			set {
				if (value != isEnabled) {
					isEnabled = value;
					OnPropertyChanged();
				}
			}
		}

		public string Description { get; set; }

		public string Category { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		static string GetResourceString(string key)
		{
			var str = !string.IsNullOrEmpty(key) ? Resources.ResourceManager.GetString(key) : null;
			return string.IsNullOrEmpty(key) || string.IsNullOrEmpty(str) ? key : str;
		}
	}
}
