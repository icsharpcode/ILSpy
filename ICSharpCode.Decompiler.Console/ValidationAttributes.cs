using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ICSharpCode.Decompiler.Console
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ProjectOptionRequiresOutputDirectoryValidationAttribute : ValidationAttribute
	{
		public ProjectOptionRequiresOutputDirectoryValidationAttribute()
		{
		}

		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			if (value is ILSpyCmdProgram obj)
			{
				if (obj.CreateCompilableProjectFlag && string.IsNullOrEmpty(obj.OutputDirectory))
				{
					return new ValidationResult("--project cannot be used unless --outputdir is also specified");
				}
			}
			return ValidationResult.Success;
		}
	}

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
