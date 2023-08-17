using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler;

using ICSharpCode.Decompiler.TypeSystem;
using BenchmarkDotNet.Running;

namespace ILSpy.Benchmarks
{
	[ShortRunJob]
	// [NativeMemoryProfiler] // needs elevation, see also https://benchmarkdotnet.org/articles/configs/diagnosers.html#sample-intronativememory
	[MemoryDiagnoser]
	public class DecompilerBenchmarks
	{
		/*
		|              Method |     Mean |    Error |   StdDev |      Gen0 |     Gen1 |     Gen2 | Allocated |
		|-------------------- |---------:|---------:|---------:|----------:|---------:|---------:|----------:|
		| SimpleDecompilation | 46.69 ms | 26.13 ms | 1.432 ms | 2090.9091 | 909.0909 | 272.7273 |  15.33 MB | 
		*/
		[Benchmark]
		public void SimpleDecompilation()
		{
			string testAssemblyPath = typeof(CSharpDecompiler).Assembly.Location;
			var decompiler = new CSharpDecompiler(testAssemblyPath, new DecompilerSettings());

			// ICSharpCode.Decompiler.Util.Empty<T> -> translates to `n, where n is the # of generic parameters
			var nameOfGenericType = new FullTypeName("ICSharpCode.Decompiler.Util.Empty`1");
			string decompiledStuff = decompiler.DecompileTypeAsString(nameOfGenericType);

			//var nameOfUniResolver = new FullTypeName("ICSharpCode.Decompiler.Metadata.UniversalAssemblyResolver");
			//ITypeDefinition typeInfo = decompiler.TypeSystem.FindType(nameOfUniResolver).GetDefinition();
			//var tokenOfFirstMethod = typeInfo.Methods.First().MetadataToken;
			//decompiledStuff = decompiler.DecompileAsString(tokenOfFirstMethod);
		}
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}
