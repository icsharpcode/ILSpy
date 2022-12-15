using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ICSharpCode.ILSpy
{
	public class Program
	{
		[STAThread]
		public static void Main()
		{
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
