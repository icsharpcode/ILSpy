// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using Microsoft.VisualStudio.Composition;

using TomsToolbox.Composition;
using TomsToolbox.Essentials;

namespace ICSharpCode.ILSpy;

#nullable enable

/// <summary>
/// Adapter for Microsoft.VisualStudio.Composition.<see cref="ExportProvider"/> to <see cref="IExportProvider"/>.
/// </summary>
public sealed class ExportProviderAdapter : IExportProvider
{
	private static readonly Type DefaultMetadataType = typeof(Dictionary<string, object>);

	private readonly ExportProvider _exportProvider;

	/// <summary>
	/// Initializes a new instance of the <see cref="ExportProviderAdapter"/> class.
	/// </summary>
	/// <param name="exportProvider">The export provider.</param>
	public ExportProviderAdapter(ExportProvider exportProvider)
	{
		_exportProvider = exportProvider;
	}

	event EventHandler<EventArgs>? IExportProvider.ExportsChanged { add { } remove { } }

	T IExportProvider.GetExportedValue<T>(string? contractName) where T : class
	{
		return _exportProvider.GetExportedValue<T>(contractName) ?? throw new InvalidOperationException($"No export found for type {typeof(T).FullName} with contract '{contractName}'");
	}

	T? IExportProvider.GetExportedValueOrDefault<T>(string? contractName) where T : class
	{
		return _exportProvider.GetExportedValues<T>(contractName).SingleOrDefault();
	}

	bool IExportProvider.TryGetExportedValue<T>(string? contractName, [NotNullWhen(true)] out T? value) where T : class
	{
		value = _exportProvider.GetExportedValues<T>(contractName).SingleOrDefault();

		return !Equals(value, default(T));
	}

	IEnumerable<T> IExportProvider.GetExportedValues<T>(string? contractName) where T : class
	{
		return _exportProvider.GetExportedValues<T>(contractName);
	}

	IEnumerable<object> IExportProvider.GetExportedValues(Type contractType, string? contractName)
	{
		return _exportProvider
			.GetExports(contractType, DefaultMetadataType, contractName)
			.Select(item => item.Value)
			.ExceptNullItems();
	}

	IEnumerable<IExport<object>> IExportProvider.GetExports(Type contractType, string? contractName)
	{
		return _exportProvider
			.GetExports(contractType, DefaultMetadataType, contractName)
			.Select(item => new ExportAdapter<object>(() => item.Value, new MetadataAdapter((IDictionary<string, object?>)item.Metadata)));
	}

	IEnumerable<IExport<T>> IExportProvider.GetExports<T>(string? contractName) where T : class
	{
		return _exportProvider
			.GetExports(typeof(T), DefaultMetadataType, contractName)
			.Select(item => new ExportAdapter<T>(() => (T?)item.Value, new MetadataAdapter((IDictionary<string, object?>)item.Metadata)));
	}

	IEnumerable<IExport<T, TMetadataView>> IExportProvider.GetExports<T, TMetadataView>(string? contractName) where T : class where TMetadataView : class
	{
		return _exportProvider
			.GetExports<T, TMetadataView>(contractName)
			.Select(item => new ExportAdapter<T, TMetadataView>(() => item.Value, item.Metadata));
	}
}