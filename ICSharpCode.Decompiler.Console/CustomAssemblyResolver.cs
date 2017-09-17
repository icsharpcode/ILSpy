using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.Console
{
    class CustomAssemblyResolver : DefaultAssemblyResolver
    {
        DotNetCorePathFinder dotNetCorePathFinder;
        readonly string assemblyFileName;
        readonly Dictionary<string, UnresolvedAssemblyNameReference> loadedAssemblyReferences;

        public string TargetFramework { get; set; }

        public CustomAssemblyResolver(string fileName)
        {
            this.assemblyFileName = fileName;
            this.loadedAssemblyReferences = new Dictionary<string, UnresolvedAssemblyNameReference>();
            AddSearchDirectory(Path.GetDirectoryName(fileName));
            RemoveSearchDirectory(".");
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            var targetFramework = TargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
            string file = null;
            switch (targetFramework[0]) {
                case ".NETCoreApp":
                case ".NETStandard":
                    if (targetFramework.Length != 2) goto default;
                    if (dotNetCorePathFinder == null) {
                        var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
                        dotNetCorePathFinder = new DotNetCorePathFinder(assemblyFileName, TargetFramework, version, this.loadedAssemblyReferences);
                    }
                    file = dotNetCorePathFinder.TryResolveDotNetCore(name);
                    if (file == null) {
                        string dir = Path.GetDirectoryName(assemblyFileName);
                        if (File.Exists(Path.Combine(dir, name.Name + ".dll")))
                            file = Path.Combine(dir, name.Name + ".dll");
                        else if (File.Exists(Path.Combine(dir, name.Name + ".exe")))
                            file = Path.Combine(dir, name.Name + ".exe");
                    }
                    if (file == null)
                        return base.Resolve(name);
                    else
                        return ModuleDefinition.ReadModule(file, new ReaderParameters() { AssemblyResolver = this }).Assembly;
                default:
                    return base.Resolve(name);
            }
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            var targetFramework = TargetFramework.Split(new[] { ",Version=v" }, StringSplitOptions.None);
            string file = null;
            switch (targetFramework[0]) {
                case ".NETCoreApp":
                case ".NETStandard":
                    if (targetFramework.Length != 2) goto default;
                    if (dotNetCorePathFinder == null) {
                        var version = targetFramework[1].Length == 3 ? targetFramework[1] + ".0" : targetFramework[1];
                        dotNetCorePathFinder = new DotNetCorePathFinder(assemblyFileName, TargetFramework, version, this.loadedAssemblyReferences);
                    }
                    file = dotNetCorePathFinder.TryResolveDotNetCore(name);
                    if (file == null) {
                        string dir = Path.GetDirectoryName(assemblyFileName);
                        if (File.Exists(Path.Combine(dir, name.Name + ".dll")))
                            file = Path.Combine(dir, name.Name + ".dll");
                        else if (File.Exists(Path.Combine(dir, name.Name + ".exe")))
                            file = Path.Combine(dir, name.Name + ".exe");
                    }
                    if (file == null)
                        return base.Resolve(name, parameters);
                    else
                        return ModuleDefinition.ReadModule(file, parameters).Assembly;
                default:
                    return base.Resolve(name, parameters);
            }
        }
    }
}
