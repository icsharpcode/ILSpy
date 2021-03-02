using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ICSharpCode.Decompiler.Console
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class FileExistsOrNullAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			var s = value as string;
			if (string.IsNullOrEmpty(s))
				return ValidationResult.Success;
			if (File.Exists(s))
				return ValidationResult.Success;
			return new ValidationResult($"File '{s}' does not exist!");
		}
	}
}
