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
				AppendTypeName(b, member);
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
				AppendTypeName(b, (EnvDTE.CodeElement)member.Collection.Parent);
				b.Append('.');
				b.Append(member.Name.Replace('.', '#'));
				EnvDTE.CodeElements parameters;
				string[] genericMethodParameters = cEmptyStringArray;
				EnvDTE.CodeTypeRef explicitReturnType = null;
				if (member.Kind == EnvDTE.vsCMElement.vsCMElementProperty) {
					parameters = ((EnvDTE.CodeProperty)member).Getter.Parameters;
				}
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE80.CodeFunction2 mr = (EnvDTE80.CodeFunction2)member;
					if (mr.IsGeneric) {
						genericMethodParameters = GetGenericParameters(member);
						b.Append("``");
						b.Append(genericMethodParameters.Length);
					}
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
						AppendTypeName(b, parameter, genericMethodParameters);
						++i;
					}
					b.Append(')');
				}
				if (explicitReturnType != null) {
					b.Append('~');
					AppendTypeName(b, (EnvDTE.CodeElement)explicitReturnType);
				}
			}
			return b.ToString();
		}

		static void AppendTypeName(StringBuilder b, EnvDTE.CodeElement type)
		{
			if (type == null) {
				// could happen when a TypeSpecification has no ElementType; e.g. function pointers in C++/CLI assemblies
				return;
			}

			bool isGeneric =
				((type.Kind == EnvDTE.vsCMElement.vsCMElementClass) && ((EnvDTE80.CodeClass2)type).IsGeneric) ||
				((type.Kind == EnvDTE.vsCMElement.vsCMElementStruct) && ((EnvDTE80.CodeStruct2)type).IsGeneric) ||
				((type.Kind == EnvDTE.vsCMElement.vsCMElementInterface) && ((EnvDTE80.CodeInterface2)type).IsGeneric);

			if (isGeneric) {
				AppendTypeNameWithArguments(b, type, GetGenericParameters(type));
			}
			else {
				// TODO: handle generic parameters, but above?
				//GenericParameter gp = type as GenericParameter;
				//if (gp != null) {
				//	b.Append('`');
				//	if (gp.Owner.GenericParameterType == GenericParameterType.Method) {
				//		b.Append('`');
				//	}
				//	b.Append(gp.Position);
				//}
				EnvDTE.CodeElement declaringType = GetDeclaringType(type);
				if (declaringType != null) {
					AppendTypeName(b, declaringType);
					b.Append('.');
					b.Append(type.Name);
				}
				else {
					b.Append(type.FullName);
				}
			}
		}

		static void AppendTypeName(StringBuilder b, EnvDTE80.CodeParameter2 parameter, string[] genericMethodParameters)
		{
			EnvDTE80.CodeTypeRef2 typeRef = (EnvDTE80.CodeTypeRef2)parameter.Type;

			// Parameters of generic types from generic method are indicated as ``1, ``2, etc.
			// TODO: parameters of generic types from enclosing class (and nested classes??)
			int indexOfGenericParameter = Array.IndexOf(genericMethodParameters, typeRef.AsFullName);
			if (indexOfGenericParameter >= 0) {
				b.Append("``");
				b.Append(indexOfGenericParameter);
			}
			else if (typeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefArray) {
				AppendTypeNameWithArguments(b, (EnvDTE.CodeElement)typeRef.ElementType, genericMethodParameters);
			}
			else {
				AppendTypeNameWithArguments(b, (EnvDTE.CodeElement)typeRef.CodeType, genericMethodParameters);
			}

			// TODO: think we need to handle generic arguments here (from enclosing type or method, but how?)
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

			if ((parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindRef) ||
				(parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindOut)) {
				b.Append('@');
			}

			if (typeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefPointer) {
				b.Append('*');
			}
		}

		static int AppendTypeNameWithArguments(StringBuilder b, EnvDTE.CodeElement type, string[] genericArguments)
		{
			int outerTypeParameterCount = 0;
			EnvDTE.CodeElement declaringType = GetDeclaringType(type);
			if (declaringType != null) {
				outerTypeParameterCount = AppendTypeNameWithArguments(b, declaringType, genericArguments);
				b.Append('.');
			}
			else {
				string typeNamespace = GetNamespace(type);
				if (!string.IsNullOrEmpty(typeNamespace)) {
					b.Append(typeNamespace);
					b.Append('.');
				}
			}

			string[] localTypeParameters = GetGenericParameters(type);
			int localTypeParameterCount = localTypeParameters.Length;
			b.Append(type.Name);

			if (localTypeParameterCount > 0) {
				int totalTypeParameterCount = outerTypeParameterCount + localTypeParameterCount;
				b.Append('{');
				for (int i = 0; i < localTypeParameterCount; i++) {
					if (i > 0) b.Append(',');
					string localTypeParameter = localTypeParameters[i];
					int genericIndex = Array.IndexOf(genericArguments, localTypeParameter);
					if (genericIndex >= 0) {
						b.Append("``");
						b.Append(genericIndex);
					}
					else {
						b.Append(localTypeParameter);
					}
				}
				b.Append('}');
			}
			return outerTypeParameterCount + localTypeParameterCount;
		}

		private static string[] GetGenericParameters(EnvDTE.CodeElement codeElement)
		{
			int iGenericParameters = codeElement.FullName.LastIndexOf('<');
			if (iGenericParameters >= 0) {
				return codeElement.FullName.Substring(iGenericParameters).Split(cGenericParameterSeparators, StringSplitOptions.RemoveEmptyEntries);
			}
			else {
				return cEmptyStringArray;
			}
		}
		private static readonly char[] cGenericParameterSeparators = new char[] { '<', ',', ' ', '>' };
		private static readonly string[] cEmptyStringArray = new string[0];

		private static EnvDTE.CodeElement GetDeclaringType(EnvDTE.CodeElement codeElement)
		{
			EnvDTE.CodeElement declaringElement = null;
			if (codeElement != null) {
				declaringElement = (EnvDTE.CodeElement)codeElement.Collection.Parent;
				bool hasDeclaringType =
					(declaringElement.Kind == EnvDTE.vsCMElement.vsCMElementClass) ||
					(declaringElement.Kind == EnvDTE.vsCMElement.vsCMElementStruct) ||
					(declaringElement.Kind == EnvDTE.vsCMElement.vsCMElementInterface);
				if (!hasDeclaringType) {
					declaringElement = null;
				}
			}
			return declaringElement;
		}

		private static string GetNamespace(EnvDTE.CodeElement codeElement)
		{
			if (codeElement != null) {
				EnvDTE.CodeElement parent = (EnvDTE.CodeElement)codeElement.Collection.Parent;
				if (parent.Kind == EnvDTE.vsCMElement.vsCMElementNamespace) {
					return parent.FullName;
				}
			}
			return null;
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