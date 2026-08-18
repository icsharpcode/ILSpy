// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public partial class ExplainDialog : Window
	{
		readonly ExplainDialogViewModel? viewModel;

		public ExplainDialog()
		{
			InitializeComponent();
		}

		public ExplainDialog(IEntity entity, AISettings settings, IAIProviderFactory providerFactory)
		{
			InitializeComponent();
			viewModel = new ExplainDialogViewModel(entity, settings, providerFactory);
			DataContext = viewModel;
			Opened += OnOpened;
		}

		async void OnOpened(object? sender, EventArgs e)
		{
			Opened -= OnOpened;
			if (viewModel is not null)
				await viewModel.StartAsync();
		}

		async void CopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (viewModel is not null)
				await viewModel.CopyToClipboardAsync(Clipboard);
		}

		void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();

		protected override void OnClosed(EventArgs e)
		{
			viewModel?.Dispose();
			base.OnClosed(e);
		}
	}
}
