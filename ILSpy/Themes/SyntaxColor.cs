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
		color.Foreground = Foreground is { } foreground ? new SimpleHighlightingBrush(foreground) : null;
		color.Background = Background is { } background ? new SimpleHighlightingBrush(background) : null;
		color.FontWeight = FontWeight ?? FontWeights.Normal;
		color.FontStyle = FontStyle ?? FontStyles.Normal;
	}

	public static void ResetColor(HighlightingColor color)
	{
		color.Foreground = null;
		color.Background = null;
		color.FontWeight = null;
		color.FontStyle = null;
	}
}
