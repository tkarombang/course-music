
namespace Backend.Interface
{
  public interface IFileUploadInterface
  {
    Task<string> UploadAsync(IFormFile file, string folder, string prefix);
    void DeleteFile(string filePath);
    
  }
}