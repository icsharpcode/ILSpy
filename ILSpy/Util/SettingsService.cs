// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Xml.Linq;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using DecompilerSettings = ICSharpCode.ILSpy.Options.DecompilerSettings;

#nullable enable

namespace ICSharpCode.ILSpy.Util
{
	public interface IChildSettings
	{
		ISettingsSection Parent { get; }
	}

	public interface ISettingsSection : INotifyPropertyChanged
	{
		XName SectionName { get; }

		void LoadFromXml(XElement section);

		XElement SaveToXml();
	}

	public abstract class SettingsServiceBase(ISettingsProvider spySettings)
	{
		protected readonly ConcurrentDictionary<Type, ISettingsSection> sections = new();

		protected ISettingsProvider SpySettings { get; set; } = spySettings;

		public T GetSettings<T>() where T : ISettingsSection, new()
		{
			return (T)sections.GetOrAdd(typeof(T), _ => {
				T section = new T();

				var sectionElement = SpySettings[section.SectionName];

				section.LoadFromXml(sectionElement);
				section.PropertyChanged += Section_PropertyChanged;

				return section;
			});
		}

		protected static void SaveSection(ISettingsSection section, XElement root)
		{
			var element = section.SaveToXml();

			var existingElement = root.Element(section.SectionName);
			if (existingElement != null)
				existingElement.ReplaceWith(element);
			else
				root.Add(element);
		}

		protected virtual void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
		}
	}

	public class SettingsSnapshot(SettingsService parent, ISettingsProvider spySettings) : SettingsServiceBase(spySettings)
	{
		public void Save()
		{
			SpySettings.Update(root => {
				foreach (var section in sections.Values)
				{
					SaveSection(section, root);
				}
			});

			parent.Reload();
		}
	}

	public class SettingsService() : SettingsServiceBase(LoadSettings())
	{
		public SessionSettings SessionSettings => GetSettings<SessionSettings>();

		public DecompilerSettings DecompilerSettings => GetSettings<DecompilerSettings>();

		public DisplaySettings DisplaySettings => GetSettings<DisplaySettings>();

		public MiscSettings MiscSettings => GetSettings<MiscSettings>();

		private AssemblyListManager? assemblyListManager;
		public AssemblyListManager AssemblyListManager => assemblyListManager ??= new(SpySettings) {
			ApplyWinRTProjections = DecompilerSettings.ApplyWindowsRuntimeProjections,
			UseDebugSymbols = DecompilerSettings.UseDebugSymbols
		};

		public AssemblyList LoadInitialAssemblyList()
		{
			var loadPreviousAssemblies = MiscSettings.LoadPreviousAssemblies;

			if (loadPreviousAssemblies)
			{
				return AssemblyListManager.LoadList(SessionSettings.ActiveAssemblyList);
			}
			else
			{
				AssemblyListManager.ClearAll();
				return AssemblyListManager.CreateList(AssemblyListManager.DefaultListName);
			}
		}

		public AssemblyList CreateEmptyAssemblyList()
		{
			return AssemblyListManager.CreateList(string.Empty);
		}

		private bool reloading;

		public void Reload()
		{
			reloading = true;

			try
			{
				SpySettings = ILSpySettings.Load();

				foreach (var section in sections.Values)
				{
					var element = SpySettings[section.SectionName];

					section.LoadFromXml(element);
				}
			}
			finally
			{
				reloading = false;
			}
		}

		public SettingsSnapshot CreateSnapshot()
		{
			return new(this, SpySettings);
		}

		protected override void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			base.Section_PropertyChanged(sender, e);

			if (!reloading)
			{
				var section = (sender as IChildSettings)?.Parent ?? sender as ISettingsSection;

				if (section != null)
				{
					SpySettings.Update(root => {
						SaveSection(section, root);
					});
				};
			}

			if (sender is DecompilerSettings decompilerSettings && assemblyListManager != null)
			{
				assemblyListManager.ApplyWinRTProjections = decompilerSettings.ApplyWindowsRuntimeProjections;
				assemblyListManager.UseDebugSymbols = decompilerSettings.UseDebugSymbols;
			}

			MessageBus.Send(sender, new SettingsChangedEventArgs(e));
		}

		private static ILSpySettings LoadSettings()
		{
			ILSpySettings.SettingsFilePathProvider = new ILSpySettingsFilePathProvider();
			return ILSpySettings.Load();
		}
	}
}
