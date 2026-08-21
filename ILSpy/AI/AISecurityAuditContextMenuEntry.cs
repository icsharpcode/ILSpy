using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Analyzers.Builtin;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>Runs an explicitly confirmed, bounded AI audit over the selected type's module.</summary>
	[ExportContextMenuEntry(Header = "Run AI Security Audit for Module", Category = "AI", Order = 1030)]
	[Shared]
	public sealed class AISecurityAuditContextMenuEntry : IContextMenuEntry
	{
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;

		[ImportingConstructor]
		public AISecurityAuditContextMenuEntry(IAIProviderFactory providerFactory, AISelectionService selectionService)
		{
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
		}

		public bool IsVisible(TextViewContext context) => ResolveType(context) is not null;
		public bool IsEnabled(TextViewContext context) => ResolveType(context) is not null && selectionService.CanAttemptRequest;

		public void Execute(TextViewContext context)
		{
			if (IsEnabled(context) && ResolveType(context) is { } type)
				_ = RunAsync(type, context);
		}

		async Task RunAsync(ITypeDefinition selectedType, TextViewContext context)
		{
			AISelectionSnapshot snapshot;
			try
			{
				snapshot = await selectionService.ResolveSnapshotAsync();
			}
			catch (AIConfigurationException)
			{
				return;
			}
			IReadOnlyList<ITypeDefinition> types = selectedType.ParentModule?.Compilation.GetAllTypeDefinitions()
				.Where(type => type.ParentModule == selectedType.ParentModule)
				.ToArray() ?? Array.Empty<ITypeDefinition>();
			var service = new AISecurityAuditService();
			var plan = service.CreatePlan(types);
			if (plan.IsOverLimit)
			{
				await ShowMessageAsync(context, "AI Security Audit", $"{plan.TotalEligible} eligible types exceed the safety limit of {plan.MaximumTypes}. Reduce the module scope before starting the audit.");
				return;
			}
			var moduleName = selectedType.ParentModule?.AssemblyName ?? "selected module";
			var window = CreateProgressWindow(plan, snapshot, providerFactory, moduleName);
			if (TopLevel.GetTopLevel(context.OriginalSource as Visual) is Window owner)
				await window.ShowDialog(owner);
			else
				window.Show();
		}

		static Window CreateProgressWindow(AISecurityAuditPlan plan, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory, string moduleName)
		{
			var status = new TextBlock { Text = $"Module: {moduleName}\n{plan.TotalEligible} eligible types. Confirm to start.", TextWrapping = TextWrapping.Wrap };
			var bar = new ProgressBar { Minimum = 0, Maximum = Math.Max(1, plan.Types.Count), Height = 18 };
			var cancel = new Button { Content = "Cancel", HorizontalAlignment = HorizontalAlignment.Right };
			var start = new Button { Content = "Start audit", HorizontalAlignment = HorizontalAlignment.Right };
			var panel = new StackPanel { Margin = new Thickness(16), Spacing = 10, Children = { status, bar, new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 8, Children = { start, cancel } } } };
			var window = new Window { Title = "AI Security Audit", Width = 520, SizeToContent = SizeToContent.Height, Content = panel };
			var cts = new System.Threading.CancellationTokenSource();
			cancel.Click += (_, _) => { cts.Cancel(); cancel.IsEnabled = false; status.Text = "Canceling…"; };
			bool completed = false;
			start.Click += async (_, _) => {
				if (completed)
				{
					window.Close();
					return;
				}
				start.IsEnabled = false;
				cancel.IsEnabled = true;
				status.Text = "Starting audit…";
				var progress = new Progress<AISecurityAuditProgress>(value => {
					bar.Value = value.Completed;
					status.Text = value.IsPartial ? $"Canceled after {value.Completed}/{value.Total} types; {value.FindingCount} findings." : $"{value.Completed}/{value.Total}: {value.CurrentType}";
				});
				try
				{
					var result = await new AISecurityAuditService().RunAsync(plan, snapshot, providerFactory, progress, cts.Token);
					status.Text = result.IsPartial ? $"Canceled: {result.Findings.Count} findings, {result.FailedCount} failures." : $"Complete: {result.Findings.Count} findings, {result.FailedCount} failures.";
					start.Content = "Close";
					start.IsEnabled = true;
					completed = true;
				}
				catch (AIConfigurationException ex)
				{
					status.Text = ex.Message;
					start.IsEnabled = true;
				}
			};
			window.Closed += (_, _) => cts.Dispose();
			return window;
		}

		static Task ShowMessageAsync(TextViewContext context, string title, string message)
		{
			var window = new Window { Title = title, Width = 520, SizeToContent = SizeToContent.Height, Content = new StackPanel { Margin = new Thickness(16), Spacing = 10, Children = { new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap }, new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Right } } } };
			if (window.Content is StackPanel panel && panel.Children.OfType<Button>().FirstOrDefault() is { } button)
				button.Click += (_, _) => window.Close();
			if (TopLevel.GetTopLevel(context.OriginalSource as Visual) is Window owner)
				return window.ShowDialog(owner);
			window.Show();
			return Task.CompletedTask;
		}

		static ITypeDefinition? ResolveType(TextViewContext context)
		{
			IEntity? entity = context.SelectedTreeNodes is { Length: 1 } nodes
				? (nodes[0] as IMemberTreeNode)?.Member
				: context.Reference?.Reference as IEntity;
			return entity switch { ITypeDefinition type => type, IMethod method => method.DeclaringTypeDefinition, _ => null };
		}
	}
}
