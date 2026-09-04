// Copyright (c) 2026 Siegfried Pammer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.BamlDecompiler.Rewrite
{
	/// <summary>
	/// Recovers the StartupUri of an application from the code the markup compiler generated for it.
	/// <para>
	/// StartupUri is written in App.xaml, but it does not reach the BAML: the markup compiler turns
	/// the attribute into an assignment inside InitializeComponent. The project decompiler deletes
	/// the generated members, so a document decompiled without this would build into an application
	/// that starts and shows nothing.
	/// </para>
	/// </summary>
	internal class StartupUriRewritePass : IRewritePass
	{
		const string StartupUriPropertyName = "StartupUri";

		public void Run(XamlContext ctx, XDocument document)
		{
			var root = document.Elements().FirstOrDefault()?.Elements().FirstOrDefault();
			if (root == null || root.Attribute(StartupUriPropertyName) != null)
				return;
			// The type of the document, which the x:Class pass has recorded by the time this runs.
			if (ctx.XClassNames.FirstOrDefault() is not string className)
				return;
			var typeDefinition = ctx.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(className).TopLevelTypeName);
			if (typeDefinition == null)
				return;

			string startupUri = FindAssignedStartupUri(typeDefinition);
			if (startupUri != null)
				root.Add(new XAttribute(StartupUriPropertyName, startupUri));
		}

		/// <summary>
		/// The string assigned to a StartupUri property in InitializeComponent, if there is one.
		/// The generated code reads
		/// <c>StartupUri = new Uri("MainWindow.xaml", UriKind.Relative)</c>, so the string wanted is
		/// the last one loaded before the call to the setter.
		/// </summary>
		static string FindAssignedStartupUri(ITypeDefinition typeDefinition)
		{
			var method = typeDefinition.Methods.FirstOrDefault(
				m => m.Name == "InitializeComponent" && m.Parameters.Count == 0);
			if (method?.MetadataToken.IsNil != false)
				return null;
			var module = typeDefinition.ParentModule?.MetadataFile;
			if (module == null)
				return null;

			try
			{
				var metadata = module.Metadata;
				var methodDefinition = metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
				if (methodDefinition.RelativeVirtualAddress == 0)
					return null;
				var body = module.GetMethodBody(methodDefinition.RelativeVirtualAddress);
				var reader = body.GetILReader();
				string lastLoadedString = null;
				while (reader.RemainingBytes > 0)
				{
					var opCode = reader.DecodeOpCode();
					switch (opCode)
					{
						case ILOpCode.Ldstr:
							lastLoadedString = metadata.GetUserString(
								MetadataTokens.UserStringHandle(reader.ReadInt32()));
							break;
						case ILOpCode.Call:
						case ILOpCode.Callvirt:
							var target = MetadataTokens.EntityHandle(reader.ReadInt32());
							if (lastLoadedString != null && IsStartupUriSetter(metadata, target))
								return lastLoadedString;
							break;
						default:
							ILParser.SkipOperand(ref reader, opCode);
							break;
					}
				}
			}
			catch (BadImageFormatException)
			{
				// A method body nobody can read says nothing about the StartupUri.
			}
			return null;
		}

		static bool IsStartupUriSetter(MetadataReader metadata, EntityHandle handle)
		{
			StringHandle name;
			switch (handle.Kind)
			{
				case HandleKind.MethodDefinition:
					name = metadata.GetMethodDefinition((MethodDefinitionHandle)handle).Name;
					break;
				case HandleKind.MemberReference:
					name = metadata.GetMemberReference((MemberReferenceHandle)handle).Name;
					break;
				default:
					return false;
			}
			return metadata.StringComparer.Equals(name, "set_" + StartupUriPropertyName);
		}
	}
}
