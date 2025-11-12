using Admin.DTOs.Users;
using Admin.Interfaces;
using Admin.Models;
using Blazored.LocalStorage;

namespace Admin.Services.UsersService
{
  public class UsersApiService : AdminApiService, IUsersApiInterface
  {
    // private readonly IAdminApiInterface _adminApiService;

    public UsersApiService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage): base(httpClient, configuration, localStorage)
    {
      // _adminApiService = adminApiService;
    }




    // public async Task<string> GetPagedUsersAsync(string queryParams)
    // {
    //   await _adminApiService.InitializeAsync();
    //   try
    //   {
    //     var response = await _httpClient.GetAsync($"api/User/paged?{queryParams}");
    //     Console.WriteLine($"Debug: GetPagedUsersAsync response status: {response.StatusCode}");

    //     response.EnsureSuccessStatusCode();
    //     return await response.Content.ReadAsStringAsync();
    //   }
    //   catch (Exception err)
    //   {
    //     Console.WriteLine($"ERROR TANGKAP DATA USERS: {err.Message}");
    //     throw;
    //   }
    // }


    public async Task<PageUserListDto> GetUsersPagedAsync(
      string searchTerm = null!,
      int pagedNumber = 1,
      int pageSize = 10
    )
    {
      try
      {
        await InitializeAsync();
        var url = $"api/User/paged?pageNumber={pagedNumber}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
          url += $"&searchTerm={searchTerm}";
        }

        // var query = new Uri(_httpClient.BaseAddress!, url);
        // Console.WriteLine($"[DEBUG QUERY STRING]: {query}");

        var response = await _httpClient.GetFromJsonAsync<PageUserListDto>(url);
        Console.WriteLine($"[DEBUG RESPONSE]: {response}");

        return response ?? new PageUserListDto { Users = []};
      }
      catch (Exception err)
      {
        Console.WriteLine($"[ERROR FETCH USERS]: {err.Message}");
        throw;
      }
    }




    public async Task<List<UserManModel>> GetAllUsersAsync()
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.GetFromJsonAsync<List<UserManModel>>("api/User");

        if (response != null)
        {
          Console.WriteLine($"SUCCESS: Fetched {response.Count} Users");
        }
        return response ?? new List<UserManModel>();
      }
      catch (Exception err)
      {
        Console.WriteLine($"FATAL ERROR FETCHING USERS: {err.Message}");
        throw;
      }
    }




    public async Task UpdateUserAsync(UserManModel user)
    {
      await InitializeAsync();
      Console.WriteLine("[USER ID]:" + user.Id);
      var requestBody = new UserManModel
      {
        UserName = user.UserName,
        Email = user.Email,
        IsActive = user.IsActive,
      };

      try
      {
        var response = await _httpClient.PutAsJsonAsync($"api/User/{user.Id}", requestBody);
        response.EnsureSuccessStatusCode();

        Console.WriteLine($"SUKSES: USER ID {user.Id} UPDATED");
      }
      catch (HttpRequestException err)
      {
        Console.WriteLine($"ERROR UPDATE: {user.Id} - {err.Message}");
        throw new Exception($"GAGAL UPDATE USER, KODE: {err.StatusCode}");
      }
    }




    public async Task AddNewUserAsync(UserManModel newUser)
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.PostAsJsonAsync($"api/User", newUser);
        response.EnsureSuccessStatusCode();

        Console.WriteLine($"[SUKSES-ADD]: USER BARU");
      }
      catch (HttpRequestException err)
      {
        Console.WriteLine($"ERROR ADDING USER: {err.Message}");
        throw new Exception($"GAGAL ADD USER BARU, KODE: {err.StatusCode}");
      }
    }

  }
}