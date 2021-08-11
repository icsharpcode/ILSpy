#nullable enable
// Copyright (c) 2017 Daniel Grunwald
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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.IL
{
	public class ILAstWritingOptions : INotifyPropertyChanged
	{
		private bool useLogicOperationSugar;
		private bool useFieldSugar;
		private bool showILRanges;
		private bool showChildIndexInBlock;

		/// <summary>
		/// Sugar for logic.not/and/or.
		/// </summary>
		public bool UseLogicOperationSugar {
			get { return useLogicOperationSugar; }
			set {
				if (useLogicOperationSugar != value)
				{
					useLogicOperationSugar = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Sugar for ldfld/stfld.
		/// </summary>
		public bool UseFieldSugar {
			get { return useFieldSugar; }
			set {
				if (useFieldSugar != value)
				{
					useFieldSugar = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Show IL ranges in ILAst output.
		/// </summary>
		public bool ShowILRanges {
			get { return showILRanges; }
			set {
				if (showILRanges != value)
				{
					showILRanges = value;
					OnPropertyChanged();
				}
			}
		}

		/// <summary>
		/// Show the child index of the instruction in ILAst output.
		/// </summary>
		public bool ShowChildIndexInBlock {
			get { return showChildIndexInBlock; }
			set {
				if (showChildIndexInBlock != value)
				{
					showChildIndexInBlock = value;
					OnPropertyChanged();
				}
			}
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
