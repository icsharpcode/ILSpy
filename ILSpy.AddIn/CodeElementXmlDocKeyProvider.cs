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
	/// specification for formatting requirements, and Samples/ILSpyAddInSamples.cs for examples.
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

				// Name substitutions for special cases.
				if (member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE80.CodeFunction2 mr = (EnvDTE80.CodeFunction2)member;
					if (mr.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionConstructor) {
						memberName = memberName.Replace(member.Name, "#ctor");
					}
					else if (mr.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionDestructor) {
						memberName = memberName.Replace(member.Name, "Finalize");
					}
					else if (mr.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionOperator) {
						if (memberName.StartsWith("implicit operator")) {
							memberName = "op_Implicit";
						}
						else if (memberName.StartsWith("explicit operator")) {
							memberName = "op_Explicit";
						}
						else {
							// NRefactory has a handy mapping we can make use of, just need to extract the operator sybol first.
							string[] memberNameWords = member.Name.Split(' ');
							if (memberNameWords.Length >= 2) {
								string operatorSymbol = memberNameWords[1];
								string operatorName = ICSharpCode.NRefactory.MonoCSharp.Operator.GetMetadataName(operatorSymbol);
								if (operatorName != null) {
									memberName = memberName.Replace(member.Name, operatorName);
								}
							}
						}
					}
				}
				else if (member.Kind == EnvDTE.vsCMElement.vsCMElementProperty) {
					if (member.Name == "this") {
						memberName = memberName.Replace(member.Name, "Item");
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
					if (memberName == "op_Implicit" || memberName == "op_Explicit") {
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
				char ch = parameterTypeString[i];
				switch (ch) {
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

					case '[':
						AppendParameterTypeSubstring(b, parameterTypeString, substringStart, i, genericTypeParameters, genericMethodParameters);
						b.Append('[');

						// Skip ahead to the closing bracket, counting commas to determine array rank.
						int rank = 1;
						do {
							++i;
							ch = parameterTypeString[i];
							if (ch == ',') {
								++rank;
							}
						}
						while (ch != ']');
						substringStart = i + 1;

						// For multi-dimensional arrays, add "0:" default array bounds. Note that non-standard bounds are not possible via C# declaration.
						if (rank > 1) {
							for (int r = 0; r < rank; ++r) {
								if (r != 0) {
									b.Append(',');
								}
								b.Append("0:");
							}
						}

						b.Append(']');
						break;

					case ',':
						AppendParameterTypeSubstring(b, parameterTypeString, substringStart, i, genericTypeParameters, genericMethodParameters);
						substringStart = i + 1;
						// Skip space after comma if present. (e.g. System.Collections.Generic.KeyValuePair`2{System.String,System.String}.)
						if (parameterTypeString[substringStart] == ' ') {
							++substringStart;
						}
						b.Append(',');
						break;
				}
			}

			AppendParameterTypeSubstring(b, parameterTypeString, substringStart, parameterTypeString.Length, genericTypeParameters, genericMethodParameters);

			// Append ref / out indicator if needed.
			if ((parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindRef) ||
				(parameter.ParameterKind == EnvDTE80.vsCMParameterKind.vsCMParameterKindOut)) {
				b.Append('@');
			}

			// Note there is no need to append a '*' for pointers, as this is included in the full name of the type.
			// Multi-dimensional and jagged arrays are handled above during string parsing.
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
					b.Append(substring);
				}
			}
		}
		#endregion
	}
}