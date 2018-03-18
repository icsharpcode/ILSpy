using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace ICSharpCode.Decompiler.Metadata
{
	public class UniversalAssemblyResolver : IAssemblyResolver
	{
		DotNetCorePathFinder dotNetCorePathFinder;
		readonly bool throwOnError;
		readonly string mainAssemblyFileName;
		readonly string baseDirectory;
		readonly List<string> directories = new List<string>();
		readonly List<string> gac_paths = GetGacPaths();

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

		public string TargetFramework { get; set; }

		protected UniversalAssemblyResolver(string mainAssemblyFileName, bool throwOnError)
		{
			this.mainAssemblyFileName = mainAssemblyFileName;
			this.baseDirectory = Path.GetDirectoryName(mainAssemblyFileName);
			this.throwOnError = throwOnError;
			if (string.IsNullOrWhiteSpace(this.baseDirectory))
				this.baseDirectory = Environment.CurrentDirectory;
			AddSearchDirectory(baseDirectory);
		}

		public static PEFile LoadMainModule(string mainAssemblyFileName, bool throwOnError = true, bool inMemory = false)
		{
			var resolver = new UniversalAssemblyResolver(mainAssemblyFileName, throwOnError);

			var module = new PEReader(new FileStream(mainAssemblyFileName, FileMode.Open), inMemory ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default);

			resolver.TargetFramework = module.DetectTargetFrameworkId();

			return new PEFile(mainAssemblyFileName, module, resolver);
		}

		public PEFile Resolve(IAssemblyReference name)
		{
			var file = FindAssemblyFile(name);
			if (file == null) {
				if (throwOnError)
					throw new AssemblyResolutionException(name);
				return null;
			}
			return new PEFile(file, GetAssembly(file), this);
		}

		public string FindAssemblyFile(IAssemblyReference name)
		{
			var targetFramework = TargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
			string file = null;
			switch (targetFramework[0]) {
				case ".NETCoreApp":
				case ".NETStandard":
					if (targetFramework.Length != 2)
						return ResolveInternal(name);
					if (dotNetCorePathFinder == null) {
						var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
						dotNetCorePathFinder = new DotNetCorePathFinder(mainAssemblyFileName, TargetFramework, version);
					}
					file = dotNetCorePathFinder.TryResolveDotNetCore(name);
					if (file != null)
						return file;
					return ResolveInternal(name);
				default:
					return ResolveInternal(name);
			}
		}

		string ResolveInternal(IAssemblyReference name)
		{
			if (name.IsNil())
				throw new ArgumentNullException(nameof(name));

			var assembly = SearchDirectory(name, directories);
			if (assembly != null)
				return assembly;

			var framework_dir = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
			var framework_dirs = DetectMono()
				? new[] { framework_dir, Path.Combine(framework_dir, "Facades") }
				: new[] { framework_dir };

			if (IsZeroVersionOrRetargetable(name)) {
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
		string SearchDirectory(IAssemblyReference name, IEnumerable<string> directories)
		{
			var extensions = name.IsWindowsRuntime ? new[] { ".winmd", ".dll" } : new[] { ".exe", ".dll" };
			foreach (var directory in directories) {
				foreach (var extension in extensions) {
					string file = Path.Combine(directory, name.Name + extension);
					if (!File.Exists(file))
						continue;
					try {
						return file;
					} catch (System.BadImageFormatException) {
						continue;
					}
				}
			}

			return null;
		}

		static bool IsZeroVersionOrRetargetable(IAssemblyReference reference)
		{
			return IsZero(reference.Version) || reference.IsRetargetable;
		}

		static bool IsZero(Version version)
		{
			return version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;
		}

		string GetCorlib(IAssemblyReference reference)
		{
			var version = reference.Version;
			var corlib = typeof(object).Assembly.GetName();

			if (corlib.Version == version || IsZeroVersionOrRetargetable(reference))
				return typeof(object).Module.FullyQualifiedName;

			var path = Directory.GetParent(
				Directory.GetParent(
					typeof(object).Module.FullyQualifiedName).FullName
				).FullName;

			if (DetectMono()) {
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
			} else {
				switch (version.Major) {
					case 1:
						if (version.MajorRevision == 3300)
							path = Path.Combine(path, "v1.0.3705");
						else
							path = Path.Combine(path, "v1.0.5000.0");
						break;
					case 2:
						path = Path.Combine(path, "v2.0.50727");
						break;
					case 4:
						path = Path.Combine(path, "v4.0.30319");
						break;
					default:
						if (throwOnError)
							throw new NotSupportedException("Version not supported: " + version);
						return null;
				}
			}

			var file = Path.Combine(path, "mscorlib.dll");
			if (File.Exists(file))
				return file;

			return null;
		}

		static List<string> GetGacPaths()
		{
			if (DetectMono())
				return GetDefaultMonoGacPaths();

			var paths = new List<string>(2);
			var windir = Environment.GetEnvironmentVariable("WINDIR");
			if (windir == null)
				return paths;

			paths.Add(Path.Combine(windir, "assembly"));
			paths.Add(Path.Combine(windir, Path.Combine("Microsoft.NET", "assembly")));
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

		PEReader GetAssembly(string file)
		{
			return new PEReader(new FileStream(file, FileMode.Open));
		}

		string GetAssemblyInGac(IAssemblyReference reference)
		{
			if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
				return null;

			if (DetectMono())
				return GetAssemblyInMonoGac(reference);

			return GetAssemblyInNetGac(reference);
		}

		string GetAssemblyInMonoGac(IAssemblyReference reference)
		{
			for (int i = 0; i < gac_paths.Count; i++) {
				var gac_path = gac_paths[i];
				var file = GetAssemblyFile(reference, string.Empty, gac_path);
				if (File.Exists(file))
					return file;
			}

			return null;
		}

		string GetAssemblyInNetGac(IAssemblyReference reference)
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

		static string GetAssemblyFile(IAssemblyReference reference, string prefix, string gac)
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
