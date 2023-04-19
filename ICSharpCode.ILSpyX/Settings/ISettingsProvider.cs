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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using static System.Collections.Specialized.BitVector32;

namespace ICSharpCode.ILSpyX.Settings
{
	public interface ISettingsProvider
	{
		XElement this[XName section] { get; }

		void Update(Action<XElement> action);
		ISettingsProvider Load();

		public static ICSharpCode.Decompiler.DecompilerSettings LoadDecompilerSettings(ISettingsProvider settingsProvider)
		{
			XElement e = settingsProvider["DecompilerSettings"];
			var newSettings = new Decompiler.DecompilerSettings();
			var properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);
			foreach (var p in properties)
			{
				var value = (bool?)e.Attribute(p.Name);
				if (value.HasValue)
					p.SetValue(newSettings, value.Value);
			}
			return newSettings;
		}

		public static void SaveDecompilerSettings(XElement root, ICSharpCode.Decompiler.DecompilerSettings newSettings)
		{
			var properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);

			XElement section = new XElement("DecompilerSettings");
			foreach (var p in properties)
			{
				section.SetAttributeValue(p.Name, p.GetValue(newSettings));
			}

			XElement? existingElement = root.Element("DecompilerSettings");
			if (existingElement != null)
				existingElement.ReplaceWith(section);
			else
				root.Add(section);
		}
	}
}
