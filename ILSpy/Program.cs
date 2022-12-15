// Copyright (c) 2022 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

using ICSharpCode.ILSpy.Options;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ICSharpCode.ILSpy
{
	public class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			var cmdArgs = Environment.GetCommandLineArgs().Skip(1);
			App.CommandLineArguments = new CommandLineArguments(cmdArgs);

			var settings = ILSpySettings.Load();
			var miscSettings = PersistedMiscSettings.FromILSpySettings(settings);

			bool forceSingleInstance = (App.CommandLineArguments.SingleInstance ?? true) && !miscSettings.AllowMultipleInstances;
			if (forceSingleInstance)
			{
				SingleInstanceHandling.ForceSingleInstance(cmdArgs);
			}

			// https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host
			var host = Host.CreateDefaultBuilder()
				.ConfigureServices(services => {
					services.AddSingleton<App>();
				})
				.Build();

			var app = host.Services.GetService<App>();
			app.Run();
		}
	}
}
