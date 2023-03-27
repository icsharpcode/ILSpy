using ICSharpCode.ILSpy.Options;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.Settings;

using System;
using System.Windows.Navigation;
using OpenAI_API;
using OpenAI_API.Completions;
using OpenAI_API.Models;
using System.Threading.Tasks;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.TextView
{
	public static class ExplainThisCodeTabPageModelExtensions
	{
		public static void ShowExplainThisCode(this TabPageModel tabPage, object state, Action<DecompilerTextView, object> action)
		{
			var textView = new DecompilerTextView();
			textView.textEditor.WordWrap = true;
			tabPage.Content = textView;
			tabPage.Title = Resources.ExplainThisCode;

			action(textView, state);
		}
	}

	public record ExplainThisCodeState(string TextToExplain, string ApiKey);

	[ExportContextMenuEntry(Header = nameof(Resources.ExplainThisCode), Category = nameof(Resources.Editor))]
	sealed class ExplainThisCode : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			return context.TextView != null;
		}

		public bool IsEnabled(TextViewContext context)
		{
			// Nothing selected? Nothing to explain
			if (!(context.TextView != null && context.TextView.textEditor.SelectionLength > 0))
				return false;

			if (!MiscSettingsPanel.CurrentMiscSettings.EnableExplainThisCode)
				return false;

			return true;
		}

		public void Execute(TextViewContext context)
		{
			MainWindow.Instance.NavigateTo(
				new RequestNavigateEventArgs(new Uri("resource://explainthiscode"), null),
				inNewTabPage: true,
				state: new ExplainThisCodeState(context.TextView.textEditor.SelectedText, MiscSettingsPanel.CurrentMiscSettings.OpenAIApiKey)
			);
		}

		public static void Display(DecompilerTextView textView, object state)
		{
			AvalonEditTextOutput output = new AvalonEditTextOutput() {
				Title = Resources.ExplainThisCode,
				EnableHyperlinks = true
			};

			output.WriteLine("Loading...");
			textView.ShowText(output);

			GetExplanation(textView, state as ExplainThisCodeState);
		}

		private static async Task GetExplanation(DecompilerTextView textView, ExplainThisCodeState state)
		{
			AvalonEditTextOutput output = new AvalonEditTextOutput() {
				Title = Resources.ExplainThisCode,
				EnableHyperlinks = true
			};

			output.WriteLine("Original code:\r\n");
			output.WriteLine(state.TextToExplain);
			output.WriteLine();

			try
			{
				OpenAIAPI api = new OpenAIAPI(state.ApiKey);

				CompletionResult result = await api.Completions.CreateCompletionAsync(
					new CompletionRequest("Explain this code:\n" + state.TextToExplain,
					max_tokens: 2048,
					model: Model.DavinciText,
					temperature: 0.1));

				foreach (var c in result.Completions)
				{
					output.WriteLine(c.Text);
				}

				if (result.Completions.Count == 0)
				{
					output.WriteLine("No explanation was sent back");
				}
			}
			catch (Exception ex)
			{
				output.WriteLine("Failed to get explanation: " + ex.ToString());
			}

			textView.ShowText(output);
		}
	}
}
