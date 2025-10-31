using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace Frontend.Services
{
  public class CustomAuthenticationStateProvider : AuthenticationStateProvider
  {
    private readonly LocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationState _anonymous;
    private AuthenticationState? _cachedstate;

    public CustomAuthenticationStateProvider(LocalStorageService localStorage, HttpClient httpClient)
    {
      _localStorage = localStorage;
      _httpClient = httpClient;
      _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())); 
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
      string token = null;
      try
      {
        token = await _localStorage.GetItemAsync("authToken");
      }
      catch (InvalidOperationException)
      {
        return _anonymous;
      }
      catch (Exception)
      {
        return _anonymous;
      }


      if (string.IsNullOrEmpty(token)) return _anonymous;
      
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var claims = ParseClaimsFromJwt(token);
      var identity = new ClaimsIdentity(claims, "JwtAuth");

      return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string token)
    {
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var claims = ParseClaimsFromJwt(token);
      var identity = new ClaimsIdentity(claims, "JwtAuth");
      var user = new ClaimsPrincipal(identity);

      _cachedstate = new AuthenticationState(user);
      NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
      _httpClient.DefaultRequestHeaders.Authorization = null;
      NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
      var claims = new List<Claim>();

      var payload = jwt.Split('.')[1];
      var jsonBytes = ParseBase64WithoutPadding(payload);

      using (var jsonDoc = JsonDocument.Parse(jsonBytes))
      {
        var root = jsonDoc.RootElement;
        foreach (var property in root.EnumerateObject())
        {
          if (property.Name == ClaimTypes.Role || property.Name == "role")
          {
            claims.Add(new Claim(ClaimTypes.Role, property.Value.ToString()!));
          }
          else if (property.Name == ClaimTypes.NameIdentifier || property.Name == "nameid")
          {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, property.Value.ToString()!));
          }
          else if (property.Name == ClaimTypes.Email || property.Name == "email")
          {
            claims.Add(new Claim(ClaimTypes.Email, property.Value.ToString()!));
          }
          else if (property.Name == ClaimTypes.Name || property.Name == "unique_name")
          {
            claims.Add(new Claim(ClaimTypes.Name, property.Value.ToString()!));
          }
        }
        return claims;
      }
    }

    private byte[] ParseBase64WithoutPadding(string base64)
    {
      switch (base64.Length % 4)
      {
        case 2: base64 += "=="; break;
        case 3: base64 += "="; break;
      }

      return Convert.FromBase64String(base64);
    }
  }
}