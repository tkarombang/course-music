using Admin.Models;
using Blazored.LocalStorage;

namespace Admin.Services.CourseService
{
  public class CoursesApiService : AdminApiService
  {
    public CoursesApiService(HttpClient httpClient, IConfiguration configuration, ILocalStorageService localStorage) : base(httpClient, configuration, localStorage){}







    public async Task<List<CourseModel>> GetAllCourses()
    {
      await InitializeAsync();
      try
      {
        var response = await _httpClient.GetFromJsonAsync<List<CourseModel>>("api/Course");
        if(response != null) Console.WriteLine($"[SUCCESS]: FETCH {response.Count} Courses Methods");

        foreach (var imgCourse in response)
        {
          var baseUrl = _configuration["ApiUrls:AdminApi"];
          if(!string.IsNullOrEmpty(imgCourse.ImageCourse))
          {
            imgCourse.ImageCourse = $"{baseUrl}{imgCourse.ImageCourse}";
          }
        }

        return response ?? new List<CourseModel>();
      }
      catch (Exception err)
      {
        Console.WriteLine($"GAGAL FETCHING DATA COURSE:  {err.Message}");
        throw;
      }
    }








    public async Task AddNewCourse(CourseCreateModel model)
    {
      await InitializeAsync();
      using var content = new MultipartFormDataContent();

      // Pastikan nama field sesuai backend
      content.Add(new StringContent(model.NamaCourse ?? ""), "NamaCourse");
      content.Add(new StringContent(model.Harga.ToString()), "Harga");
      content.Add(new StringContent(model.IdCategory.ToString()), "IdCategory");
      content.Add(new StringContent(model.Deskripsi ?? ""), "Deskripsi");

      if (model.ImageCourse != null)
      {
        var stream = model.ImageCourse.OpenReadStream(maxAllowedSize: 5_000_000);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(model.ImageCourse.ContentType);
        content.Add(fileContent, "ImageFile", model.ImageCourse.Name); // perhatikan: ImageCourse
      }

      var response = await _httpClient.PostAsync("api/Course/upload", content);

      // Cek response
      var responseBody = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"[API RESPONSE] {responseBody}");

      response.EnsureSuccessStatusCode();
    }





    public async Task UpdateCourse(CourseCreateModel course)
    {
      await InitializeAsync();
      if (course.IdCourse == null || course.IdCourse <= 0)
        throw new ArgumentException("ID Course diperlukan untuk update");

      var endpoint = $"api/Course/{course.IdCourse}/upload";
      using var content = new MultipartFormDataContent();

      content.Add(new StringContent(course.NamaCourse ?? ""), "NamaCourse");
      content.Add(new StringContent(course.Harga.ToString()), "Harga");
      content.Add(new StringContent(course.IdCategory.ToString()), "IdCategory");
      content.Add(new StringContent(course.Deskripsi ?? ""), "Deskripsi");

      // Hanya kirim gambar jika ada perubahan
      if (course.ImageCourse != null)
      {
        var stream = course.ImageCourse.OpenReadStream(maxAllowedSize: 5_000_000);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue(course.ImageCourse.ContentType);
        content.Add(fileContent, "ImageFile", course.ImageCourse.Name);
      }

      var response = await _httpClient.PutAsync(endpoint, content);

      var body = await response.Content.ReadAsStringAsync();
      Console.WriteLine($"[API RESPONSE UPDATE COURSE] {body}");

      response.EnsureSuccessStatusCode();
    }






    // Method untuk delete course
    public async Task DeleteCourse(int courseId)
    {
      await InitializeAsync();
      var response = await _httpClient.DeleteAsync($"api/Course/{courseId}");
      response.EnsureSuccessStatusCode();
    }



  }
}