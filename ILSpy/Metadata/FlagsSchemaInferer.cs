// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ICSharpCode.ILSpy.Metadata.Filters
{
	/// <summary>
	/// Builds a <see cref="FlagsSchema"/> for a <see cref="FlagsAttribute"/> enum by
	/// partitioning its public-static fields against the <c>*Mask</c> fields the runtime
	/// libraries use to mark mutually-exclusive sub-ranges (<c>VisibilityMask</c>,
	/// <c>LayoutMask</c>, <c>StringFormatMask</c>, <c>MemberAccessMask</c>, …). Cached
	/// per type for the lifetime of the process.
	/// </summary>
	public static class FlagsSchemaInferer
	{
		static readonly ConcurrentDictionary<Type, FlagsSchema> cache = new();

		/// <summary>
		/// Bits that exist in ECMA-335 but have no member in the runtime's enum, so the
		/// field walk cannot surface them. Appended to the inferred independent flags.
		/// </summary>
		static readonly Dictionary<Type, IndependentFlag[]> extraFlags = new() {
			// ECMA-335 II.23.1.15 Forwarder: set on ExportedType rows that forward the
			// type to another assembly.
			[typeof(TypeAttributes)] = [new IndependentFlag("IsTypeForwarder", 0x00200000)],
		};

		public static FlagsSchema For(Type enumType)
		{
			ArgumentNullException.ThrowIfNull(enumType);
			if (!enumType.IsEnum)
				throw new ArgumentException($"Type '{enumType}' is not an enum.", nameof(enumType));
			return cache.GetOrAdd(enumType, Build);
		}

		static FlagsSchema Build(Type enumType)
		{
			var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
			var maskFields = new List<(string Name, uint Mask)>();
			var valueFields = new List<(string Name, uint Value)>();
			// A zero-valued member fits every mask, so value alone cannot attribute it to a
			// group. ECMA-335 and the reflection enums declare each mask followed directly by
			// its members (VisibilityMask, NotPublic, Public, ...), so a zero member is
			// claimed as the zero label of the most recently declared mask. First wins.
			var zeroNames = new Dictionary<uint, string>();
			uint? currentMask = null;
			foreach (var f in fields)
			{
				uint v = ToUInt32(f.GetRawConstantValue());
				if (f.Name.EndsWith("Mask", StringComparison.Ordinal))
				{
					maskFields.Add((f.Name, v));
					currentMask = v;
				}
				else
				{
					valueFields.Add((f.Name, v));
					if (v == 0 && currentMask is uint mask && !zeroNames.ContainsKey(mask))
						zeroNames[mask] = f.Name;
				}
			}

			// Sort masks ascending by mask value so that when a value fits in more than one
			// mask (e.g. value 0 fits all of them), the smallest one wins. This keeps the
			// partition deterministic and prefers the most specific axis.
			maskFields.Sort((a, b) => a.Mask.CompareTo(b.Mask));

			// First pass: bucket every non-zero value into the first mask it fits inside.
			// Zero-value fields were already claimed (or not) as group zero labels above;
			// each mutex group always carries a zero option regardless.
			var groupBuckets = maskFields.ToDictionary(m => m.Mask,
				m => new List<MutexValue>());
			var independentFlags = new List<IndependentFlag>();
			var seenValues = new HashSet<uint>();
			foreach (var (name, value) in valueFields)
			{
				if (value == 0)
					continue; // zero defaults are synthesised below
				if (!seenValues.Add(value))
					continue; // skip aliases (multiple names for the same bits)
				bool placed = false;
				foreach (var (_, mask) in maskFields)
				{
					if ((value & ~mask) != 0)
						continue;
					groupBuckets[mask].Add(new MutexValue($"{name} ({value:X4})", value));
					placed = true;
					break;
				}
				if (!placed)
					independentFlags.Add(new IndependentFlag(name, value));
			}

			if (extraFlags.TryGetValue(enumType, out var extras))
				independentFlags.AddRange(extras);

			// Build the final mutex groups. Drop empty buckets — a *Mask field whose
			// values weren't named in the enum (or were aliased away) doesn't earn a
			// dropdown section. Each group leads with its zero entry: the enum's declared
			// zero member (NotPublic, AutoLayout, ...) when one was claimed for the mask,
			// a synthesised "(none)" otherwise.
			var groups = new List<MutexGroup>();
			foreach (var (maskName, mask) in maskFields)
			{
				var values = groupBuckets[mask];
				if (values.Count == 0)
					continue;
				var groupName = maskName.EndsWith("Mask", StringComparison.Ordinal)
					? maskName[..^"Mask".Length]
					: maskName;
				values.Insert(0, zeroNames.TryGetValue(mask, out var zeroName)
					? new MutexValue($"{zeroName} (0000)", 0)
					: new MutexValue("(none)", 0));
				groups.Add(new MutexGroup(groupName, mask, values));
			}

			return new FlagsSchema(enumType, groups, independentFlags);
		}

		static uint ToUInt32(object? raw)
		{
			if (raw is null)
				return 0;
			return Convert.ToUInt32(Convert.ToInt64(raw, CultureInfo.InvariantCulture)
				& 0xFFFFFFFFL, CultureInfo.InvariantCulture);
		}
	}
}
