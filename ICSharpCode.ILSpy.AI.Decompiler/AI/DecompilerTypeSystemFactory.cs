// Copyright (c) 2026 Dr. Masroor Ehsan

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.AI.Decompiler
{
	internal static class DecompilerTypeSystemFactory
	{
		internal static ICompilation Create(MetadataFile module)
		{
			var settings = new DecompilerSettings();
			var resolver = new UniversalAssemblyResolver(module.FileName, settings.ThrowOnAssemblyResolveErrors, module.DetectTargetFrameworkId());
			return new DecompilerTypeSystem(module, resolver, settings);
		}
	}
}
