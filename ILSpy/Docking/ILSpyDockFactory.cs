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
using System.IO;
using System.Linq;

using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.Mvvm;
using Dock.Model.Mvvm.Controls;
using Dock.Serializer.SystemTextJson;

using ILSpy.Commands;
using ILSpy.TextView;
using ILSpy.ViewModels;

namespace ILSpy.Docking
{
	public class ILSpyDockFactory : Factory
	{
		readonly IReadOnlyList<ToolPaneEntry> panes;

		public IDocumentDock? Documents { get; private set; }

		/// <summary>
		/// The single persistent Document the host puts in <see cref="Documents"/>. Its
		/// <see cref="ContentTabPage.Content"/> swaps between viewmodels (decompiler text /
		/// metadata grid) on tree-node selection — DockWorkspace owns the population.
		/// </summary>
		public ContentTabPage? MainTab { get; private set; }

		public ILSpyDockFactory(ToolPaneRegistry registry)
		{
			this.panes = registry.Panes;
		}

		/// <summary>
		/// Lazily-built serializer wired with a custom <see cref="System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver"/>
		/// that strips properties whose type would otherwise drag in framework-cycle
		/// state (<see cref="System.Threading.Tasks.Task"/>, <see cref="System.Threading.CancellationToken"/>,
		/// etc.). Without this filter the serializer follows a tool-pane VM's
		/// <c>Task</c>-shaped property into <c>TaskCompletionSource</c> internals and
		/// hits System.Text.Json's <c>MaxDepth=64</c> on real cycles that
		/// <c>ReferenceHandler.Preserve</c> can't repair (BCL types don't carry
		/// <c>$id</c> markers).
		/// </summary>
		static readonly DockSerializer dockSerializer = BuildSerializer();

