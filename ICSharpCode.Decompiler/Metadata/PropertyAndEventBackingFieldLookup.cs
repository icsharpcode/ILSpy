// Copyright (c) 2025 Siegfried Pammer
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

using System.Collections.Generic;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.Metadata
{
	class PropertyAndEventBackingFieldLookup
	{
		private readonly MetadataReader metadata;
		private readonly Dictionary<FieldDefinitionHandle, PropertyDefinitionHandle> propertyLookup
			= new();
		private readonly Dictionary<FieldDefinitionHandle, EventDefinitionHandle> eventLookup
			= new();

		public PropertyAndEventBackingFieldLookup(MetadataReader metadata)
		{
			this.metadata = metadata;

			var nameToFieldMap = new MultiDictionary<string, FieldDefinitionHandle>();

			HashSet<string> eventNames = new();

			foreach (var tdh in metadata.TypeDefinitions)
			{
				var type = metadata.GetTypeDefinition(tdh);

				foreach (var fdh in type.GetFields())
				{
					var field = metadata.GetFieldDefinition(fdh);
					var name = metadata.GetString(field.Name);
					nameToFieldMap.Add(name, fdh);
				}

				foreach (var pdh in type.GetProperties())
				{
					var property = metadata.GetPropertyDefinition(pdh);
					var name = metadata.GetString(property.Name);
					// default C# property backing field name is "<PropertyName>k__BackingField"
					if (nameToFieldMap.TryGetValues($"<{name}>k__BackingField", out var fieldHandles))
					{
						foreach (var fieldHandle in fieldHandles)
						{
							propertyLookup[fieldHandle] = pdh;
						}
					}
					else if (nameToFieldMap.TryGetValues($"_{name}", out fieldHandles))
					{
						foreach (var fieldHandle in fieldHandles)
						{
							if (fieldHandle.IsCompilerGenerated(metadata))
							{
								propertyLookup[fieldHandle] = pdh;
							}
						}
					}
				}

				// first get all names of events defined, so that we can make sure we don't accidentally
				// associate the wrong backing field with the event, in case there is an event called "Something"
				// without a backing field (i.e., custom event) as well as an auto/field event called "SomethingEvent"
				// declared in the same type.
				foreach (var edh in type.GetEvents())
				{
					var ev = metadata.GetEventDefinition(edh);
					eventNames.Add(metadata.GetString(ev.Name));
				}

				foreach (var edh in type.GetEvents())
				{
					var ev = metadata.GetEventDefinition(edh);
					var name = metadata.GetString(ev.Name);
					if (nameToFieldMap.TryGetValues(name, out var fieldHandles))
					{
						foreach (var fieldHandle in fieldHandles)
						{
							eventLookup[fieldHandle] = edh;
						}
					}
					else
					{
						var nameWithSuffix = $"{name}Event";
						if (!eventNames.Contains(nameWithSuffix) && nameToFieldMap.TryGetValues(nameWithSuffix, out fieldHandles))
						{
							foreach (var fieldHandle in fieldHandles)
							{
								eventLookup[fieldHandle] = edh;
							}
						}
					}
				}

				eventNames.Clear();
				nameToFieldMap.Clear();
			}
		}

		public bool IsPropertyBackingField(FieldDefinitionHandle field, out PropertyDefinitionHandle handle)
		{
			return propertyLookup.TryGetValue(field, out handle);
		}

		public bool IsEventBackingField(FieldDefinitionHandle field, out EventDefinitionHandle handle)
		{
			return eventLookup.TryGetValue(field, out handle);
		}
	}
}
