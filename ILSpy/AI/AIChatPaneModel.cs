// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.ObjectModel;
using System.Composition;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 3, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class AIChatPaneModel : ToolPaneModel, IDisposable
	{
		public const string PaneContentId = "AIChat";
		const int MaxMessages = 100;
		const string SystemPrompt = "You are an assistant for .NET decompilation. Answer questions about the code clearly and concisely.";
		readonly SettingsService settingsService;
		readonly IAIProviderFactory providerFactory;
		readonly AISelectionService selectionService;
		readonly AssemblyTreeModel assemblyTree;
		readonly DockWorkspace dockWorkspace;
		readonly IEnumerable<ExportFactory<IOptionPage, IOptionsMetadata>> optionPages;
		CancellationTokenSource? cancellation;
		string loadedHistoryPath = string.Empty;
		AIConversationTarget? loadedTarget;
		long conversationGeneration;

		public ObservableCollection<ChatMessage> Messages { get; } = new();
		public IReadOnlyList<AIProfile> Profiles => selectionService.Profiles;
		public AIProfile ActiveProfile => selectionService.ActiveProfile;
		public IReadOnlyList<string> Models => ActiveProfile.Models;
		[ObservableProperty] string readinessMessage = string.Empty;
		public bool IsReady => string.IsNullOrEmpty(ReadinessMessage);
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ShowSuggestions))]
		string input = string.Empty;
		[ObservableProperty] bool isBusy;
		[ObservableProperty] string statusMessage = "Ready";
		[ObservableProperty] string errorMessage = string.Empty;
		public bool ShowSuggestions => Input.StartsWith("/", StringComparison.Ordinal);
		public string[] CommandSuggestions { get; } = { "/explain", "/rename ", "/audit", "/summary" };

		[ImportingConstructor]
		public AIChatPaneModel(SettingsService settingsService, IAIProviderFactory providerFactory, AssemblyTreeModel assemblyTree, DockWorkspace dockWorkspace, [ImportMany("OptionPages")] IEnumerable<ExportFactory<IOptionPage, IOptionsMetadata>> optionPages)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			selectionService = AppComposition.TryGetExport<AISelectionService>() ?? throw new InvalidOperationException("AI selection service is unavailable.");
			this.assemblyTree = assemblyTree ?? throw new ArgumentNullException(nameof(assemblyTree));
			this.dockWorkspace = dockWorkspace ?? throw new ArgumentNullException(nameof(dockWorkspace));
			this.optionPages = optionPages ?? throw new ArgumentNullException(nameof(optionPages));
			Id = PaneContentId;
			Title = "AI Chat";
			assemblyTree.PropertyChanged += OnAssemblyTreePropertyChanged;
			selectionService.SelectionChanged += OnSelectionChanged;
			LoadHistory();
			_ = RefreshReadinessAsync();
		}

		async Task RefreshReadinessAsync()
		{
			AIConfigurationState state = await selectionService.EvaluateReadinessAsync().ConfigureAwait(false);
			await Dispatcher.UIThread.InvokeAsync(() => {
				ReadinessMessage = state.IsReady ? string.Empty : state.Message;
				OnPropertyChanged(nameof(IsReady));
				OnPropertyChanged(nameof(ActiveProfile));
				OnPropertyChanged(nameof(Models));
			});
		}

		async void OnSelectionChanged(object? sender, EventArgs e)
		{
			AIConversationTarget target = GetCurrentTarget();
			if (loadedTarget?.BelongsTo(target.ProfileId, target.ProviderType, target.Endpoint, target.Model) == true)
			{
				if (loadedHistory.ActiveConversation.Target is { } metadata)
					loadedHistory.ActiveConversation.Target = metadata with { ProfileName = target.ProfileName };
				await RefreshReadinessAsync();
				return;
			}
			cancellation?.Cancel();
			SaveHistory();
			Messages.Clear();
			conversationGeneration++;
			LoadHistory(target);
			await RefreshReadinessAsync();
		}

		[RelayCommand]
		async Task SelectProfileAsync(AIProfile? profile)
		{
			if (profile is null)
				return;
			await selectionService.ApplySelectionAsync(profile.Id, profile.ResolveModel());
		}

		[RelayCommand]
		async Task SelectModelAsync(string? model)
		{
			if (string.IsNullOrWhiteSpace(model))
				return;
			await selectionService.ApplySelectionAsync(ActiveProfile.Id, model);
		}

		void OnAssemblyTreePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(AssemblyTreeModel.SelectedItem))
				return;
			cancellation?.Cancel();
			SaveHistory(loadedHistoryPath);
			Messages.Clear();
			conversationGeneration++;
			LoadHistory();
		}

		[RelayCommand]
		async Task SendAsync()
		{
			string text = Input.Trim();
			if (text.Length == 0 || IsBusy)
				return;
			AISelectionSnapshot snapshot;
			try
			{
				snapshot = await selectionService.ResolveSnapshotAsync().ConfigureAwait(false);
			}
			catch (AIConfigurationException ex)
			{
				ErrorMessage = ex.Message;
				StatusMessage = "AI settings required";
				return;
			}
			Input = string.Empty;
			EnsureConversation(snapshot);
			if (text.StartsWith('/'))
				text = ExpandCommand(text);
			var user = new ChatMessage { Role = "user", Content = text };
			Messages.Add(user);
			TrimHistory();
			var assistant = new ChatMessage { Role = "assistant" };
			Messages.Add(assistant);
			long requestGeneration = conversationGeneration;
			string requestConversationId = loadedHistory.ActiveConversationId;
			IsBusy = true;
			ErrorMessage = string.Empty;
			StatusMessage = "Generating…";
			cancellation?.Cancel();
			var cts = new CancellationTokenSource();
			cancellation = cts;
			try
			{
				var requestMessages = Messages.Where(m => m.Content.Length != 0).Select(m => new LLMMessage(m.Role, m.Content)).ToArray();
				var provider = await providerFactory.CreateAsync(snapshot, cts.Token).ConfigureAwait(false);
				var context = GetActiveContext(text);
				var request = new LLMRequest(SystemPrompt, requestMessages.Append(new LLMMessage("user", context)).ToArray(), 2048, 0.3);
				var builder = new StringBuilder();
				await foreach (var chunk in provider.CompleteAsync(request, cts.Token).ConfigureAwait(false))
				{
					builder.Append(chunk);
					string contentSnapshot = builder.ToString();
					await Dispatcher.UIThread.InvokeAsync(() => {
						if (requestGeneration == conversationGeneration
							&& requestConversationId == loadedHistory.ActiveConversationId
							&& Messages.Contains(assistant))
							assistant.Content = contentSnapshot;
					});
				}
				StatusMessage = "Complete";
				SaveHistory();
			}
			catch (OperationCanceledException) { StatusMessage = "Canceled"; }
			catch (Exception ex) { ErrorMessage = ex.Message; StatusMessage = "Request failed"; }
			finally { IsBusy = false; if (ReferenceEquals(cancellation, cts)) cancellation = null; cts.Dispose(); }
		}

		string ExpandCommand(string command)
		{
			int space = command.IndexOf(' ');
			string name = (space < 0 ? command : command[..space]).ToLowerInvariant();
			return name switch {
				"/explain" => "Explain the currently selected symbol.",
				"/rename" => "Suggest a meaningful rename for " + (space < 0 ? "the currently selected symbol." : command[(space + 1)..]),
				"/audit" => "Audit the currently loaded assembly for security risks.",
				"/summary" => "Summarize the currently loaded assembly.",
				_ => command
			};
		}

		string GetActiveContext(string requestText)
		{
			var selected = assemblyTree.SelectedItem as IMemberTreeNode;
			string symbol = selected?.Member?.FullName ?? AppComposition.TryGetExport<DockWorkspace>()?.ActiveDecompilerTab?.Title ?? "(none)";
			return "Currently viewing: " + symbol + ".\n\nUser request: " + requestText;
		}

		[RelayCommand]
		void Cancel() { cancellation?.Cancel(); }

		[RelayCommand]
		void Clear() { cancellation?.Cancel(); Messages.Clear(); SaveHistory(); conversationGeneration++; StatusMessage = "Ready"; ErrorMessage = string.Empty; }

		[RelayCommand]
		void OpenSettings()
		{
			ContentTabPage tab = dockWorkspace.OpenSingletonTab("options", () => {
				var options = new OptionsPageModel(settingsService, optionPages);
				options.SelectedPage = options.Pages.OfType<AISettingsViewModel>().FirstOrDefault() ?? options.SelectedPage;
				return dockWorkspace.OpenNewTab(options);
			});
			if (tab.Content is OptionsPageModel existing)
				existing.SelectedPage = existing.Pages.OfType<AISettingsViewModel>().FirstOrDefault() ?? existing.SelectedPage;
		}

		[RelayCommand]
		void Export()
		{
			string path = GetHistoryPath();
			if (path.Length != 0)
			{
				try
				{ File.WriteAllText(Path.ChangeExtension(path, ".md"), loadedHistory.ToMarkdown(), Encoding.UTF8); }
				catch (UnauthorizedAccessException) { StatusMessage = "Export failed"; return; }
				catch (IOException) { StatusMessage = "Export failed"; return; }
			}
			StatusMessage = "Exported";
		}

		void TrimHistory() { while (Messages.Count > MaxMessages) Messages.RemoveAt(0); }
		ChatHistory loadedHistory = new();
		void EnsureConversation(AISelectionSnapshot snapshot)
		{
			AIConversationTarget target = new(snapshot.ProfileId, snapshot.ProfileName, snapshot.ProviderType, snapshot.Endpoint, snapshot.Model);
			ChatConversation current = loadedHistory.ActiveConversation;
			if (!current.ReadOnly && current.Target?.BelongsTo(target.ProfileId, target.ProviderType, target.Endpoint, target.Model) == true)
				return;
			SaveHistory();
			current = loadedHistory.GetOrCreate(target);
			current.ReadOnly = false;
			Messages.Clear();
			conversationGeneration++;
			foreach (ChatMessage message in current.Messages.TakeLast(MaxMessages))
				Messages.Add(message);
			loadedTarget = target;
		}
		string GetHistoryPath()
		{
			var entity = (assemblyTree.SelectedItem as IMemberTreeNode)?.Member;
			string? file = entity?.ParentModule?.MetadataFile?.FileName;
			if (string.IsNullOrWhiteSpace(file) && assemblyTree.SelectedItem is ILSpyTreeNode node)
				file = node.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault()?.LoadedAssembly.FileName;
			return string.IsNullOrWhiteSpace(file) ? string.Empty : Path.Combine(Path.GetDirectoryName(file)!, ".ilspy-chat-history.json");
		}
		void LoadHistory(AIConversationTarget? target = null)
		{
			string path = GetHistoryPath();
			loadedHistoryPath = path;
			if (path.Length != 0)
				loadedHistory = ChatHistory.Load(path);
			MarkDeletedConversationsReadOnly();
			if (target is not null)
				loadedHistory.GetOrCreate(target);
			loadedTarget = loadedHistory.ActiveConversation.Target;
			foreach (var message in loadedHistory.ActiveConversation.Messages.TakeLast(MaxMessages))
				Messages.Add(message);
		}

		void MarkDeletedConversationsReadOnly()
		{
			var liveProfileIds = selectionService.Profiles.Select(profile => profile.Id).ToHashSet(StringComparer.Ordinal);
			foreach (ChatConversation conversation in loadedHistory.Conversations)
			{
				if (conversation.Target is { } target && !liveProfileIds.Contains(target.ProfileId))
					conversation.ReadOnly = true;
			}
		}

		AIConversationTarget GetCurrentTarget()
		{
			AIProfile profile = selectionService.ActiveProfile;
			return new AIConversationTarget(profile.Id, profile.Name, profile.ProviderType, profile.BaseUrl, profile.ResolveModel());
		}
		void SaveHistory() => SaveHistory(GetHistoryPath());
		void SaveHistory(string path)
		{
			if (path.Length == 0)
				return;
			try
			{ loadedHistory.AssemblyPath = GetAssemblyPath(); loadedHistory.ActiveConversation.Messages = Messages.ToList(); loadedHistory.Save(path); }
			catch (UnauthorizedAccessException) { }
			catch (IOException) { }
		}
		string GetAssemblyPath()
		{
			var entity = (assemblyTree.SelectedItem as IMemberTreeNode)?.Member;
			if (!string.IsNullOrWhiteSpace(entity?.ParentModule?.MetadataFile?.FileName))
				return entity.ParentModule.MetadataFile.FileName;
			return assemblyTree.SelectedItem is ILSpyTreeNode node
				? node.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault()?.LoadedAssembly.FileName ?? string.Empty
				: string.Empty;
		}
		public void Dispose() { assemblyTree.PropertyChanged -= OnAssemblyTreePropertyChanged; selectionService.SelectionChanged -= OnSelectionChanged; cancellation?.Cancel(); SaveHistory(); }
	}
}
