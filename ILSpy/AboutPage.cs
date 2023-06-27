// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;

using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpy.Updates;
using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Help), Header = nameof(Resources._About), MenuOrder = 99999)]
	sealed class AboutPage : SimpleCommand
	{
		public override void Execute(object parameter)
		{
			MainWindow.Instance.NavigateTo(
				new RequestNavigateEventArgs(new Uri("resource://aboutpage"), null),
				inNewTabPage: true
			);
		}

		public static void Display(DecompilerTextView textView)
		{
			AvalonEditTextOutput output = new AvalonEditTextOutput() {
				Title = Resources.About,
				EnableHyperlinks = true
			};
			output.WriteLine(Resources.ILSpyVersion + DecompilerVersionInfo.FullVersion);

			string prodVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(Uri).Assembly.Location).ProductVersion;
			output.WriteLine(Resources.NETFrameworkVersion + prodVersion);

			output.AddUIElement(
			delegate {
				StackPanel stackPanel = new StackPanel();
				stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
				stackPanel.Orientation = Orientation.Horizontal;
				if (NotifyOfUpdatesStrategy.LatestAvailableVersion == null)
				{
					AddUpdateCheckButton(stackPanel, textView);
				}
				else
				{
					// we already retrieved the latest version sometime earlier
					ShowAvailableVersion(NotifyOfUpdatesStrategy.LatestAvailableVersion, stackPanel);
				}
				CheckBox checkBox = new CheckBox();
				checkBox.Margin = new Thickness(4);
				checkBox.Content = Resources.AutomaticallyCheckUpdatesEveryWeek;
				UpdateSettings settings = new UpdateSettings(ILSpySettings.Load());
				checkBox.SetBinding(CheckBox.IsCheckedProperty, new Binding("AutomaticUpdateCheckEnabled") { Source = settings });
				return new StackPanel {
					Margin = new Thickness(0, 4, 0, 0),
					Cursor = Cursors.Arrow,
					Children = { stackPanel, checkBox }
				};
			});
			output.WriteLine();

			foreach (var plugin in App.ExportProvider.GetExportedValues<IAboutPageAddition>())
				plugin.Write(output);
			output.WriteLine();
			output.Address = new Uri("resource://AboutPage");
			using (Stream s = typeof(AboutPage).Assembly.GetManifestResourceStream(typeof(AboutPage), Resources.ILSpyAboutPageTxt))
			{
				using (StreamReader r = new StreamReader(s))
				{
					string line;
					while ((line = r.ReadLine()) != null)
					{
						output.WriteLine(line);
					}
				}
			}
			output.AddVisualLineElementGenerator(new MyLinkElementGenerator("MIT License", "resource:license.txt"));
			output.AddVisualLineElementGenerator(new MyLinkElementGenerator("third-party notices", "resource:third-party-notices.txt"));
			textView.ShowText(output);
		}

		sealed class MyLinkElementGenerator : LinkElementGenerator
		{
			readonly Uri uri;

			public MyLinkElementGenerator(string matchText, string url) : base(new Regex(Regex.Escape(matchText)))
			{
				this.uri = new Uri(url);
				this.RequireControlModifierForClick = false;
			}

			protected override Uri GetUriFromMatch(Match match)
			{
				return uri;
			}
		}

		static void AddUpdateCheckButton(StackPanel stackPanel, DecompilerTextView textView)
		{
			Button button = ThemeManager.Current.CreateButton();
			button.Content = Resources.CheckUpdates;
			button.Cursor = Cursors.Arrow;
			stackPanel.Children.Add(button);

			button.Click += async delegate {
				button.Content = Resources.Checking;
				button.IsEnabled = false;

				try
				{
					AvailableVersionInfo vInfo = await NotifyOfUpdatesStrategy.GetLatestVersionAsync();
					stackPanel.Children.Clear();
					ShowAvailableVersion(vInfo, stackPanel);
				}
				catch (Exception ex)
				{
					AvalonEditTextOutput exceptionOutput = new AvalonEditTextOutput();
					exceptionOutput.WriteLine(ex.ToString());
					textView.ShowText(exceptionOutput);
				}
			};
		}

		static void ShowAvailableVersion(AvailableVersionInfo availableVersion, StackPanel stackPanel)
		{
			if (AppUpdateService.CurrentVersion == availableVersion.Version)
			{
				stackPanel.Children.Add(
					new Image {
						Width = 16, Height = 16,
						Source = Images.OK,
						Margin = new Thickness(4, 0, 4, 0)
					});
				stackPanel.Children.Add(
					new TextBlock {
						Text = Resources.UsingLatestRelease,
						VerticalAlignment = VerticalAlignment.Bottom
					});
			}
			else if (AppUpdateService.CurrentVersion < availableVersion.Version)
			{
				stackPanel.Children.Add(
					new TextBlock {
						Text = string.Format(Resources.VersionAvailable, availableVersion.Version),
						Margin = new Thickness(0, 0, 8, 0),
						VerticalAlignment = VerticalAlignment.Bottom
					});
				if (availableVersion.DownloadUrl != null)
				{
					Button button = ThemeManager.Current.CreateButton();
					button.Content = Resources.Download;
					button.Cursor = Cursors.Arrow;
					button.Click += delegate {
						MainWindow.OpenLink(availableVersion.DownloadUrl);
					};
					stackPanel.Children.Add(button);
				}
			}
			else
			{
				stackPanel.Children.Add(new TextBlock { Text = Resources.UsingNightlyBuildNewerThanLatestRelease });
			}
		}
	}

	/// <summary>
	/// Interface that allows plugins to extend the about page.
	/// </summary>
	public interface IAboutPageAddition
	{
		void Write(ISmartTextOutput textOutput);
	}
}
