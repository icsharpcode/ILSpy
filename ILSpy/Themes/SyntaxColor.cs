#nullable enable

using System.Windows;
using System.Windows.Media;

using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.ILSpy.Themes;

public class SyntaxColor
{
	public Color? Foreground { get; set; }
	public Color? Background { get; set; }
	public FontWeight? FontWeight { get; set; }
	public FontStyle? FontStyle { get; set; }

	public void ApplyTo(HighlightingColor color)
	{
		if (Foreground is { } foreground)
			color.Foreground = new SimpleHighlightingBrush(foreground);

		if (Background is { } background)
			color.Background = new SimpleHighlightingBrush(background);

		if (FontWeight is { } fontWeight)
			color.FontWeight = fontWeight;

		if (FontStyle is { } fontStyle)
			color.FontStyle = fontStyle;
	}
}
