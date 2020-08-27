// Copyright (c) 2018 Daniel Grunwald
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

using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	static class DecimalConstantHelper
	{
		public static bool AllowsDecimalConstants(MetadataModule module)
		{
			return ((module.TypeSystemOptions & TypeSystemOptions.DecimalConstants) == TypeSystemOptions.DecimalConstants);
		}

		public static bool IsDecimalConstant(MetadataModule module, CustomAttributeHandleCollection attributeHandles)
		{
			return attributeHandles.HasKnownAttribute(module.metadata, KnownAttribute.DecimalConstant);
		}

		public static object GetDecimalConstantValue(MetadataModule module, CustomAttributeHandleCollection attributeHandles)
		{
			var metadata = module.metadata;
			foreach (var attributeHandle in attributeHandles)
			{
				var attribute = metadata.GetCustomAttribute(attributeHandle);
				if (attribute.IsKnownAttribute(metadata, KnownAttribute.DecimalConstant))
					return TryDecodeDecimalConstantAttribute(module, attribute);
			}
			return null;
		}

		static decimal? TryDecodeDecimalConstantAttribute(MetadataModule module, System.Reflection.Metadata.CustomAttribute attribute)
		{
			var attrValue = attribute.DecodeValue(module.TypeProvider);
			if (attrValue.FixedArguments.Length != 5)
				return null;
			// DecimalConstantAttribute has the arguments (byte scale, byte sign, uint hi, uint mid, uint low) or (byte scale, byte sign, int hi, int mid, int low)
			// Both of these invoke the Decimal constructor (int lo, int mid, int hi, bool isNegative, byte scale) with explicit argument conversions if required.
			if (!(attrValue.FixedArguments[0].Value is byte scale && attrValue.FixedArguments[1].Value is byte sign))
				return null;
			unchecked
			{
				if (attrValue.FixedArguments[2].Value is uint hi
					&& attrValue.FixedArguments[3].Value is uint mid
					&& attrValue.FixedArguments[4].Value is uint lo)
				{
					return new decimal((int)lo, (int)mid, (int)hi, sign != 0, scale);
				}
			}
			{
				if (attrValue.FixedArguments[2].Value is int hi
					&& attrValue.FixedArguments[3].Value is int mid
					&& attrValue.FixedArguments[4].Value is int lo)
				{
					return new decimal(lo, mid, hi, sign != 0, scale);
				}
			}
			return null;
		}
	}
}
