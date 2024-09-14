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
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Linq;

using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.Search;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Per-session setting:
	/// Loaded at startup; saved at exit.
	/// </summary>
	public sealed class SessionSettings : ISettingsSection
	{
		public XName SectionName => "SessionSettings";

		public void LoadFromXml(XElement section)
		{
			XElement filterSettings = section.Element("FilterSettings") ?? new XElement("FilterSettings");

			LanguageSettings = new(filterSettings, this);
			LanguageSettings.PropertyChanged += (sender, e) => PropertyChanged?.Invoke(sender, e);

			ActiveAssemblyList = (string)section.Element("ActiveAssemblyList");
			ActiveTreeViewPath = section.Element("ActiveTreeViewPath")?.Elements().Select(e => Unescape((string)e)).ToArray();
			ActiveAutoLoadedAssembly = (string)section.Element("ActiveAutoLoadedAssembly");
			WindowState = FromString((string)section.Element("WindowState"), WindowState.Normal);
			WindowBounds = FromString((string)section.Element("WindowBounds"), DefaultWindowBounds);
			SelectedSearchMode = FromString((string)section.Element("SelectedSearchMode"), SearchMode.TypeAndMember);
			Theme = FromString((string)section.Element(nameof(Theme)), ThemeManager.Current.DefaultTheme);
			var culture = (string)section.Element(nameof(CurrentCulture));
			CurrentCulture = string.IsNullOrEmpty(culture) ? null : culture;
			DockLayout = new(section.Element("DockLayout"));
		}

		public LanguageSettings LanguageSettings { get; set; }

		public SearchMode SelectedSearchMode { get; set; }

		private string theme;
		public string Theme {
			get => theme;
			set => SetProperty(ref theme, value);
		}

		public string[] ActiveTreeViewPath;
		public string ActiveAutoLoadedAssembly;

		string currentCulture;

		public string CurrentCulture {
			get { return currentCulture; }
			set {
				if (currentCulture != value)
				{
					currentCulture = value;
					OnPropertyChanged();
				}
			}
		}

		public string ActiveAssemblyList {
			get => activeAssemblyList;
			set {
				if (value != null && value != activeAssemblyList)
				{
					activeAssemblyList = value;
					OnPropertyChanged();
				}
			}
		}

		public WindowState WindowState;
		public Rect WindowBounds;
		internal static Rect DefaultWindowBounds = new(10, 10, 750, 550);

		public DockLayoutSettings DockLayout { get; set; }

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			section.Add(this.LanguageSettings.SaveAsXml());
			if (this.ActiveAssemblyList != null)
			{
				section.Add(new XElement("ActiveAssemblyList", this.ActiveAssemblyList));
			}
			if (this.ActiveTreeViewPath != null)
			{
				section.Add(new XElement("ActiveTreeViewPath", ActiveTreeViewPath.Select(p => new XElement("Node", Escape(p)))));
			}
			if (this.ActiveAutoLoadedAssembly != null)
			{
				section.Add(new XElement("ActiveAutoLoadedAssembly", this.ActiveAutoLoadedAssembly));
			}
			section.Add(new XElement("WindowState", ToString(this.WindowState)));
			section.Add(new XElement("WindowBounds", ToString(this.WindowBounds)));
			section.Add(new XElement("SelectedSearchMode", ToString(this.SelectedSearchMode)));
			section.Add(new XElement(nameof(Theme), ToString(this.Theme)));
			if (this.CurrentCulture != null)
			{
				section.Add(new XElement(nameof(CurrentCulture), this.CurrentCulture));
			}
			var dockLayoutElement = new XElement("DockLayout");
			if (DockLayout.Valid)
			{
				dockLayoutElement.Add(DockLayout.SaveAsXml());
			}
			section.Add(dockLayoutElement);

			return section;
		}

		static Regex regex = new("\\\\x(?<num>[0-9A-f]{4})");
		private string activeAssemblyList;

		static string Escape(string p)
		{
			StringBuilder sb = new();
			foreach (char ch in p)
			{
				if (char.IsLetterOrDigit(ch))
					sb.Append(ch);
				else
					sb.AppendFormat("\\x{0:X4}", (int)ch);
			}
			return sb.ToString();
		}

		static string Unescape(string p)
		{
			return regex.Replace(p, m => ((char)int.Parse(m.Groups["num"].Value, NumberStyles.HexNumber)).ToString());
		}

		static T FromString<T>(string s, T defaultValue)
		{
			if (s == null)
				return defaultValue;
			try
			{
				TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
				return (T)c.ConvertFromInvariantString(s);
			}
			catch (FormatException)
			{
				return defaultValue;
			}
		}

		static string ToString<T>(T obj)
		{
			TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
			return c.ConvertToInvariantString(obj);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new(propertyName));
		}

		private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value))
				return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
