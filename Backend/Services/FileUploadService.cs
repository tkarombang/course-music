using Backend.Interface;
namespace Backend.Services
{
  public class FileUploadService : IFileUploadInterface
  {
    private readonly IWebHostEnvironment _env;
    public FileUploadService(IWebHostEnvironment env)
    {
      _env = env;
    }

    public async Task<string?> UploadAsync(IFormFile file, string folder, string prefix)
    {
      var allowed = new[] {".jpg", ".png"};
      var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

      if(!allowed.Contains(ext)) return null;
      if(file.Length > 5 * 1024 * 1024) return null;

      var uploadsFolder = Path.Combine(_env.WebRootPath, "img", folder);
      Directory.CreateDirectory(uploadsFolder);

      var fileName = $"{prefix}_{Guid.NewGuid()}{ext}";
      var filePath = Path.Combine(uploadsFolder, fileName);

      await using var stream = new FileStream(filePath, FileMode.Create);
      await file.CopyToAsync(stream);

      return $"/img/{folder}/{fileName}";
    }

    public void DeleteFile(string? filePath)
    {
      if (string.IsNullOrEmpty(filePath)) return;
      var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
      if (File.Exists(fullPath)) File.Delete(fullPath);
    }
  }
}