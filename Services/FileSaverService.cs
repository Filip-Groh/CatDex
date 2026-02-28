using CatDex.Services.Interfaces;

namespace CatDex.Services;

public class FileSaverService : IFileSaverService
{
	private readonly HttpClient _httpClient;

	public FileSaverService()
	{
		_httpClient = new HttpClient();
	}

	public async Task<bool> SaveImageAsync(string? url, byte[]? imageBytes, string fileName)
	{
		try
		{
			byte[] data;

			if (imageBytes != null && imageBytes.Length > 0)
			{
				data = imageBytes;
			}
			else if (!string.IsNullOrEmpty(url))
			{
				data = await _httpClient.GetByteArrayAsync(url);
			}
			else
			{
				return false;
			}

			var downloadFolder = Path.Combine(FileSystem.Current.CacheDirectory, "Downloads");
			Directory.CreateDirectory(downloadFolder);

			var filePath = Path.Combine(downloadFolder, fileName);
			await File.WriteAllBytesAsync(filePath, data);

#if ANDROID
			await SaveToGalleryAndroid(data, fileName);
#elif IOS || MACCATALYST
			await SaveToGalleryIOS(data, fileName);
#elif WINDOWS
			await SaveToGalleryWindows(data, fileName);
#endif

			return true;
		}
		catch
		{
			return false;
		}
	}

#if ANDROID
	private async Task SaveToGalleryAndroid(byte[] data, string fileName)
	{
		var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
			Android.OS.Environment.DirectoryPictures);
		var catDexFolder = Path.Combine(picturesPath!.AbsolutePath, "CatDex");
		Directory.CreateDirectory(catDexFolder);

		var filePath = Path.Combine(catDexFolder, fileName);
		await File.WriteAllBytesAsync(filePath, data);

		var mediaScanIntent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
		mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(filePath)));
		Android.App.Application.Context.SendBroadcast(mediaScanIntent);
	}
#endif

#if IOS || MACCATALYST
	private Task SaveToGalleryIOS(byte[] data, string fileName)
	{
		var tempFilePath = Path.Combine(FileSystem.Current.CacheDirectory, fileName);
		File.WriteAllBytes(tempFilePath, data);

		var tcs = new TaskCompletionSource<bool>();

		Photos.PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
		{
			Photos.PHAssetChangeRequest.FromImage(Foundation.NSUrl.FromFilename(tempFilePath));
		}, (success, error) =>
		{
			if (success)
				tcs.SetResult(true);
			else
				tcs.SetResult(false);
		});

		return tcs.Task;
	}
#endif

#if WINDOWS
	private async Task SaveToGalleryWindows(byte[] data, string fileName)
	{
		var picturesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
		var catDexFolder = Path.Combine(picturesPath, "CatDex");
		Directory.CreateDirectory(catDexFolder);

		var filePath = Path.Combine(catDexFolder, fileName);
		await File.WriteAllBytesAsync(filePath, data);
	}
#endif
}
