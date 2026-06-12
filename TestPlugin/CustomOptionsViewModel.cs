// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.Composition;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Options;

namespace TestPlugin
{
	// The Options host renders an [ExportOptionPage] viewmodel by resolving its view through the
	// app-wide ViewLocator (SomeViewModel -> SomeView, searched across every loaded assembly,
	// including plugins). So this viewmodel pairs with CustomOptionsView.axaml by name.
	[ExportOptionPage(Order = 0)]
	[Shared]
	public sealed partial class CustomOptionsViewModel : ObservableObject, IOptionPage
	{
		public string Title => "TestPlugin";

		[ObservableProperty]
		private Options options = null!;

		public void Load(SettingsService settings)
		{
			Options = settings.GetSettings<Options>();
		}

		public void LoadDefaults()
		{
			Options.LoadFromXml(new XElement("dummy"));
		}
	}

	public sealed partial class Options : ObservableObject, ISettingsSection
	{
		static readonly XNamespace ns = "http://www.ilspy.net/testplugin";

		[ObservableProperty]
		private bool uselessOption1;

		[ObservableProperty]
		private double uselessOption2;

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
