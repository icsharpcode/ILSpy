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
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace ICSharpCode.ILSpyX.Settings
{
	/// <summary>
	/// Manages ILSpy settings.
	/// </summary>
	public class ILSpySettings : ISettingsProvider
	{
		/// <summary>
		/// This settings file path provider determines where to load settings file from, includes filename
		/// </summary>
		public static Func<string>? SettingsFilePathProvider { get; set; }

		XElement root;

		ILSpySettings(XElement? root = null)
		{
			this.root = root ?? new XElement("ILSpy");
		}

		public XElement this[XName section] {
			get {
				return root.Element(section) ?? new XElement(section);
			}
		}

		/// <summary>
		/// Loads the settings file from disk.
		/// </summary>
		/// <returns>
		/// An instance used to access the loaded settings.
		/// </returns>
		public static ILSpySettings Load()
		{
			using (new MutexProtector(ConfigFileMutex))
			{
				try
				{
					return new ILSpySettings(LoadFile(GetConfigFile()).Root);
				}
				catch (IOException)
				{
					return new ILSpySettings();
				}
				catch (XmlException)
				{
					return new ILSpySettings();
				}
			}
		}

		static XDocument LoadFile(string fileName)
		{
			return XDocument.Load(fileName, LoadOptions.None);
		}

		/// <summary>
		/// Saves a setting section.
		/// </summary>
		public void SaveSettings(XElement section)
		{
			Update(rootElement => {
				XElement? existingElement = rootElement.Element(section.Name);
				if (existingElement != null)
					existingElement.ReplaceWith(section);
				else
					rootElement.Add(section);
			});
		}

		/// <summary>
		/// Updates the saved settings.
		/// We always reload the file on updates to ensure we aren't overwriting unrelated changes performed
		/// by another ILSpy instance.
		/// </summary>
		public void Update(Action<XElement> action)
		{
			using (new MutexProtector(ConfigFileMutex))
			{
				string config = GetConfigFile();
				XDocument doc;
				try
				{
					doc = LoadFile(config);
				}
				catch (IOException)
				{
					// ensure the directory exists
					Directory.CreateDirectory(Path.GetDirectoryName(config)!);
					doc = new XDocument(new XElement("ILSpy"));
				}
				catch (XmlException)
				{
					// The file cannot be parsed, and it is about to be replaced by the one written
					// below. It is the only copy of whatever the user had - assembly lists above all -
					// and a file that fails to parse is usually one typo away from readable, so it is
					// kept instead of overwritten (issue #2919).
					KeepUnreadableFile(config);
					doc = new XDocument(new XElement("ILSpy"));
				}
				doc.Root!.SetAttributeValue("version", DecompilerVersionInfo.Major + "." + DecompilerVersionInfo.Minor + "." + DecompilerVersionInfo.Build + "." + DecompilerVersionInfo.Revision);
				action(doc.Root);
				doc.Save(config, SaveOptions.None);
				this.root = doc.Root;
			}
		}

		/// <summary>
		/// Moves a settings file that could not be read out of the way, under a name that says what
		/// it is and never replaces an earlier one - two bad starts in a row must not cost the copy
		/// that still has the data.
		/// </summary>
		static void KeepUnreadableFile(string config)
		{
			try
			{
				if (!File.Exists(config))
					return;
				string baseName = config + ".broken-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
				string kept = baseName;
				for (int attempt = 1; File.Exists(kept); attempt++)
					kept = baseName + "-" + attempt;
				File.Move(config, kept);
			}
			catch (IOException)
			{
				// Keeping the file is what matters; failing to name the copy must not stop settings
				// from being saved.
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		static string GetConfigFile()
		{
			return SettingsFilePathProvider?.Invoke() ?? throw new ArgumentNullException(nameof(SettingsFilePathProvider));
		}

		const string ConfigFileMutex = "01A91708-49D1-410D-B8EB-4DE2662B3971";
	}
}
