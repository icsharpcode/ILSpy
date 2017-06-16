using System.Windows.Controls;
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
			this.DataContext = MiscSettingsInstance.Current.LoadMiscSettings(settings);
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
