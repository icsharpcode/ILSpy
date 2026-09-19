using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public class QualifiedMethodGroup
	{
		public event EventHandler Changed;

		public int Value;

		public void Subscribe()
		{
			Changed += OnChanged;
			Value = 1;
		}

		private void OnChanged(object sender, EventArgs e)
		{
		}
	}
}
