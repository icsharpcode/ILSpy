using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.Util;
using Mono.Cecil;

namespace ICSharpCode.Decompiler
{
	public class UniversalAssemblyResolver : IAssemblyResolver
	{
		DotNetCorePathFinder dotNetCorePathFinder;
		readonly bool throwOnError;
		readonly string mainAssemblyFileName;
		readonly string baseDirectory;
		readonly List<string> directories = new List<string>();
		readonly List<string> gac_paths = GetGacPaths();
		HashSet<string> targetFrameworkSearchPaths;

		/// <summary>
		/// Detect whether we're in a Mono environment.
		/// </summary>
		/// <remarks>This is used whenever we're trying to decompile a plain old .NET framework assembly on Unix.</remarks>
		static bool DetectMono()
		{
			// TODO : test whether this works with Mono on *Windows*, not sure if we'll
			// ever need this...
			if (Type.GetType("Mono.Runtime") != null)
				return true;
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				return true;
			return false;
		}

		public void AddSearchDirectory(string directory)
		{
			directories.Add(directory);
		}

		public void RemoveSearchDirectory(string directory)
		{
			directories.Remove(directory);
		}

		public string[] GetSearchDirectories()
		{
			return directories.ToArray();
		}

		public string TargetFramework {
			get => targetFramework;
			set {
				if (targetFramework != value) {
					targetFramework = value;
					targetFrameworkSearchPaths = null;
				}
			}
		}

		protected UniversalAssemblyResolver(string mainAssemblyFileName, bool throwOnError)
		{
			this.mainAssemblyFileName = mainAssemblyFileName;
			this.baseDirectory = Path.GetDirectoryName(mainAssemblyFileName);
			this.throwOnError = throwOnError;
			if (string.IsNullOrWhiteSpace(this.baseDirectory))
				this.baseDirectory = Environment.CurrentDirectory;
			AddSearchDirectory(baseDirectory);
		}

		public static ModuleDefinition LoadMainModule(string mainAssemblyFileName, bool throwOnError = true, bool inMemory = false)
		{
			var resolver = new UniversalAssemblyResolver(mainAssemblyFileName, throwOnError);

			var module = ModuleDefinition.ReadModule(mainAssemblyFileName, new ReaderParameters {
				AssemblyResolver = resolver,
				InMemory = inMemory
			});

			resolver.TargetFramework = module.Assembly.DetectTargetFrameworkId();

			return module;
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			return Resolve(name, new ReaderParameters());
		}

		public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
		{
			var file = FindAssemblyFile(name);
			if (file == null) {
				if (throwOnError)
					throw new AssemblyResolutionException(name);
				return null;
			}
			return GetAssembly(file, parameters);
		}

		public string FindAssemblyFile(AssemblyNameReference name)
		{
			var targetFramework = TargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
			string file = null;
			switch (targetFramework[0]) {
				case ".NETCoreApp":
				case ".NETStandard":
					if (targetFramework.Length != 2)
						goto default;
					if (dotNetCorePathFinder == null) {
						var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
						dotNetCorePathFinder = new DotNetCorePathFinder(mainAssemblyFileName, TargetFramework, version);
					}
					file = dotNetCorePathFinder.TryResolveDotNetCore(name);
					if (file != null)
						return file;
					goto default;
				case "Silverlight":
					if (targetFramework.Length != 2)
						goto default;
					file = ResolveSilverlight(name, new Version(targetFramework[1]));
					if (file != null)
						return file;
					goto default;
				default:
					return ResolveInternal(name);
			}
		}

		void AddTargetFrameworkSearchPathIfExists(string path)
		{
			if (targetFrameworkSearchPaths == null) {
				targetFrameworkSearchPaths = new HashSet<string>();
			}
			if (Directory.Exists(path))
				targetFrameworkSearchPaths.Add(path);
		}

