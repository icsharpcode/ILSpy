using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Tests.Helpers;

using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class ChecksumTest
	{
		public static readonly string TestDir = Path.GetFullPath(Path.Combine(Tester.TestCasePath, "../../ILSpy-tests/CheckSumTesting"));


		void DeleteDirectory(String sPath, bool root)
		{
			foreach (String file in Directory.GetFiles(sPath))
			{
				FileInfo fi = new FileInfo(file);
				fi.Delete();
			}

			foreach (String subfolder in Directory.GetDirectories(sPath))
			{
				DeleteDirectory(subfolder, false);
			}

			if (!root)
			{
				Directory.Delete(sPath);
			}
		}

		[SetUp]
		public void Cleanup()
		{
			try
			{
				if (Directory.Exists(TestDir))
				{
					DeleteDirectory(TestDir, true);
				}
			}
			catch (Exception)
			{
			}
		}

		void AssertEqual(Tuple<String, String> tuple)
		{
			Assert.AreEqual(tuple.Item1, tuple.Item2);
		}

		void AssertNotEqual(Tuple<String, String> tuple)
		{
			Assert.AreNotEqual(tuple.Item1, tuple.Item2);
		}

		const string net472 = "net472";
		const string netcoreapp = "netcoreapp3.1";

		[Test]
		public void Basic_Net472()
		{
			AssertEqual(GenerateCheckumProject(CallerName(), net472, (writer) => { } ));
		}

		[Test]
		public void Basic_NetCore()
		{
			AssertEqual(GenerateCheckumProject(CallerName(), netcoreapp, (writer) => { }));
		}

		public static string CallerName([CallerMemberName] string callerName = "")
		{
			return callerName;
		}

		[Test]
		public void StringDataChanges_Net472()
		{
			StringDataChanges(net472);
		}

		[Test]
		public void StringDataChanges_NetCore()
		{
			StringDataChanges(netcoreapp);
		}

		public void StringDataChanges(string framework)
		{
			// To do: Include strings into hash logic
			AssertEqual(GenerateCheckumProject(CallerName(), framework, 
				(w) => {
					w.WriteLine(
					@"public class MyTestClass{ 
						void function1()
						{
							Console.WriteLine(""Test1"");
						}
					};");
				},

				(w) => {
					w.WriteLine(
						@"public class MyTestClass{ 
						void function1()
						{
							Console.WriteLine(""Test2"");
						}
					};");
				}
			));
		}

		[Test]
		public void CodeDataChanges_Net472()
		{
			CodeDataChanges(net472);
		}

		[Test]
		public void CodeDataChanges_NetCore()
		{
			CodeDataChanges(netcoreapp);
		}

		public void CodeDataChanges(string framework)
		{
			AssertNotEqual(GenerateCheckumProject(CallerName(), framework,
				(w) => {
					w.WriteLine(
					@"public class MyTestClass{ 
						void function1()
						{
							Console.WriteLine(""Test1"");
						}
					};");
				},

				(w) => {
					w.WriteLine(
						@"public class MyTestClass{ 
						void function1()
						{
							Console.WriteLine(""Test1"");
							Console.WriteLine(""Test1"");
						}
					};");
				}
			));
		}


		public Tuple<String, String> GenerateCheckumProject(string title, string targetFramework, 
			Action<PlainTextOutput> v1gen,
			Action<PlainTextOutput> v2gen = null
		)
		{
			if (v2gen == null)
			{
				v2gen = v1gen;
			}
			
			string dirV1 = null, dirV2 = null;
			string outext;

			if (targetFramework.StartsWith("netcore"))
			{
				outext = ".dll";
			}
			else
			{ 
				outext = ".exe";
			}

			foreach (bool first in new[] { true, false })
			{
				string dirName = $"{title}_{targetFramework}_" + ((first) ? "v1" : "v2");
				string dir = Path.Combine(TestDir, dirName);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				if (first)
				{
					dirV1 = dir;
				}
				else
				{ 
					dirV2 = dir;
				}

				string csproj = Path.Combine(dir, "test.csproj");
				string outdir = Path.Combine(dir, "bin");
				using (XmlTextWriter xml = new XmlTextWriter(csproj, Encoding.UTF8))
				{
					xml.Formatting = Formatting.Indented;
					xml.WriteStartElement("Project");
					xml.WriteAttributeString("Sdk", "Microsoft.NET.Sdk");

					PlaceIntoTag("PropertyGroup", xml, () => {
						xml.WriteElementString("OutputType", "Exe");
						xml.WriteElementString("TargetFramework", targetFramework);

						// This should improve to produce identical dll, but does not work in all cases.
						// Use without this code, just so we would compare binary different assemblies
						//xml.WriteElementString("Deterministic", "true");
						//xml.WriteElementString("PathMap",
						//	"$([System.IO.Path]::GetFullPath('$(MSBuildThisFileDirectory)'))=./"
						//);

						xml.WriteElementString("SignAssembly", "true");
						xml.WriteElementString("AssemblyOriginatorKeyFile", "../../../ICSharpCode.Decompiler/ICSharpCode.Decompiler.snk");
						xml.WriteElementString("DelaySign", "false");

						xml.WriteElementString("AppendTargetFrameworkToOutputPath", "false");
						xml.WriteElementString("OutputPath", "bin");
					});
				}

				using (var writer = new StreamWriter(Path.Combine(dir, "testmain.cs")))
				{
					var cscode = new PlainTextOutput(writer);
					cscode.WriteLine(@"
using System;

class Program
{
	static void Main(string[] args)
	{
		Console.WriteLine(""Hello World!"");
	}
}
");
					if (first)
						v1gen(cscode);
					else
						v2gen(cscode);

				}

				RoundtripAssembly.Compile(csproj, outdir);
			}

			string binV1 = Path.Combine(dirV1, "bin", "test" + outext);
			string binV2 = Path.Combine(dirV2, "bin", "test" + outext);

			return new Tuple<string, string>(AssemblyCheckumCalculator.Calculate(binV1), AssemblyCheckumCalculator.Calculate(binV2));
		}

		static void PlaceIntoTag(string tagName, XmlTextWriter xml, Action content)
		{
			xml.WriteStartElement(tagName);
			try
			{
				content();
			}
			finally
			{
				xml.WriteEndElement();
			}
		}

	}
}
