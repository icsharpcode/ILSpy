using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Xml.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using DecompilerSettings = ICSharpCode.ILSpy.Options.DecompilerSettings;

#nullable enable

namespace ICSharpCode.ILSpy.Util
{
	public interface ISettingsSection : INotifyPropertyChanged
	{
		XName SectionName { get; }

		void LoadFromSection(XElement section);

		void SaveToSection(XElement section);
	}

	public abstract class SettingsServiceBase
	{
		protected readonly ConcurrentDictionary<Type, ISettingsSection> sections = new();

		public ISettingsProvider SpySettings;

		protected SettingsServiceBase(ISettingsProvider spySettings)
		{
			SpySettings = spySettings;
		}

		public T GetSettings<T>() where T : ISettingsSection, new()
		{
			return (T)sections.GetOrAdd(typeof(T), _ => {
				T section = new T();

				var sectionElement = SpySettings[section.SectionName];

				section.LoadFromSection(sectionElement);
				section.PropertyChanged += Section_PropertyChanged;

				return section;
			});
		}

		protected virtual void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
		}
	}

	public class SettingsSnapshot : SettingsServiceBase
	{
		private readonly SettingsService parent;

		public SettingsSnapshot(SettingsService parent) : base(parent.SpySettings)
		{
			this.parent = parent;
		}

		public void Save()
		{
			SpySettings.Update(root => {
				foreach (var section in sections.Values)
				{
					var element = SpySettings[section.SectionName];

					section.SaveToSection(element);

					var existingElement = root.Element(section.SectionName);
					if (existingElement != null)
						existingElement.ReplaceWith(element);
					else
						root.Add(element);
				}
			});

			parent.Reload();
		}
	}

	public class SettingsService : SettingsServiceBase
	{
		public static readonly SettingsService Instance = new();

		private SettingsService() : base(LoadSettings())
		{
		}

		public SessionSettings SessionSettings => GetSettings<SessionSettings>();

		public DecompilerSettings DecompilerSettings => GetSettings<DecompilerSettings>();

		public DisplaySettings DisplaySettings => GetSettings<DisplaySettings>();

		public MiscSettings MiscSettings => GetSettings<MiscSettings>();

		private AssemblyListManager? assemblyListManager;
		public AssemblyListManager AssemblyListManager => assemblyListManager ??= new(SpySettings) {
			ApplyWinRTProjections = DecompilerSettings.ApplyWindowsRuntimeProjections,
			UseDebugSymbols = DecompilerSettings.UseDebugSymbols
		};

		public DecompilationOptions CreateDecompilationOptions(TabPageModel tabPage)
		{
			return new(SessionSettings.LanguageSettings.LanguageVersion, DecompilerSettings, DisplaySettings) { Progress = tabPage.Content as IProgress<DecompilationProgress> };
		}

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

					section.LoadFromSection(element);
				}
			}
			finally
			{
				reloading = false;
			}
		}

		public SettingsSnapshot CreateSnapshot()
		{
			return new(this);
		}

		protected override void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			base.Section_PropertyChanged(sender, e);

			if (!reloading)
			{
				throw new InvalidOperationException("Settings are read only, use a snapshot to modify.");
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
