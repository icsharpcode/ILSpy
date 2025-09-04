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

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
	public class DecompilerSettings : Decompiler.DecompilerSettings, ISettingsSection
	{
		static readonly PropertyInfo[] properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
				.ToArray();

		public XName SectionName => "DecompilerSettings";

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			foreach (var p in properties)
			{
				section.SetAttributeValue(p.Name, p.GetValue(this));
			}

			return section;
		}

		public void LoadFromXml(XElement section)
		{
			foreach (var p in properties)
			{
				var value = (bool?)section.Attribute(p.Name);
				if (value.HasValue)
					p.SetValue(this, value.Value);
			}
		}

		public override DecompilerSettings Clone()
		{
			return (DecompilerSettings)base.Clone();
		}

		public static bool IsKnownOption(string name, [NotNullWhen(true)] out PropertyInfo? property)
		{
			property = null;
			foreach (var item in properties)
			{
				if (item.Name != name)
					continue;
				property = item;
				return true;
			}

			return false;
		}
	}
}
