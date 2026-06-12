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
using System.IO;

using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.AppEnv;

using ProductionApp = ICSharpCode.ILSpy.App;
using SettingsService = ICSharpCode.ILSpy.SettingsService;

namespace ICSharpCode.ILSpy.Tests;

// Subclasses the production App and lets base.Initialize() populate this instance
// from ILSpy/App.axaml (the XAML compiler's rewrite of Load(this) inside App.Initialize
// targets App.axaml's compiled populate, so TestApp picks up the production Resources,
// Styles, and DataTemplates without duplicating the XAML).
public class TestApp : ProductionApp
{
	public override void Initialize()
	{
		base.Initialize();
	}

	public override void OnFrameworkInitializationCompleted()
	{
		// Tests boot the app repeatedly; seeding the entire shared framework (~150 assemblies)
		// per test is ~10x slower. Opt into the minimal default-list seed instead.
		ProductionApp.SeedFullFrameworkDefaultList = false;

		var sessionsRoot = Path.Combine(Path.GetTempPath(), "ILSpy.Tests");
		// Sweep any leftover session dirs from previous runs first — ProcessExit-time cleanup
		// is best-effort and the active session's settings file is sometimes still locked when
		// the runtime tears down, leaving one orphan per process. This catches them next run.
		SweepLeftovers(sessionsRoot);

		var dir = Path.Combine(sessionsRoot, Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(dir);
		var configPath = Path.Combine(dir, "ILSpy.xml");
		ILSpySettings.SettingsFilePathProvider = () => configPath;
		AppDomain.CurrentDomain.ProcessExit += (_, _) => TryDeleteDirectory(dir);

		var composition = AppComposition.Initialize();
	}

	static void SweepLeftovers(string sessionsRoot)
	{
		if (!Directory.Exists(sessionsRoot))
			return;
		foreach (var sub in Directory.EnumerateDirectories(sessionsRoot))
			TryDeleteDirectory(sub);
	}

	static void TryDeleteDirectory(string dir)
	{
		try
		{
			if (Directory.Exists(dir))
				Directory.Delete(dir, recursive: true);
		}
		catch { /* best-effort — don't crash on shutdown if a file is briefly locked */ }
	}
}
