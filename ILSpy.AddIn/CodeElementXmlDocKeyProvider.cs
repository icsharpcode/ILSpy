using System;
using System.Collections.Generic;
using System.Text;

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
		#region GetKey
		public static string GetKey(EnvDTE.CodeElement member)
		{
			StringBuilder b = new StringBuilder();
			if ((member.Kind == EnvDTE.vsCMElement.vsCMElementDelegate) ||
				(member.Kind == EnvDTE.vsCMElement.vsCMElementEnum) ||
				(member.Kind == EnvDTE.vsCMElement.vsCMElementInterface) ||
				(member.Kind == EnvDTE.vsCMElement.vsCMElementStruct) ||
				(member.Kind == EnvDTE.vsCMElement.vsCMElementClass)) {
				b.Append("T:");
				AppendTypeName(b, member.FullName, true, false);
			}
			else if (member.Kind == EnvDTE.vsCMElement.vsCMElementNamespace){
				b.Append("N:");
				b.Append(member.FullName);
			}
			else {
				if (member.Kind == EnvDTE.vsCMElement.vsCMElementVariable)
					b.Append("F:");
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementProperty)
					b.Append("P:");
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementEvent)
					b.Append("E:");
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementFunction)
					b.Append("M:");

				int nameIndex = member.FullName.LastIndexOf(member.Name);
				string typeName = member.FullName.Substring(0, nameIndex - 1);
				string memberName = member.FullName.Substring(nameIndex);

				if (member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE80.CodeFunction2 mr = (EnvDTE80.CodeFunction2)member;
					if (mr.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionConstructor) {
						memberName = memberName.Replace(member.Name, "#ctor");
					}
				}

				string[] genericTypeParameters = AppendTypeName(b, typeName, true, false);
				b.Append('.');
				string[] genericMethodParameters = AppendTypeName(b, memberName.Replace('.', '#'), true, true);
				EnvDTE.CodeElements parameters;
				EnvDTE.CodeTypeRef explicitReturnType = null;
				if (member.Kind == EnvDTE.vsCMElement.vsCMElementProperty) {
					parameters = ((EnvDTE.CodeProperty)member).Getter.Parameters;
				}
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE80.CodeFunction2 mr = (EnvDTE80.CodeFunction2)member;
					parameters = mr.Parameters;
					if (mr.Name == "op_Implicit" || mr.Name == "op_Explicit") {
						// TODO: check operator overloads to see if these work
						explicitReturnType = mr.Type;
					}
				}
				else {
					parameters = null;
				}
				if (parameters != null && parameters.Count > 0) {
					b.Append('(');
					int i = 0;
					foreach (EnvDTE80.CodeParameter2 parameter in parameters) {
						if (i > 0) b.Append(',');
						AppendParameterTypeName(b, parameter, genericTypeParameters, genericMethodParameters);
						++i;
					}
					b.Append(')');
				}
				if (explicitReturnType != null) {
					// TODO: test explicit return types
					b.Append('~');
					AppendTypeName(b, explicitReturnType.AsFullName, true, false);
				}
			}
			return b.ToString();
		}

		static string[] AppendTypeName(StringBuilder b, string typeName, bool appendGenericParameterCount, bool isMethod)
		{
			List<string> allGenericParameters = new List<string>();
			StringBuilder genericParameterName = new StringBuilder();

			bool inGenericParameters = false;
			int genericParameterCount = 0;
			foreach (char ch in typeName) {
				if (inGenericParameters) {
					switch (ch) {
						case ',':
							++genericParameterCount;
							allGenericParameters.Add(genericParameterName.ToString());
							genericParameterName.Clear();
							break;
						case '>':
							++genericParameterCount;
							allGenericParameters.Add(genericParameterName.ToString());
							genericParameterName.Clear();
							if (appendGenericParameterCount) {
								b.Append(genericParameterCount);
							}
							inGenericParameters = false;
							break;
						case ' ':
							break;
						default:
							genericParameterName.Append(ch);
							break;
					}
				}
				else {
					switch (ch) {
						case '<':
							if (appendGenericParameterCount) {
								b.Append('`');
								if (isMethod) {
									b.Append('`');
								}
							}
							inGenericParameters = true;
							genericParameterCount = 0;
							break;
						case '[':
						case ']':
							break;
						default:
							b.Append(ch);
							break;
					}
				}
			}

			return allGenericParameters.ToArray();
		}

		static void AppendParameterTypeName(StringBuilder b, EnvDTE80.CodeParameter2 parameter, string[] genericTypeParameters, string[] genericMethodParameters)
		{
			EnvDTE80.CodeTypeRef2 typeRef = (EnvDTE80.CodeTypeRef2)parameter.Type;

			int indexOfGenericTypeParameter = Array.IndexOf(genericTypeParameters, typeRef.AsFullName);
			int indexOfGenericMethodParameter = Array.IndexOf(genericMethodParameters, typeRef.AsFullName);
			if (indexOfGenericTypeParameter >= 0) {
				b.Append("`");
				b.Append(indexOfGenericTypeParameter);
			}
			else if (indexOfGenericMethodParameter >= 0) {
				b.Append("``");
				b.Append(indexOfGenericMethodParameter);
			}
			else if (typeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefPointer) {
				AppendTypeNameWithArguments(b, (EnvDTE.CodeElement)typeRef.ElementType, genericTypeParameters, genericMethodParameters);
			}
			else {
				AppendTypeNameWithArguments(b, (EnvDTE.CodeElement)typeRef.CodeType, genericTypeParameters, genericMethodParameters);
			}

			if (typeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefArray) {
				b.Append('[');
				for (int i = 0; i < typeRef.Rank; i++) {
					if (i > 0)
						b.Append(',');
					// TODO: how to get array bounds from EnvDTE code model?
					//ArrayDimension ad = arrayType.Dimensions[i];
					//if (ad.IsSized) {
					//	b.Append(ad.LowerBound);
					//	b.Append(':');
					//	b.Append(ad.UpperBound);
					//}
				}
				b.Append(']');
			}

			// TODO: test out and ref parameters
			if ((parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindRef) ||
				(parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindOut)) {
				b.Append('@');
			}

			// TODO: test pointer parameters
			if (typeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefPointer) {
				b.Append('*');
			}
		}

		private static void AppendTypeNameWithArguments(StringBuilder b, EnvDTE.CodeElement type, string[] genericTypeParameters, string[] genericMethodParameters)
		{
			int nameIndex = type.FullName.LastIndexOf(type.Name);
			string containerName = type.FullName.Substring(0, nameIndex - 1);
			string typeName = type.FullName.Substring(nameIndex);

			EnvDTE.CodeElement declaringElement = null;
			try {
				declaringElement = (EnvDTE.CodeElement)type.Collection.Parent;
			}
			catch (Exception) {
			}

			if (declaringElement == null) {
				if (!string.IsNullOrEmpty(containerName)) {
					b.Append(containerName);
					b.Append('.');
				}
			}
			else if (declaringElement.Kind == EnvDTE.vsCMElement.vsCMElementNamespace) {
				b.Append(declaringElement.FullName);
				b.Append('.');
			}
			else {
				AppendTypeNameWithArguments(b, declaringElement, genericTypeParameters, genericMethodParameters);
				b.Append('.');
			}

			string[] localTypeParameters = AppendTypeName(b, typeName, false, false);

			if (localTypeParameters.Length > 0) {
				b.Append('{');
				for (int i = 0; i < localTypeParameters.Length; i++) {
					if (i > 0) b.Append(',');
					string localTypeParameter = localTypeParameters[i];
					int indexOfGenericTypeParameter = Array.IndexOf(genericTypeParameters, localTypeParameter);
					int indexOfGenericMethodParameter = Array.IndexOf(genericMethodParameters, localTypeParameter);
					if (indexOfGenericTypeParameter >= 0) {
						b.Append("`");
						b.Append(indexOfGenericTypeParameter);
					}
					else if (indexOfGenericMethodParameter >= 0) {
						b.Append("``");
						b.Append(indexOfGenericMethodParameter);
					}
					else {
						b.Append(localTypeParameter);
					}
				}
				b.Append('}');
			}
		}
		#endregion

		public static string XXXGetKey(EnvDTE.CodeElement codeElement)
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