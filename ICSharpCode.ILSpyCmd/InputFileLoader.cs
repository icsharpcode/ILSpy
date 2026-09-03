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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.FileLoaders;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// Thrown when the input file is a package (a single-file bundle, an archive) and the entry
	/// to work on was either not named or names nothing in the package. The message is the
	/// complete text to print, including the entries to choose from.
	/// </summary>
	public sealed class PackageEntryRequiredException : Exception
	{
		public PackageEntryRequiredException(string message, int exitCode)
			: base(message)
		{
			this.ExitCode = exitCode;
		}

		public int ExitCode { get; }
	}

	/// <summary>
	/// Turns an input file name into the module to work on. Every mode of the tool loads its
	/// input through here, so a package is recognized the same way in all of them.
	/// </summary>
	static class InputFileLoader
	{
		static readonly FileLoaderRegistry loaders = new FileLoaderRegistry();

		/// <summary>
		/// Loads <paramref name="fileName"/>, or the entry named by <paramref name="entryName"/>
		/// if the file turns out to be a package.
		/// </summary>
		/// <exception cref="PackageEntryRequiredException">
		/// The file is a package and <paramref name="entryName"/> is null or unknown.
		/// </exception>
		public static PEFile Load(string fileName, string entryName, bool applyWinRTProjections = true)
		{
			var context = new FileLoadContext(applyWinRTProjections, null);
			var result = LoadFile(fileName, context);
			if (result?.Package is { } package)
			{
				return LoadPackageEntry(package, entryName, context);
			}
			if (result?.MetadataFile is PEFile module)
			{
				return module;
			}
			// Not a file any loader recognized: let PEFile report what is wrong with it, which is
			// the error the tool has always produced for such input.
			return new PEFile(fileName, metadataOptions: MetadataOptions(context));
		}

		static LoadResult LoadFile(string fileName, FileLoadContext context)
		{
			using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			foreach (var loader in loaders.RegisteredLoaders)
			{
				stream.Position = 0;
				LoadResult result;
				try
				{
					// The loaders are synchronous in fact; only their signature is not.
					result = loader.Load(fileName, stream, context).GetAwaiter().GetResult();
				}
				catch (Exception)
				{
					// A loader that chokes on a file it does not own says nothing about the file.
					// If no other loader claims it either, the caller reloads it as a PE file and
					// reports the failure from there.
					continue;
				}
				if (result?.IsSuccess == true)
					return result;
			}
			return null;
		}

		static PEFile LoadPackageEntry(LoadedPackage package, string entryName, FileLoadContext context)
		{
			var managedEntries = GetManagedEntries(package);
			string kind = package.Kind == LoadedPackage.PackageKind.Bundle ? "single-file bundle" : "package";

			if (string.IsNullOrEmpty(entryName))
			{
				throw new PackageEntryRequiredException(
					BuildListing($"error: {kind}; name an entry with --bundle-entry <name>.", package, managedEntries),
					ProgramExitCodes.EX_USAGE);
			}

			var entry = managedEntries.FirstOrDefault(e => string.Equals(entryName, e.Name, StringComparison.OrdinalIgnoreCase));
			if (entry == null)
			{
				throw new PackageEntryRequiredException(
					BuildListing($"error: '{entryName}' is not a managed entry of this {kind}.", package, managedEntries),
					ProgramExitCodes.EX_DATAERR);
			}

			using var stream = entry.TryOpenStream();
			if (stream == null)
			{
				throw new PackageEntryRequiredException(
					$"error: entry '{entry.Name}' could not be read from the {kind}.",
					ProgramExitCodes.EX_DATAERR);
			}
			stream.Position = 0;
			return new PEFile(entry.Name, stream,
				PEStreamOptions.PrefetchEntireImage | PEStreamOptions.LeaveOpen,
				MetadataOptions(context));
		}

		/// <summary>
		/// The entries worth decompiling. A bundle manifest states the type of each entry; an
		/// archive - and a bundle whose entries are all typed as unknown - is filtered by
		/// extension instead.
		/// </summary>
		static IReadOnlyList<PackageEntry> GetManagedEntries(LoadedPackage package)
		{
			var assemblyNames = GetAssemblyEntryNames(package);
			if (assemblyNames.Count > 0)
			{
				return package.Entries.Where(e => assemblyNames.Contains(e.Name)).ToList();
			}
			return package.Entries
				.Where(e => e.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
					|| e.Name.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
				.ToList();
		}

		static HashSet<string> GetAssemblyEntryNames(LoadedPackage package)
		{
			var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var entries = package.BundleHeader.Entries;
			if (!entries.IsDefaultOrEmpty)
			{
				foreach (var entry in entries)
				{
					if (entry.Type == SingleFileBundle.FileType.Assembly)
						names.Add(entry.RelativePath);
				}
			}
			return names;
		}

		/// <summary>
		/// The name of the application assembly, or null if it cannot be determined. The manifest
		/// carries no entry-point marker, so it is derived the way the host does it: from the
		/// runtime-config entry, whose name is the application name plus ".runtimeconfig.json".
		/// </summary>
		static string FindEntryPoint(LoadedPackage package, IReadOnlyList<PackageEntry> managedEntries)
		{
			const string suffix = ".runtimeconfig.json";
			var entries = package.BundleHeader.Entries;
			if (entries.IsDefaultOrEmpty)
				return null;
			foreach (var entry in entries)
			{
				if (entry.Type != SingleFileBundle.FileType.RuntimeConfigJson)
					continue;
				if (!entry.RelativePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
					continue;
				string candidate = entry.RelativePath[..^suffix.Length] + ".dll";
				var match = managedEntries.FirstOrDefault(e => string.Equals(candidate, e.Name, StringComparison.OrdinalIgnoreCase));
				if (match != null)
					return match.Name;
			}
			return null;
		}

		static string BuildListing(string message, LoadedPackage package, IReadOnlyList<PackageEntry> managedEntries)
		{
			var text = new StringBuilder(message);
			if (managedEntries.Count == 0)
			{
				text.Append(" It contains no managed entries.");
				return text.ToString();
			}
			string entryPoint = FindEntryPoint(package, managedEntries);
			text.AppendLine(" Managed entries:");
			foreach (var entry in managedEntries)
			{
				text.Append("  ").Append(entry.Name);
				if (entry.Name == entryPoint)
					text.Append("  (entry point)");
				text.AppendLine();
			}
			return text.ToString().TrimEnd();
		}

		static MetadataReaderOptions MetadataOptions(FileLoadContext context)
		{
			return context.ApplyWinRTProjections
				? MetadataReaderOptions.ApplyWindowsRuntimeProjections
				: MetadataReaderOptions.None;
		}
	}
}