		/// <summary>
		/// This only works on Windows
		/// </summary>
		string ResolveSilverlight(AssemblyNameReference name, Version version)
		{
			AddTargetFrameworkSearchPathIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Silverlight"));
			AddTargetFrameworkSearchPathIfExists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Silverlight"));

			foreach (var baseDirectory in targetFrameworkSearchPaths) {
				var versionDirectory = Path.Combine(baseDirectory, FindClosestVersionDirectory(baseDirectory, version));
				var file = SearchDirectory(name, versionDirectory);
				if (file != null)
					return file;
			}
			return null;
		}

		string FindClosestVersionDirectory(string basePath, Version version)
		{
			string path = null;
			foreach (var folder in new DirectoryInfo(basePath).GetDirectories().Select(d => DotNetCorePathFinder.ConvertToVersion(d.Name))
				.Where(v => v.Item1 != null).OrderByDescending(v => v.Item1)) {
				if (path == null || folder.Item1 >= version)
					path = folder.Item2;
			}
			return path ?? version.ToString();
		}

		string ResolveInternal(AssemblyNameReference name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			var assembly = SearchDirectory(name, directories);
			if (assembly != null)
				return assembly;

			if (name.IsRetargetable) {
				// if the reference is retargetable, zero it
				name = new AssemblyNameReference(name.Name, ZeroVersion) {
					PublicKeyToken = Empty<byte>.Array,
				};
			}

			var framework_dir = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
			var framework_dirs = DetectMono()
				? new[] { framework_dir, Path.Combine(framework_dir, "Facades") }
				: new[] { framework_dir };

			if (IsZero(name.Version)) {
				assembly = SearchDirectory(name, framework_dirs);
				if (assembly != null)
					return assembly;
			}

			if (name.Name == "mscorlib") {
				assembly = GetCorlib(name);
				if (assembly != null)
					return assembly;
			}

			assembly = GetAssemblyInGac(name);
			if (assembly != null)
				return assembly;

			assembly = SearchDirectory(name, framework_dirs);
			if (assembly != null)
				return assembly;

			if (throwOnError)
				throw new AssemblyResolutionException(name);
			return null;
		}

		#region .NET / mono GAC handling
		string SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories)
		{
			foreach (var directory in directories) {
				string file = SearchDirectory(name, directory);
				if (file != null)
					return file;
			}

			return null;
		}

		string SearchDirectory(AssemblyNameReference name, string directory)
		{
			var extensions = name.IsWindowsRuntime ? new[] { ".winmd", ".dll" } : new[] { ".exe", ".dll" };
			foreach (var extension in extensions) {
				string file = Path.Combine(directory, name.Name + extension);
				if (!File.Exists(file))
					continue;
				try {
					return file;
				} catch (BadImageFormatException) {
					continue;
				}
			}
			return null;
		}

		static bool IsZero(Version version)
		{
			return version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;
		}

		static Version ZeroVersion = new Version(0, 0, 0, 0);
		private string targetFramework;

		string GetCorlib(AssemblyNameReference reference)
		{
			var version = reference.Version;
			var corlib = typeof(object).Assembly.GetName();

			if (corlib.Version == version || IsZero(version))
				return typeof(object).Module.FullyQualifiedName;

			string path;
			if (DetectMono()) {
				path = GetMonoMscorlibBasePath(version);
			} else {
				path = GetMscorlibBasePath(version);
			}

			if (path == null)
				return null;

			var file = Path.Combine(path, "mscorlib.dll");
			if (File.Exists(file))
				return file;

			return null;
		}

		string GetMscorlibBasePath(Version version)
		{
			string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET");
			string[] frameworkPaths = new[] {
				Path.Combine(rootPath, "Framework"),
				Path.Combine(rootPath, "Framework64")
			};

			string folder = GetSubFolderForVersion();

			foreach (var path in frameworkPaths) {
				var basePath = Path.Combine(path, folder);
				if (Directory.Exists(basePath))
					return basePath;
			}

			if (throwOnError)
				throw new NotSupportedException("Version not supported: " + version);
			return null;

			string GetSubFolderForVersion()
			{
				switch (version.Major) {
					case 1:
						if (version.MajorRevision == 3300)
							return "v1.0.3705";
						return "v1.1.4322";
					case 2:
						return "v2.0.50727";
					case 4:
						return "v4.0.30319";
					default:
						if (throwOnError)
							throw new NotSupportedException("Version not supported: " + version);
						return null;
				}
			}
		}

