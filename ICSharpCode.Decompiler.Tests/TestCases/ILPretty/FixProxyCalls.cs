using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class FixProxyCalls
	{
		public class TestHandler : DelegatingHandler
		{
			protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken c)
			{
				return await base.SendAsync(r, c);
			}
		}
	}
}
