using System;
using Microsoft.Extensions.Caching.Memory;

namespace AspDotNet.Services
{
	public class ImageService
	{
		private readonly HttpClient _client;
		private readonly IMemoryCache _cache;
		private readonly string _imageCacheFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cachedImages");

		public ImageService(HttpClient client, IMemoryCache cache)
		{
			_client = client;
			_cache = cache;

			if (!Directory.Exists(_imageCacheFolder))
			{
				Directory.CreateDirectory(_imageCacheFolder);
			}
		}

		// cache image url itself as string
		public string GetCachedImageUrl(string imageUrl)
		{
			return _cache.GetOrCreate(imageUrl, entry => {

				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
				return imageUrl;
			});
		}


		// Download image bytes and cache to local

		public async Task<string> GetCachedImagePathAsync(string imageUrl)
		{
			string imageFileName = GetFileNameFromUrl(imageUrl);
            string localPath = Path.Combine(_imageCacheFolder, imageFileName);
            string cacheKey = $"image-bytes-{imageUrl}";

			if(_cache.TryGetValue(cacheKey, out string cachedPath))
			{
				return cachedPath;
			}

			if (!File.Exists(localPath))
			{
				var bytes = await _client.GetByteArrayAsync(imageUrl);
				await File.WriteAllBytesAsync(localPath, bytes);
			}

			_cache.Set(cacheKey, localPath, TimeSpan.FromMinutes(30));
			return localPath;
        }


        private string GetFileNameFromUrl(string url)
        {
            // Generate a consistent file name from the URL (hash or slug)
            var hash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(url));
            return $"{hash.GetHashCode()}.jpg";
        }
    }
}