		static DockSerializer BuildSerializer()
		{
			// Dock.Serializer.SystemTextJson's parameterless ctor wires the (internal)
			// DockModelPolymorphicTypeResolver that handles IDockable / IDock / IRootDock
			// / IDockWindow / I*Template polymorphism. We need to ADD our cycle-prone-
			// property filter and DISABLE ReferenceHandler.Preserve on the options it
			// builds — both via reflection because Dock keeps those fields internal.
			var serializer = new DockSerializer();
			var optionsField = typeof(DockSerializer).GetField(
				"_options",
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			if (optionsField?.GetValue(serializer) is System.Text.Json.JsonSerializerOptions opts
				&& opts.TypeInfoResolver is System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver poly)
			{
				poly.Modifiers.Add(StripCycleProneProperties);
				// With Owner / Window / DockWindow.Layout stripped the dock tree has no
				// structural cycles, so ReferenceHandler.Preserve isn't needed. Turning
				// it OFF is what makes Load work: with Preserve on, the emitted JSON
				// carries both $id (from Preserve) and $type (from Dock's polymorphism
				// resolver) on the same objects, and System.Text.Json's reader rejects
				// $id-not-first. Additionally, Dock's JsonConverterList<T>.Write makes a
				// per-element JsonSerializer.Serialize call which resets Preserve's $id
				// counter, so multiple unrelated objects share $id="1" — $ref resolution
				// wouldn't work even if ordering were correct. Both problems vanish when
				// Preserve is off.
				opts.ReferenceHandler = null;
				// MaxDepth bump for safety on deep tool stacks; mutation must happen
				// before first serialize call (options become immutable after first use).
				opts.MaxDepth = 256;
			}
			return serializer;
		}

		static void StripCycleProneProperties(System.Text.Json.Serialization.Metadata.JsonTypeInfo typeInfo)
		{
			if (typeInfo.Kind != System.Text.Json.Serialization.Metadata.JsonTypeInfoKind.Object)
				return;

			// For MEF-injected tool panes (AssemblyTreeModel, SearchPaneModel, …) and the
			// persistent ContentTabPage: strip EVERY property except Id + Title. The full
			// VM state (settings refs, image properties typed as IImage, Task-shaped status
			// fields) doesn't belong in a layout file — it's all reconstructed at runtime
			// from composition. The remaining JSON shape is just { "$type": "...", "Id":
			// "..." } per dockable, enough for the factory to look up the singleton on
			// Load via CreateObject below.
			bool isSingletonDockable = IsCompositionResolvableDockable(typeInfo.Type);
			for (int i = typeInfo.Properties.Count - 1; i >= 0; i--)
			{
				var prop = typeInfo.Properties[i];
				bool drop = IsCycleProneType(prop.PropertyType)
					|| IsBackReferenceProperty(typeInfo.Type, prop.Name)
					|| (isSingletonDockable && prop.Name is not "Id" and not "Title");
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
			if (!typeof(Dock.Model.Core.IDockable).IsAssignableFrom(type))
				return false;
			// ToolPaneModel-derived (AssemblyTreeModel, SearchPaneModel, AnalyzerTreeViewModel, …)
			// are all [Shared] MEF singletons.
			if (typeof(ViewModels.ToolPaneModel).IsAssignableFrom(type))
				return true;
			// ContentTabPage is the persistent main-document slot — also resolved as a
			// singleton from the dock factory, not constructed per-deserialize.
			if (type == typeof(ViewModels.ContentTabPage))
				return true;
			return false;
		}

		static object ResolveCompositionSingleton(Type type)
		{
			// JsonTypeInfo.CreateObject is `Func<object>` — null isn't a valid return.
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
			// IDockable's Owner + IDockable's Factory back-refs cycle through the parent
			// chain. Dock's own JsonConverterList<T> calls JsonSerializer.Serialize per
			// element which doesn't carry ReferenceHandler.Preserve's $id state across
			// nested calls — so Owner/Factory wouldn't round-trip as $ref anyway.
			// Stripping them removes the cycle entirely; Factory.InitLayout reconstructs
			// the chain on Load via parent-walk.
			if (type == typeof(Dock.Model.Core.IFactory))
				return true;
			return false;
		}

		// Hand-listed names of back-reference properties whose declared type isn't a
		// reliable cycle signal (e.g. IDockable is a perfectly valid forward property
		// type elsewhere). Stripping by (declaring type, name) keeps the filter
		// surgical without false positives. With all of these stripped, the layout
		// tree has NO cycles, so ReferenceHandler.Preserve can be turned off and the
		// $id/$type ordering conflict disappears.
		static bool IsBackReferenceProperty(Type declaringType, string propertyName)
		{
			if (typeof(Dock.Model.Core.IDockable).IsAssignableFrom(declaringType))
			{
				// IDockable.Owner — parent back-ref.
				if (propertyName == "Owner")
					return true;
			}
			if (typeof(Dock.Model.Controls.IRootDock).IsAssignableFrom(declaringType))
			{
				// IRootDock.Window → IDockWindow.Layout cycles back to the root.
				// Stripping it loses floating-window persistence (acceptable v1 —
				// ILSpy's docked panes don't currently float).
				if (propertyName == "Window")
					return true;
			}
			if (typeof(Dock.Model.Core.IDockWindow).IsAssignableFrom(declaringType))
			{
				// Symmetric strip — covers any other DockWindow that gets serialised
				// without going through IRootDock.Window.
				if (propertyName == "Layout")
					return true;
			}
			return false;
		}

		/// <summary>
		/// Serializes <paramref name="layout"/> to <paramref name="path"/> using
		/// the static <see cref="dockSerializer"/>. Best-effort: any IO or serialization
		/// exception is swallowed; losing the saved layout is strictly less bad than
		/// crashing the app on shutdown.
		/// </summary>
		public static void SaveLayout(string path, IRootDock layout)
		{
			ArgumentNullException.ThrowIfNull(path);
			ArgumentNullException.ThrowIfNull(layout);
			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(path)!);
				using var stream = File.Create(path);
				dockSerializer.Save(stream, layout);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[ILSpyDockFactory] SaveLayout failed: {ex}");
			}
		}

		/// <summary>
		/// Tries to deserialize a previously-saved layout from <paramref name="path"/>.
		/// Returns null on any failure — missing file, malformed JSON, version drift —
		/// so the caller can fall back to <see cref="CreateLayout"/> without a user-
		/// visible error. On success, also rehydrates <see cref="Documents"/> and
		/// <see cref="MainTab"/> by walking the loaded tree — these are the same
		/// fields <see cref="CreateLayout"/> populates on fresh starts, and
		/// <see cref="DockWorkspace.ShowSelectedNode"/> silently no-ops on null
		/// <see cref="MainTab"/> (the user-visible "decompile view stays empty on
		/// second launch" bug).
		/// </summary>
		public IRootDock? LoadLayout(string path)
		{
			ArgumentNullException.ThrowIfNull(path);
			if (!File.Exists(path))
				return null;
			IRootDock? loaded;
			try
			{
				using var stream = File.OpenRead(path);
				loaded = dockSerializer.Load<IRootDock>(stream);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[ILSpyDockFactory] LoadLayout failed: {ex}");
				return null;
			}
			if (loaded != null)
				RehydrateFromLoadedLayout(loaded);
			return loaded;
		}

		void RehydrateFromLoadedLayout(IDockable root)
		{
			// Walk the loaded tree and rebind the two structural slots that CreateLayout
			// would have set. Documents = the single DocumentDock; MainTab = its first
			// ContentTabPage child (we can't look up tab.Owner because back-references
			// are stripped during serialization to break cycles). Tool-pane singletons
			// resolve through the CreateObject hook (StripCycleProneProperties) so they
			// don't need rebinding here.
			Documents = Flatten(root).OfType<IDocumentDock>().FirstOrDefault();
			MainTab = Documents?.VisibleDockables?
				.OfType<ContentTabPage>()
				.FirstOrDefault();
		}

