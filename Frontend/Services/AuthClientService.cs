using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Frontend.DTOs;
using Frontend.Interface;
using Frontend.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace Frontend.Services
{
  public class AuthClientService : IAuthClientService
  {
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _configuration;
    private readonly NavigationManager _navigation;
    private const string TokenKey = "authToken";


    public AuthClientService(HttpClient httpClient, ILocalStorageService localStorage, IConfiguration configuration, NavigationManager navigation)
    {
      _httpClient = httpClient;
      _localStorage = localStorage;
      _configuration = configuration;
      _navigation = navigation;

      var baseAddress = _configuration["ApiUrls:ClientApi"];
      if (string.IsNullOrEmpty(baseAddress))
      {
        throw new InvalidOperationException("ClientApi base address not configured in appsettings.json");
      }
      _httpClient.BaseAddress = new Uri(baseAddress);
      Console.WriteLine($"Debug: ClientApiService HttpClient BaseAddress set to {baseAddress}");
    }


    public async Task<bool> Register(RegisterRequestDto dto)
    {
      var result = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);

      return result.IsSuccessStatusCode;
    }

    public async Task<string> Login(LoginRequestDto dto)
    {
      var response = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

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

      var allowedRoles = new[] { "Admin", "User" };
      var hasRequiredRole = jwtToken.Claims.Any(r => (r.Type == ClaimTypes.Role || r.Type == "role") && allowedRoles.Contains(r.Value));

      if (!hasRequiredRole)
      {
        await _localStorage.RemoveItemAsync(TokenKey); // Hapus token
        throw new UnauthorizedAccessException("Admin access required");
      }

      await _localStorage.SetItemAsync(TokenKey, loginResponse.Token);
      _httpClient.DefaultRequestHeaders.Authorization =
          new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponse.Token);

      return token;
    }



    public async Task<bool> ForgotPassword(ForgotPasswordRequestDto dto)
    {
      var result = await _httpClient.PostAsJsonAsync("api/Auth/forgot-password", dto);
      if (!result.IsSuccessStatusCode)
      {
        Console.WriteLine($"[FORGOT PASSWORD GAGAL] Status: {result.StatusCode}");
      }

      return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordRequestDto dto)
    {
      try
      {
        var result = await _httpClient.PostAsJsonAsync("api/Auth/reset-password", dto);
        var errorContent = await result.Content.ReadAsStringAsync();

        if (result.IsSuccessStatusCode)
        {
          return true;
        }

        Console.WriteLine($"[RESET PASSWORD FAIL] Status: {result.StatusCode}");
        Console.WriteLine($"[RESET PASSWORD FAIL] Detail: {errorContent}");

        return false;
      }
      catch (HttpRequestException err)
      {
        Console.WriteLine($"[AUTH SERVICE ERROR] KONEKSI GAGAL, {err.Message}");
        return false;
      }
    }



    public async Task Logout()
    {
      await _localStorage.RemoveItemAsync(TokenKey);
      _httpClient.DefaultRequestHeaders.Authorization = null;
      Console.WriteLine("Debug: Logged out, token removed");
    }
    public async Task<bool> IsAuthenticatedAsync()
    {
      try
      {
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      return !string.IsNullOrEmpty(token);
      }
      catch (InvalidOperationException)
      {
        Console.WriteLine("[DEBU] JSRuntime belum siap (prerender)");        
        return false;
      }
    }
    public async Task<bool> IsTokenValidAsync()
    {
      var jwt = await ParseJwtTokenAsync();
      return jwt != null && jwt.ValidTo > DateTime.UtcNow.AddMinutes(1);
    }


    public async Task CheckAndRedirectAsync()
    {
      var isValid = await IsTokenValidAsync();
      var currentPath = new Uri(_navigation.Uri).AbsolutePath;

      if (isValid)
      {
        if (currentPath == "/login" || currentPath == "/")
        {
          _navigation.NavigateTo("/landingpage");
        }
      }
      else
      {
        await Logout();
        if (currentPath != "/login")
        {
          _navigation.NavigateTo("login");
        }
      }
    }

    public async Task<int?> GetUserIdAsync()
    {
      var jwt = await ParseJwtTokenAsync();
      if (jwt == null) return null;

      var claim = jwt.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);

      return claim != null && int.TryParse(claim.Value, out int id) ? id : null;
    }

    


    public async Task<JwtSecurityToken?> ParseJwtTokenAsync()
    {
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      if (string.IsNullOrEmpty(token)) return null;

      try
      {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token);
      }
      catch
      {
        return null;
      }
    }



  }
}