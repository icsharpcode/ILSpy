using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	public class ComInterop
	{
		public static void Main()
		{
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetMethod("MyMethod1")));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetProperty("MyProperty1").GetMethod));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetMethod("MyMethod2")));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetProperty("MyProperty2").GetMethod));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetProperty("MyProperty2").SetMethod));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetEvent("MyEvent1").AddMethod));
			Console.WriteLine(Marshal.GetComSlotForMethodInfo(typeof(IMixedPropsAndMethods).GetEvent("MyEvent1").RemoveMethod));
		}


		[Guid("761618B8-3994-449A-A96B-F1FF2795EA85")]
		[ComImport]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IMixedPropsAndMethods
		{
			int MyMethod1();

			int MyProperty1 { get; }

			int MyMethod2();

			int MyProperty2 { get; set; }

			event EventHandler MyEvent1;

			int MyMethod3();
		}
	}
}
