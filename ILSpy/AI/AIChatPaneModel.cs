// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Collections.ObjectModel;
using System.Composition;
using System.IO;
using System.Linq;
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
		readonly AssemblyTreeModel assemblyTree;
		CancellationTokenSource? cancellation;
		string loadedHistoryPath = string.Empty;

		public ObservableCollection<ChatMessage> Messages { get; } = new();
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(ShowSuggestions))]
		string input = string.Empty;
		[ObservableProperty] bool isBusy;
		[ObservableProperty] string statusMessage = "Ready";
		[ObservableProperty] string errorMessage = string.Empty;
		public bool ShowSuggestions => Input.StartsWith("/", StringComparison.Ordinal);
		public string[] CommandSuggestions { get; } = { "/explain", "/rename ", "/audit", "/summary" };

		[ImportingConstructor]
		public AIChatPaneModel(SettingsService settingsService, IAIProviderFactory providerFactory, AssemblyTreeModel assemblyTree)
		{
			this.settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			this.assemblyTree = assemblyTree ?? throw new ArgumentNullException(nameof(assemblyTree));
			Id = PaneContentId;
			Title = "AI Chat";
			assemblyTree.PropertyChanged += OnAssemblyTreePropertyChanged;
			LoadHistory();
		}

		void OnAssemblyTreePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName != nameof(AssemblyTreeModel.SelectedItem))
				return;
			cancellation?.Cancel();
			SaveHistory(loadedHistoryPath);
			Messages.Clear();
			LoadHistory();
		}

		[RelayCommand]
		async Task SendAsync()
		{
			string text = Input.Trim();
			if (text.Length == 0 || IsBusy)
				return;
			Input = string.Empty;
			if (text.StartsWith('/'))
				text = ExpandCommand(text);
			var user = new ChatMessage { Role = "user", Content = text };
			Messages.Add(user);
			TrimHistory();
			var assistant = new ChatMessage { Role = "assistant" };
			Messages.Add(assistant);
			IsBusy = true;
			ErrorMessage = string.Empty;
			StatusMessage = "Generating…";
			cancellation?.Cancel();
			var cts = new CancellationTokenSource();
			cancellation = cts;
			try
			{
				var requestMessages = Messages.Where(m => m.Content.Length != 0).Select(m => new LLMMessage(m.Role, m.Content)).ToArray();
				var provider = await providerFactory.CreateAsync(settingsService.AISettings, cts.Token).ConfigureAwait(false);
				var context = GetActiveContext(text);
				var request = new LLMRequest(SystemPrompt, requestMessages.Append(new LLMMessage("user", context)).ToArray(), 2048, 0.3);
				var builder = new StringBuilder();
				await foreach (var chunk in provider.CompleteAsync(request, cts.Token).ConfigureAwait(false))
				{
					builder.Append(chunk);
					string snapshot = builder.ToString();
					await Dispatcher.UIThread.InvokeAsync(() => assistant.Content = snapshot);
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
		void Clear() { cancellation?.Cancel(); Messages.Clear(); SaveHistory(); StatusMessage = "Ready"; ErrorMessage = string.Empty; }

		[RelayCommand]
		void Export()
		{
			string path = GetHistoryPath();
			if (path.Length != 0)
			{
				try
				{ File.WriteAllText(Path.ChangeExtension(path, ".md"), new ChatHistory { Messages = Messages.ToList() }.ToMarkdown(), Encoding.UTF8); }
				catch (UnauthorizedAccessException) { StatusMessage = "Export failed"; return; }
				catch (IOException) { StatusMessage = "Export failed"; return; }
			}
			StatusMessage = "Exported";
		}

		void TrimHistory() { while (Messages.Count > MaxMessages) Messages.RemoveAt(0); }
		string GetHistoryPath()
		{
			var entity = (assemblyTree.SelectedItem as IMemberTreeNode)?.Member;
			string? file = entity?.ParentModule?.MetadataFile?.FileName;
			if (string.IsNullOrWhiteSpace(file) && assemblyTree.SelectedItem is ILSpyTreeNode node)
				file = node.AncestorsAndSelf().OfType<AssemblyTreeNode>().FirstOrDefault()?.LoadedAssembly.FileName;
			return string.IsNullOrWhiteSpace(file) ? string.Empty : Path.Combine(Path.GetDirectoryName(file)!, ".ilspy-chat-history.json");
		}
		void LoadHistory() { string path = GetHistoryPath(); loadedHistoryPath = path; if (path.Length == 0) return; var history = ChatHistory.Load(path); foreach (var message in history.Messages.TakeLast(MaxMessages)) Messages.Add(message); }
		void SaveHistory() => SaveHistory(GetHistoryPath());
		void SaveHistory(string path)
		{
			if (path.Length == 0)
				return;
			try
			{ new ChatHistory { AssemblyPath = GetAssemblyPath(), Messages = Messages.ToList() }.Save(path); }
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
		public void Dispose() { assemblyTree.PropertyChanged -= OnAssemblyTreePropertyChanged; cancellation?.Cancel(); SaveHistory(); }
	}
}
