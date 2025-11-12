using System.Net.Http.Headers;
using Admin.DTOs.Category;
using Admin.Interfaces;
using Blazored.LocalStorage;

namespace Admin.Services.CategoriesService
{
  public class CategoryApiService : AdminApiService
  {
    // private readonly HttpClient _httpClient;
    // private readonly IConfiguration _configuration;
    // private readonly ILocalStorageService _localStorage;
    // private const string TokenKey = "authToken";
    // private readonly IAdminApiInterface _adminApiService;

    public CategoryApiService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage) : base(httpClient, configuration, localStorage)
    {
      // _httpClient = httpClient;
      // _configuration = configuration;
      // _localStorage = localStorage;
      // _adminApiService = adminApiService;

    }


    public async Task<CategoryListDto> GetAllCategoriesAsync(
      string searchTerm = null!,
      int currentPage = 1,
      int pageSize = 5
    )
    {
      try
      {
        await InitializeAsync();
        var url = $"api/Category?pageNumber={currentPage}&pageSize={pageSize}";
        // var query = _httpClient.BaseAddress + url;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
          url += $"&searchTerm={searchTerm}";
        }

        var response = await _httpClient.GetFromJsonAsync<CategoryListDto>(url);

        var baseUrl = _configuration["ApiUrls:AdminApi"];
        foreach (var imgCategory in response!.Categories)
        {
          if (!string.IsNullOrEmpty(imgCategory.ImageCategory))
          {
            imgCategory.ImageCategory = $"{baseUrl}{imgCategory.ImageCategory}";
          }
        }

        foreach (var imgBanner in response!.Categories)
        {
          if (!string.IsNullOrEmpty(imgBanner.ImageBanner))
          {
            imgBanner.ImageBanner = $"{baseUrl}{imgBanner.ImageBanner}";
          }
        }

        return response ?? new CategoryListDto { Categories = []};
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERROR FETCHING CATEGORIES]: {ex.Message}");
        throw;
      }
    }

    public async Task AddNewCategory(CategoryCreateModel model)
    {
      // await InitializeAsync();
      await InitializeAsync();
      using var content = new MultipartFormDataContent();

      content.Add(new StringContent(model.NameCategory ?? ""), "NameCategory");
      content.Add(new StringContent(model.Deskripsi ?? ""), "Deskripsi");

      if (model.ImageCategory != null)
      {
        var stream = model.ImageCategory.OpenReadStream(5_000_000);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageCategory.ContentType);
        content.Add(fileContent, "ImageCategory", model.ImageCategory.Name);
      }

      if (model.ImageBanner != null)
      {
        var stream = model.ImageBanner.OpenReadStream(5_000_000);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageBanner.ContentType);
        content.Add(fileContent, "ImageBanner", model.ImageBanner.Name);
      }

      var response = await _httpClient.PostAsync("api/Category", content);
      response.EnsureSuccessStatusCode();
    }


    public async Task UpdateCategory(CategoryCreateModel model)
    {
      await InitializeAsync();
      try
      {
        using var content = new MultipartFormDataContent
      {
        { new StringContent(model.IdCategory.ToString()), "IdCategory" },
        { new StringContent(model.NameCategory ?? ""), "NameCategory" },
        { new StringContent(model.Deskripsi ?? ""), "Deskripsi" },
      };

        if (model.ImageCategory != null)
        {
          var stream = model.ImageCategory.OpenReadStream(maxAllowedSize: 5_000_000);
          var fileContent = new StreamContent(stream);
          fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageCategory.ContentType);
          content.Add(fileContent, "ImageCategory", model.ImageCategory.Name);
        }

        if (model.ImageBanner != null)
        {
          var stream = model.ImageBanner.OpenReadStream(maxAllowedSize: 5_000_000);
          var fileContent = new StreamContent(stream);
          fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageBanner.ContentType);
          content.Add(fileContent, "ImageBanner", model.ImageBanner.Name);
        }

        var response = await _httpClient.PutAsync($"api/Category/{model.IdCategory}", content);
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"[API RESPONSE UPDATE CATEGORY]: {responseBody}");

        response.EnsureSuccessStatusCode();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[ERROR UPDATING CATEGORY]: {ex.Message}");
        throw;
      }

    }


    public async Task DeleteCategory(int categoryId)
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.DeleteAsync($"api/Category/{categoryId}");
        response.EnsureSuccessStatusCode();
      }
      catch (HttpRequestException ex)
      {
        Console.WriteLine($"ERROR DELETE CATEGORY ID {categoryId}: {ex.Message}");
        throw;
      }
    }
  }


}
