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
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.ILSpy
{
	abstract class CommandWrapper : ICommand
	{
		private readonly ICommand wrappedCommand;

		protected CommandWrapper(ICommand wrappedCommand)
		{
			this.wrappedCommand = wrappedCommand;

			Application.Current.MainWindow?.CommandBindings.Add(new CommandBinding(wrappedCommand, OnExecute, OnCanExecute));
		}

		public static ICommand Unwrap(ICommand command)
		{
			if (command is CommandWrapper w)
				return w.wrappedCommand;

			return command;
		}

		public event EventHandler CanExecuteChanged {
			add { wrappedCommand.CanExecuteChanged += value; }
			remove { wrappedCommand.CanExecuteChanged -= value; }
		}

		public void Execute(object parameter)
		{
			wrappedCommand.Execute(parameter);
		}

		public bool CanExecute(object parameter)
		{
			return wrappedCommand.CanExecute(parameter);
		}

		protected abstract void OnExecute(object sender, ExecutedRoutedEventArgs e);

		protected virtual void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
	}
}
