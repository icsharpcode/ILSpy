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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy
{
	public class LoadedNugetPackage : INotifyPropertyChanged
	{
		public List<Entry> Entries { get; } = new List<Entry>();
		public List<Entry> SelectedEntries { get; } = new List<Entry>();

		public LoadedNugetPackage(string file)
		{
			using (var archive = ZipFile.OpenRead(file))
			{
				foreach (var entry in archive.Entries)
				{
					switch (Path.GetExtension(entry.FullName))
					{
						case ".dll":
						case ".exe":
							var memory = new MemoryStream();
							entry.Open().CopyTo(memory);
							memory.Position = 0;
							var e = new Entry(Uri.UnescapeDataString(entry.FullName), memory);
							e.PropertyChanged += EntryPropertyChanged;
							Entries.Add(e);
							break;
					}
				}
			}
		}

		void EntryPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Entry.IsSelected))
			{
				var entry = (Entry)sender;
				if (entry.IsSelected)
					SelectedEntries.Add(entry);
				else
					SelectedEntries.Remove(entry);
				OnPropertyChanged(nameof(SelectedEntries));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Entry : INotifyPropertyChanged
	{
		public string Name { get; }

		public bool IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value)
				{
					isSelected = value;
					OnPropertyChanged();
				}
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChanged?.Invoke(this, e);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		public Stream Stream { get; }

		bool isSelected;

		public event PropertyChangedEventHandler PropertyChanged;

		public Entry(string name, Stream stream)
		{
			this.Name = name;
			this.Stream = stream;
		}
	}
}
