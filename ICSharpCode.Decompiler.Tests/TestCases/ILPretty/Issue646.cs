using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	[StandardModule]
	internal sealed class Issue646
	{
		[STAThread]
		public static void Main()
		{
			var list = new List<string>();
			foreach (var item in list) {
				Debug.WriteLine(item);
			}
		}
	}
}
