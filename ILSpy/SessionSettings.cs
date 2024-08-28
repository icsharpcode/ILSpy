﻿// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpyX.Search;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Per-session setting:
	/// Loaded at startup; saved at exit.
	/// </summary>
	public sealed class SessionSettings : INotifyPropertyChanged
	{
		public SessionSettings(ILSpySettings spySettings)
		{
			XElement doc = spySettings["SessionSettings"];

			XElement filterSettings = doc.Element("FilterSettings");
			if (filterSettings == null)
				filterSettings = new XElement("FilterSettings");

			this.LanguageSettings = new LanguageSettings(filterSettings);

			this.ActiveAssemblyList = (string)doc.Element("ActiveAssemblyList");

			XElement activeTreeViewPath = doc.Element("ActiveTreeViewPath");
			if (activeTreeViewPath != null)
			{
				this.ActiveTreeViewPath = activeTreeViewPath.Elements().Select(e => Unescape((string)e)).ToArray();
			}
			this.ActiveAutoLoadedAssembly = (string)doc.Element("ActiveAutoLoadedAssembly");

			this.WindowState = FromString((string)doc.Element("WindowState"), WindowState.Normal);
			this.WindowBounds = FromString((string)doc.Element("WindowBounds"), DefaultWindowBounds);
			this.SelectedSearchMode = FromString((string)doc.Element("SelectedSearchMode"), SearchMode.TypeAndMember);
			this.Theme = FromString((string)doc.Element(nameof(Theme)), ThemeManager.Current.DefaultTheme);
			string currentCulture = (string)doc.Element(nameof(CurrentCulture));
			this.CurrentCulture = string.IsNullOrEmpty(currentCulture) ? null : currentCulture;

			this.DockLayout = new DockLayoutSettings(doc.Element("DockLayout"));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			var args = new PropertyChangedEventArgs(propertyName);

			PropertyChanged?.Invoke(this, args);

			MessageBus.Send(this, new SessionSettingsChangedEventArgs(args));
		}

		public LanguageSettings LanguageSettings { get; }

		public SearchMode SelectedSearchMode { get; set; }

		public string Theme {
			get => ThemeManager.Current.Theme;
			set {
				ThemeManager.Current.Theme = value;
				OnPropertyChanged();
			}
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
		internal static Rect DefaultWindowBounds = new Rect(10, 10, 750, 550);

		public DockLayoutSettings DockLayout { get; }

		public XElement ToXml()
		{
			XElement doc = new XElement("SessionSettings");
			doc.Add(this.LanguageSettings.SaveAsXml());
			if (this.ActiveAssemblyList != null)
			{
				doc.Add(new XElement("ActiveAssemblyList", this.ActiveAssemblyList));
			}
			if (this.ActiveTreeViewPath != null)
			{
				doc.Add(new XElement("ActiveTreeViewPath", ActiveTreeViewPath.Select(p => new XElement("Node", Escape(p)))));
			}
			if (this.ActiveAutoLoadedAssembly != null)
			{
				doc.Add(new XElement("ActiveAutoLoadedAssembly", this.ActiveAutoLoadedAssembly));
			}
			doc.Add(new XElement("WindowState", ToString(this.WindowState)));
			doc.Add(new XElement("WindowBounds", ToString(this.WindowBounds)));
			doc.Add(new XElement("SelectedSearchMode", ToString(this.SelectedSearchMode)));
			doc.Add(new XElement(nameof(Theme), ToString(this.Theme)));
			if (this.CurrentCulture != null)
			{
				doc.Add(new XElement(nameof(CurrentCulture), this.CurrentCulture));
			}

			var dockLayoutElement = new XElement("DockLayout");
			if (DockLayout.Valid)
			{
				dockLayoutElement.Add(DockLayout.SaveAsXml());
			}
			doc.Add(dockLayoutElement);
			return doc;
		}

		public void Save()
		{
			var doc = ToXml();
			ILSpySettings.SaveSettings(doc);
		}

		static Regex regex = new Regex("\\\\x(?<num>[0-9A-f]{4})");
		private string activeAssemblyList;

		static string Escape(string p)
		{
			StringBuilder sb = new StringBuilder();
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
	}
}
