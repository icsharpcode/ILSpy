using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		
		[Test]
		public void Test1()
		{
			GenerateCheckumProject("basic", "net472");
			GenerateCheckumProject("basic", "netcoreapp3.1");
		}

		public void GenerateCheckumProject(string title, string targetFramework)
		{
			foreach (bool first in new[] { true, false })
			{
				string dirName = $"{title}_{targetFramework}_" + ((first) ? "v1" : "v2");
				string dir = Path.Combine(TestDir, dirName);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
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
						xml.WriteElementString("Deterministic", "true");
						xml.WriteElementString("PathMap", "\"$(EnlistmentRoot)\"=C:\\ILSpy\\");
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
				}

				RoundtripAssembly.Compile(csproj, outdir);
			}
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
