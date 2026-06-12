// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

#if DEBUG

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Thin renderer over <see cref="DebugStepsPaneModel"/>. All cross-language /
	/// cross-decompile state lives on the ViewModel, so the View has no event subscriptions
	/// and no awareness of language switches — it just binds. The two pointer / keyboard
	/// handlers below translate user gestures into ViewModel command invocations.
	/// </summary>
	public partial class DebugSteps : UserControl
	{
		public DebugSteps()
		{
			InitializeComponent();
		}

		void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
			=> (DataContext as DebugStepsPaneModel)?.ShowStateAfterCommand.Execute(null);

		void OnTreeKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Return)
				return;
			if (DataContext is not DebugStepsPaneModel vm)
				return;
			if ((e.KeyModifiers & KeyModifiers.Shift) != 0)
				vm.ShowStateBeforeCommand.Execute(null);
			else
				vm.ShowStateAfterCommand.Execute(null);
			e.Handled = true;
		}
	}
}

#endif
