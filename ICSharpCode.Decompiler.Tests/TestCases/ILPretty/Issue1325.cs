using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

[assembly: Embedded]
[assembly: AssemblyInformationalVersion("1.0.0")]
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyFileVersion("1.0.0.0")]

namespace Issue1325
{
	[StandardModule]
	internal sealed class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
		}
		public static void TestCode(Test t, int i)
		{
			string str = "";
			str += File.ReadAllText("Test.txt");
			str += "asdf";
			t.set_Parameterized(i, str);
			t.Unparameterized = str + "asdf";
		}
	}

	internal class Test
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private string _Unparameterized;
		public string Parameterized {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		public string Unparameterized {
			get;
			set;
		}
	}
}
namespace Microsoft.VisualBasic
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class, Inherited = false)]
	[CompilerGenerated]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Embedded]
	internal sealed class Embedded : Attribute
	{
	}
}
namespace Microsoft.VisualBasic.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[CompilerGenerated]
	[Embedded]
	internal sealed class StandardModuleAttribute : Attribute
	{
	}
}
