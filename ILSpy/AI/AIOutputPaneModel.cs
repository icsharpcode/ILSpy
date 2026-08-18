// Copyright (c) 2026 Masroor
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

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.AI;
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

		readonly AISettings settings;
		readonly AIExplanationService explanationService;
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
		public AIOutputPaneModel(SettingsService settingsService, IAIProviderFactory providerFactory, ILoggerFactory loggerFactory)
		{
			settings = settingsService?.AISettings ?? throw new ArgumentNullException(nameof(settingsService));
			explanationService = new AIExplanationService(settings, providerFactory ?? throw new ArgumentNullException(nameof(providerFactory)));
			logger = loggerFactory?.CreateLogger<AIOutputPaneModel>() ?? throw new ArgumentNullException(nameof(loggerFactory));
			Id = PaneContentId;
			Title = "AI Output";
		}

		public bool CanCopy => !IsBusy && IsComplete && Response.Length != 0;

		public Task StartAsync(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			MetadataFile module = entity.ParentModule?.MetadataFile
				?? throw new InvalidOperationException("The selected symbol has no decompilable module.");
			var decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(true), new ICSharpCode.Decompiler.DecompilerSettings());

			// Re-resolve the entity from the new decompiler's type system using its metadata token.
			// The entity was resolved from a different decompiler instance, so we cannot pass it directly
			// to ExplainStreamingAsync — ContextBuilder.Build validates that entity.ParentModule equals
			// decompiler.TypeSystem.MainModule via ReferenceEquals, which would fail.
			IEntity resolvedEntity = ResolveEntity(entity, decompiler)
				?? throw new InvalidOperationException($"Failed to resolve entity '{entity.FullName}' in the decompiler type system.");
			return StartAsync(entity.FullName, token => explanationService.ExplainStreamingAsync(resolvedEntity, decompiler, token));
		}

		static IEntity? ResolveEntity(IEntity entity, CSharpDecompiler decompiler)
		{
			var token = entity.MetadataToken;
			if (token.IsNil)
				return null;
			return token.Kind switch {
				System.Reflection.Metadata.HandleKind.TypeDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((System.Reflection.Metadata.TypeDefinitionHandle)token),
				System.Reflection.Metadata.HandleKind.MethodDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((System.Reflection.Metadata.MethodDefinitionHandle)token),
				System.Reflection.Metadata.HandleKind.FieldDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((System.Reflection.Metadata.FieldDefinitionHandle)token),
				System.Reflection.Metadata.HandleKind.PropertyDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((System.Reflection.Metadata.PropertyDefinitionHandle)token),
				System.Reflection.Metadata.HandleKind.EventDefinition =>
					decompiler.TypeSystem.MainModule.GetDefinition((System.Reflection.Metadata.EventDefinitionHandle)token),
				_ => null
			};
		}

		public async Task StartAsync(string name, Func<CancellationToken, IAsyncEnumerable<string>> streamFactory)
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

		async Task ConsumeAsync(Func<CancellationToken, IAsyncEnumerable<string>> streamFactory, CancellationTokenSource requestCancellation)
		{
			var response = new StringBuilder();
			logger.LogDebug("Starting to consume AI response stream");
			int chunkCount = 0;
			await foreach (string chunk in streamFactory(requestCancellation.Token).ConfigureAwait(false))
			{
				if (string.IsNullOrEmpty(chunk))
					continue;
				chunkCount++;
				response.Append(chunk);
				logger.LogTrace("Received chunk #{ChunkNumber}, length: {Length}", chunkCount, chunk.Length);
				await Dispatcher.UIThread.InvokeAsync(() => {
					if (ReferenceEquals(cancellation, requestCancellation))
						Response = response.ToString();
				});
			}
			logger.LogInformation("AI response stream complete. Total chunks: {ChunkCount}, total length: {Length}", chunkCount, response.Length);
			await Dispatcher.UIThread.InvokeAsync(() => {
				if (ReferenceEquals(cancellation, requestCancellation))
				{
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
