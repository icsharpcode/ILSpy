// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpyX.AI;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AI
{
	public partial class RenameDialog : Window
	{
		readonly RenameDialogViewModel? viewModel;

		public RenameDialog() => InitializeComponent();

		public RenameDialog(IEntity entity, AISettings settings, IAIProviderFactory providerFactory)
		{
			InitializeComponent();
			viewModel = new RenameDialogViewModel(entity, settings, providerFactory);
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
