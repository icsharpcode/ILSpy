// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.AI
{
	internal static class AIEntityDecompilation
	{
		public static CSharpDecompiler CreateDecompiler(IEntity entity)
		{
			MetadataFile module = entity.ParentModule?.MetadataFile
				?? throw new InvalidOperationException("The selected symbol has no decompilable module.");
			return new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());
		}

		// Re-resolve the entity from the new decompiler's type system using its metadata token.
		// The entity was resolved from a different decompiler instance, so we cannot pass it directly
		// to ContextBuilder.Build — it validates that entity.ParentModule equals
		// decompiler.TypeSystem.MainModule via ReferenceEquals, which would fail.
		public static IEntity? ResolveEntity(IEntity entity, CSharpDecompiler decompiler)
		{
			var token = entity.MetadataToken;
			if (token.IsNil)
				return null;
			return token.Kind switch {
				HandleKind.TypeDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((TypeDefinitionHandle)token),
				HandleKind.MethodDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((MethodDefinitionHandle)token),
				HandleKind.FieldDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((FieldDefinitionHandle)token),
				HandleKind.PropertyDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((PropertyDefinitionHandle)token),
				HandleKind.EventDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((EventDefinitionHandle)token),
				_ => null
			};
		}
	}
}
