// ILSpyBugs.Program
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ILSpyBugs
{
	internal class Program
	{
		private static void Main(string[] args)
		{
		}

		private static HttpResponseMessage Aaa(Image image, string fileName)
		{
			MemoryStream memoryStream = new MemoryStream();
			image.Save(memoryStream, ImageFormat.Jpeg);
			memoryStream.Close();
			MemoryStream memoryStream2 = new MemoryStream(memoryStream.ToArray());
			return new HttpResponseMessage {
				Content = new StreamContent(memoryStream2) {
					Headers =  {
						ContentDisposition = new ContentDispositionHeaderValue("aaa") {
							FileName = fileName
						},
						ContentType = new MediaTypeHeaderValue("bbb"),
						ContentLength = new long?(memoryStream2.Length)
					}
				}
			};
		}
	}
}