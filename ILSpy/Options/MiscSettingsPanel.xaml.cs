using ICSharpCode.Decompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for MiscSettingsPanel.xaml
	/// </summary>
	[ExportOptionPage(Title = "Misc", Order = 2)]
	public partial class MiscSettingsPanel : UserControl, IOptionPage
	{
		public MiscSettingsPanel()
		{
			InitializeComponent();
		}

		public void Load(ILSpySettings settings)
		{
			this.DataContext = LoadDecompilerSettings(settings);
		}

		private MiscSettings LoadDecompilerSettings(ILSpySettings settings)
		{
			XElement e = settings["MiscSettings"];
			MiscSettings s = new MiscSettings();
			s.AllowMultipleInstances = (bool?)e.Attribute("allowMultipleInstance") ?? s.AllowMultipleInstances;
			return s;
		}

		public void Save(XElement root)
		{
			MiscSettings s = (MiscSettings)this.DataContext;
			XElement section = new XElement("MiscSettings");
			section.SetAttributeValue("allowMultipleInstance", s.AllowMultipleInstances);

			XElement existingElement = root.Element("MiscSettings");
			if (existingElement != null)
				existingElement.ReplaceWith(section);
			else
				root.Add(section);
		}
	}
}
