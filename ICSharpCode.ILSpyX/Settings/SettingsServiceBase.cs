// Copyright (c) 2024 Tom Englert
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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
	public interface IChildSettings
	{
		ISettingsSection Parent { get; }
	}

	public interface ISettingsSection : INotifyPropertyChanged
	{
		XName SectionName { get; }

		void LoadFromXml(XElement section);

		XElement SaveToXml();
	}

	public class SettingsServiceBase(ISettingsProvider spySettings)
	{
		protected readonly ConcurrentDictionary<Type, ISettingsSection> sections = new();

		protected ISettingsProvider SpySettings { get; set; } = spySettings;

		public T GetSettings<T>() where T : ISettingsSection, new()
		{
			return (T)sections.GetOrAdd(typeof(T), _ => {
				T section = new T();

				var sectionElement = SpySettings[section.SectionName];

				section.LoadFromXml(sectionElement);
				section.PropertyChanged += Section_PropertyChanged;

				return section;
			});
		}

		protected static void SaveSection(ISettingsSection section, XElement root)
		{
			var element = section.SaveToXml();

			var existingElement = root.Element(section.SectionName);
			if (existingElement != null)
				existingElement.ReplaceWith(element);
			else
				root.Add(element);
		}

		protected virtual void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
		}
	}
}
