using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

public class QualifiedMethodGroup
{
	public int Value;

	public event EventHandler Changed;

	public void Subscribe()
	{
		this.Changed += new EventHandler(this.OnChanged);
		this.Value = 1;
	}

	private void OnChanged(object sender, EventArgs e)
	{
	}
}
