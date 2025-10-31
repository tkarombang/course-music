using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace Admin.Services
{
    public class AuthenticatedHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private const string TokenKey = "authToken";

        public AuthenticatedHttpClient(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        public async Task<HttpClient> GetClientAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(TokenKey);
            _httpClient.DefaultRequestHeaders.Authorization = 
                !string.IsNullOrEmpty(token) 
                    ? new AuthenticationHeaderValue("Bearer", token)
                    : null;
            return _httpClient;
        }
    }
}