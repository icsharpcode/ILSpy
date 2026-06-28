using System;

internal class MemberInitializerEvents
{
	private event EventHandler Changed = Handler;

	public MemberInitializerEvents()
	{
		this.Changed?.Invoke(this, EventArgs.Empty);
	}

	public MemberInitializerEvents(int value)
	{
		this.Changed?.Invoke(this, EventArgs.Empty);
	}

	private static void Handler(object sender, EventArgs e)
	{
	}

	private static void Main()
	{
		new MemberInitializerEvents();
		new MemberInitializerEvents(3);
	}
}
