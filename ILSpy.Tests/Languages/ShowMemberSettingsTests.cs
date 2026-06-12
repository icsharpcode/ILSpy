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
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

// CSharpLanguage.ShowMember drives which members the assembly tree shows at all: every member
// tree node consults it (unless the API-visibility filter is set to "All"). It must evaluate
// CSharpDecompiler.MemberIsHidden against the LIVE decompiler settings, not a default-constructed
// DecompilerSettings — otherwise toggling a decompiler option that unhides compiler-generated
// members (here: ArrayInitializers, which hides "<PrivateImplementationDetails>") has no effect
// on the tree.
[TestFixture]
public class ShowMemberSettingsTests
{
	/// <summary>
	/// Emits an assembly containing a top-level [CompilerGenerated] type named
	/// "&lt;PrivateImplementationDetails&gt;" and returns its type definition.
	/// MemberIsHidden hides that type iff <c>DecompilerSettings.ArrayInitializers</c> is set,
	/// which makes it a minimal settings-sensitive probe for ShowMember.
	/// </summary>
	static ITypeDefinition EmitPrivateImplementationDetails()
	{
		const string name = "ShowMemberFixture";
		var ab = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
		var module = ab.DefineDynamicModule(name);

		var details = module.DefineType("<PrivateImplementationDetails>",
			TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.Sealed);
		details.SetCustomAttribute(new CustomAttributeBuilder(
			typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes)!, []));
		details.CreateType();

		var dir = Path.Combine(Path.GetTempPath(), $"ILSpyShowMember_{Guid.NewGuid():N}");
		Directory.CreateDirectory(dir);
		var path = Path.Combine(dir, $"{name}.dll");
		ab.Save(path);

		var file = new PEFile(path);
		var resolver = new UniversalAssemblyResolver(path, throwOnError: false,
			targetFramework: file.DetectTargetFrameworkId());
		var typeSystem = new DecompilerTypeSystem(file, resolver);
		return typeSystem.MainModule.TypeDefinitions
			.Single(t => t.Name == "<PrivateImplementationDetails>");
	}

	[AvaloniaTest]
	public void ShowMember_hides_PrivateImplementationDetails_under_default_settings()
	{
		var language = AppComposition.Current.GetExport<LanguageService>().GetLanguage("C#");

		language.ShowMember(EmitPrivateImplementationDetails()).Should().BeFalse(
			"ArrayInitializers defaults to true, which hides <PrivateImplementationDetails>");
	}

	[AvaloniaTest]
	public void ShowMember_honours_the_live_decompiler_settings()
	{
		var settingsService = AppComposition.Current.GetExport<SettingsService>();
		settingsService.DecompilerSettings.ArrayInitializers = false;

		var language = AppComposition.Current.GetExport<LanguageService>().GetLanguage("C#");

		language.ShowMember(EmitPrivateImplementationDetails()).Should().BeTrue(
			"with ArrayInitializers off, MemberIsHidden no longer hides <PrivateImplementationDetails>, " +
			"so the tree must show it");
	}
}
