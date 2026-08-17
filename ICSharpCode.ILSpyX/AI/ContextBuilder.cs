// Copyright (c) 2026 Masroor
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class ContextBuilder
	{
		readonly AISettings settings;

		public ContextBuilder(AISettings settings)
		{
			this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
		}

		public DecompilationContext Build(IEntity entity, CSharpDecompiler decompiler)
		{
			if (entity is null)
				throw new ArgumentNullException(nameof(entity));
			if (decompiler is null)
				throw new ArgumentNullException(nameof(decompiler));
			if (entity.ParentModule is null)
				throw new ArgumentException("The entity must belong to a module.", nameof(entity));
			if (entity.MetadataToken.IsNil)
				throw new ArgumentException("The entity has no metadata token.", nameof(entity));
			if (!ReferenceEquals(entity.ParentModule, decompiler.TypeSystem.MainModule))
				throw new ArgumentException("The entity does not belong to the decompiler module.", nameof(entity));
			if (!IsSupportedHandle(entity.MetadataToken.Kind))
				throw new ArgumentException($"Metadata handle kind '{entity.MetadataToken.Kind}' is not supported.", nameof(entity));

			var context = new DecompilationContext {
				DecompiledCSharp = Decompile(entity, decompiler),
				FullyQualifiedName = entity.FullName,
				AssemblyName = entity.ParentModule.AssemblyName,
				TargetFramework = DetectTargetFramework(entity.ParentModule),
				Attributes = entity.GetAttributes().Select(attribute => attribute.AttributeType.FullName).ToArray(),
				ImplementedInterfaces = GetImplementedInterfaces(entity),
				StringLiterals = Array.Empty<string>(),
				Callers = Array.Empty<string>(),
				Callees = Array.Empty<string>(),
				IL = null
			};
			return EnforceBudget(context);
		}

		static bool IsSupportedHandle(HandleKind kind)
		{
			return kind is HandleKind.TypeDefinition
				or HandleKind.MethodDefinition
				or HandleKind.FieldDefinition
				or HandleKind.PropertyDefinition
				or HandleKind.EventDefinition;
		}

		static string Decompile(IEntity entity, CSharpDecompiler decompiler)
		{
			if (entity is ITypeDefinition type)
				return decompiler.DecompileTypeAsString(type.FullTypeName);
			return decompiler.DecompileAsString(entity.MetadataToken);
		}

		static IReadOnlyList<string> GetImplementedInterfaces(IEntity entity)
		{
			if (entity is not ITypeDefinition type)
				return Array.Empty<string>();
			return type.DirectBaseTypes.Where(baseType => baseType.Kind == TypeKind.Interface).Select(baseType => baseType.FullName).ToArray();
		}

		static string DetectTargetFramework(IModule module)
		{
			MetadataFile? metadataFile = module.MetadataFile;
			return metadataFile is null ? string.Empty : metadataFile.DetectTargetFrameworkId();
		}

		DecompilationContext EnforceBudget(DecompilationContext context)
		{
			int budget = settings.MaxContextTokens;
			if (budget <= 0)
				return EmptyContext();

			context = WithTokenCount(context);
			if (context.ApproximateTokenCount <= budget)
				return context;

			context = WithTokenCount(context with { IL = null });
			if (context.ApproximateTokenCount <= budget)
				return context;

			context = WithTokenCount(context with { Callees = Array.Empty<string>() });
			if (context.ApproximateTokenCount <= budget)
				return context;

			context = WithTokenCount(context with { Callers = Array.Empty<string>() });
			if (context.ApproximateTokenCount <= budget)
				return context;

			context = WithTokenCount(context with { StringLiterals = Array.Empty<string>() });
			if (context.ApproximateTokenCount <= budget)
				return context;

			if (TryFitCode(context, budget, out DecompilationContext fitted))
				return fitted;

			context = context with {
				Attributes = Array.Empty<string>(),
				ImplementedInterfaces = Array.Empty<string>()
			};
			if (TryFitCode(context, budget, out fitted))
				return fitted;

			context = context with { TargetFramework = string.Empty };
			if (TryFitCode(context, budget, out fitted))
				return fitted;

			context = context with { AssemblyName = string.Empty };
			if (TryFitCode(context, budget, out fitted))
				return fitted;

			context = context with { FullyQualifiedName = string.Empty };
			if (TryFitCode(context, budget, out fitted))
				return fitted;

			return EmptyContext();
		}

		static bool TryFitCode(DecompilationContext context, int budget, out DecompilationContext fitted)
		{
			int low = 0;
			int high = context.DecompiledCSharp.Length;
			DecompilationContext? best = null;
			while (low <= high)
			{
				int middle = low + (high - low) / 2;
				int length = GetUnicodeSafePrefixLength(context.DecompiledCSharp, middle);
				DecompilationContext candidate = WithTokenCount(context with { DecompiledCSharp = context.DecompiledCSharp[..length] });
				if (candidate.ApproximateTokenCount <= budget)
				{
					best = candidate;
					low = middle + 1;
				}
				else
				{
					high = middle - 1;
				}
			}

			fitted = best ?? context;
			return best is not null;
		}

		static DecompilationContext EmptyContext()
		{
			return new DecompilationContext { ApproximateTokenCount = 0 };
		}

		static DecompilationContext WithTokenCount(DecompilationContext context)
		{
			return context with { ApproximateTokenCount = TokenCounter.CountTokens(context.ToMarkdown(), true) };
		}

		internal static int GetUnicodeSafePrefixLength(string text, int length)
		{
			if (length > 0 && length < text.Length && char.IsHighSurrogate(text[length - 1]) && char.IsLowSurrogate(text[length]))
				return length - 1;
			return length;
		}

	}
}
