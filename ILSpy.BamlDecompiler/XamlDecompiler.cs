/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Xml.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.BamlDecompiler.Baml;
using ILSpy.BamlDecompiler.Rewrite;

namespace ILSpy.BamlDecompiler
{
	public class XamlDecompiler
	{
		static readonly IRewritePass[] rewritePasses = new IRewritePass[] {
			new XClassRewritePass(),
			new MarkupExtensionRewritePass(),
			new AttributeRewritePass(),
			new ConnectionIdRewritePass(),
			new DocumentRewritePass(),
		};

		private BamlDecompilerTypeSystem typeSystem;
		private BamlDecompilerSettings settings;
		private MetadataModule module;

		public BamlDecompilerSettings Settings {
			get { return settings; }
			set { settings = value; }
		}

		public CancellationToken CancellationToken { get; set; }

		public XamlDecompiler(string fileName, BamlDecompilerSettings settings)
			: this(CreateTypeSystemFromFile(fileName, settings), settings)
		{
		}

		public XamlDecompiler(string fileName, IAssemblyResolver assemblyResolver, BamlDecompilerSettings settings)
			: this(LoadPEFile(fileName, settings), assemblyResolver, settings)
		{
		}

		public XamlDecompiler(PEFile module, IAssemblyResolver assemblyResolver, BamlDecompilerSettings settings)
			: this(new BamlDecompilerTypeSystem(module, assemblyResolver), settings)
		{
		}

		internal XamlDecompiler(BamlDecompilerTypeSystem typeSystem, BamlDecompilerSettings settings)
		{
			this.typeSystem = typeSystem ?? throw new ArgumentNullException(nameof(typeSystem));
			this.settings = settings;
			this.module = typeSystem.MainModule;
			if (module.TypeSystemOptions.HasFlag(TypeSystemOptions.Uncached))
				throw new ArgumentException("Cannot use an uncached type system in the decompiler.");
		}

		static PEFile LoadPEFile(string fileName, BamlDecompilerSettings settings)
		{
			return new PEFile(
				fileName,
				new FileStream(fileName, FileMode.Open, FileAccess.Read),
				streamOptions: PEStreamOptions.PrefetchEntireImage,
				metadataOptions: MetadataReaderOptions.None
			);
		}

		static BamlDecompilerTypeSystem CreateTypeSystemFromFile(string fileName, BamlDecompilerSettings settings)
		{
			var file = LoadPEFile(fileName, settings);
			var resolver = new UniversalAssemblyResolver(fileName, settings.ThrowOnAssemblyResolveErrors,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack(),
				PEStreamOptions.PrefetchMetadata,
				MetadataReaderOptions.None);
			return new BamlDecompilerTypeSystem(file, resolver);
		}

		public BamlDecompilationResult Decompile(Stream stream)
		{
			var ct = CancellationToken;
			var document = BamlReader.ReadDocument(stream, ct);
			var ctx = XamlContext.Construct(typeSystem, document, ct, settings);

			var handler = HandlerMap.LookupHandler(ctx.RootNode.Type);
			var elem = handler.Translate(ctx, ctx.RootNode, null);

			var xaml = new XDocument();
			xaml.Add(elem.Xaml.Element);

			foreach (var pass in rewritePasses)
			{
				ct.ThrowIfCancellationRequested();
				pass.Run(ctx, xaml);
			}

			var assemblyReferences = ctx.Baml.AssemblyIdMap.Select(a => a.Value.AssemblyFullName);
			var typeName = ctx.XClassNames.FirstOrDefault() is string s ? (FullTypeName?)new FullTypeName(s) : null;
			return new BamlDecompilationResult(xaml, typeName, assemblyReferences);
		}
	}
}