		string GetMonoMscorlibBasePath(Version version)
		{
			var path = Directory.GetParent(typeof(object).Module.FullyQualifiedName).Parent.FullName;
			if (version.Major == 1)
				path = Path.Combine(path, "1.0");
			else if (version.Major == 2) {
				if (version.MajorRevision == 5)
					path = Path.Combine(path, "2.1");
				else
					path = Path.Combine(path, "2.0");
			} else if (version.Major == 4)
				path = Path.Combine(path, "4.0");
			else {
				if (throwOnError)
					throw new NotSupportedException("Version not supported: " + version);
				return null;
			}
			return path;
		}

		static List<string> GetGacPaths()
		{
			if (DetectMono())
				return GetDefaultMonoGacPaths();

			var paths = new List<string>(2);
			var windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			if (windir == null)
				return paths;

			paths.Add(Path.Combine(windir, "assembly"));
			paths.Add(Path.Combine(windir, "Microsoft.NET", "assembly"));
			return paths;
		}

		static List<string> GetDefaultMonoGacPaths()
		{
			var paths = new List<string>(1);
			var gac = GetCurrentMonoGac();
			if (gac != null)
				paths.Add(gac);

			var gac_paths_env = Environment.GetEnvironmentVariable("MONO_GAC_PREFIX");
			if (string.IsNullOrEmpty(gac_paths_env))
				return paths;

			var prefixes = gac_paths_env.Split(Path.PathSeparator);
			foreach (var prefix in prefixes) {
				if (string.IsNullOrEmpty(prefix))
					continue;

				var gac_path = Path.Combine(Path.Combine(Path.Combine(prefix, "lib"), "mono"), "gac");
				if (Directory.Exists(gac_path) && !paths.Contains(gac))
					paths.Add(gac_path);
			}

			return paths;
		}

		static string GetCurrentMonoGac()
		{
			return Path.Combine(
				Directory.GetParent(
					Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName)).FullName,
				"gac");
		}

		AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
		{
			if (parameters.AssemblyResolver == null)
				parameters.AssemblyResolver = this;

			return ModuleDefinition.ReadModule(file, parameters).Assembly;
		}

		string GetAssemblyInGac(AssemblyNameReference reference)
		{
			if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
				return null;

			if (DetectMono())
				return GetAssemblyInMonoGac(reference);

			return GetAssemblyInNetGac(reference);
		}

		string GetAssemblyInMonoGac(AssemblyNameReference reference)
		{
			for (int i = 0; i < gac_paths.Count; i++) {
				var gac_path = gac_paths[i];
				var file = GetAssemblyFile(reference, string.Empty, gac_path);
				if (File.Exists(file))
					return file;
			}

			return null;
		}

		string GetAssemblyInNetGac(AssemblyNameReference reference)
		{
			var gacs = new[] { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
			var prefixes = new[] { string.Empty, "v4.0_" };

			for (int i = 0; i < gac_paths.Count; i++) {
				for (int j = 0; j < gacs.Length; j++) {
					var gac = Path.Combine(gac_paths[i], gacs[j]);
					var file = GetAssemblyFile(reference, prefixes[i], gac);
					if (Directory.Exists(gac) && File.Exists(file))
						return file;
				}
			}

			return null;
		}

		static string GetAssemblyFile(AssemblyNameReference reference, string prefix, string gac)
		{
			var gac_folder = new StringBuilder()
				.Append(prefix)
				.Append(reference.Version)
				.Append("__");

			for (int i = 0; i < reference.PublicKeyToken.Length; i++)
				gac_folder.Append(reference.PublicKeyToken[i].ToString("x2"));

			return Path.Combine(
				Path.Combine(
					Path.Combine(gac, reference.Name), gac_folder.ToString()),
				reference.Name + ".dll");
		}

		#endregion

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
