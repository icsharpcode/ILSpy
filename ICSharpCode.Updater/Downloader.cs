using System;
using System.Net;

namespace ICSharpCode.Updater
{
	public class Downloader
	{
		public bool DownloadUpdate(String url, String path)
		{
			bool result = StartDownload(url, path + "\\ILSpy_Update.zip");
			if (result == true)
			{
				return true;
			}
			else
			{
				return false;  //Downloading update has failed.
			}
		}

		private bool StartDownload(String url, String path)
		{
			try
			{
				String downloadconst = "0";
				WebClient webClient = new WebClient();
				webClient.Headers["User-Agent"] = "Mozilla/5.0";
				webClient.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs args)
				{
					downloadconst = args.ProgressPercentage.ToString();
				};
				webClient.DownloadFileAsync(new Uri(url), path);
				if (downloadconst == "100")
				{
					webClient.Dispose();
				}
				return true;
			}
			#pragma warning disable CS0168
			catch (Exception ex)
			{
				return false;
			}
			#pragma warning restore CS0168
		}
	}
}
