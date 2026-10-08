using System;
using System.Runtime.CompilerServices;

public class Issue2199
{
	private class Item
	{
		public string Text;
	}

	public event EventHandler Click;

	public event EventHandler DoubleClick;

	public void Register(string value)
	{
		Item item = new Item();
		item.Text = value.Trim();
#if EXPECTED_OUTPUT
		Click += [SpecialName] () => {
			Console.WriteLine(item.Text);
		};
		DoubleClick += [SpecialName] () => {
			Console.WriteLine(item.Text);
		};
#else
		Click += delegate {
			Console.WriteLine(item.Text);
		};
		DoubleClick += delegate {
			Console.WriteLine(item.Text);
		};
#endif
	}
}
