namespace ICSharpCode.ILSpy
{
    public static class LoadedAssemblyExtensions
    {
        public static bool IsNet45(this LoadedAssembly assembly)
        {
            foreach (var custom in assembly.AssemblyDefinition.CustomAttributes)
            {
                if (custom.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute")
                {
                    var framework = custom.Properties[0].Argument.Value;
                    if (framework.ToString().StartsWith(".NET Framework 4.5"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
