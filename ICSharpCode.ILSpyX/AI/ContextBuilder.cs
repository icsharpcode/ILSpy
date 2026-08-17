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
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;

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
				StringLiterals = GetStringLiterals(entity, decompiler),
				Callers = settings.SendCallGraph ? GetCallers(entity, decompiler.TypeSystem.MainModule) : Array.Empty<string>(),
				Callees = settings.SendCallGraph ? GetCallees(entity, decompiler.TypeSystem.MainModule) : Array.Empty<string>(),
				IL = settings.SendIL ? GetIL(entity) : null
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

		static IReadOnlyList<string> GetStringLiterals(IEntity entity, CSharpDecompiler decompiler)
		{
			var visitor = new StringLiteralVisitor();
			try
			{
				decompiler.Decompile(new[] { entity.MetadataToken }).AcceptVisitor(visitor);
			}
			catch (Exception) when (entity is not null)
			{
				return Array.Empty<string>();
			}
			return visitor.Values.Distinct(StringComparer.Ordinal).Take(20).ToArray();
		}

		static string? GetIL(IEntity entity)
		{
			if (entity is not IMethod method || method.ParentModule is not MetadataModule module)
				return null;
			try
			{
				var output = new PlainTextOutput();
				new MethodBodyDisassembler(output, default).Disassemble(module.MetadataFile, (MethodDefinitionHandle)method.MetadataToken);
				return output.ToString();
			}
			catch (BadImageFormatException)
			{
				return null;
			}
		}

		static IReadOnlyList<string> GetCallees(IEntity entity, IModule mainModule)
		{
			if (entity is not IMethod method || mainModule is not MetadataModule module)
				return Array.Empty<string>();
			return ScanMethodReferences(method, module).Select(member => member.FullName)
				.Distinct(StringComparer.Ordinal).Take(10).ToArray();
		}

		static IReadOnlyList<string> GetCallers(IEntity entity, IModule mainModule)
		{
			if (entity is not IMethod target || mainModule is not MetadataModule module)
				return Array.Empty<string>();
			var callers = new List<string>();
			foreach (var handle in module.MetadataFile.Metadata.MethodDefinitions)
			{
				IMethod? caller;
				try
				{
					caller = module.GetDefinition(handle) as IMethod;
				}
				catch (BadImageFormatException)
				{
					continue;
				}
				if (caller is null || caller.MetadataToken == target.MetadataToken)
					continue;
				if (ScanMethodReferences(caller, module).Any(member => member.MetadataToken == target.MetadataToken))
					callers.Add(caller.FullName);
				if (callers.Count == 10)
					break;
			}
			return callers;
		}

		static IEnumerable<IMember> ScanMethodReferences(IMethod method, MetadataModule module)
		{
			if (!method.HasBody || method.MetadataToken.Kind != HandleKind.MethodDefinition)
				return Array.Empty<IMember>();
			var definition = module.MetadataFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			if (definition.RelativeVirtualAddress == 0)
				return Array.Empty<IMember>();
			try
			{
				var body = module.MetadataFile.GetMethodBody(definition.RelativeVirtualAddress);
				var reader = body.GetILReader();
				var references = new List<IMember>();
				while (reader.RemainingBytes > 0)
				{
					ILOpCode opCode = reader.DecodeOpCode();
					if (opCode.GetOperandType() != OperandType.Method)
					{
						reader.SkipOperand(opCode);
						continue;
					}
					EntityHandle handle = MetadataTokenHelpers.EntityHandleOrNil(reader.ReadInt32());
					if (module.ResolveEntity(handle, default) is IMember member)
						references.Add(member.MemberDefinition);
				}
				return references;
			}
			catch (BadImageFormatException)
			{
				return Array.Empty<IMember>();
			}
		}

		sealed class StringLiteralVisitor : DepthFirstAstVisitor
		{
			public List<string> Values { get; } = new();

			public override void VisitPrimitiveExpression(PrimitiveExpression primitiveExpression)
			{
				if (primitiveExpression.Value is string value)
					Values.Add(value);
				base.VisitPrimitiveExpression(primitiveExpression);
			}
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

		internal static bool TryFitCode(DecompilationContext context, int budget, out DecompilationContext fitted)
		{
			const string truncationSuffix = "...";
			int low = 0;
			int high = context.DecompiledCSharp.Length;
			int bestLength = -1;

			while (low <= high)
			{
				int middle = low + (high - low) / 2;
				int length = GetUnicodeSafePrefixLength(context.DecompiledCSharp, middle);
				DecompilationContext candidate = WithTokenCount(context with {
					DecompiledCSharp = context.DecompiledCSharp[..length] + truncationSuffix
				});
				if (candidate.ApproximateTokenCount <= budget)
				{
					bestLength = length;
					low = middle + 1;
				}
				else
				{
					high = middle - 1;
				}
			}

			if (bestLength < 0)
			{
				fitted = context;
				return false;
			}

			int lastNewline = bestLength > 0
				? context.DecompiledCSharp.LastIndexOf('\n', bestLength - 1)
				: -1;
			if (lastNewline > 0)
				bestLength = lastNewline;
			if (bestLength > 0 && context.DecompiledCSharp[bestLength - 1] == '\r')
				bestLength--;

			fitted = WithTokenCount(context with {
				DecompiledCSharp = context.DecompiledCSharp[..bestLength] + truncationSuffix
			});
			return true;
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
