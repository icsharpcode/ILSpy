using System;
using System.ComponentModel.DataAnnotations;

namespace ICSharpCode.Decompiler.Console
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ProjectOptionRequiresOutputDirectoryValidationAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			if (value is ILSpyCmdProgram obj) {
				if (obj.CreateCompilableProjectFlag && String.IsNullOrEmpty(obj.OutputDirectory)) {
					return new ValidationResult("--project cannot be used unless --outputdir is also specified");
				}
			}
			return ValidationResult.Success;
		}
	}
}
