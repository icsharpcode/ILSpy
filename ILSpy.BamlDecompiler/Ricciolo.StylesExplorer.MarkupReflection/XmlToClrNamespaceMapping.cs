// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
	/// <summary>
	/// Represents a mapping between an XML namespace and a CLR namespace and assembly.
	/// </summary>
	public class XmlToClrNamespaceMapping
	{
		public const string XamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";
		public const string PresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public const string PresentationOptionsNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation/options";
		public const string McNamespace = "http://schemas.openxmlformats.org/markup-compatibility/2006";

		public XmlToClrNamespaceMapping(string xmlNamespace, short assemblyId, string assemblyName, string clrNamespace)
		{
			XmlNamespace = xmlNamespace;
			AssemblyId = assemblyId;
			AssemblyName = assemblyName;
			ClrNamespace = clrNamespace;
		}

		public short AssemblyId { get; }
		public string XmlNamespace { get; set; }
		public string AssemblyName { get; }
		public string ClrNamespace { get; }
	}
}