		static IEnumerable<IDockable> Flatten(IDockable root)
		{
			yield return root;
			if (root is IDock dock && dock.VisibleDockables is { } kids)
				foreach (var k in kids)
					foreach (var f in Flatten(k))
						yield return f;
		}

		public override IRootDock CreateLayout()
		{
			var documents = new DocumentDock {
				Id = "Documents",
				Title = "Documents",
				IsCollapsable = false,
				Proportion = 0.6,
				// Dock's theme template binds against DockCapabilityPolicy.{CanDrag, CanDrop,
				// CanPin, CanClose} on every dock. Pre-populate so the binding sees a non-null
				// source on first evaluation; otherwise every dock startup logs ~6 [Binding]
				// errors as the chrome template is realised.
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
			Documents = documents;

			MainTab = new ContentTabPage { Title = "(no selection)" };
			documents.VisibleDockables = CreateList<IDockable>(MainTab);
			documents.ActiveDockable = MainTab;

			ToolDock? leftToolDock = BuildToolDock("LeftTools", ToolPaneAlignment.Left, 0.25);
			ToolDock? topToolDock = BuildToolDock("TopTools", ToolPaneAlignment.Top, 0.2);
			ToolDock? rightToolDock = BuildToolDock("RightTools", ToolPaneAlignment.Right, 0.25);
			ToolDock? bottomToolDock = BuildToolDock("BottomTools", ToolPaneAlignment.Bottom, 0.2);

			// Vertical column: top tool dock (if any), documents, bottom tool dock (if any),
			// with splitters between siblings.
			var verticalChildren = new List<IDockable>();
			if (topToolDock != null)
			{
				verticalChildren.Add(topToolDock);
				verticalChildren.Add(new ProportionalDockSplitter { Id = "TopSplitter" });
			}
			verticalChildren.Add(documents);
			if (bottomToolDock != null)
			{
				verticalChildren.Add(new ProportionalDockSplitter { Id = "BottomSplitter" });
				verticalChildren.Add(bottomToolDock);
			}

			var rightVertical = new ProportionalDock {
				Id = "MiddleColumn",
				Orientation = Orientation.Vertical,
				Proportion = ComputeMiddleColumnProportion(leftToolDock, rightToolDock),
				VisibleDockables = CreateList(verticalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			// Horizontal row: left tool dock, middle column, right tool dock — splitters
			// between siblings only.
			var horizontalChildren = new List<IDockable>();
			if (leftToolDock != null)
			{
				horizontalChildren.Add(leftToolDock);
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "LeftSplitter" });
			}
			horizontalChildren.Add(rightVertical);
			if (rightToolDock != null)
			{
				horizontalChildren.Add(new ProportionalDockSplitter { Id = "RightSplitter" });
				horizontalChildren.Add(rightToolDock);
			}

			var horizontal = new ProportionalDock {
				Id = "MainLayout",
				Orientation = Orientation.Horizontal,
				VisibleDockables = CreateList(horizontalChildren.ToArray()),
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};

			var root = CreateRootDock();
			root.Id = "Root";
			root.IsCollapsable = false;
			root.VisibleDockables = CreateList<IDockable>(horizontal);
			root.DefaultDockable = horizontal;
			root.ActiveDockable = horizontal;
			root.DockCapabilityPolicy = new DockCapabilityPolicy();

			return root;
		}

		ToolDock? BuildToolDock(string id, ToolPaneAlignment alignment, double proportion)
		{
			var dockables = panes
				.Where(p => p.Metadata.Alignment == alignment)
				.OrderBy(p => p.Metadata.Order)
				.Select(p => (IDockable)p.Pane)
				.ToArray();
			if (dockables.Length == 0)
				return null;
			return new ToolDock {
				Id = id,
				Proportion = proportion,
				VisibleDockables = CreateList(dockables),
				ActiveDockable = dockables[0],
				Alignment = alignment switch {
					ToolPaneAlignment.Top => Alignment.Top,
					ToolPaneAlignment.Right => Alignment.Right,
					ToolPaneAlignment.Bottom => Alignment.Bottom,
					_ => Alignment.Left,
				},
				DockCapabilityPolicy = new DockCapabilityPolicy(),
			};
		}

		static double ComputeMiddleColumnProportion(ToolDock? left, ToolDock? right)
		{
			double remaining = 1.0;
			if (left != null)
				remaining -= left.Proportion;
			if (right != null)
				remaining -= right.Proportion;
			return remaining > 0 ? remaining : 0.5;
		}
	}
}
