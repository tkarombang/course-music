// using Admin.Components.Pages;
using Admin.Models;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
using Admin.DTOs.Category;
using Admin.DTOs.Users;
using Admin.Interfaces;


namespace Admin.Services
{
  public class AdminApiService
  {
    protected readonly HttpClient _httpClient;
    protected readonly IConfiguration _configuration;
    protected readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    
    public AdminApiService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage)
    {
      _httpClient = httpClient;
      _configuration = configuration;
      _localStorage = localStorage;

      var baseAddress = _configuration["ApiUrls:AdminApi"];
      if (string.IsNullOrEmpty(baseAddress))
      {
        throw new InvalidOperationException("AdminApi base address not configured in appsettings.json");
      }
      _httpClient.BaseAddress = new Uri(baseAddress);
      Console.WriteLine($"Debug: AdminApiService HttpClient BaseAddress set to {baseAddress}");
    }


    public async Task InitializeAsync()
    {
      var token = await _localStorage.GetItemAsync<string>(TokenKey);
      if (!string.IsNullOrEmpty(token))
      {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        Console.WriteLine("Debug: Token set in AdminApiService HttpClient");
      }
    }

    public async Task<string> GetAdminDataAsync()
    {
      await InitializeAsync();
      var response = await _httpClient.GetAsync("api/User");
      Console.WriteLine($"Debug: GetAdminDataAsync response status: {response.StatusCode}");
      response.EnsureSuccessStatusCode();
      return await response.Content.ReadAsStringAsync();
    }



    public async Task<List<InvoiceModel>> GetDashboardInvoicesAsync()
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.GetFromJsonAsync<List<InvoiceModel>>("api/Invoice/dashboard");

        if (response != null)
        {
          Console.WriteLine($"SUCCESS: Fetched {response.Count} invoices.");
        }
        return response ?? new List<InvoiceModel>();
      }
      catch (Exception err)
      {
        Console.WriteLine($"FATAL ERROR FETCHING INVOICE DATA: {err.Message}");
        throw;
      }
    }

    public async Task<List<PayMetModel>> GetAllPaymentMethods()
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.GetFromJsonAsync<List<PayMetModel>>("api/PaymentMethod");
        if (response != null) Console.WriteLine($"SUCCESS: Fetched {response.Count} Payment Methods");

        foreach (var getLogo in response)
        {
          var baseUrl = _configuration["ApiUrls:AdminApi"];
          if (!string.IsNullOrEmpty(getLogo.LogoMethod))
          {
            getLogo.LogoMethod = $"{baseUrl}{getLogo.LogoMethod}";
          }
        }

        return response ?? new List<PayMetModel>();
      }
      catch (Exception err)
      {
        Console.WriteLine($"GAGAL ERROR FETCHING: {err.Message}");
        throw;
      }
    }

    public async Task AddNewPaymentMethod(PayMetCreateModel newPayment)
    {await InitializeAsync();
      try
      {
        var content = new MultipartFormDataContent
        {
          { new StringContent(newPayment.NameMethod), "NameMethod" },
          { new StringContent(newPayment.Status ?? "Active"), "Status" }
        };

        if (newPayment.LogoMethod != null)
        {
          var fileContent = new StreamContent(newPayment.LogoMethod.OpenReadStream());
          fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(newPayment.LogoMethod.ContentType);
          content.Add(fileContent, "LogoMethod", newPayment.LogoMethod.Name);
        }

        var response = await _httpClient.PostAsync($"api/PaymentMethod", content);
        response.EnsureSuccessStatusCode();

        Console.WriteLine($"[SUKSES-ADD]: PAYMENT");
      }
      catch (HttpRequestException err)
      {
        Console.WriteLine($"ERROR ADDING PAYMENT: {err.Message}");
        throw new Exception($"GAGAL ADD PAYMENT: KODE {err.StatusCode}");
      }
    }

    public async Task UpdatePaymentMethod(PayMetCreateModel updPayment)
    {
      await InitializeAsync();
      if (updPayment.Id <= 0) throw new ArgumentException("ID PAYMENT METHOD DIPERLUKAN UNTUK PERUBAHAN");

      var endpoint = $"api/PaymentMethod/{updPayment.Id}";

      try
      {
        var content = new MultipartFormDataContent
        {
          { new StringContent(updPayment.NameMethod), "NameMethod" },
          { new StringContent(updPayment.Status ?? "Active"), "Status" }
        };

        if (updPayment.LogoMethod != null)
        {
          var fileContent = new StreamContent(updPayment.LogoMethod.OpenReadStream());
          fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(updPayment.LogoMethod.ContentType);

          content.Add(fileContent, "LogoMethod", updPayment.LogoMethod.Name);
        }
        else
        {
          Console.WriteLine("[TIDAK ADA PERUBAHAN LOGO]");
        }

        var response = await _httpClient.PutAsync(endpoint, content);
        response.EnsureSuccessStatusCode();


        Console.WriteLine($"[SUKSES UPDATE]: PAYMENT ID {updPayment.Id}");
      }
      catch (HttpRequestException err)
      {
        Console.WriteLine($"ERROR UPDATING PAYMENT ID {updPayment.Id}:{err.Message}");
        throw new Exception($"GAGAL UPDATE PAYMENT: KODE {err.StatusCode}");
      }
    }




    


  }
}