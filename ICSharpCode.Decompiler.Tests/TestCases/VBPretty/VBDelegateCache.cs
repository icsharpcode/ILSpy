using System;
using System.Runtime.CompilerServices;

public class VBDelegateCache
{
	public Action MakeAction()
	{
		return [SpecialName] () => {
			Console.WriteLine("fixed");
		};
	}
}
