using System;
using AspDotNet.Models;
using System.Text;
using Newtonsoft.Json;

namespace AspDotNet.Services
{
	public class UserService
	{
		private readonly HttpClient _httpClient;

		public UserService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

        public async Task<List<NestedUserModel>> GetUsersAsync()
        {
			try
			{
                var response = await _httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");

				if (response!=null && response.IsSuccessStatusCode)
				{
					var json = await response.Content.ReadAsStringAsync();
					var users = JsonConvert.DeserializeObject<List<NestedUserModel>>(json);
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

