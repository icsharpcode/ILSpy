using System.Windows;
using System.Windows.Controls;

namespace ILSpy.BamlDecompiler.Tests.Cases
{
	public class CustomControl : ContentControl
	{
		public static string SimpleProperty = "Hi!";

		public static readonly DependencyProperty CustomNameProperty = DependencyProperty.RegisterAttached("CustomName", typeof(string), typeof(CustomControl));
		
		public static string GetCustomName(DependencyObject target)
		{
			return (string)target.GetValue(CustomNameProperty);
		}
		
		public static void SetCustomName(DependencyObject target, string value)
		{
			target.SetValue(CustomNameProperty, value);
		}
	}
}
