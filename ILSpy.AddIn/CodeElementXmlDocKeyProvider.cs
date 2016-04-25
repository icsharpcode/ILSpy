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

		private static void AppendParameterTypeName(StringBuilder b, EnvDTE80.CodeParameter2 parameter, string[] genericTypeParameters, string[] genericMethodParameters)
		{
			EnvDTE80.CodeTypeRef2 parameterTypeRef = (EnvDTE80.CodeTypeRef2)parameter.Type;
			string parameterTypeString = parameterTypeRef.AsFullName;
			int substringStart = 0;
			for (int i = 0; i < parameterTypeString.Length; ++i) {
				switch (parameterTypeString[i]) {
					case '<':
						AppendParameterTypeSubstring(b, parameterTypeString, substringStart, i, genericTypeParameters, genericMethodParameters);
						substringStart = i + 1;
						b.Append('{');
						break;
					case '>':
						AppendParameterTypeSubstring(b, parameterTypeString, substringStart, i, genericTypeParameters, genericMethodParameters);
						substringStart = i + 1;
						b.Append('}');
						break;
					case ',':
						AppendParameterTypeSubstring(b, parameterTypeString, substringStart, i, genericTypeParameters, genericMethodParameters);
						substringStart = i + 1;
						b.Append(',');
						break;
				}
			}

			AppendParameterTypeSubstring(b, parameterTypeString, substringStart, parameterTypeString.Length, genericTypeParameters, genericMethodParameters);

			if (parameterTypeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefArray) {
				b.Append('[');
				for (int i = 0; i < parameterTypeRef.Rank; i++) {
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
			if (parameterTypeRef.TypeKind == EnvDTE.vsCMTypeRef.vsCMTypeRefPointer) {
				b.Append('*');
			}
		}

		private static void AppendParameterTypeSubstring(StringBuilder b, string parameterTypeString, int substringStart, int substringStop, string[] genericTypeParameters, string[] genericMethodParameters)
		{
			if (substringStart < substringStop) {
				string substring = parameterTypeString.Substring(substringStart, substringStop - substringStart);
				int indexOfGenericTypeParameter = Array.IndexOf(genericTypeParameters, substring);
				int indexOfGenericMethodParameter = Array.IndexOf(genericMethodParameters, substring);
				if (indexOfGenericTypeParameter >= 0) {
					b.Append("`");
					b.Append(indexOfGenericTypeParameter);
				}
				else if (indexOfGenericMethodParameter >= 0) {
					b.Append("``");
					b.Append(indexOfGenericMethodParameter);
				}
				else {
					b.Append(substring.Trim());
				}
			}
		}
		#endregion
	}
}