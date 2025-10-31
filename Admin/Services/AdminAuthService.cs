using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Admin.Models;
using Blazored.LocalStorage;

namespace Admin.Services
{
  public interface IAdminAuthService
  {
    Task<string> LoginAsync(LoginModel model);
    Task LogoutAsync();
    Task<bool> IsAuthenticatedAsync();
  }

  public class AdminAuthService : IAdminAuthService
  {
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _configuration;
    private const string TokenKey = "authToken";

    public AdminAuthService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _localStorage = localStorage;
      _configuration = configuration;

      // Pastikan BaseAddress diset
      var baseAddress = _configuration["ApiUrls:AdminApi"];
      if (string.IsNullOrEmpty(baseAddress))
      {
        throw new InvalidOperationException("AdminApi base address not configured in appsettings.json");
      }
      _httpClient.BaseAddress = new Uri(baseAddress);
      Console.WriteLine($"Debug: HttpClient BaseAddress set to {baseAddress}");
    }

    public async Task<string> LoginAsync(LoginModel model)
    {

      var response = await _httpClient.PostAsJsonAsync("api/auth/login", model);

      if (!response.IsSuccessStatusCode)
      {
        throw new UnauthorizedAccessException("Invalid email or password");
      }

      var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
      if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
      {
        throw new UnauthorizedAccessException("Failed to get token");
      }

      var token = loginResponse.Token;
      var handler = new JwtSecurityTokenHandler();
      var jwtToken = handler.ReadJwtToken(token);

      var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
      if (roleClaim == null || roleClaim.Value != "Admin")
      {
        await _localStorage.RemoveItemAsync(TokenKey); // Hapus token
        throw new UnauthorizedAccessException("Admin access required");
      }

      await _localStorage.SetItemAsync(TokenKey, loginResponse.Token);
      _httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

      return token;
    }



    public async Task LogoutAsync()
    {
      await _localStorage.RemoveItemAsync(TokenKey);
      _httpClient.DefaultRequestHeaders.Authorization = null;
      Console.WriteLine("Debug: Logged out, token removed");
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      return !string.IsNullOrEmpty(token);
    }
  }
}