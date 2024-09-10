using System.ComponentModel;

using ICSharpCode.ILSpyX.Settings;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

#nullable enable

namespace ICSharpCode.ILSpy.Options
{
	public class DecompilerSettings : Decompiler.DecompilerSettings, ISettingsSection
	{
		static readonly PropertyInfo[] properties = typeof(Decompiler.DecompilerSettings).GetProperties()
				.Where(p => p.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false)
				.ToArray();

		public XName SectionName => "DecompilerSettings";

		public void SaveToSection(XElement section)
		{
			foreach (var p in properties)
			{
				section.SetAttributeValue(p.Name, p.GetValue(this));
			}
		}

		public void LoadFromSection(XElement section)
		{
			foreach (var p in properties)
			{
				var value = (bool?)section.Attribute(p.Name);
				if (value.HasValue)
					p.SetValue(this, value.Value);
			}
		}

		public new DecompilerSettings Clone()
		{
			var section = new XElement("DecompilerSettings");
			SaveToSection(section);
			var newSettings = new DecompilerSettings();
			newSettings.LoadFromSection(section);

			return newSettings;
		}
	}
}
