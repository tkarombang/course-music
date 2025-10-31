using Frontend.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using Frontend.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Frontend.Components.Pages.Account;

namespace Frontend.Services
{
  public class ClientApiService
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";

    public ClientApiService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
    {
      _httpClient = httpClient;
      _configuration = configuration;
      _localStorage = localStorage;

      var baseAddress = _configuration["ApiUrls:ClientApi"];
      if (string.IsNullOrEmpty(baseAddress))
      {
        throw new InvalidOperationException("ClientApi base address not configured in appsettings.json");
      }
      _httpClient.BaseAddress = new Uri(baseAddress);
      Console.WriteLine($"Debug: ClientApiService HttpClient BaseAddress set to {baseAddress}");
    }


    public async Task InitializeAsync()
    {
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      if (!string.IsNullOrEmpty(token))
      {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        Console.WriteLine($"Debug: Token {token.Substring(0, 10)} set in ClientApiService HttpClient");
      }
    }

    public async Task<int?> GetCurrentUserId()
    {
      await InitializeAsync();
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      if (string.IsNullOrEmpty(token)) return null;

      var handler = new JwtSecurityTokenHandler();
      var jwtToken = handler.ReadJwtToken(token);

      var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
        c.Type == "userId" ||
        c.Type == "sub" ||
        c.Type == ClaimTypes.NameIdentifier
        );

      if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int id))
      {
        Console.WriteLine($"USER ID: {id}");
        return id;
      }

      return null;
    }

    public async Task<T?> GetAsync<T>(string requestUri)
    {
      await InitializeAsync();
      var response = await _httpClient.GetAsync(requestUri);
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<T>();
      }
      return default;
    }

    public async Task<HttpResponseMessage> GetRawAsync(string requestUri)
    {
      await InitializeAsync();
      string fullUrl = _httpClient.BaseAddress.AbsoluteUri + requestUri; // Cetak URL Penuh
      Console.WriteLine($"DEBUG FULL API CALL: {fullUrl}");
      return await _httpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> PostRawAsync<T>(string requestUri, T content)
    {
      await InitializeAsync();
      return await _httpClient.PostAsJsonAsync(requestUri, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string requestUri)
    {
      await InitializeAsync();
      return await _httpClient.DeleteAsync(requestUri);
    }



    public async Task<UserDto> GetCurrentUserAsync()
    {
      await InitializeAsync();
      var userId = await GetCurrentUserId();
      if (userId == null) return null!;

      var response = await _httpClient.GetAsync($"api/User/{userId}");
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<UserDto>();
      }
      return null!;
    }



    public async Task<List<CheckoutDto>?> GetCheckoutByUserAsync(int userId)
    {
      await InitializeAsync();
      var response = await _httpClient.GetAsync($"api/Checkout/{userId}");
      if (response.IsSuccessStatusCode)
      {
        return await response.Content.ReadFromJsonAsync<List<CheckoutDto>>();
      }
      return null;
    }

    public async Task<HttpResponseMessage> PostCheckoutAsync(object checkoutDto)
    {
      await InitializeAsync();
      return await _httpClient.PostAsJsonAsync("api/Checkout", checkoutDto);
    }

    public async Task<HttpResponseMessage> PostPaymentAsync(object paymentDto)
    {
      await InitializeAsync();
      return await _httpClient.PostAsJsonAsync("api/Payment", paymentDto);
    }


  }
}