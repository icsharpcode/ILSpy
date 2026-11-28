// Copyright (c) 2026 Siegfried Pammer
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Settings;

[TestFixture]
[NonParallelizable]
public class FileNamesInSettingsFileTests
{
	sealed class QueuedSynchronizationContext : SynchronizationContext
	{
		public int PendingCallbacks { get; private set; }

		public override void Post(SendOrPostCallback callback, object? state)
		{
			PendingCallbacks++;
		}
	}

	static readonly TestCaseData[] FileNames = {
		new TestCaseData("Invoice\uFFFF.dll").SetArgDisplayNames("U+FFFF"),
		new TestCaseData("Invoice\uFFFE.dll").SetArgDisplayNames("U+FFFE"),
		new TestCaseData("Invoice\u0001.dll").SetArgDisplayNames("U+0001"),
		new TestCaseData("Invoice\uD800.dll").SetArgDisplayNames("unpaired high surrogate"),
		new TestCaseData("Invoice\uDC00\uD800.dll").SetArgDisplayNames("unpaired low and high surrogate"),
		new TestCaseData("Invoice\r.dll").SetArgDisplayNames("carriage return"),
		new TestCaseData("Invoice\n.dll").SetArgDisplayNames("line feed"),
		new TestCaseData("Invoice\uFFFF\\x0041\U0001F600.dll").SetArgDisplayNames("invalid character beside escape-shaped path text"),
	};

	Func<string>? savedProvider;
	string tempDir = "";
	string configFile = "";

	[SetUp]
	public void SetUp()
	{
		savedProvider = ILSpySettings.SettingsFilePathProvider;
		tempDir = Path.Combine(Path.GetTempPath(), "ILSpyFileNamesInSettings_" + Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempDir);
		configFile = Path.Combine(tempDir, "ILSpy.xml");
		ILSpySettings.SettingsFilePathProvider = () => configFile;
	}

	[TearDown]
	public void TearDown()
	{
		ILSpySettings.SettingsFilePathProvider = savedProvider;
		try
		{
			Directory.Delete(tempDir, recursive: true);
		}
		catch
		{
			// Best-effort cleanup.
		}
	}

	[TestCaseSource(nameof(FileNames))]
	public void AssemblyListRoundTripsFileName(string fileName)
	{
		string fullPath = Path.GetFullPath(Path.Combine(tempDir, fileName));
		var manager = new AssemblyListManager(ILSpySettings.Load());
		var list = manager.CreateList("My project");
		manager.AddListIfNotExists(list);

		Assert.DoesNotThrow(() => list.OpenAssembly(fullPath));
		manager.SaveList(list);

		var reloadedManager = new AssemblyListManager(ILSpySettings.Load());
		Assert.That(reloadedManager.AssemblyLists, Does.Contain("My project"));
		Assert.That(reloadedManager.LoadList("My project").GetAssemblies().Select(asm => asm.FileName),
			Is.EqualTo(new[] { fullPath }));
	}

	[TestCaseSource(nameof(FileNames))]
	public void SessionSettingsRoundTripsAutoLoadedFileName(string fileName)
	{
		var original = new SessionSettings { ActiveAutoLoadedAssembly = fileName };

		Assert.DoesNotThrow(() => ILSpySettings.Load().SaveSettings(original.SaveToXml()));

		var restored = new SessionSettings();
		restored.LoadFromXml(ILSpySettings.Load()[restored.SectionName]);
		Assert.That(restored.ActiveAutoLoadedAssembly, Is.EqualTo(fileName));
	}

	[Test]
	public void FailedSettingsSavePreservesTheOriginalFile()
	{
		const string originalSettings = "<ILSpy version=\"9.0.0.0\"><Existing keep=\"yes\" /></ILSpy>";
		File.WriteAllText(configFile, originalSettings);

		Assert.Throws<ArgumentException>(() =>
			ILSpySettings.Load().SaveSettings(new XElement("Invalid", "\uFFFF")));

		Assert.That(File.ReadAllText(configFile), Is.EqualTo(originalSettings));
		Assert.That(Directory.GetFiles(tempDir), Has.Length.EqualTo(1));
	}

	[TestCase(@"C:\xABCD\a.dll")]
	[TestCase("/home/user/\U0001F600 & <assembly>/a.dll")]
	public void OrdinaryFileNameKeepsItsExistingXmlShape(string fileName)
	{
		var original = new SessionSettings { ActiveAutoLoadedAssembly = fileName };

		ILSpySettings.Load().SaveSettings(original.SaveToXml());

		var element = XDocument.Load(configFile).Root!.Element("SessionSettings")!.Element("ActiveAutoLoadedAssembly")!;
		Assert.That(element.HasAttributes, Is.False);
		Assert.That(element.Value, Is.EqualTo(fileName));
	}

	[Test]
	public void AssemblyListSaveUsesBuildConfigurationPolicy()
	{
		var previousContext = SynchronizationContext.Current;
		var queuedContext = new QueuedSynchronizationContext();
		string assemblyPath = Path.Combine(tempDir, "Opened.dll");
		try
		{
			SynchronizationContext.SetSynchronizationContext(queuedContext);
			var manager = new AssemblyListManager(ILSpySettings.Load());
			var list = manager.CreateList("My project");
			manager.AddListIfNotExists(list);

			list.OpenAssembly(assemblyPath);

			var savedList = ILSpySettings.Load()["AssemblyLists"]
				.Elements("List")
				.Single(element => (string?)element.Attribute("name") == "My project");
#if DEBUG
			Assert.That(savedList.Elements("Assembly").Select(FileNameXml.FromXml), Does.Contain(assemblyPath));
			Assert.That(queuedContext.PendingCallbacks, Is.Zero);
#else
			Assert.That(savedList.Elements("Assembly"), Is.Empty);
			Assert.That(queuedContext.PendingCallbacks, Is.EqualTo(1));
#endif
		}
		finally
		{
			SynchronizationContext.SetSynchronizationContext(previousContext);
		}
	}
}
