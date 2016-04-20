using System;

namespace ICSharpCode.ILSpy.AddIn
{
	/// <summary>
	/// Provides XML documentation tags for Visual Studio CodeElements.
	/// </summary>
	/// <remarks>
	/// Used to support the "/navigateTo" command line option when opening ILSpy. Must match
	/// the logic of ICSharpCode.ILSpy.XmlDoc.XmlDocKeyProvider, which does the same thing for
	/// a Mono.Cecil.MemberReference. See "ID string format" in Appendix A of the C# language
	/// specification for formatting requirements.
	/// </remarks>
	public static class CodeElementXmlDocKeyProvider
	{
		public static string GetKey(EnvDTE.CodeElement codeElement)
		{
			switch (codeElement.Kind) {
				case EnvDTE.vsCMElement.vsCMElementEvent:
					return string.Concat("E:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementVariable:
					return string.Concat("F:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementFunction: {
						var codeFunction = (EnvDTE80.CodeFunction2)codeElement;

						var idBuilder = new System.Text.StringBuilder();
						idBuilder.Append("M:");

						// Constructors need to be called "#ctor" for navigation purposes.
						string[] genericClassTypeParameters;
						string classFullName = GetCodeElementContainerString((EnvDTE.CodeElement)codeFunction.Parent, out genericClassTypeParameters);
						string functionName = (codeFunction.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionConstructor ? "#ctor" : codeFunction.Name);
						idBuilder.Append(classFullName);
						idBuilder.Append('.');
						idBuilder.Append(functionName);

						// Get type parameters to generic method, if present.
						string[] genericMethodTypeParameters = new string[0];
						int iGenericParams = codeFunction.FullName.LastIndexOf('<');
						if ((codeFunction.IsGeneric) && (iGenericParams >= 0)) {
							genericMethodTypeParameters = codeFunction.FullName.Substring(iGenericParams).Split(new char[] {'<', ',', ' ', '>'}, StringSplitOptions.RemoveEmptyEntries);
							idBuilder.Append("``");
							idBuilder.Append(genericMethodTypeParameters.Length);
						}

						// Append parameter types, to disambiguate overloaded methods.
						if (codeFunction.Parameters.Count > 0) {
							idBuilder.Append("(");
							bool first = true;
							foreach (EnvDTE.CodeParameter parameter in codeFunction.Parameters) {
								if (!first) {
									idBuilder.Append(",");
								}
								first = false;
								int genericClassTypeParameterIndex = Array.IndexOf(genericClassTypeParameters, parameter.Type.AsFullName);
								if (genericClassTypeParameterIndex >= 0) {
									idBuilder.Append('`');
									idBuilder.Append(genericClassTypeParameterIndex);
								}
								else {
									int genericMethodTypeParameterIndex = Array.IndexOf(genericMethodTypeParameters, parameter.Type.AsFullName);
									if (genericMethodTypeParameterIndex >= 0) {
										idBuilder.Append("``");
										idBuilder.Append(genericMethodTypeParameterIndex);
									}
									else {
										// Special handling for arrays, because AsFullName for an array is empty.
										if (parameter.Type.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefArray) {
											idBuilder.Append(parameter.Type.ElementType.AsFullName);
											idBuilder.Append("[]");
										}
										else {
											idBuilder.Append(parameter.Type.AsFullName);
										}
									}
								}
							}
							idBuilder.Append(")");
						}
						return idBuilder.ToString();
					}

				case EnvDTE.vsCMElement.vsCMElementNamespace:
					return string.Concat("N:",
						codeElement.FullName);

				case EnvDTE.vsCMElement.vsCMElementProperty:
					return string.Concat("P:",
						GetCodeElementContainerString((EnvDTE.CodeElement)codeElement.Collection.Parent),
						".", codeElement.Name);

				case EnvDTE.vsCMElement.vsCMElementDelegate:
				case EnvDTE.vsCMElement.vsCMElementEnum:
				case EnvDTE.vsCMElement.vsCMElementInterface:
				case EnvDTE.vsCMElement.vsCMElementStruct:
				case EnvDTE.vsCMElement.vsCMElementClass:
						return string.Concat("T:",
							GetCodeElementContainerString(codeElement));

				default:
					return string.Format("!:Code element {0} is of unsupported type {1}", codeElement.FullName, codeElement.Kind.ToString());
			}
		}

		private static string GetCodeElementContainerString(EnvDTE.CodeElement containerElement)
		{
			string[] genericTypeParametersIgnored;
			return GetCodeElementContainerString(containerElement, out genericTypeParametersIgnored);
		}

		private static string GetCodeElementContainerString(EnvDTE.CodeElement containerElement, out string[] genericTypeParameters)
		{
			genericTypeParameters = new string[0];

			switch (containerElement.Kind) {
				case EnvDTE.vsCMElement.vsCMElementNamespace:
					return containerElement.FullName;

				case EnvDTE.vsCMElement.vsCMElementInterface:
				case EnvDTE.vsCMElement.vsCMElementStruct:
				case EnvDTE.vsCMElement.vsCMElementClass: {
						var idBuilder = new System.Text.StringBuilder();
						idBuilder.Append(GetCodeElementContainerString((EnvDTE.CodeElement)containerElement.Collection.Parent));
						idBuilder.Append('.');
						idBuilder.Append(containerElement.Name);

						// For "Generic<T1,T2>" we need "Generic`2".
						bool isGeneric =
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementClass) && ((EnvDTE80.CodeClass2)containerElement).IsGeneric) ||
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementStruct) && ((EnvDTE80.CodeStruct2)containerElement).IsGeneric) ||
							((containerElement.Kind == EnvDTE.vsCMElement.vsCMElementInterface) && ((EnvDTE80.CodeInterface2)containerElement).IsGeneric);
						int iGenericParams = containerElement.FullName.LastIndexOf('<');
						if (isGeneric && (iGenericParams >= 0)) {
							genericTypeParameters = containerElement.FullName.Substring(iGenericParams).Split(new char[] {'<', ',', ' ', '>'}, StringSplitOptions.RemoveEmptyEntries);
							idBuilder.Append('`');
							idBuilder.Append(genericTypeParameters.Length);
						}

						return idBuilder.ToString();
					}

				default:
					return string.Format("!:Code element {0} is of unsupported container type {1}", containerElement.FullName, containerElement.Kind.ToString());
			}
		}
	}
}