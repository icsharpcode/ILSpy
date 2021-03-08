using System;
using System.IO;
using System.Security.Cryptography;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	public class AssemblyCheckumCalculator
	{
		/// <summary>
		/// Calculates checksum over assembly.
		/// </summary>
		/// <param name="path">Assembly path</param>
		/// <returns>Unique string</returns>
		public static string Calculate(string path)
		{
			var module = new PEFile(path);
			string dir = Path.GetDirectoryName(path);
			var resolver = new LocalAssemblyResolver(path, dir, module.Reader.DetectTargetFrameworkId());
			resolver.AddSearchDirectory(dir);
			resolver.RemoveSearchDirectory(".");

			var settings = new DecompilerSettings();
			settings.checksumCalc.EnableChecksumCalculation(HashAlgorithmName.SHA256);
			settings.ProduceSourceCode = false;

			var decompiler = new WholeProjectDecompiler(settings, Guid.Empty, resolver, resolver, debugInfoProvider: null);

			// not needed, but just in case someone will write it
			string decompiledDir = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
			decompiler.DecompileProject(module, decompiledDir);

			return settings.checksumCalc.GetHashString();
		}
	}
}
