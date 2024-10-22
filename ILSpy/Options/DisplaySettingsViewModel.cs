using ICSharpCode.ILSpyX.Settings;
using System.Windows.Media;
using System.Xml.Linq;
using System;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using TomsToolbox.Wpf;
using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.Options
{
	[ExportOptionPage(Order = 20)]
	[NonShared]
	public class DisplaySettingsViewModel : ObservableObject, IOptionPage
	{
		private DisplaySettings settings = new();
		private FontFamily[] fontFamilies;
		private SessionSettings sessionSettings;

		public DisplaySettingsViewModel()
		{
			fontFamilies = [settings.SelectedFont];

			Task.Run(FontLoader).ContinueWith(continuation => {
				FontFamilies = continuation.Result;
				if (continuation.Exception == null)
					return;
				foreach (var ex in continuation.Exception.InnerExceptions)
				{
					MessageBox.Show(ex.ToString());
				}
			});
		}

		public string Title => Properties.Resources.Display;

		public DisplaySettings Settings {
			get => settings;
			set => SetProperty(ref settings, value);
		}

		public SessionSettings SessionSettings {
			get => sessionSettings;
			set => SetProperty(ref sessionSettings, value);
		}

		public FontFamily[] FontFamilies {
			get => fontFamilies;
			set => SetProperty(ref fontFamilies, value);
		}

		public int[] FontSizes { get; } = Enumerable.Range(6, 24 - 6 + 1).ToArray();

		public void Load(SettingsSnapshot snapshot)
		{
			Settings = snapshot.GetSettings<DisplaySettings>();
			SessionSettings = snapshot.GetSettings<SessionSettings>();
		}

		static bool IsSymbolFont(FontFamily fontFamily)
		{
			foreach (var tf in fontFamily.GetTypefaces())
			{
				try
				{
					if (tf.TryGetGlyphTypeface(out GlyphTypeface glyph))
						return glyph.Symbol;
				}
				catch (Exception)
				{
					return true;
				}
			}
			return false;
		}

		static FontFamily[] FontLoader()
		{
			return Fonts.SystemFontFamilies
				.Where(ff => !IsSymbolFont(ff))
				.OrderBy(ff => ff.Source)
				.ToArray();
		}

		public void LoadDefaults()
		{
			Settings.LoadFromXml(new XElement("empty"));
			SessionSettings.Theme = ThemeManager.Current.DefaultTheme;
		}
	}
}
