// C:\Users\Siegfried\Documents\Visual Studio 2017\Projects\ConsoleApp13\ConsoleApplication1\bin\Debug\ConsoleApplication1.exe
// ConsoleApplication1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Global type: <Module>
// Entry point: Program.main
// Architecture: AnyCPU (32-bit preferred)
// Runtime: .NET 4.0

using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Core.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: FSharpInterfaceDataVersion(2, 0, 0)]
[assembly: AssemblyTitle("ConsoleApplication1")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ConsoleApplication1")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("e0674ff5-5e8f-4d4e-a88f-e447192454c7")]
[CompilationMapping(SourceConstructFlags.Module)]
public static class Program
{
	[Serializable]
	[CompilationMapping(SourceConstructFlags.Closure)]
	internal sealed class disposable_00403 : IDisposable
	{
		private void System_002DIDisposable_002DDispose()
		{
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in System-IDisposable-Dispose
			this.System_002DIDisposable_002DDispose();
		}
	}

	[Serializable]
	[CompilationMapping(SourceConstructFlags.Closure)]
	internal sealed class getSeq_00405 : GeneratedSequenceBase<int>
	{
		[DebuggerNonUserCode]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		public int pc = pc;

		[DebuggerNonUserCode]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		public int current = current;

		public getSeq_00405(int pc, int current)
		{
		}

		public override int GenerateNext(ref IEnumerable<int> next)
		{
			switch (pc)
			{
			default:
				pc = 1;
				current = 1;
				return 1;
			case 1:
				pc = 2;
				break;
			case 2:
				break;
			}
			current = 0;
			return 0;
		}

		public override void Close()
		{
			pc = 2;
		}

		public override bool get_CheckClose()
		{
			switch (pc)
			{
			default:
				return false;
			case 0:
			case 2:
				return false;
			}
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		public override int get_LastGenerated()
		{
			return current;
		}

		[DebuggerNonUserCode]
		[CompilerGenerated]
		public override IEnumerator<int> GetFreshEnumerator()
		{
			return new getSeq_00405(0, 0);
		}
	}

	public static IDisposable disposable()
	{
		return new disposable_00403();
	}

	public static IEnumerable<int> getSeq()
	{
		return new getSeq_00405(0, 0);
	}

	public static FSharpList<int> getList()
	{
		return FSharpList<int>.Cons(1, FSharpList<int>.Empty);
	}

	public static int[] getArray()
	{
		return new int[1] {
			1
		};
	}

	[EntryPoint]
	public static int main(string[] argv)
	{
		IDisposable disposable;
		using (Program.disposable())
		{
			Console.WriteLine("Hello 1");
			disposable = Program.disposable();
		}
		using (disposable)
		{
			IEnumerable<int> seq = getSeq();
			foreach (int item in seq)
			{
				Console.WriteLine(item);
			}
			FSharpList<int> fSharpList = getList();
			for (FSharpList<int> tailOrNull = fSharpList.TailOrNull; tailOrNull != null; tailOrNull = fSharpList.TailOrNull)
			{
				int headOrDefault = fSharpList.HeadOrDefault;
				Console.WriteLine(headOrDefault);
				fSharpList = tailOrNull;
			}
			int[] array = getArray();
			foreach (int value in array)
			{
				Console.WriteLine(value);
			}
			return 0;
		}
	}
}
namespace _003CStartupCode_0024ConsoleApplication1_003E
{
	internal static class _0024AssemblyInfo
	{
	}
	internal static class _0024Program
	{
	}
}
namespace _003CStartupCode_0024ConsoleApplication1_003E._0024.NETFramework_002CVersion_003Dv4._6._1
{
	internal static class AssemblyAttributes
	{
	}
}
