using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;


namespace Backend.Services
{
  public class DataSeeder
  {
    private readonly AppDbContext _context;
    public DataSeeder(AppDbContext context)
    {
      _context = context;
    }

    public async Task SeedDataAsync()
    {
      if (!await _context.Categories.AnyAsync())
      {
        var categories = new CategoryModel[]
        {
          new CategoryModel { NameCategory = "Drum", ImageCategory = "Drum.png" },
          new CategoryModel { NameCategory = "Piano", ImageCategory = "Piano.png" },
          new CategoryModel { NameCategory = "Gitar", ImageCategory = "Gitar.png" },
          new CategoryModel { NameCategory = "Biola", ImageCategory = "Biola.png" },
          new CategoryModel { NameCategory = "Saxophone", ImageCategory = "Saxophone.png" },
          new CategoryModel { NameCategory = "Flute", ImageCategory = "Saxophone.png" },
          new CategoryModel { NameCategory = "Bass", ImageCategory = "Gitar.png" },
          new CategoryModel { NameCategory = "Menyanyi", ImageCategory = "Menyanyi.png" }
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
      }

      if(!await _context.Courses.AnyAsync())
      {
        var categoriesMap = await _context.Categories.ToDictionaryAsync(c => c.NameCategory, c => c.IdCategory);
        var courses = new DataCourseModel[]
        {
          new DataCourseModel
                {
                    NamaCourse = "Kursus Drummer Special Coach (Eno Netral)",
                    Harga = 8500000,
                    ImageCourse = "d4.png",
                    IdCategory = categoriesMap["Drum"]
                },

                new DataCourseModel
                {
                    NamaCourse = "Expert Level Drummer Lessons",
                    Harga = 5450000,
                    ImageCourse = "d1.png",
                    IdCategory = categoriesMap["Drum"]
                },

                new DataCourseModel
                {
                    NamaCourse = "From Zero to Professional Drummer (Complit Package)",
                    Harga = 13000000,
                    ImageCourse = "d2.png",
                    IdCategory = categoriesMap["Drum"]
                },

                new DataCourseModel
                {
                    NamaCourse = "Drummer for kids (Level Basic/1)",
                    Harga = 2200000,
                    ImageCourse = "d3.png",
                    IdCategory = categoriesMap["Drum"]
                },

                new DataCourseModel
                {
                    NamaCourse = "Kursu Piano : From Zero to Pro (Full Package)",
                    Harga = 11650000,
                    ImageCourse = "Copy of Rectangle 12-5.png",
                    IdCategory = categoriesMap["Piano"]
                },

                new DataCourseModel
                {
                    NamaCourse = "[Beginner] Guitar class for kids ",
                    Harga = 1600000,
                    ImageCourse = "Copy of Rectangle 12-2.png",
                    IdCategory = categoriesMap["Gitar"]
                },

                new DataCourseModel
                {
                    NamaCourse = "Biola Mid-Level Course",
                    Harga = 3000000,
                    ImageCourse = "Copy of Rectangle 12-3.png",
                    IdCategory = categoriesMap["Biola"]
                },

                new DataCourseModel
                {
                    NamaCourse = "Expert Level Saxophone",
                    Harga = 7350000,
                    ImageCourse = "Copy of Rectangle 12-6.png",
                    IdCategory = categoriesMap["Saxophone"]
                }
        };
        await _context.Courses.AddRangeAsync(courses);
        await _context.SaveChangesAsync();
      }
    }
  }
}