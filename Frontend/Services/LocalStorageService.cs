using Microsoft.JSInterop;

namespace Frontend.Services
{
  public class LocalStorageService
  {
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jSRuntime)
    {
      _jsRuntime = jSRuntime;
    }

    public async Task SetItemAsync(string key, string value)
    {
      await _jsRuntime.InvokeVoidAsync("localStorageHelper.setItem", key, value);
    }

    public async Task<string> GetItemAsync(string key)
    {
      return await _jsRuntime.InvokeAsync<string>("localStorageHelper.getItem", key);
    }

    public async Task RemoveItemAsync(string key)
    {
      await _jsRuntime.InvokeVoidAsync("localStorageHelper.removeItem", key);
    }

    public async Task ClearAsync()
    {
      await _jsRuntime.InvokeVoidAsync("localStorageHelper.clear");
    }
  }
}