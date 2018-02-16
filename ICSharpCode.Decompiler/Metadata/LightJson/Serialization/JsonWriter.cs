// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

namespace LightJson.Serialization
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using ErrorType = JsonSerializationException.ErrorType;

	/// <summary>
	/// Represents a writer that can write string representations of JsonValues.
	/// </summary>
	internal sealed class JsonWriter : IDisposable
	{
		private int indent;
		private bool isNewLine;
		private TextWriter writer;

		/// <summary>
		/// A set of containing all the collection objects (JsonObject/JsonArray) being rendered.
		/// It is used to prevent circular references; since collections that contain themselves
		/// will never finish rendering.
		/// </summary>
		private HashSet<IEnumerable<JsonValue>> renderingCollections;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		public JsonWriter()
			: this(false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="pretty">
		/// A value indicating whether the output of the writer should be human-readable.
		/// </param>
		public JsonWriter(bool pretty)
		{
			if (pretty) {
				this.IndentString = "\t";
				this.SpacingString = " ";
				this.NewLineString = "\n";
			}
		}

		/// <summary>
		/// Gets or sets the string representing a indent in the output.
		/// </summary>
		/// <value>
		/// The string representing a indent in the output.
		/// </value>
		public string IndentString { get; set; }

		/// <summary>
		/// Gets or sets the string representing a space in the output.
		/// </summary>
		/// <value>
		/// The string representing a space in the output.
		/// </value>
		public string SpacingString { get; set; }

		/// <summary>
		/// Gets or sets the string representing a new line on the output.
		/// </summary>
		/// <value>
		/// The string representing a new line on the output.
		/// </value>
		public string NewLineString { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether JsonObject properties should be written in a deterministic order.
		/// </summary>
		/// <value>
		/// A value indicating whether JsonObject properties should be written in a deterministic order.
		/// </value>
		public bool SortObjects { get; set; }

		/// <summary>
		/// Returns a string representation of the given JsonValue.
		/// </summary>
		/// <param name="jsonValue">The JsonValue to serialize.</param>
		/// <returns>The serialized value.</returns>
		public string Serialize(JsonValue jsonValue)
		{
			this.Initialize();

			this.Render(jsonValue);

			return this.writer.ToString();
		}

		/// <summary>
		/// Releases all the resources used by this object.
		/// </summary>
		public void Dispose()
		{
			if (this.writer != null) {
				this.writer.Dispose();
			}
		}

		private static bool IsValidNumber(double number)
		{
			return !(double.IsNaN(number) || double.IsInfinity(number));
		}

		private void Initialize()
		{
			this.indent = 0;
			this.isNewLine = true;
			this.writer = new StringWriter();
			this.renderingCollections = new HashSet<IEnumerable<JsonValue>>();
		}

		private void Write(string text)
		{
			if (this.isNewLine) {
				this.isNewLine = false;
				this.WriteIndentation();
			}

			this.writer.Write(text);
		}

		private void WriteEncodedJsonValue(JsonValue value)
		{
			switch (value.Type) {
				case JsonValueType.Null:
					this.Write("null");
					break;

				case JsonValueType.Boolean:
					this.Write(value.AsString);
					break;

				case JsonValueType.Number:
					this.Write(((double)value).ToString(CultureInfo.InvariantCulture));
					break;

				default:
					Debug.Assert(value.Type == JsonValueType.String, "value.Type == JsonValueType.String");
					this.WriteEncodedString((string)value);
					break;
			}
		}

		private void WriteEncodedString(string text)
		{
			this.Write("\"");

			for (int i = 0; i < text.Length; i += 1) {
				var currentChar = text[i];

				// Encoding special characters.
				switch (currentChar) {
					case '\\':
						this.writer.Write("\\\\");
						break;

					case '\"':
						this.writer.Write("\\\"");
						break;

					case '/':
						this.writer.Write("\\/");
						break;

					case '\b':
						this.writer.Write("\\b");
						break;

					case '\f':
						this.writer.Write("\\f");
						break;

					case '\n':
						this.writer.Write("\\n");
						break;

					case '\r':
						this.writer.Write("\\r");
						break;

					case '\t':
						this.writer.Write("\\t");
						break;

					default:
						this.writer.Write(currentChar);
						break;
				}
			}

			this.writer.Write("\"");
		}

		private void WriteIndentation()
		{
			for (var i = 0; i < this.indent; i += 1) {
				this.Write(this.IndentString);
			}
		}

		private void WriteSpacing()
		{
			this.Write(this.SpacingString);
		}

		private void WriteLine()
		{
			this.Write(this.NewLineString);
			this.isNewLine = true;
		}

		private void WriteLine(string line)
		{
			this.Write(line);
			this.WriteLine();
		}

		private void AddRenderingCollection(IEnumerable<JsonValue> value)
		{
			if (!this.renderingCollections.Add(value)) {
				throw new JsonSerializationException(ErrorType.CircularReference);
			}
		}

		private void RemoveRenderingCollection(IEnumerable<JsonValue> value)
		{
			this.renderingCollections.Remove(value);
		}

		private void Render(JsonValue value)
		{
			switch (value.Type) {
				case JsonValueType.Null:
				case JsonValueType.Boolean:
				case JsonValueType.Number:
				case JsonValueType.String:
					this.WriteEncodedJsonValue(value);
					break;

				case JsonValueType.Object:
					this.Render((JsonObject)value);
					break;

				case JsonValueType.Array:
					this.Render((JsonArray)value);
					break;

				default:
					throw new JsonSerializationException(ErrorType.InvalidValueType);
			}
		}

		private void Render(JsonArray value)
		{
			this.AddRenderingCollection(value);

			this.WriteLine("[");

			this.indent += 1;

			using (var enumerator = value.GetEnumerator()) {
				var hasNext = enumerator.MoveNext();

				while (hasNext) {
					this.Render(enumerator.Current);

					hasNext = enumerator.MoveNext();

					if (hasNext) {
						this.WriteLine(",");
					} else {
						this.WriteLine();
					}
				}
			}

			this.indent -= 1;

			this.Write("]");

			this.RemoveRenderingCollection(value);
		}

		private void Render(JsonObject value)
		{
			this.AddRenderingCollection(value);

			this.WriteLine("{");

			this.indent += 1;

			using (var enumerator = this.GetJsonObjectEnumerator(value)) {
				var hasNext = enumerator.MoveNext();

				while (hasNext) {
					this.WriteEncodedString(enumerator.Current.Key);
					this.Write(":");
					this.WriteSpacing();
					this.Render(enumerator.Current.Value);

					hasNext = enumerator.MoveNext();

					if (hasNext) {
						this.WriteLine(",");
					} else {
						this.WriteLine();
					}
				}
			}

			this.indent -= 1;

			this.Write("}");

			this.RemoveRenderingCollection(value);
		}

		/// <summary>
		/// Gets an JsonObject enumerator based on the configuration of this JsonWriter.
		/// If JsonWriter.SortObjects is set to true, then a ordered enumerator is returned.
		/// Otherwise, a faster non-deterministic enumerator is returned.
		/// </summary>
		/// <param name="jsonObject">The JsonObject for which to get an enumerator.</param>
		/// <returns>An enumerator for the properties in a <see cref="JsonObject"/>.</returns>
		private IEnumerator<KeyValuePair<string, JsonValue>> GetJsonObjectEnumerator(JsonObject jsonObject)
		{
			if (this.SortObjects) {
				var sortedDictionary = new SortedDictionary<string, JsonValue>(StringComparer.Ordinal);

				foreach (var item in jsonObject) {
					sortedDictionary.Add(item.Key, item.Value);
				}

				return sortedDictionary.GetEnumerator();
			} else {
				return jsonObject.GetEnumerator();
			}
		}
	}
}
