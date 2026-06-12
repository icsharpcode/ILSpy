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
using System.Composition;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

using ICSharpCode.ILSpyX.Analyzers;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Builds the MEF composition host: ILSpyX + this assembly + every *.Plugin.dll next to us.
	/// </summary>
	public static class AppComposition
	{
		static CompositionHost? current;

		// Plugin discovery (disk scan + AssemblyLoadContext.Resolving hookup) is process-global and
		// must happen exactly once: the Resolving handler would otherwise accumulate on every call,
		// and the plugins load into the default ALC regardless. CreateContainer() reuses this cached
		// set, so it can be called repeatedly (the headless test harness rebuilds the container per
		// test to get a fresh, isolated app without re-running this one-time work).
		static bool resolverRegistered;
		static List<Assembly>? composedAssemblies;

		public static CompositionHost Current
			=> current ?? throw new InvalidOperationException("Composition host is not yet initialized.");

		/// <summary>
		/// Resolves a single MEF export, or returns <see langword="null"/> when the composition host
		/// isn't initialized yet (design-time previewer, headless pre-boot) or no such export exists.
		/// Unlike a blanket try/catch this does NOT swallow composition/activation errors: host
		/// absence is checked explicitly and the built-in <c>TryGetExport</c> reports a missing export
		/// as <see langword="false"/>, so a genuinely broken export still surfaces its exception.
		/// </summary>
		public static T? TryGetExport<T>() where T : class
			=> current is { } host && host.TryGetExport(out T? export) ? export : null;

		/// <summary>
		/// Resolves all MEF exports of <typeparamref name="T"/>, or an empty sequence when the host
		/// isn't initialized yet. <see cref="TryGetExport{T}"/> for the single-export case.
		/// </summary>
		public static IEnumerable<T> TryGetExports<T>() where T : class
			=> current?.GetExports<T>() ?? Enumerable.Empty<T>();

		public static CompositionHost Initialize()
		{
			RegisterPluginResolver();
			return CreateContainer();
		}

		/// <summary>
		/// One-time, idempotent: hook the assembly resolver and discover the composed assembly set
		/// (ILSpyX + this assembly + every *.Plugin.dll next to us). Safe to call more than once;
		/// subsequent calls are no-ops.
		/// </summary>
		public static void RegisterPluginResolver()
		{
			if (resolverRegistered)
				return;
			resolverRegistered = true;

			AssemblyLoadContext.Default.Resolving += ResolvePluginDependency;

			var assemblies = new List<Assembly> {
				typeof(IAnalyzer).Assembly,
				typeof(AppComposition).Assembly,
			};
			using (AppLog.Phase("AppComposition.LoadPlugins"))
				assemblies.AddRange(LoadPlugins());
			composedAssemblies = assemblies;
		}

		/// <summary>
		/// (Re)builds the MEF container from the cached assembly set and makes it <see cref="Current"/>,
		/// disposing any previous container first. <see cref="RegisterPluginResolver"/> must have run.
		/// </summary>
		public static CompositionHost CreateContainer()
		{
			if (composedAssemblies == null)
				throw new InvalidOperationException($"{nameof(RegisterPluginResolver)} must be called before {nameof(CreateContainer)}.");

			current?.Dispose();
			using (AppLog.Phase($"AppComposition.CreateContainer ({composedAssemblies.Count} assemblies)"))
				current = new ContainerConfiguration()
					.WithAssemblies(composedAssemblies)
					.CreateContainer();

			return current;
		}

		static IEnumerable<Assembly> LoadPlugins()
		{
			var baseDir = Path.GetDirectoryName(typeof(AppComposition).Assembly.Location);
			if (string.IsNullOrEmpty(baseDir))
				yield break;

			foreach (var file in Directory.GetFiles(baseDir, "*.Plugin.dll"))
			{
				Assembly? assembly = null;
				var name = Path.GetFileNameWithoutExtension(file);
				try
				{
					assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
				}
				catch (Exception ex)
				{
					// Non-fatal: a plugin that won't load is skipped and reported to the user,
					// rather than aborting startup into the error window.
					CompositionErrors.Report($"Plugin '{name}'", ex);
				}
				if (assembly != null)
					yield return assembly;
			}
		}

		static Assembly? ResolvePluginDependency(AssemblyLoadContext context, AssemblyName assemblyName)
		{
			var root = Path.GetDirectoryName(typeof(AppComposition).Assembly.Location);
			if (string.IsNullOrEmpty(root))
				return null;
			var path = Path.Combine(root, assemblyName.Name + ".dll");
			return File.Exists(path) ? context.LoadFromAssemblyPath(path) : null;
		}
	}
}