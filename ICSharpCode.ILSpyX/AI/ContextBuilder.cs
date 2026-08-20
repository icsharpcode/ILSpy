// Copyright (c) 2026 Dr. Masroor Ehsan

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
using ICSharpCode.ILSpyX.Analyzers.Builtin;

namespace ICSharpCode.ILSpyX.AI
{
	public sealed class ContextBuilder
	{
		readonly int maxContextTokens;
		readonly bool sendIL;
		readonly bool sendCallGraph;

		public ContextBuilder(AISelectionSnapshot snapshot)
			: this((snapshot ?? throw new ArgumentNullException(nameof(snapshot))).MaxContextTokens, snapshot.SendIL, snapshot.SendCallGraph)
		{
		}

		public ContextBuilder(int maxContextTokens, bool sendIL, bool sendCallGraph)
		{
			this.maxContextTokens = maxContextTokens;
			this.sendIL = sendIL;
			this.sendCallGraph = sendCallGraph;
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

			var unavailable = new List<string>();
			string decompiledCode;
			try
			{
				decompiledCode = Decompile(entity, decompiler);
			}
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				decompiledCode = string.Empty;
				unavailable.Add("Decompiled code is unavailable because the assembly metadata could not be read.");
			}

			var context = new DecompilationContext {
				DecompiledCSharp = decompiledCode,
				FullyQualifiedName = entity.FullName,
				AssemblyName = entity.ParentModule.AssemblyName,
				TargetFramework = DetectTargetFramework(entity.ParentModule),
				Attributes = entity.GetAttributes().Select(attribute => attribute.AttributeType.FullName).ToArray(),
				ImplementedInterfaces = GetImplementedInterfaces(entity),
				StringLiterals = GetStringLiterals(entity, decompiler, unavailable),
				Callers = sendCallGraph ? GetCallers(entity, decompiler.TypeSystem.MainModule, unavailable) : Array.Empty<string>(),
				Callees = sendCallGraph ? GetCallees(entity, decompiler.TypeSystem.MainModule, unavailable) : Array.Empty<string>(),
				IL = sendIL ? GetIL(entity, unavailable) : null,
				UnavailableSections = unavailable
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

		static IReadOnlyList<string> GetStringLiterals(IEntity entity, CSharpDecompiler decompiler, List<string> unavailable)
		{
			var visitor = new StringLiteralVisitor();
			try
			{
				decompiler.Decompile(new[] { entity.MetadataToken }).AcceptVisitor(visitor);
			}
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				unavailable.Add("String literal information is unavailable because the assembly metadata could not be read.");
				return Array.Empty<string>();
			}
			return visitor.Values.Distinct(StringComparer.Ordinal).Take(20).ToArray();
		}

		static string? GetIL(IEntity entity, List<string> unavailable)
		{
			if (entity is not IMethod method || method.ParentModule is not MetadataModule module)
				return null;
			try
			{
				var output = new PlainTextOutput();
				new MethodBodyDisassembler(output, default).Disassemble(module.MetadataFile, (MethodDefinitionHandle)method.MetadataToken);
				return output.ToString();
			}
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				unavailable.Add("IL information is unavailable because the assembly metadata could not be read.");
				return null;
			}
		}

		static IReadOnlyList<string> GetCallees(IEntity entity, IModule mainModule, List<string> unavailable)
		{
			if (entity is not IMethod method || mainModule is not MetadataModule module)
				return Array.Empty<string>();
			string? declaringType = method.DeclaringTypeDefinition?.FullName;
			try
			{
				return ScanMethodReferences(method, module)
				.GroupBy(member => member.FullName, StringComparer.Ordinal)
				.Select(group => group.First())
				.OrderByDescending(member => member.DeclaringTypeDefinition?.FullName == declaringType)
				.ThenBy(member => member.FullName, StringComparer.Ordinal)
				.Take(10)
				.Select(member => member.FullName)
					.ToArray();
			}
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				unavailable.Add("Callee information is unavailable because the assembly metadata could not be read.");
				return Array.Empty<string>();
			}
		}

		static IReadOnlyList<string> GetCallers(IEntity entity, IModule mainModule, List<string> unavailable)
		{
			if (entity is not IMethod target || mainModule is not MetadataModule module)
				return Array.Empty<string>();
			string? declaringType = target.DeclaringTypeDefinition?.FullName;
			try
			{
				return MethodUsedByAnalyzer.FindCallers(target, module)
				.OrderByDescending(caller => caller.DeclaringTypeDefinition?.FullName == declaringType)
				.ThenBy(caller => caller.FullName, StringComparer.Ordinal)
				.Take(10)
				.Select(caller => caller.FullName)
					.ToArray();
			}
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				unavailable.Add("Caller information is unavailable because the assembly metadata could not be read.");
				return Array.Empty<string>();
			}
		}

		internal static IEnumerable<IMember> ScanMethodReferences(IMethod method, MetadataModule module)
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
					if (opCode is not (ILOpCode.Call or ILOpCode.Callvirt or ILOpCode.Newobj))
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
			catch (Exception exception) when (IsRecoverableMetadataException(exception))
			{
				return Array.Empty<IMember>();
			}
		}

		static bool IsRecoverableMetadataException(Exception exception)
		{
			return exception is BadImageFormatException
				or ArgumentException
				or InvalidOperationException
				or NotSupportedException
				or DecompilerException;
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
			int budget = maxContextTokens;
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

			bestLength = FindStatementBoundary(context.DecompiledCSharp, bestLength);
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

		internal static int FindStatementBoundary(string text, int length)
		{
			int boundary = -1;
			int newline = -1;
			int i = 0;
			while (i < length)
			{
				char current = text[i];
				if (current is ';' or '}')
					boundary = i + 1;
				else if (current == '\n')
					newline = i;

				if (current == '/' && i + 1 < length)
				{
					if (text[i + 1] == '/')
					{
						i += 2;
						while (i < length && text[i] != '\n')
							i++;
						continue;
					}
					if (text[i + 1] == '*')
					{
						i += 2;
						while (i + 1 < length && (text[i] != '*' || text[i + 1] != '/'))
							i++;
						i = Math.Min(length, i + 2);
						continue;
					}
				}

				if (current is '"' or '\'')
				{
					bool verbatim = current == '"' && i > 0 && text[i - 1] == '@';
					int rawQuotes = current == '"' ? CountQuoteRun(text, i) : 0;
					if (rawQuotes >= 3)
					{
						i += rawQuotes;
						while (i < length)
						{
							if (text[i] == '"' && CountQuoteRun(text, i) >= rawQuotes)
							{
								i += rawQuotes;
								break;
							}
							i++;
						}
						continue;
					}

					i++;
					while (i < length)
					{
						if (text[i] == '\\' && !verbatim)
						{
							i = Math.Min(length, i + 2);
							continue;
						}
						if (text[i] == current)
						{
							if (verbatim && i + 1 < length && text[i + 1] == '"')
							{
								i += 2;
								continue;
							}
							i++;
							break;
						}
						i++;
					}
					continue;
				}

				i++;
			}

			return boundary >= 0 ? boundary : (newline > 0 ? newline : length);
		}

		static int CountQuoteRun(string text, int start)
		{
			int count = 0;
			while (start + count < text.Length && text[start + count] == '"')
				count++;
			return count;
		}

	}
}
