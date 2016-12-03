using System;
using System.Net;

namespace ICSharpCode.Updater
{
	public class Downloader
	{
		public bool DownloadUpdate(string url, string path)
		{
			/*
				ILSpy Must Suply the URL to update itself. It Must be a constant one that never changes except for the content.
				Why? To Ensure all versions of ILSpy after they include this to be able to get the latest one.
				Who knows it might be possible to configure ILSpy to check for Nightly builds when on the stable one
				and then have this download the Nightly build for them.
			*/
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

		private bool StartDownload(string url, string path)
		{
			try
			{
				string downloadconst = "0";
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
