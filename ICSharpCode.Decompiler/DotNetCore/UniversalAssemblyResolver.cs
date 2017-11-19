using System;
using System.Collections.Generic;
using System.IO;
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

		public static readonly bool OnMono = Type.GetType("Mono.Runtime") != null;

		public event AssemblyResolveEventHandler ResolveFailed;

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
			var targetFramework = TargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
			string file = null;
			switch (targetFramework[0]) {
				case ".NETCoreApp":
				case ".NETStandard":
					if (targetFramework.Length != 2)
						return ResolveInternal(name, parameters);
					if (dotNetCorePathFinder == null) {
						var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
						dotNetCorePathFinder = new DotNetCorePathFinder(mainAssemblyFileName, TargetFramework, version);
					}
					file = dotNetCorePathFinder.TryResolveDotNetCore(name);
					if (file == null)
						return ResolveInternal(name, parameters);
					else {
						var asm = ModuleDefinition.ReadModule(file, parameters).Assembly;
						if (throwOnError && asm == null)
							throw new AssemblyResolutionException(name);
						return asm;
					}
				default:
					return ResolveInternal(name, parameters);
			}
		}

		AssemblyDefinition ResolveInternal(AssemblyNameReference name, ReaderParameters parameters)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (parameters == null)
				throw new ArgumentNullException(nameof(parameters));

			var assembly = SearchDirectory(name, directories, parameters);
			if (assembly != null)
				return assembly;

			if (name.IsRetargetable) {
				// if the reference is retargetable, zero it
				name = new AssemblyNameReference(name.Name, ZeroVersion) {
					PublicKeyToken = Empty<byte>.Array,
				};
			}

			var framework_dir = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
			var framework_dirs = OnMono
				? new[] { framework_dir, Path.Combine(framework_dir, "Facades") }
				: new[] { framework_dir };

			if (IsZero(name.Version)) {
				assembly = SearchDirectory(name, framework_dirs, parameters);
				if (assembly != null)
					return assembly;
			}

			if (name.Name == "mscorlib") {
				assembly = GetCorlib(name, parameters);
				if (assembly != null)
					return assembly;
			}

			assembly = GetAssemblyInGac(name, parameters);
			if (assembly != null)
				return assembly;

			assembly = SearchDirectory(name, framework_dirs, parameters);
			if (assembly != null)
				return assembly;

			if (ResolveFailed != null) {
				assembly = ResolveFailed(this, name);
				if (assembly != null)
					return assembly;
			}

			if (throwOnError)
				throw new AssemblyResolutionException(name);
			return null;
		}

		#region .NET / mono GAC handling
		AssemblyDefinition SearchDirectory(AssemblyNameReference name, IEnumerable<string> directories, ReaderParameters parameters)
		{
			var extensions = name.IsWindowsRuntime ? new[] { ".winmd", ".dll" } : new[] { ".exe", ".dll" };
			foreach (var directory in directories) {
				foreach (var extension in extensions) {
					string file = Path.Combine(directory, name.Name + extension);
					if (!File.Exists(file))
						continue;
					try {
						return GetAssembly(file, parameters);
					} catch (System.BadImageFormatException) {
						continue;
					}
				}
			}

			return null;
		}

		static bool IsZero(Version version)
		{
			return version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;
		}

		static Version ZeroVersion = new Version(0, 0, 0, 0);

		AssemblyDefinition GetCorlib(AssemblyNameReference reference, ReaderParameters parameters)
		{
			var version = reference.Version;
			var corlib = typeof(object).Assembly.GetName();

			if (corlib.Version == version || IsZero(version))
				return GetAssembly(typeof(object).Module.FullyQualifiedName, parameters);

			var path = Directory.GetParent(
				Directory.GetParent(
					typeof(object).Module.FullyQualifiedName).FullName
				).FullName;

			if (OnMono) {
				if (version.Major == 1)
					path = Path.Combine(path, "1.0");
				else if (version.Major == 2) {
					if (version.MajorRevision == 5)
						path = Path.Combine(path, "2.1");
					else
						path = Path.Combine(path, "2.0");
				} else if (version.Major == 4)
					path = Path.Combine(path, "4.0");
				else
					throw new NotSupportedException("Version not supported: " + version);
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
						throw new NotSupportedException("Version not supported: " + version);
				}
			}

			var file = Path.Combine(path, "mscorlib.dll");
			if (File.Exists(file))
				return GetAssembly(file, parameters);

			return null;
		}

		static List<string> GetGacPaths()
		{
			if (OnMono)
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

		AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
		{
			if (parameters.AssemblyResolver == null)
				parameters.AssemblyResolver = this;

			return ModuleDefinition.ReadModule(file, parameters).Assembly;
		}

		AssemblyDefinition GetAssemblyInGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			if (reference.PublicKeyToken == null || reference.PublicKeyToken.Length == 0)
				return null;

			if (OnMono)
				return GetAssemblyInMonoGac(reference, parameters);

			return GetAssemblyInNetGac(reference, parameters);
		}

		AssemblyDefinition GetAssemblyInMonoGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			for (int i = 0; i < gac_paths.Count; i++) {
				var gac_path = gac_paths[i];
				var file = GetAssemblyFile(reference, string.Empty, gac_path);
				if (File.Exists(file))
					return GetAssembly(file, parameters);
			}

			return null;
		}

		AssemblyDefinition GetAssemblyInNetGac(AssemblyNameReference reference, ReaderParameters parameters)
		{
			var gacs = new[] { "GAC_MSIL", "GAC_32", "GAC_64", "GAC" };
			var prefixes = new[] { string.Empty, "v4.0_" };

			for (int i = 0; i < 2; i++) {
				for (int j = 0; j < gacs.Length; j++) {
					var gac = Path.Combine(gac_paths[i], gacs[j]);
					var file = GetAssemblyFile(reference, prefixes[i], gac);
					if (Directory.Exists(gac) && File.Exists(file))
						return GetAssembly(file, parameters);
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
