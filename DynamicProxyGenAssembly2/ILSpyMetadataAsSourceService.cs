using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.MetadataAsSource;
using Microsoft.CodeAnalysis.Text;
using Mono.Cecil;

namespace DynamicProxyGenAssembly2
{
	[ExportLanguageServiceFactory(typeof(IMetadataAsSourceService), LanguageNames.CSharp, layer: ServiceLayer.Host)]
	public class ILSpyMetadataAsSourceServiceFactory : ILanguageServiceFactory
	{
		public ILanguageService CreateLanguageService(HostLanguageServices languageServices)
		{
			return new ILSpyMetadataAsSourceService();
		}
	}

	public class ILSpyMetadataAsSourceService : IMetadataAsSourceService
	{
		
		public async Task<Document> AddSourceToAsync(Document document, ISymbol symbol, CancellationToken cancellationToken = default(CancellationToken))
		{
			var assembly = symbol.ContainingAssembly;
			var assemblies = document.Project.MetadataReferences.OfType<PortableExecutableReference>().Select(p => p.FilePath)
				.ToArray();
			var moduleDef = LoadAssembly(assemblies.First(p => Path.GetFileNameWithoutExtension(p) == assembly.Name), assemblies);
			var output = new PlainTextOutput();

			
			if (symbol is ITypeSymbol typeSymbol || (typeSymbol = symbol.ContainingType) != null)
			{
				var typeRef = moduleDef.GetType(typeSymbol.ContainingNamespace.ToDisplayString(), typeSymbol.MetadataName);
				new CSharpLanguage().DecompileType(typeRef.Resolve(), output, new DecompilationOptions());
			}

			return document.WithText(SourceText.From(output.ToString()));
		}

		private static ModuleDefinition LoadAssembly(string fileName, string[] allAssemblies)
		{
			ReaderParameters p = new ReaderParameters();
			var loader = new DefaultAssemblyResolver();
			foreach (var item in allAssemblies.Select(Path.GetDirectoryName).Distinct())
			{
				loader.AddSearchDirectory(item);
			}
			p.AssemblyResolver = loader;
			//p.AssemblyResolver = new MyAssemblyResolver(allAssemblies);

			var module = ModuleDefinition.ReadModule(fileName, p);

			if (true)
			{
				try
				{
					LoadSymbols(module, fileName);
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}
				catch (InvalidOperationException)
				{
					// ignore any errors during symbol loading
				}
			}
			return module;
		}

		//class MyAssemblyResolver : IAssemblyResolver
		//{
		//	private string[] allAssemblies;

		//	public MyAssemblyResolver(string[] allAssemblies)
		//	{
		//		this.allAssemblies = allAssemblies;
		//	}

		//	public AssemblyDefinition Resolve(string fullName)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	public AssemblyDefinition Resolve(AssemblyNameReference name)
		//	{
		//		return null;
		//	}

		//	public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
		//	{
		//		throw new NotImplementedException();
		//	}

		//	public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		//	{
		//		throw new NotImplementedException();
		//	}
		//}

		private static void LoadSymbols(ModuleDefinition module, string fileName)
		{
			if (!module.HasDebugHeader)
			{
				return;
			}
			byte[] headerBytes;
			var debugHeader = module.GetDebugHeader(out headerBytes);
			if (debugHeader.Type != 2)
			{
				// the debug type is not IMAGE_DEBUG_TYPE_CODEVIEW
				return;
			}
			if (debugHeader.MajorVersion != 0 || debugHeader.MinorVersion != 0)
			{
				// the PDB type is not compatible with PdbReaderProvider. It is probably a Portable PDB
				return;
			}

			// search for pdb in same directory as dll
			string pdbName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".pdb");
			if (File.Exists(pdbName))
			{
				using (Stream s = File.OpenRead(pdbName))
				{
					module.ReadSymbols(new Mono.Cecil.Pdb.PdbReaderProvider().GetSymbolReader(module, s));
				}
				return;
			}

			// TODO: use symbol cache, get symbols from microsoft
		}

		
	}
}
