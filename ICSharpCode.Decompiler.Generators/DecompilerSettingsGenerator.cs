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

using System.Collections.Immutable;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ICSharpCode.Decompiler.Generators;

/// <summary>
/// Generates the boilerplate behind [DecompilerSetting] partial properties: the backing field,
/// the accessors with change notification, and - from the per-setting language version - the
/// [Category] attribute plus the SetLanguageVersion and GetMinimumRequiredVersion methods,
/// so that a setting's version is declared in exactly one place.
/// </summary>
[Generator]
internal class DecompilerSettingsGenerator : IIncrementalGenerator
{
	static readonly DiagnosticDescriptor InvalidSettingProperty = new(
		id: "DSTG002",
		title: "[DecompilerSetting] target must be a partial instance bool property",
		messageFormat: "Setting property '{0}' must be a partial instance property of type bool with both a get and a set accessor",
		category: "DecompilerSettingsGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	// The generated [Category] would join a handwritten one on the merged partial property, and
	// GetCustomAttribute<CategoryAttribute>() (used by the settings UI) throws on duplicates.
	static readonly DiagnosticDescriptor CategoryOnVersionedSetting = new(
		id: "DSTG003",
		title: "Version-gated setting must not declare [Category]",
		messageFormat: "Setting '{0}' derives its [Category] from the language version; remove the handwritten [Category] attribute",
		category: "DecompilerSettingsGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	static readonly DiagnosticDescriptor UnsupportedLanguageVersion = new(
		id: "DSTG004",
		title: "Language version has no display category",
		messageFormat: "Language version '{0}' has no display category; gate settings on a released C# version, or add the new version to DecompilerSettingsGenerator.CategoryByVersion",
		category: "DecompilerSettingsGenerator",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true);

	// Display category per released C# version; the settings UI groups options by these strings.
	static readonly Dictionary<string, string> CategoryByVersion = new() {
		["CSharp2"] = "C# 2.0 / VS 2005",
		["CSharp3"] = "C# 3.0 / VS 2008",
		["CSharp4"] = "C# 4.0 / VS 2010",
		["CSharp5"] = "C# 5.0 / VS 2012",
		["CSharp6"] = "C# 6.0 / VS 2015",
		["CSharp7"] = "C# 7.0 / VS 2017",
		["CSharp7_1"] = "C# 7.1 / VS 2017.3",
		["CSharp7_2"] = "C# 7.2 / VS 2017.4",
		["CSharp7_3"] = "C# 7.3 / VS 2017.7",
		["CSharp8_0"] = "C# 8.0 / VS 2019",
		["CSharp9_0"] = "C# 9.0 / VS 2019.8",
		["CSharp10_0"] = "C# 10.0 / VS 2022",
		["CSharp11_0"] = "C# 11.0 / VS 2022.4",
		["CSharp12_0"] = "C# 12.0 / VS 2022.8",
		["CSharp13_0"] = "C# 13.0 / VS 2022.12",
		["CSharp14_0"] = "C# 14.0 / VS 2026",
	};

	readonly record struct SettingInfo(
		string Namespace, string ClassName, string Accessibility, string PropertyName, string FieldName,
		bool DefaultValue, int VersionValue, string? VersionName, string? Category, bool AffectsMinimumRequiredVersion,
		string FilePath, int SpanStart);

	// A diagnostic captured during the transform; kept as plain values so the pipeline stays cacheable.
	readonly record struct DiagInfo(string Id, string MessageArg, string FilePath, int SpanStart, int SpanLength,
		int StartLine, int StartChar, int EndLine, int EndChar);

	readonly record struct SettingResult(SettingInfo? Setting, EquatableArray<DiagInfo>? Diagnostics);

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		context.RegisterPostInitializationOutput(i => i.AddSource("DecompilerSettingsGeneratorAttributes.g.cs", @"

using System;

namespace Microsoft.CodeAnalysis
{
    internal sealed partial class EmbeddedAttribute : global::System.Attribute
    {
    }
}

namespace ICSharpCode.Decompiler
{
	[global::Microsoft.CodeAnalysis.EmbeddedAttribute]
	[global::System.AttributeUsage(global::System.AttributeTargets.Property)]
	sealed class DecompilerSettingAttribute : global::System.Attribute
	{
		public DecompilerSettingAttribute() { }

		public DecompilerSettingAttribute(global::ICSharpCode.Decompiler.CSharp.LanguageVersion introducedIn) { }

		/// <summary>Initial value of the setting. Defaults to true.</summary>
		public bool DefaultValue { get; set; } = true;

		/// <summary>
		/// Whether enabling the setting raises GetMinimumRequiredVersion() to the version the
		/// setting was introduced in. Defaults to true; only meaningful on version-gated settings.
		/// </summary>
		public bool AffectsMinimumRequiredVersion { get; set; } = true;
	}
}

"));

		var settings = context.SyntaxProvider.ForAttributeWithMetadataName(
			"ICSharpCode.Decompiler.DecompilerSettingAttribute",
			(n, ct) => n is PropertyDeclarationSyntax,
			GetSetting);

		context.RegisterSourceOutput(settings.Collect(), WriteSettingsClasses);
	}

	static SettingResult GetSetting(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
	{
		var property = (IPropertySymbol)context.TargetSymbol;
		var node = (PropertyDeclarationSyntax)context.TargetNode;
		var diagnostics = new List<DiagInfo>();

		if (property.Type.SpecialType != SpecialType.System_Boolean || property.IsStatic
			|| property.GetMethod == null || property.SetMethod == null
			|| !node.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
		{
			diagnostics.Add(MakeDiagInfo(InvalidSettingProperty.Id, property.Name, node));
			return new SettingResult(null, diagnostics.ToEquatableArray());
		}

		var attribute = context.Attributes[0];
		int versionValue = 0;
		string? versionName = null;
		string? category = null;
		if (attribute.ConstructorArguments.Length == 1)
		{
			versionValue = (int)attribute.ConstructorArguments[0].Value!;
			versionName = attribute.ConstructorArguments[0].Type!.GetMembers()
				.OfType<IFieldSymbol>()
				.FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, versionValue))?.Name
				?? versionValue.ToString();
			if (!CategoryByVersion.TryGetValue(versionName, out category))
			{
				diagnostics.Add(MakeDiagInfo(UnsupportedLanguageVersion.Id, versionName, node));
				return new SettingResult(null, diagnostics.ToEquatableArray());
			}
			if (property.GetAttributes().Any(a => a.AttributeClass?.Name == "CategoryAttribute"))
			{
				diagnostics.Add(MakeDiagInfo(CategoryOnVersionedSetting.Id, property.Name, node));
			}
		}

		bool defaultValue = true;
		bool affectsMinimumRequiredVersion = true;
		foreach (var named in attribute.NamedArguments)
		{
			if (named.Key == "DefaultValue")
				defaultValue = (bool)named.Value.Value!;
			else if (named.Key == "AffectsMinimumRequiredVersion")
				affectsMinimumRequiredVersion = (bool)named.Value.Value!;
		}

		string fieldName = char.ToLowerInvariant(property.Name[0]) + property.Name.Substring(1);
		if (SyntaxFacts.GetKeywordKind(fieldName) != SyntaxKind.None)
			fieldName = "@" + fieldName;

		var setting = new SettingInfo(
			property.ContainingNamespace.ToDisplayString(),
			property.ContainingType.Name,
			SyntaxFacts.GetText(property.DeclaredAccessibility),
			property.Name,
			fieldName,
			defaultValue,
			versionValue,
			versionName,
			category,
			affectsMinimumRequiredVersion,
			node.SyntaxTree.FilePath,
			node.SpanStart);
		return new SettingResult(setting, diagnostics.Count == 0 ? null : diagnostics.ToEquatableArray());
	}

	static DiagInfo MakeDiagInfo(string id, string messageArg, SyntaxNode node)
	{
		var lineSpan = node.GetLocation().GetLineSpan();
		return new DiagInfo(id, messageArg, node.SyntaxTree.FilePath, node.Span.Start, node.Span.Length,
			lineSpan.StartLinePosition.Line, lineSpan.StartLinePosition.Character,
			lineSpan.EndLinePosition.Line, lineSpan.EndLinePosition.Character);
	}

	static void WriteSettingsClasses(SourceProductionContext context, ImmutableArray<SettingResult> results)
	{
		foreach (var result in results)
		{
			if (result.Diagnostics is not { } resultDiagnostics)
				continue;
			foreach (var diag in resultDiagnostics)
			{
				var descriptor = diag.Id switch {
					"DSTG002" => InvalidSettingProperty,
					"DSTG003" => CategoryOnVersionedSetting,
					_ => UnsupportedLanguageVersion,
				};
				var location = Location.Create(diag.FilePath, new TextSpan(diag.SpanStart, diag.SpanLength),
					new LinePositionSpan(new LinePosition(diag.StartLine, diag.StartChar), new LinePosition(diag.EndLine, diag.EndChar)));
				context.ReportDiagnostic(Diagnostic.Create(descriptor, location, diag.MessageArg));
			}
		}

		var settings = results
			.Where(r => r.Setting != null)
			.Select(r => r.Setting!.Value)
			.OrderBy(s => s.FilePath, StringComparer.Ordinal)
			.ThenBy(s => s.SpanStart);

		foreach (var settingsClass in settings.GroupBy(s => (s.Namespace, s.ClassName)))
		{
			WriteSettingsClass(context, settingsClass.Key.Namespace, settingsClass.Key.ClassName, settingsClass.ToArray());
		}
	}

	static void WriteSettingsClass(SourceProductionContext context, string ns, string className, SettingInfo[] settings)
	{
		var builder = new StringBuilder();
		builder.AppendLine("// <auto-generated/>");
		builder.AppendLine("#nullable enable");
		builder.AppendLine();
		builder.AppendLine($"namespace {ns}");
		builder.AppendLine("{");
		builder.AppendLine($"\tpartial class {className}");
		builder.AppendLine("\t{");

		foreach (var setting in settings)
		{
			builder.AppendLine($"\t\tbool {setting.FieldName} = {(setting.DefaultValue ? "true" : "false")};");
			builder.AppendLine();
			if (setting.Category != null)
			{
				builder.AppendLine($"\t\t[global::System.ComponentModel.Category(\"{setting.Category}\")]");
			}
			builder.AppendLine($"\t\t{setting.Accessibility} partial bool {setting.PropertyName} {{");
			builder.AppendLine($"\t\t\tget {{ return {setting.FieldName}; }}");
			builder.AppendLine("\t\t\tset {");
			builder.AppendLine($"\t\t\t\tif ({setting.FieldName} != value)");
			builder.AppendLine("\t\t\t\t{");
			builder.AppendLine($"\t\t\t\t\t{setting.FieldName} = value;");
			builder.AppendLine("\t\t\t\t\tOnPropertyChanged();");
			builder.AppendLine("\t\t\t\t}");
			builder.AppendLine("\t\t\t}");
			builder.AppendLine("\t\t}");
			builder.AppendLine();
		}

		var versionBuckets = settings
			.Where(s => s.VersionName != null)
			.GroupBy(s => s.VersionValue)
			.OrderBy(g => g.Key)
			.ToArray();
		if (versionBuckets.Length > 0)
		{
			WriteSetLanguageVersion(builder, versionBuckets);
			builder.AppendLine();
			WriteGetMinimumRequiredVersion(builder, versionBuckets);
		}

		builder.AppendLine("\t}");
		builder.AppendLine("}");
		context.AddSource($"{className}.Settings.g.cs", SourceText.From(builder.ToString().Replace("\r\n", "\n"), Encoding.UTF8));
	}

	static void WriteSetLanguageVersion(StringBuilder builder, IGrouping<int, SettingInfo>[] versionBuckets)
	{
		builder.AppendLine("\t\t/// <summary>");
		builder.AppendLine("\t\t/// Deactivates all language features from versions newer than <paramref name=\"languageVersion\"/>.");
		builder.AppendLine("\t\t/// </summary>");
		builder.AppendLine("\t\tpublic void SetLanguageVersion(CSharp.LanguageVersion languageVersion)");
		builder.AppendLine("\t\t{");
		builder.AppendLine("\t\t\t// By default, all decompiler features are enabled.");
		builder.AppendLine("\t\t\t// Disable some of them based on language version:");
		foreach (var bucket in versionBuckets)
		{
			builder.AppendLine($"\t\t\tif (languageVersion < CSharp.LanguageVersion.{bucket.First().VersionName})");
			builder.AppendLine("\t\t\t{");
			foreach (var setting in bucket)
			{
				builder.AppendLine($"\t\t\t\t{setting.FieldName} = false;");
			}
			builder.AppendLine("\t\t\t}");
		}
		builder.AppendLine("\t\t}");
	}

	static void WriteGetMinimumRequiredVersion(StringBuilder builder, IGrouping<int, SettingInfo>[] versionBuckets)
	{
		builder.AppendLine("\t\t/// <summary>");
		builder.AppendLine("\t\t/// Gets the lowest language version that includes all currently enabled language features.");
		builder.AppendLine("\t\t/// </summary>");
		builder.AppendLine("\t\tpublic CSharp.LanguageVersion GetMinimumRequiredVersion()");
		builder.AppendLine("\t\t{");
		foreach (var bucket in versionBuckets.Reverse())
		{
			var fields = bucket.Where(s => s.AffectsMinimumRequiredVersion).Select(s => s.FieldName).ToArray();
			if (fields.Length == 0)
				continue;
			builder.AppendLine($"\t\t\tif ({string.Join(" || ", fields)})");
			builder.AppendLine($"\t\t\t\treturn CSharp.LanguageVersion.{bucket.First().VersionName};");
		}
		builder.AppendLine("\t\t\treturn CSharp.LanguageVersion.CSharp1;");
		builder.AppendLine("\t\t}");
	}
}
