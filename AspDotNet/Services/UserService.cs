using System;
using AspDotNet.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace AspDotNet.Services
{
	public class UserService
	{
		private readonly HttpClient _httpClient;
		private readonly IMemoryCache _cache;

		public UserService(HttpClient httpClient, IMemoryCache cache)
		{
			_httpClient = httpClient;
			_cache = cache;
		}

        public async Task<List<NestedUserModel>> GetUsersAsync()
        {
			const string cacheKey = "userList";

			if(_cache.TryGetValue(cacheKey, out List<NestedUserModel> cachedUsers))
			{
				Console.WriteLine("Returning Users From Cache");
				return cachedUsers;
			}

			try
			{
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

				if (response!=null && response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var users = JsonConvert.DeserializeObject<List<NestedUserModel>>(json);

					// set users into cache

					_cache.Set(cacheKey, users, TimeSpan.FromMinutes(1));

					Console.WriteLine("Returning Users From API");
					return users;
				}
            }
            catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

            return new List<NestedUserModel>();
        }
    }

}

