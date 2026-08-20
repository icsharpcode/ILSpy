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
		long requestGeneration;
		string loadedHistoryPath = string.Empty;
		AIConversationTarget? loadedTarget;
		long conversationGeneration;

		public ObservableCollection<ChatMessage> Messages { get; } = new();
		public ObservableCollection<ChatConversation> Conversations { get; } = new();
		public IReadOnlyList<AIProfile> Profiles => selectionService.Profiles;
		public AIProfile ActiveProfile => selectionService.ActiveProfile;
		public IReadOnlyList<string> Models => ActiveProfile.Models;
		[ObservableProperty] ChatConversation? selectedConversation;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanSend))]
		string readinessMessage = string.Empty;
		public bool IsReady => string.IsNullOrEmpty(ReadinessMessage);
		public bool CanOpenAISettings => !IsReady;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ShowSuggestions))]
		string input = string.Empty;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanSend))]
		bool isBusy;
		[ObservableProperty] string statusMessage = "Ready";
		[ObservableProperty] string errorMessage = string.Empty;
		public bool ShowSuggestions => Input.StartsWith("/", StringComparison.Ordinal);
		public string[] CommandSuggestions { get; } = { "/help", "/clear", "/explain", "/rename ", "/audit", "/summary" };

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

		partial void OnSelectedConversationChanged(ChatConversation? value)
		{
			if (value is null)
				return;
			SaveHistory();
			if (!loadedHistory.TrySelect(value.Id))
				return;
			Messages.Clear();
			conversationGeneration++;
			foreach (ChatMessage message in value.Messages.TakeLast(MaxMessages))
				Messages.Add(message);
			OnPropertyChanged(nameof(IsConversationReadOnly));
			StatusMessage = value.ReadOnly ? "Read-only conversation" : "Ready";
		}

		public bool IsConversationReadOnly {
			get {
				ChatConversation conversation = loadedHistory.ActiveConversation;
				if (conversation.ReadOnly || conversation.Target is null)
					return true;
				AIConversationTarget current = GetCurrentTarget();
				return !conversation.Target.BelongsTo(current.ProfileId, current.ProviderType, current.Endpoint, current.Model);
			}
		}
		public bool CanSend => !IsBusy && IsReady && !IsConversationReadOnly;

		async Task RefreshReadinessAsync()
		{
			AIConfigurationState state = await selectionService.EvaluateReadinessAsync().ConfigureAwait(false);
			await Dispatcher.UIThread.InvokeAsync(() => {
				ReadinessMessage = state.IsReady ? string.Empty : state.Message;
				OnPropertyChanged(nameof(IsReady));
				OnPropertyChanged(nameof(CanOpenAISettings));
				OnPropertyChanged(nameof(ActiveProfile));
				OnPropertyChanged(nameof(Models));
			});
		}

		async void OnSelectionChanged(object? sender, EventArgs e)
		{
			AIConversationTarget target = GetCurrentTarget();
			if (loadedTarget?.BelongsTo(target.ProfileId, target.ProviderType, target.Endpoint, target.Model) == true)
			{
				await SetUiStateAsync(() => {
					OnPropertyChanged(nameof(IsConversationReadOnly));
				});
				await RefreshReadinessAsync();
				return;
			}
			await SetUiStateAsync(() => {
				StartConversation(target);
			});
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
			_ = SetUiStateAsync(() => {
				cancellation?.Cancel();
				SaveHistory(loadedHistoryPath);
				Messages.Clear();
				conversationGeneration++;
				LoadHistory();
			});
		}

		[RelayCommand]
		async Task SendAsync()
		{
			string text = Input.Trim();
			if (text.Length == 0 || IsBusy)
				return;
			if (await DispatchLocalCommandAsync(text).ConfigureAwait(false))
				return;
			if (IsConversationReadOnly)
			{
				await SetUiStateAsync(() => {
					StatusMessage = "Start a new conversation before sending.";
					ErrorMessage = "This conversation is read-only because its target is unavailable or comes from legacy history.";
				});
				return;
			}
			AISelectionSnapshot snapshot;
			try
			{
				snapshot = await selectionService.ResolveSnapshotAsync().ConfigureAwait(false);
			}
			catch (AIConfigurationException ex)
			{
				await SetUiStateAsync(() => {
					ReadinessMessage = ex.Message;
					ErrorMessage = ex.Message;
					StatusMessage = "AI settings required";
					OnPropertyChanged(nameof(IsReady));
					OnPropertyChanged(nameof(CanOpenAISettings));
				});
				return;
			}
			ChatMessage assistant = new() { Role = "assistant" };
			long requestId = 0;
			ChatConversation requestConversation = null!;
			LLMMessage[] requestMessages = Array.Empty<LLMMessage>();
			string requestContext = string.Empty;
			await SetUiStateAsync(() => {
				Input = string.Empty;
				EnsureConversation(snapshot);
				if (text.StartsWith('/'))
					text = ExpandCommand(text);
				Messages.Add(new ChatMessage { Role = "user", Content = text });
				TrimHistory();
				Messages.Add(assistant);
				requestId = Interlocked.Increment(ref requestGeneration);
				requestConversation = loadedHistory.ActiveConversation;
				requestMessages = Messages.Where(m => m.Content.Length != 0).Select(m => new LLMMessage(m.Role, m.Content)).ToArray();
				requestContext = GetActiveContext(text);
				IsBusy = true;
				ErrorMessage = string.Empty;
				StatusMessage = "Generating…";
			});
			cancellation?.Cancel();
			var cts = new CancellationTokenSource();
			cancellation = cts;
			try
			{
				var provider = await providerFactory.CreateAsync(snapshot, cts.Token).ConfigureAwait(false);
				var request = new LLMRequest(SystemPrompt, requestMessages.Append(new LLMMessage("user", requestContext)).ToArray(), 2048, 0.3);
				var builder = new StringBuilder();
				await foreach (var chunk in provider.CompleteAsync(request, cts.Token).ConfigureAwait(false))
				{
					builder.Append(chunk);
					string contentSnapshot = builder.ToString();
					await Dispatcher.UIThread.InvokeAsync(() => {
						if (requestConversation.Messages.Contains(assistant))
							assistant.Content = contentSnapshot;
					});
				}
				await SetUiStateAsync(() => {
					if (requestId == requestGeneration)
						StatusMessage = "Complete";
				});
				await SetUiStateAsync(SaveHistory);
			}
			catch (OperationCanceledException)
			{
				await SetUiStateAsync(() => {
					if (requestId == requestGeneration)
						StatusMessage = "Canceled";
				});
			}
			catch (Exception ex)
			{
				await SetUiStateAsync(() => {
					if (requestId == requestGeneration)
					{ ErrorMessage = ex.Message; StatusMessage = "Request failed"; }
				});
			}
			finally
			{
				await SetUiStateAsync(() => {
					if (requestId == requestGeneration)
						IsBusy = false;
				});
				if (ReferenceEquals(cancellation, cts))
					cancellation = null;
				cts.Dispose();
			}
		}

		async Task<bool> DispatchLocalCommandAsync(string text)
		{
			if (!text.StartsWith("/", StringComparison.Ordinal))
				return false;
			int space = text.IndexOf(' ');
			string command = (space < 0 ? text : text[..space]).ToLowerInvariant();
			switch (command)
			{
				case "/help":
					await SetUiStateAsync(() => Input = string.Empty);
					await AppendLocalMessageAsync("Supported commands: /help, /clear, /explain, /rename, /audit, /summary.");
					await SetUiStateAsync(() => StatusMessage = "Ready");
					return true;
				case "/clear":
					await SetUiStateAsync(ClearCore);
					return true;
				case "/audit":
				case "/summary":
					await SetUiStateAsync(() => Input = string.Empty);
					await AppendLocalMessageAsync($"{command} is unavailable from chat until its host pipeline is connected. Use the corresponding application command.");
					await SetUiStateAsync(() => StatusMessage = "Command unavailable");
					return true;
				case "/explain":
				case "/rename":
					return false;
				default:
					await SetUiStateAsync(() => Input = string.Empty);
					await AppendLocalMessageAsync($"Unsupported command '{command}'. Type /help for supported commands.");
					await SetUiStateAsync(() => StatusMessage = "Unknown command");
					return true;
			}
		}

		async Task AppendLocalMessageAsync(string content)
		{
			await SetUiStateAsync(() => {
				Messages.Add(new ChatMessage { Role = "assistant", Content = content });
				TrimHistory();
				SaveHistory();
			});
		}

		static Task SetUiStateAsync(Action action)
		{
			if (Dispatcher.UIThread.CheckAccess())
			{ action(); return Task.CompletedTask; }
			return Dispatcher.UIThread.InvokeAsync(action).GetTask();
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
		void Clear()
		{
			if (Dispatcher.UIThread.CheckAccess())
				ClearCore();
			else
				Dispatcher.UIThread.Post(ClearCore);
		}

		void ClearCore()
		{
			cancellation?.Cancel();
			Input = string.Empty;
			Messages.Clear();
			loadedHistory.ActiveConversation.Messages.Clear();
			SaveHistory();
			conversationGeneration++;
			Interlocked.Increment(ref requestGeneration);
			StatusMessage = "Ready";
			ErrorMessage = string.Empty;
		}

		[RelayCommand]
		void NewConversation()
		{
			if (Dispatcher.UIThread.CheckAccess())
				StartConversation(GetCurrentTarget());
			else
				Dispatcher.UIThread.Post(() => StartConversation(GetCurrentTarget()));
		}

		[RelayCommand]
		void SelectConversation(ChatConversation? conversation)
		{
			if (conversation is not null)
				SelectedConversation = conversation;
		}

		[RelayCommand]
		void OpenSettings()
		{
			ContentTabPage tab = dockWorkspace.OpenSingletonTab("options", () => {
				var options = new OptionsPageModel(settingsService, optionPages);
				options.SelectPage<AISettingsViewModel>();
				return dockWorkspace.OpenNewTab(options);
			});
			if (tab.Content is OptionsPageModel existing)
				existing.SelectPage<AISettingsViewModel>();
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
			SyncConversations();
		}

		void StartConversation(AIConversationTarget target)
		{
			SaveHistory();
			Messages.Clear();
			conversationGeneration++;
			ChatConversation conversation = loadedHistory.StartNew(target);
			loadedTarget = target;
			SyncConversations();
			SelectedConversation = conversation;
			OnPropertyChanged(nameof(IsConversationReadOnly));
			StatusMessage = "Ready";
			ErrorMessage = string.Empty;
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
			SyncConversations();
			SelectedConversation = loadedHistory.ActiveConversation;
			OnPropertyChanged(nameof(IsConversationReadOnly));
		}

		void SyncConversations()
		{
			Conversations.Clear();
			foreach (ChatConversation conversation in loadedHistory.Conversations)
				Conversations.Add(conversation);
			OnPropertyChanged(nameof(IsConversationReadOnly));
		}

		void MarkDeletedConversationsReadOnly()
		{
			var liveProfileIds = selectionService.Profiles.Select(profile => profile.Id).ToHashSet(StringComparer.Ordinal);
			foreach (ChatConversation conversation in loadedHistory.Conversations)
			{
				if (conversation.Target is { } target && !liveProfileIds.Contains(target.ProfileId))
				{
					conversation.TargetDeleted = true;
					conversation.ReadOnly = true;
				}
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
