// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public partial class BatchRenameDialog : Window
	{
		readonly BatchRenameDialogViewModel? viewModel;

		public BatchRenameDialog() => InitializeComponent();

		public BatchRenameDialog(ITypeDefinition type, AISelectionSnapshot snapshot, IAIProviderFactory providerFactory)
		{
			InitializeComponent();
			viewModel = new BatchRenameDialogViewModel(type, snapshot, providerFactory);
			DataContext = viewModel;
			Opened += OnOpened;
		}

		async void OnOpened(object? sender, EventArgs e)
		{
			Opened -= OnOpened;
			if (viewModel is not null)
				await viewModel.StartAsync();
		}

		void CloseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
		void ApplyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (viewModel?.ApplyCommand.CanExecute(null) == true)
			{
				viewModel.ApplyCommand.Execute(null);
				AppComposition.TryGetExport<Docking.DockWorkspace>()?.ForceRefreshActiveTab();
				Close();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			viewModel?.Dispose();
			base.OnClosed(e);
		}
	}
}
