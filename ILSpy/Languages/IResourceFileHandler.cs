using System;
using System.IO;

namespace ICSharpCode.ILSpy
{
	public interface IResourceFileHandler
	{
		string EntryType { get; }
		bool CanHandle(string name, DecompilationOptions options);
		string WriteResourceToFile(LoadedAssembly assembly, string fileName, Stream stream, DecompilationOptions options);
	}
}
