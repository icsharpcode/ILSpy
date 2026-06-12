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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Windows.Input;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Serializer.SystemTextJson;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Docking
{
	/// <summary>
	/// STJ counterpart of Dock.Serializer.Newtonsoft's DockSerializer. Mirrors the
	/// Newtonsoft baseline's three working pieces — polymorphism on every IDockable
	/// interface, IList&lt;T&gt; → ObservableCollection&lt;T&gt; substitution at
	/// deserialisation, writable-only property persistence — and adds the STJ-specific
	/// adaptations needed because STJ's reference-preservation + polymorphism don't
	/// compose the way Newtonsoft's do (see <see cref="BuildOptions"/>).
	/// <para>
	/// Wraps the configured <see cref="JsonSerializerOptions"/> as a static property so
	/// callers can <c>JsonSerializer.Serialize(stream, value, ILSpyDockJson.Options)</c>
	/// without reflecting into <c>Dock.Serializer.SystemTextJson</c>'s internal
	/// <c>_options</c> field.
	/// </para>
	/// </summary>
	static class ILSpyDockJson
	{
		public static JsonSerializerOptions Options { get; } = BuildOptions();

		static readonly Type[] PolymorphicBaseTypes = new[] {
			typeof(IDockable),
			typeof(IDock),
			typeof(IRootDock),
			typeof(IDockWindow),
			typeof(IDocumentTemplate),
			typeof(IToolTemplate),
		};

		static JsonSerializerOptions BuildOptions()
		{
			var resolver = new DefaultJsonTypeInfoResolver();
			// Modifiers run in registration order; each mutates the JsonTypeInfo built
			// by the default resolver. The filter chain mirrors Dock.Serializer's own
			// pipeline plus the two ILSpy-specific stages on the end.
			resolver.Modifiers.Add(AddPolymorphism);
			resolver.Modifiers.Add(RemoveIgnoredMembers);
			resolver.Modifiers.Add(KeepWritablePropertiesOnly);
			resolver.Modifiers.Add(StripCycleProneProperties);
			return new JsonSerializerOptions {
				WriteIndented = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
				TypeInfoResolver = resolver,
				// Dock's converter handles IList<T> → ObservableCollection<T> at deserialise
				// time, matching Newtonsoft's ListContractResolver substitution.
				Converters = { new JsonConverterFactoryList(typeof(ObservableCollection<>)) },
				MaxDepth = 256,
				// ReferenceHandler.Preserve is intentionally NOT set. Newtonsoft tolerates
				// $id/$ref/$type in any order; STJ requires $type first when polymorphism
				// is in play, and Dock's JsonConverterList<T>.Write makes per-element
				// JsonSerializer.Serialize calls which reset Preserve's $id counter — so
				// multiple unrelated objects would share $id="1". Both incompatibilities
				// vanish once back-reference properties (Owner / Window / Layout) are
				// stripped, leaving a DAG that doesn't need $id/$ref. StripCycleProneProperties
				// below does the stripping.
			};
		}

		static void AddPolymorphism(JsonTypeInfo typeInfo)
		{
			if (!PolymorphicBaseTypes.Contains(typeInfo.Type))
				return;
			// FallBackToNearestAncestor for root / templates (Dock's own resolver does the
			// same — these have stable enough class hierarchies for ancestor fallback to
			// make sense). All other polymorphic bases fall back to the base type if a
			// saved discriminator no longer matches a known concrete.
			bool nearestAncestor = typeInfo.Type == typeof(IRootDock)
				|| typeInfo.Type == typeof(IDocumentTemplate)
				|| typeInfo.Type == typeof(IToolTemplate);
			var options = new JsonPolymorphismOptions {
				TypeDiscriminatorPropertyName = "$type",
				UnknownDerivedTypeHandling = nearestAncestor
					? JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor
					: JsonUnknownDerivedTypeHandling.FallBackToBaseType,
				IgnoreUnrecognizedTypeDiscriminators = true,
			};
			foreach (var derived in GetConcreteDescendantsOf(typeInfo.Type))
				options.DerivedTypes.Add(new JsonDerivedType(derived, derived.FullName ?? derived.Name));
			typeInfo.PolymorphismOptions = options;
		}

		// One-shot scan of every loaded assembly for concrete, public, non-generic candidate
		// types. The result feeds <see cref="GetConcreteDescendantsOf"/> which previously
		// re-scanned the entire AppDomain once per polymorphic base type — 6 base types, ~100
		// loaded assemblies, ~500 ms of repeated reflection. With the cache, the scan runs
		// exactly once and each per-base lookup is a fast LINQ filter over the cached list.
		static readonly Lazy<IReadOnlyList<Type>> CandidateTypes = new(ScanCandidateTypes);

		static IReadOnlyList<Type> ScanCandidateTypes()
		{
			// Filter at the assembly level so we don't call GetTypes() on assemblies that
			// couldn't possibly contain Dock-model types — System.*, Avalonia.*, Microsoft.*,
			// AvaloniaEdit, ICSharpCode.Decompiler, etc. all qualify as "won't define an
			// IDockable". Keep Dock.* (the dock library), the ILSpy assembly itself,
			// and *.Plugin.dll (which can add custom panes).
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(IsRelevantAssembly)
				.SelectMany(SafeTypes)
				.Where(t => t != null
					&& t.IsClass && !t.IsAbstract
					&& !t.ContainsGenericParameters
					&& (t.IsPublic || t.IsNestedPublic))
				.Cast<Type>()
				.Distinct()
				.ToArray();
		}

		static bool IsRelevantAssembly(Assembly a)
		{
			if (a.IsDynamic)
				return false;
			var name = a.GetName().Name;
			if (string.IsNullOrEmpty(name))
				return false;
			return name.StartsWith("Dock.", StringComparison.Ordinal)
				|| name == "ILSpy"
				|| name.EndsWith(".Plugin", StringComparison.Ordinal);
		}

		static IEnumerable<Type> GetConcreteDescendantsOf(Type baseType)
			=> CandidateTypes.Value
				.Where(t => baseType.IsAssignableFrom(t))
				.OrderBy(t => t.FullName, StringComparer.Ordinal);

		static IEnumerable<Type?> SafeTypes(Assembly assembly)
		{
			try
			{ return assembly.GetTypes(); }
			catch (ReflectionTypeLoadException ex) { return ex.Types; }
		}

		static void RemoveIgnoredMembers(JsonTypeInfo typeInfo)
		{
			// Same ignore-set Dock's own polymorphic resolver applies: [IgnoreDataMember]
			// and ICommand properties never belong in the serialised tree.
			for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
			{
				var prop = typeInfo.Properties[i];
				if (prop.AttributeProvider?.IsDefined(typeof(IgnoreDataMemberAttribute), true) == true
					|| typeof(ICommand).IsAssignableFrom(prop.PropertyType))
				{
					typeInfo.Properties.RemoveAt(i);
				}
			}
		}

		static void KeepWritablePropertiesOnly(JsonTypeInfo typeInfo)
		{
			// Mirrors Newtonsoft's ListContractResolver.CreateProperties filter: only
			// keep properties STJ can write back at deserialisation. Computed / get-only
			// properties on the dock model never round-trip — they derive their value
			// from other state, so persisting them just bloats the file.
			if (typeInfo.Kind != JsonTypeInfoKind.Object)
				return;
			for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
			{
				if (typeInfo.Properties[i].Set is null)
					typeInfo.Properties.RemoveAt(i);
			}
		}

		static void StripCycleProneProperties(JsonTypeInfo typeInfo)
		{
			if (typeInfo.Kind != JsonTypeInfoKind.Object)
				return;

			// For MEF-injected tool panes (AssemblyTreeModel, SearchPaneModel, …) and the
			// persistent ContentTabPage: strip EVERY property except Id + Title. The full
			// VM state (settings refs, image properties typed as IImage, Task-shaped
			// status fields) doesn't belong in a layout file — it's all reconstructed at
			// runtime from composition. The remaining JSON shape is just { "$type": "...",
			// "Id": "..." } per dockable, enough for the factory to look up the singleton
			// on Load via CreateObject below.
			bool isSingletonDockable = IsCompositionResolvableDockable(typeInfo.Type);
			bool isDocumentDock = typeof(IDocumentDock).IsAssignableFrom(typeInfo.Type);
			// On IRootDock we strip the per-dock-active-pointer trio too because they
			// transitively reference a ContentTabPage (the focused/active document on the
			// previous session). Keeping IRootDock.VisibleDockables intact preserves the
			// proportional layout tree — only the focus pointers are dropped.
			bool isRootDock = typeof(IRootDock).IsAssignableFrom(typeInfo.Type);
			for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
			{
				var prop = typeInfo.Properties[i];
				bool drop = IsCycleProneType(prop.PropertyType)
					|| IsBackReferenceProperty(typeInfo.Type, prop.Name)
					|| (isSingletonDockable && prop.Name is not "Id" and not "Title")
					|| (isDocumentDock && IsDocumentChildSlot(prop.Name))
					|| (isRootDock && IsActivePointerSlot(prop.Name));
				if (drop)
					typeInfo.Properties.RemoveAt(i);
			}

			// CreateObject bypasses the [ImportingConstructor] mismatch — STJ would
			// otherwise fail with "Each parameter in the deserialization constructor on
			// type X must bind to an object property". Returning the MEF singleton means
			// subsequent property assignments (Id/Title) hit fields whose current value
			// already matches what was saved, so they're effectively no-ops.
			if (isSingletonDockable)
				typeInfo.CreateObject = () => ResolveCompositionSingleton(typeInfo.Type);
		}

		static bool IsCompositionResolvableDockable(Type type)
		{
			if (!typeof(IDockable).IsAssignableFrom(type))
				return false;
			// ToolPaneModel-derived (AssemblyTreeModel, SearchPaneModel, AnalyzerTreeViewModel, …)
			// are all [Shared] MEF singletons.
			if (typeof(ToolPaneModel).IsAssignableFrom(type))
				return true;
			// ContentTabPage is the persistent main-document slot — resolved as a
			// singleton from the dock factory, not constructed per-deserialize.
			if (type == typeof(ContentTabPage))
				return true;
			return false;
		}

		static object ResolveCompositionSingleton(Type type)
		{
			// JsonTypeInfo.CreateObject is Func<object> — null isn't a valid return.
			// The two fallbacks below cover (a) live runtime (MEF resolves the singleton),
			// (b) design-time previews / isolated tests where AppComposition isn't wired
			// (Activator gives us a fresh instance with default Id/Title that STJ then
			// overwrites from the saved JSON). If both fail, throw with the type name so
			// the failure surfaces as something diagnosable instead of as a downstream
			// "CreateObject returned null" from STJ.
			object? instance = null;
			try
			{
				var getExport = typeof(System.Composition.CompositionContext)
					.GetMethods()
					.First(m => m.Name == "GetExport" && m.IsGenericMethod && m.GetParameters().Length == 0)
					.MakeGenericMethod(type);
				instance = getExport.Invoke(AppEnv.AppComposition.Current, null);
			}
			catch
			{
				try
				{ instance = Activator.CreateInstance(type); }
				catch { /* fall through to throw below */ }
			}
			return instance
				?? throw new InvalidOperationException(
					$"Could not resolve composition singleton for {type.FullName}: "
					+ "AppComposition has no export for this type and Activator.CreateInstance failed too.");
		}

		// Document children (decompiler text, metadata grid, compare diff, options page)
		// are session-only: their viewmodels hold live IEntity references, decompiler Tasks,
		// CancellationTokenSources, and AssemblyLoaded contexts that can't survive a
		// process restart. Persisting the ContentTabPage shells through Activator brings
		// them back with Content=null, which renders blank tabs — the user-reported
		// "documents are broken except the first tab" symptom. Strip every IDocumentDock
		// child slot at save-time so only the structural document slot survives; the
		// factory repopulates a fresh MainTab on load.
		static bool IsDocumentChildSlot(string propertyName)
			=> propertyName is "VisibleDockables"
				or "ActiveDockable"
				or "FocusedDockable"
				or "DefaultDockable";

		// The three "currently selected child" pointers on a dock. Used by IRootDock-only
		// stripping — the strict-stronger IsDocumentChildSlot above also covers
		// VisibleDockables, which we must keep on IRootDock.
		static bool IsActivePointerSlot(string propertyName)
			=> propertyName is "ActiveDockable"
				or "FocusedDockable"
				or "DefaultDockable";

		static bool IsCycleProneType(Type type)
		{
			// Task / Task<T> / ValueTask / ValueTask<T>: TaskCompletionSource's internal
			// state forms a true cycle that ReferenceHandler.Preserve can't repair.
			if (type == typeof(System.Threading.Tasks.Task) || type == typeof(System.Threading.Tasks.ValueTask))
				return true;
			if (type.IsGenericType)
			{
				var def = type.GetGenericTypeDefinition();
				if (def == typeof(System.Threading.Tasks.Task<>) || def == typeof(System.Threading.Tasks.ValueTask<>))
					return true;
			}
			// CancellationToken / CancellationTokenSource carry similar internal state.
			if (type == typeof(System.Threading.CancellationToken)
				|| type == typeof(System.Threading.CancellationTokenSource))
				return true;
			// IDockable.Factory back-ref cycles through the parent chain. Stripping it
			// removes the cycle entirely; Factory.InitLayout reconstructs the chain on
			// Load via parent-walk.
			if (type == typeof(IFactory))
				return true;
			return false;
		}

		// Hand-listed names of back-reference properties whose declared type isn't a
		// reliable cycle signal (IDockable is a perfectly valid forward property type
		// elsewhere). Stripping by (declaring type, name) keeps the filter surgical
		// without false positives. With all of these stripped, the layout tree has no
		// cycles, so ReferenceHandler.Preserve can be turned off and the $id/$type
		// ordering conflict disappears.
		static bool IsBackReferenceProperty(Type declaringType, string propertyName)
		{
			if (typeof(IDockable).IsAssignableFrom(declaringType))
			{
				// IDockable.Owner — parent back-ref.
				if (propertyName == "Owner")
					return true;
			}
			if (typeof(IRootDock).IsAssignableFrom(declaringType))
			{
				// IRootDock.Window → IDockWindow.Layout cycles back to the root.
				// Stripping it loses floating-window persistence (acceptable for now —
				// ILSpy's docked panes don't currently float).
				if (propertyName == "Window")
					return true;
			}
			if (typeof(IDockWindow).IsAssignableFrom(declaringType))
			{
				// Symmetric strip — covers any other DockWindow that gets serialised
				// without going through IRootDock.Window.
				if (propertyName == "Layout")
					return true;
			}
			return false;
		}
	}
}
