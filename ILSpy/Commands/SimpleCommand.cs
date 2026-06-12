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

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Minimal ICommand base. Avalonia has no global RequerySuggested signal like WPF's CommandManager,
	/// so CanExecuteChanged is routed through our own <see cref="CommandManager"/> (mirroring how WPF's
	/// SimpleCommand routed it through CommandManager.RequerySuggested). State-change sites call
	/// <see cref="CommandManager.InvalidateRequerySuggested"/> and every bound command re-evaluates --
	/// no per-command wiring, and the menu/toolbar item updates its enabled state on every platform.
	/// </summary>
	public abstract class SimpleCommand : ICommand
	{
		public event EventHandler? CanExecuteChanged {
			add { if (value is not null) CommandManager.AddRequerySuggested(value); }
			remove { if (value is not null) CommandManager.RemoveRequerySuggested(value); }
		}

		public abstract void Execute(object? parameter);

		public virtual bool CanExecute(object? parameter) => true;

		/// <summary>Asks all bound commands to re-evaluate CanExecute (a global invalidate -- the
		/// re-query is shared, so this is not scoped to this single command).</summary>
		protected static void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
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
