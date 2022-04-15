using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

using McMaster.Extensions.CommandLineUtils.Abstractions;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace ICSharpCode.ILSpyCmd
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
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var path = value as string;
			if (string.IsNullOrEmpty(path))
			{
				return ValidationResult.Success;
			}

			if (!Path.IsPathRooted(path) && validationContext.GetService(typeof(CommandLineContext)) is CommandLineContext context)
			{
				path = Path.Combine(context.WorkingDirectory, path);
			}

			if (File.Exists(path))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult($"File '{path}' does not exist!");
		}
	}

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class FilesExistAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			switch (value)
			{
				case string path:
					return ValidatePath(path);
				case string[] paths:
				{
					foreach (string path in paths)
					{
						ValidationResult result = ValidatePath(path);
						if (result != ValidationResult.Success)
							return result;
					}
					return ValidationResult.Success;
				}
				default:
					return new ValidationResult($"File '{value}' does not exist!");
			}

			ValidationResult ValidatePath(string path)
			{
				if (!string.IsNullOrWhiteSpace(path))
				{
					if (!Path.IsPathRooted(path) && validationContext.GetService(typeof(CommandLineContext)) is CommandLineContext context)
					{
						path = Path.Combine(context.WorkingDirectory, path);
					}

					if (File.Exists(path))
					{
						return ValidationResult.Success;
					}
				}

				return new ValidationResult($"File '{path}' does not exist!");
			}
		}
	}
}
