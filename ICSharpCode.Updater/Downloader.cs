using System;
using System.Net;

namespace ICSharpCode.Updater
{
	public class Downloader
	{
		/*
			Should Probably have the Second Arg being a String of Path be Application.StartupPath.
			Note: I Recommend Adding a Option to ILSpy for the Extractor to Re-execute ILSpy right before it Closes.
			And for ILSpy to Close when and if the file is done Updating.
			Luckily I have it return a bool on the updater. So that means if it returns true then close ILSpy and execute the extractor.
		*/
		public bool DownloadUpdate(String url, String path)
		{
			bool result = StartDownload(url, path + "\\ILSpy_Update.zip");
			if (result == true)
			{
				return true;
			}
			else
			{
				return false;  //Downloading update failed?
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
