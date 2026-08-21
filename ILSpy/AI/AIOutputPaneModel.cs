// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Composition;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AI.Decompiler;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using Microsoft.Extensions.Logging;

namespace ICSharpCode.ILSpy.AI
{
	[Export]
	[ExportToolPane(ContentId = PaneContentId, Alignment = ToolPaneAlignment.Bottom, Order = 2, IsVisibleByDefault = false)]
	[Shared]
	public sealed partial class AIOutputPaneModel : ToolPaneModel, IDisposable
	{
		public const string PaneContentId = "AIOutput";

		readonly AISelectionService selectionService;
		readonly IAIProviderFactory providerFactory;
		readonly ILogger logger;
		CancellationTokenSource? cancellation;

		[ObservableProperty]
		string targetName = string.Empty;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		string response = string.Empty;
		[ObservableProperty]
		string errorMessage = string.Empty;
		[ObservableProperty]
		string statusMessage = "Ready";
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		bool isBusy;
		[ObservableProperty]
		[NotifyPropertyChangedFor(nameof(CanCopy))]
		bool isComplete;

		[ImportingConstructor]
		public AIOutputPaneModel(AISelectionService selectionService, IAIProviderFactory providerFactory, ILoggerFactory loggerFactory)
		{
			this.selectionService = selectionService ?? throw new ArgumentNullException(nameof(selectionService));
			this.providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			logger = loggerFactory?.CreateLogger<AIOutputPaneModel>() ?? throw new ArgumentNullException(nameof(loggerFactory));
			Id = PaneContentId;
			Title = "AI Output";
		}

		public bool CanCopy => !IsBusy && IsComplete && Response.Length != 0;

		public Task StartAsync(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var decompiler = AIEntityDecompilation.CreateDecompiler(entity);
			IEntity resolvedEntity = AIEntityDecompilation.ResolveEntity(entity, decompiler)
				?? throw new InvalidOperationException($"Failed to resolve entity '{entity.FullName}' in the decompiler type system.");
			return StartAsync(entity.FullName, async token => {
				AISelectionSnapshot snapshot = await selectionService.ResolveSnapshotAsync(token).ConfigureAwait(false);
				return new AIExplanationService(snapshot, providerFactory).ExplainStreamingAsync(resolvedEntity, decompiler, token);
			});
		}

		public Task StartAsync(string name, Func<CancellationToken, IAsyncEnumerable<string>> streamFactory)
		{
			ArgumentNullException.ThrowIfNull(streamFactory);
			return StartAsync(name, token => Task.FromResult(streamFactory(token)));
		}

		public async Task StartAsync(string name, Func<CancellationToken, Task<IAsyncEnumerable<string>>> streamFactory)
		{
			ArgumentNullException.ThrowIfNull(streamFactory);
			cancellation?.Cancel();
			var requestCancellation = new CancellationTokenSource();
			cancellation = requestCancellation;
			TargetName = name ?? string.Empty;
			Response = string.Empty;
			IsComplete = false;
			ErrorMessage = string.Empty;
			StatusMessage = "Generating…";
			IsBusy = true;
			logger.LogInformation("Starting AI request for '{TargetName}'", name);
			try
			{
				await Task.Run(() => ConsumeAsync(streamFactory, requestCancellation), requestCancellation.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
				logger.LogInformation("AI request for '{TargetName}' was canceled", name);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						StatusMessage = "Canceled";
					}
				});
			}
			catch (AIRequestException exception)
			{
				logger.LogError(exception, "AI request failed for '{TargetName}': {Message}", name, exception.Message);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = exception.Message;
						StatusMessage = "Request failed";
					}
				});
			}
			catch (AIConfigurationException exception)
			{
				logger.LogError(exception, "AI configuration error for '{TargetName}': {Message}", name, exception.Message);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = exception.Message;
						StatusMessage = "Configuration required";
					}
				});
			}
			catch (Exception exception)
			{
				logger.LogError(exception, "Unexpected error during AI request for '{TargetName}': {ExceptionType} - {Message}\nStack trace: {StackTrace}",
					name, exception.GetType().FullName, exception.Message, exception.StackTrace);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsComplete = false;
						ErrorMessage = $"The AI request failed: {exception.GetType().Name}: {exception.Message}";
						StatusMessage = "Request failed";
					}
				});
			}
			finally
			{
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
					{
						IsBusy = false;
						cancellation = null;
					}
				});
				requestCancellation.Dispose();
			}
		}

		async Task ConsumeAsync(Func<CancellationToken, Task<IAsyncEnumerable<string>>> streamFactory, CancellationTokenSource requestCancellation)
		{
			var response = new StringBuilder();
			logger.LogDebug("Starting to consume AI response stream");
			int chunkCount = 0;
			int chunksSinceUpdate = 0;
			// Batch full-document replacement on the UI thread to a whole number of chunks; this
			// cuts the number of document rebuilds (and any repaint flicker) without holding the
			// UI thread for every single token.
			const int UpdateInterval = 5;

			IAsyncEnumerable<string> stream = await streamFactory(requestCancellation.Token).ConfigureAwait(false);
			await foreach (string chunk in stream.ConfigureAwait(false))
			{
				if (string.IsNullOrEmpty(chunk))
					continue;
				chunkCount++;
				chunksSinceUpdate++;
				response.Append(chunk);
				logger.LogTrace("Received chunk #{ChunkNumber}, length: {Length}", chunkCount, chunk.Length);

				if (chunksSinceUpdate >= UpdateInterval)
				{
					await Dispatcher.UIThread.InvokeAsync(() => {
						if (ReferenceEquals(cancellation, requestCancellation))
							Response = response.ToString();
					});
					chunksSinceUpdate = 0;
				}
			}

			// Always push the final (possibly partial) buffer so the last chunks are shown.
			logger.LogInformation("AI response stream complete. Total chunks: {ChunkCount}, total length: {Length}", chunkCount, response.Length);
			await Dispatcher.UIThread.InvokeAsync(() => {
				if (ReferenceEquals(cancellation, requestCancellation))
				{
					Response = response.ToString();
					IsComplete = response.Length != 0;
					StatusMessage = response.Length == 0 ? "The provider returned an empty response." : "Complete";
				}
			});
		}

		[RelayCommand]
		void Cancel()
		{
			var requestCancellation = cancellation;
			if (requestCancellation is null)
				return;
			cancellation = null;
			requestCancellation.Cancel();
			IsBusy = false;
			IsComplete = false;
			StatusMessage = "Canceled";
		}

		[RelayCommand]
		void Clear()
		{
			var requestCancellation = cancellation;
			cancellation = null;
			requestCancellation?.Cancel();
			IsBusy = false;
			Response = string.Empty;
			IsComplete = false;
			ErrorMessage = string.Empty;
			TargetName = string.Empty;
			StatusMessage = "Ready";
		}

		public async Task<bool> CopyToClipboardAsync(IClipboard? clipboard)
		{
			if (!CanCopy || clipboard is null)
				return false;
			await clipboard.SetTextAsync(Response);
			StatusMessage = "Copied to clipboard";
			return true;
		}

		public void Dispose()
		{
			cancellation?.Cancel();
			cancellation?.Dispose();
		}
	}
}
