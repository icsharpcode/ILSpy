// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;
using System.Xml.Linq;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Util;

using TomsToolbox.Wpf;
using TomsToolbox.Wpf.Composition.AttributedModel;

namespace TestPlugin
{
	[DataTemplate(typeof(CustomOptionsViewModel))]
	[NonShared]
	partial class CustomOptionPage
	{
		public CustomOptionPage()
		{
			InitializeComponent();
		}
	}

	[ExportOptionPage(Order = 0)]
	[NonShared]
	class CustomOptionsViewModel : ObservableObject, IOptionPage
	{
		private Options options;

		public string Title => "TestPlugin";

		public Options Options {
			get => options;
			set => SetProperty(ref options, value);
		}

		public void Load(SettingsSnapshot snapshot)
		{
			this.Options = snapshot.GetSettings<Options>();
		}

		public void LoadDefaults()
		{
			Options.LoadFromXml(new XElement("dummy"));
		}
	}

	class Options : ObservableObject, ISettingsSection
	{
		static readonly XNamespace ns = "http://www.ilspy.net/testplugin";

		bool uselessOption1;

		public bool UselessOption1 {
			get => uselessOption1;
			set => SetProperty(ref uselessOption1, value);
		}

		double uselessOption2;

		public double UselessOption2 {
			get => uselessOption2;
			set => SetProperty(ref uselessOption2, value);
		}

		public XName SectionName { get; } = ns + "CustomOptions";

		public void LoadFromXml(XElement e)
		{
			UselessOption1 = (bool?)e.Attribute("useless1") ?? false;
			UselessOption2 = (double?)e.Attribute("useless2") ?? 50.0;
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);

			section.SetAttributeValue("useless1", UselessOption1);
			section.SetAttributeValue("useless2", UselessOption2);

			return section;
		}
	}
}