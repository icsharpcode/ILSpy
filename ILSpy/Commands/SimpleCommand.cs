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

using System;
using System.Windows.Input;

using global::Avalonia.Data;

namespace ILSpy.Commands
{
	/// <summary>
	/// Minimal ICommand base. Avalonia has no global RequerySuggested signal like WPF's
	/// CommandManager, so derived commands fire CanExecuteChanged themselves when they care.
	/// MenuItems also re-query CanExecute when the parent menu opens, which covers most cases.
	/// </summary>
	public abstract class SimpleCommand : ICommand
	{
		public event EventHandler? CanExecuteChanged;

		public abstract void Execute(object? parameter);

		public virtual bool CanExecute(object? parameter) => true;

		protected void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	/// <summary>
	/// Allows a command to declare an Avalonia <see cref="BindingBase"/> that the menu/toolbar
	/// builder should attach to the host item's CommandParameter property — used when the
	/// parameter must come from the visual tree (e.g. the owning Window) rather than be a
	/// fixed value.
	/// </summary>
	public interface IProvideParameterBinding
	{
		BindingBase ParameterBinding { get; }
	}
}
