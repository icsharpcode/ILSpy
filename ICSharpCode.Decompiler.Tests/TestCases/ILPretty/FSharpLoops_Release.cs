
// C:\Users\Siegfried\Documents\Visual Studio 2017\Projects\ConsoleApp13\ConsoleApplication1\bin\Release\ConsoleApplication1.exe
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
using System.Runtime.Versioning;

[assembly: FSharpInterfaceDataVersion(2, 0, 0)]
[assembly: TargetFramework(".NETFramework,Version=v4.6.1", FrameworkDisplayName = ".NET Framework 4.6.1")]
[assembly: AssemblyTitle("ConsoleApplication1")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("ConsoleApplication1")]
[assembly: AssemblyCopyright("Copyright ©  2017")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("e0674ff5-5e8f-4d4e-a88f-e447192454c7")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.None)]
[assembly: AssemblyVersion("1.0.0.0")]
[CompilationMapping(SourceConstructFlags.Module)]
public static class Program
{
	[Serializable]
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	[CompilationMapping(SourceConstructFlags.Closure)]
	internal sealed class disposable@3 : IDisposable
	{
		public disposable@3()
		{
			((object)this)..ctor();
		}

		private void System-IDisposable-Dispose()
		{
		}

		void IDisposable.Dispose()
		{
			//ILSpy generated this explicit interface implementation from .override directive in System-IDisposable-Dispose
			this.System-IDisposable-Dispose();
		}
	}

	[Serializable]
	[StructLayout(LayoutKind.Auto, CharSet = CharSet.Auto)]
	[CompilationMapping(SourceConstructFlags.Closure)]
	internal sealed class getSeq@5 : GeneratedSequenceBase<int>
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		[DebuggerNonUserCode]
		public int pc = pc;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		[DebuggerNonUserCode]
		public int current = current;

		public getSeq@5(int pc, int current)
		{
		}

		public override int GenerateNext(ref IEnumerable<int> next)
		{
			switch (this.pc)
			{
			default:
				this.pc = 1;
				this.current = 1;
				return 1;
			case 1:
				this.pc = 2;
				break;
			case 2:
				break;
			}
			this.current = 0;
			return 0;
		}

		public override void Close()
		{
			this.pc = 2;
		}

		public override bool get_CheckClose()
		{
			switch (this.pc)
			{
			default:
				return false;
			case 0:
			case 2:
				return false;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		public override int get_LastGenerated()
		{
			return this.current;
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		public override IEnumerator<int> GetFreshEnumerator()
		{
			return new getSeq@5(0, 0);
		}
	}

	public static IDisposable disposable()
	{
		return new disposable@3();
	}

	public static IEnumerable<int> getSeq()
	{
		return new getSeq@5(0, 0);
	}

	public static FSharpList<int> getList()
	{
		return FSharpList<int>.Cons(1, FSharpList<int>.Empty);
	}

	public static int[] getArray()
	{
		return new int[1]
		{
			1
		};
	}

	[EntryPoint]
	public static int main(string[] argv)
	{
		IDisposable disposable = default(IDisposable);
		using (Program.disposable())
		{
			Console.WriteLine("Hello 1");
			disposable = Program.disposable();
		}
		using (disposable)
		{
			IEnumerable<int> seq = Program.getSeq();
			using (IEnumerator<int> enumerator = seq.GetEnumerator())
			{
				while (true)
				{
					if (!enumerator.MoveNext())
						break;
					Console.WriteLine(enumerator.Current);
				}
			}
			FSharpList<int> fSharpList = FSharpList<int>.Cons(1, FSharpList<int>.Empty);
			FSharpList<int> tailOrNull = fSharpList.TailOrNull;
			while (true)
			{
				if (tailOrNull == null)
					break;
				int j = fSharpList.HeadOrDefault;
				Console.WriteLine(j);
				fSharpList = tailOrNull;
				tailOrNull = fSharpList.TailOrNull;
			}
			int[] array = new int[1]
			{
				1
			};
			for (int j = 0; j < array.Length; j++)
			{
				Console.WriteLine(array[j]);
			}
			return 0;
		}
	}
}
namespace <StartupCode$ConsoleApplication1>
{
	internal static class $Program
	{
	}
	internal static class $AssemblyInfo
	{
	}
}
namespace <StartupCode$ConsoleApplication1>.$.NETFramework,Version=v4.6.1
{
	internal static class AssemblyAttributes
	{
	}
}
