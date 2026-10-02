// Copyright (c) 2026 Piero Viano
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
using System.Composition;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Abstractions;
using ICSharpCode.ILSpyX.Analyzers;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>One entity a rename would touch, with how it relates to the renamed symbol: the
	/// header of the analyzer that found it, "Declaration", or "Constructor".</summary>
	public sealed record RenameUsage(IEntity Entity, string Relation);

	/// <summary>
	/// Everything a rename of <see cref="Symbol"/> would have to change: its declaration, related
	/// declarations that share its name (overridden / implemented base members, overrides,
	/// implementations, a type's constructors) and the entities whose code uses any of them.
	/// </summary>
	public sealed class RenameUsageResult(IEntity symbol, IReadOnlyList<RenameUsage> declarations,
		IReadOnlyList<RenameUsage> relatedDeclarations, IReadOnlyList<RenameUsage> usages)
	{
		public IEntity Symbol { get; } = symbol;
		public IReadOnlyList<RenameUsage> DeclarationEntries { get; } = declarations;
		public IReadOnlyList<RenameUsage> RelatedDeclarationEntries { get; } = relatedDeclarations;
		public IReadOnlyList<RenameUsage> UsageEntries { get; } = usages;

		public IEnumerable<IEntity> Declarations => DeclarationEntries.Select(u => u.Entity);
		public IEnumerable<IEntity> RelatedDeclarations => RelatedDeclarationEntries.Select(u => u.Entity);
		public IEnumerable<IEntity> Usages => UsageEntries.Select(u => u.Entity);
	}

	/// <summary>
	/// Read-only rename preview ("Find Usages for Rename"): collects the places a rename would touch
	/// by running the registered analyzers. Related declarations come from the analyzers headed
	/// "Overrides", "Implements", "Overridden By" and "Implemented By"; usages from "Used By",
	/// "Read By", "Assigned By", "Address Taken By" and "Applied To", run over the symbol, its
	/// related declarations and their accessors. Analyzers are matched by header, so plugin
	/// analyzers that contribute the same relations take part as well.
	/// </summary>
	public static class RenameUsageFinder
	{
		static readonly HashSet<string> RelationHeaders = new(StringComparer.Ordinal) {
			"Overrides", "Implements", "Overridden By", "Implemented By",
		};

		static readonly HashSet<string> UsageHeaders = new(StringComparer.Ordinal) {
			"Used By", "Read By", "Assigned By", "Address Taken By", "Applied To",
		};

		/// <summary>
		/// Runs the analysis on a background thread. The analyzer instances are created on the calling
		/// thread from the composition's <see cref="AnalyzerRegistry"/>.
		/// </summary>
		public static Task<RenameUsageResult> FindAsync(IEntity symbol, AssemblyList assemblyList, ILanguage language, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(symbol);
			ArgumentNullException.ThrowIfNull(assemblyList);
			ArgumentNullException.ThrowIfNull(language);
			var analyzers = new List<(string Header, IAnalyzer Analyzer)>();
			foreach (var factory in AppEnv.AppComposition.TryGetExport<AnalyzerRegistry>()?.Analyzers ?? [])
			{
				if (factory.Metadata?.Header is { } header && (RelationHeaders.Contains(header) || UsageHeaders.Contains(header)))
					analyzers.Add((header, factory.CreateExport().Value));
			}
			return Task.Run(() => Find(symbol, assemblyList, language, analyzers, cancellationToken), cancellationToken);
		}

		internal static RenameUsageResult Find(IEntity symbol, AssemblyList assemblyList, ILanguage language,
			IReadOnlyList<(string Header, IAnalyzer Analyzer)> analyzers, CancellationToken cancellationToken)
		{
			symbol = Normalize(symbol);
			var context = new AnalyzerContext {
				AssemblyList = assemblyList,
				Language = language,
				CancellationToken = cancellationToken,
			};
			var seenDeclarations = new HashSet<EntityKey> { EntityKey.Of(symbol) };
			var declarations = new List<RenameUsage> { new(symbol, "Declaration") };
			var related = new List<RenameUsage>();

			foreach (var (header, analyzer) in analyzers.Where(a => RelationHeaders.Contains(a.Header)))
			{
				foreach (var entity in Run(analyzer, symbol, context))
				{
					if (seenDeclarations.Add(EntityKey.Of(entity)))
						related.Add(new RenameUsage(entity, header));
				}
			}
			if (symbol is ITypeDefinition type)
			{
				foreach (var ctor in type.Methods.Where(m => m.IsConstructor))
				{
					if (seenDeclarations.Add(EntityKey.Of(ctor)))
						related.Add(new RenameUsage(ctor, "Constructor"));
				}
			}

			// Whose code refers to the symbol or to a declaration sharing its name. A type's
			// constructors are only referenced together with the type, so they need no scan of their own.
			var targets = new List<IEntity> { symbol };
			if (symbol is not ITypeDefinition)
				targets.AddRange(related.Select(r => r.Entity));
			var seenUsages = new HashSet<EntityKey>();
			var usages = new List<RenameUsage>();
			foreach (var target in targets.SelectMany(WithAccessors))
			{
				foreach (var (header, analyzer) in analyzers.Where(a => UsageHeaders.Contains(a.Header)))
				{
					foreach (var entity in Run(analyzer, target, context))
					{
						if (seenUsages.Add(EntityKey.Of(entity)))
							usages.Add(new RenameUsage(entity, header));
					}
				}
			}
			return new RenameUsageResult(symbol, declarations, related, usages);
		}

		static List<IEntity> Run(IAnalyzer analyzer, IEntity target, AnalyzerContext context)
		{
			context.CancellationToken.ThrowIfCancellationRequested();
			if (!analyzer.Show(target))
				return new List<IEntity>();
			return analyzer.Analyze(target, context).OfType<IEntity>().Select(Normalize).ToList();
		}

		static IEnumerable<IEntity> WithAccessors(IEntity entity)
		{
			yield return entity;
			switch (entity)
			{
				case IProperty property:
					if (property.Getter != null)
						yield return property.Getter;
					if (property.Setter != null)
						yield return property.Setter;
					break;
				case IEvent ev:
					if (ev.AddAccessor != null)
						yield return ev.AddAccessor;
					if (ev.RemoveAccessor != null)
						yield return ev.RemoveAccessor;
					if (ev.InvokeAccessor != null)
						yield return ev.InvokeAccessor;
					break;
			}
		}

		/// <summary>Maps a specialized member to its definition; other entities are returned as is.</summary>
		public static IEntity Normalize(IEntity entity) => entity is IMember member ? member.MemberDefinition : entity;

		/// <summary>
		/// Resolves a code reference (a resolved member or type) to the entity a rename would target,
		/// or null when it does not denote a renamable entity.
		/// </summary>
		public static IEntity? ResolveEntity(object? reference) => reference switch {
			IMember member => member.MemberDefinition,
			IType type => type.GetDefinition(),
			IEntity entity => entity,
			_ => null,
		};

		// Entities from different analyzer type systems compare by module and token; generated
		// entities without a token fall back to object identity.
		readonly record struct EntityKey(MetadataFile? Module, EntityHandle Token, object? Identity)
		{
			public static EntityKey Of(IEntity entity)
				=> entity.MetadataToken.IsNil || entity.ParentModule?.MetadataFile is null
					? new EntityKey(null, default, entity)
					: new EntityKey(entity.ParentModule.MetadataFile, entity.MetadataToken, null);
		}
	}

	/// <summary>Renders a <see cref="RenameUsageResult"/> as a text report whose rows link to their entities.</summary>
	public static class RenameUsageReport
	{
		const ConversionFlags RowFlags = ConversionFlags.ShowParameterList | ConversionFlags.ShowTypeParameterList;

		public static AvaloniaEditTextOutput Write(RenameUsageResult result, Language language, string title)
		{
			ArgumentNullException.ThrowIfNull(result);
			ArgumentNullException.ThrowIfNull(language);
			var output = new AvaloniaEditTextOutput { Title = title };
			var symbolText = language.EntityToString(result.Symbol,
				ConversionFlags.ShowDeclaringType | ConversionFlags.UseFullyQualifiedEntityNames | RowFlags);
			var all = result.DeclarationEntries.Concat(result.RelatedDeclarationEntries).Concat(result.UsageEntries);
			int assemblies = all.Select(u => AssemblyName(u.Entity)).Distinct().Count();
			WriteLine(output, $"// {Resources.FindUsagesForRename}: {symbolText}");
			WriteLine(output, $"// Read-only preview, nothing is renamed: {result.DeclarationEntries.Count} declaration(s), "
				+ $"{result.RelatedDeclarationEntries.Count} related declaration(s), "
				+ $"{result.UsageEntries.Count} usage(s) in {assemblies} assembly(ies).");
			WriteSection(output, language, "Declaration", result.DeclarationEntries);
			WriteSection(output, language, "Related declarations", result.RelatedDeclarationEntries);
			WriteSection(output, language, "Usages", result.UsageEntries);
			return output;
		}

		static void WriteSection(AvaloniaEditTextOutput output, Language language, string heading, IReadOnlyList<RenameUsage> entries)
		{
			output.WriteLine();
			WriteLine(output, $"{heading} ({entries.Count})");
			output.Indent();
			foreach (var byAssembly in entries.GroupBy(e => AssemblyName(e.Entity)).OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase))
			{
				WriteLine(output, byAssembly.Key);
				output.Indent();
				foreach (var byType in byAssembly.GroupBy(e => GroupName(e.Entity)).OrderBy(g => g.Key, StringComparer.Ordinal))
				{
					WriteLine(output, byType.Key);
					output.Indent();
					foreach (var entry in byType.OrderBy(e => e.Entity.Name, StringComparer.Ordinal))
					{
						WriteEntity(output, language, entry.Entity);
						output.Write("  // " + entry.Relation);
						output.WriteLine();
					}
					output.Unindent();
				}
				output.Unindent();
			}
			output.Unindent();
		}

		internal static void WriteLine(AvaloniaEditTextOutput output, string text)
		{
			output.Write(text);
			output.WriteLine();
		}

		static void WriteEntity(AvaloniaEditTextOutput output, Language language, IEntity entity)
		{
			var text = language.EntityToString(entity, RowFlags);
			switch (entity)
			{
				case IMember member:
					output.WriteReference(member, text);
					break;
				case ITypeDefinition type:
					output.WriteReference(type, text);
					break;
				default:
					output.Write(text);
					break;
			}
		}

		static string AssemblyName(IEntity entity)
			=> entity.ParentModule?.AssemblyName ?? entity.ParentModule?.MetadataFile?.Name ?? "?";

		// Members group under their declaring type; a type groups under its declaring type, or under
		// its namespace when it is a top-level type.
		static string GroupName(IEntity entity) => entity switch {
			ITypeDefinition { DeclaringTypeDefinition: { } outer } => outer.FullName,
			ITypeDefinition type => string.IsNullOrEmpty(type.Namespace) ? "<global namespace>" : type.Namespace,
			_ => entity.DeclaringTypeDefinition?.FullName ?? string.Empty,
		};
	}

	/// <summary>Opens the rename preview for an entity in a new, frozen report tab.</summary>
	[Export]
	[Shared]
	[method: ImportingConstructor]
	public sealed class FindUsagesForRenameService(DockWorkspace dockWorkspace, AssemblyTreeModel assemblyTreeModel, LanguageService languageService)
	{
		/// <summary>
		/// Runs the analysis off the UI thread inside a new tab (cancellable through the tab's wait
		/// overlay) and shows the report there. Clicking a row navigates to it and highlights the
		/// renamed symbol in the navigated-to code.
		/// </summary>
		public async Task ShowAsync(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			if (assemblyTreeModel.AssemblyList is not { } assemblyList)
				return;
			var symbol = RenameUsageFinder.Normalize(entity);
			var language = languageService.CurrentLanguage;
			var title = $"{Resources.FindUsagesForRename}: {symbol.Name}";
			var content = new DecompilerTabPageModel { Language = language, Title = title };
			content.NavigateRequested += (_, e) =>
				MessageBus.Send(this, new NavigateToReferenceEventArgs(e.Reference, symbol, e.InNewTabPage));
			dockWorkspace.OpenNewTab(content);
			try
			{
				var output = await content.RunWithCancellation(async token => {
					var result = await RenameUsageFinder.FindAsync(symbol, assemblyList, language, token).ConfigureAwait(false);
					return RenameUsageReport.Write(result, language, title);
				}, title).ConfigureAwait(true);
				content.ShowText(output);
			}
			catch (OperationCanceledException)
			{
				var cancelled = new AvaloniaEditTextOutput { Title = title };
				RenameUsageReport.WriteLine(cancelled, Resources.OperationWasCancelled);
				content.ShowText(cancelled);
			}
		}

		/// <summary>The entity a rename invoked from <paramref name="context"/> targets, or null.</summary>
		public static IEntity? GetTarget(TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			if (context.SelectedTreeNodes is { Length: > 0 } nodes)
				return nodes.Length == 1 && nodes[0] is IMemberTreeNode { Member: { } member } ? member : null;
			if (context.Reference is { Kind: not ReferenceMode.LocalHighlight and not ReferenceMode.HoverOnly } segment)
				return RenameUsageFinder.ResolveEntity(segment.Reference);
			return null;
		}
	}

	/// <summary>
	/// Navigate &gt; Find Usages for Rename (F2): the read-only rename preview for the symbol under
	/// the caret of the focused code view, or else for the member selected in the assembly tree.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.FindUsagesForRename), MenuCategory = "Rename", MenuOrder = 70, InputGestureText = "F2")]
	[Shared]
	[method: ImportingConstructor]
	sealed class FindUsagesForRenameCommand(FindUsagesForRenameService service, AssemblyTreeModel assemblyTreeModel) : SimpleCommand
	{
		public override bool CanExecute(object? parameter) => GetTarget() != null;

		public override void Execute(object? parameter)
		{
			if (GetTarget() is { } entity)
				service.ShowAsync(entity).HandleExceptions();
		}

		IEntity? GetTarget()
		{
			if (ActiveTextViewLocator.Focused() is { } view)
			{
				return view.GetReferenceSegmentAtCaret() is { } segment
					? FindUsagesForRenameService.GetTarget(new TextViewContext { TextView = view, Reference = segment })
					: null;
			}
			return assemblyTreeModel.SelectedItem is IMemberTreeNode { Member: { } member } ? member : null;
		}
	}

	/// <summary>Context-menu "Find Usages for Rename" on a code reference or a single tree member.</summary>
	[ExportContextMenuEntry(Header = nameof(Resources.FindUsagesForRename), Category = "Navigation", InputGestureText = "F2", Order = 420)]
	[Shared]
	[method: ImportingConstructor]
	public sealed class FindUsagesForRenameContextMenuEntry(FindUsagesForRenameService service) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => FindUsagesForRenameService.GetTarget(context) != null;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (FindUsagesForRenameService.GetTarget(context) is { } entity)
				service.ShowAsync(entity).HandleExceptions();
		}
	}
}
