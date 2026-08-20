// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;

using System.Collections.Specialized;
using System.ComponentModel;

using ICSharpCode.ILSpy.AI.Controls;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.AI
{
	public partial class AIChatPane : UserControl
	{
		readonly AIFollowTailController followTail = new();
		AIChatPaneModel? model;

		public AIChatPane()
		{
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
			AttachedToVisualTree += OnAttachedToVisualTree;
		}

		void OnAttachedToVisualTree(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
		{
			Dispatcher.UIThread.Post(() => {
				followTail.Attach(AIEditorScrollState.FindViewer(MessageList));
				ScheduleRestore();
			}, DispatcherPriority.Loaded);
		}

		void OnDataContextChanged(object? sender, EventArgs e)
		{
			UnbindModel();
			if (DataContext is AIChatPaneModel next)
			{
				model = next;
				model.Messages.CollectionChanged += OnMessagesChanged;
				foreach (var message in model.Messages)
					message.PropertyChanged += OnMessagePropertyChanged;
			}
		}

		void UnbindModel()
		{
			if (model is null)
				return;
			model.Messages.CollectionChanged -= OnMessagesChanged;
			foreach (var message in model.Messages)
				message.PropertyChanged -= OnMessagePropertyChanged;
			model = null;
		}

		void OnMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems is not null)
				foreach (ChatMessage message in e.OldItems)
					message.PropertyChanged -= OnMessagePropertyChanged;
			if (e.NewItems is not null)
				foreach (ChatMessage message in e.NewItems)
					message.PropertyChanged += OnMessagePropertyChanged;
			ScheduleRestore();
		}

		void OnMessagePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ChatMessage.Content))
				ScheduleRestore();
		}

		async void OnProfileChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (DataContext is AIChatPaneModel chat && e.AddedItems.Count > 0)
				await chat.SelectProfileCommand.ExecuteAsync(e.AddedItems[0] as AIProfile);
		}

		async void OnModelChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (DataContext is AIChatPaneModel chat && e.AddedItems.Count > 0)
				await chat.SelectModelCommand.ExecuteAsync(e.AddedItems[0] as string);
		}

		void ScheduleRestore()
		{
			var snapshot = followTail.Capture();
			followTail.RestoreLater(snapshot);
		}

		protected override void OnDetachedFromVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
		{
			UnbindModel();
			followTail.Dispose();
			base.OnDetachedFromVisualTree(e);
		}
	}
}
