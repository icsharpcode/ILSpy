// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Trim/AOT annotation attributes for netstandard2.0, where the framework does not define them.
// ILLink and ILC look these attributes up by namespace and name, not by assembly, so an internal
// copy is honored exactly like the framework's own (.NET 5 and later).

#nullable enable

namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Suppresses reporting of a specific rule violation, allowing multiple suppressions on a
	/// single code artifact. Unlike <see cref="SuppressMessageAttribute"/> it is preserved in the
	/// compiled assembly, which is what ILLink and ILC read.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
	internal sealed class UnconditionalSuppressMessageAttribute : Attribute
	{
		public UnconditionalSuppressMessageAttribute(string category, string checkId)
		{
			Category = category;
			CheckId = checkId;
		}

		public string Category { get; }

		public string CheckId { get; }

		public string? Scope { get; set; }

		public string? Target { get; set; }

		public string? MessageId { get; set; }

		public string? Justification { get; set; }
	}
}
