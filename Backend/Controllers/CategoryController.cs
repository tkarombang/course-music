using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs.Course;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly IWebHostEnvironment _env;

        public CategoryController(AppDbContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        // ✅ GET: api/Category/{categoryName}
        [HttpGet("{categoryName}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<ActionResult<CategoryWithCoursesDto>> GetCategoryWithCourses(string categoryName)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Courses)
                    .FirstOrDefaultAsync(c => c.NameCategory.ToLower() == categoryName.ToLower());

                if (category == null)
                    return NotFound(new { message = $"Kategori '{categoryName}' tidak ditemukan." });

                var categoryDto = _mapper.Map<CategoryWithCoursesDto>(category);
                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data kategori.", error = ex.Message });
            }
        }
        // ✅ GET: api/Category
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Select(c => new
                    {
                        c.IdCategory,
                        c.NameCategory,
                        c.Deskripsi,
                        c.ImageCategory,
                        c.ImageBanner
                    })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil semua kategori.", error = ex.Message });
            }
        }

        // ✅ POST: api/Category
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var category = new Backend.Models.CategoryModel
                {
                    NameCategory = dto.NameCategory,
                    Deskripsi = dto.Deskripsi
                };

                // Simpan ImageCategory jika ada
                if (dto.ImageCategory != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{dto.ImageCategory.FileName}";
                    var filePath = Path.Combine("wwwroot/img/list_category", fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.ImageCategory.CopyToAsync(stream);
                    category.ImageCategory = fileName;
                }

                // Simpan ImageBanner jika ada
                if (dto.ImageBanner != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{dto.ImageBanner.FileName}";
                    var filePath = Path.Combine("wwwroot/img/list_category", fileName);
                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await dto.ImageBanner.CopyToAsync(stream);
                    category.ImageBanner = fileName;
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetCategoryWithCourses),
                    new { categoryName = category.NameCategory },
                    new { message = "Kategori berhasil dibuat.", category.IdCategory });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Gagal membuat kategori.", error = ex.Message });
            }
        }

        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            try
            {
                // Ambil data form
                var form = await Request.ReadFormAsync();

                // Update text fields
                if (form.TryGetValue("NameCategory", out var name))
                    category.NameCategory = name;

                if (form.TryGetValue("Deskripsi", out var deskripsi))
                    category.Deskripsi = deskripsi;

                // Update ImageCategory jika ada file baru
                var imageFile = form.Files["ImageCategory"];
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "img/list_category");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await imageFile.CopyToAsync(stream);

                    category.ImageCategory = fileName;
                }

                // Update ImageBanner jika ada file baru
                var bannerFile = form.Files["ImageBannerFile"]; // harus sama dengan key yang dikirim dari AdminApiService
                if (bannerFile != null && bannerFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_env.WebRootPath, "img/list_category");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{bannerFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    await using var stream = new FileStream(filePath, FileMode.Create);
                    await bannerFile.CopyToAsync(stream);

                    category.ImageBanner = fileName;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Category updated successfully.", category.IdCategory });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update category.", error = ex.Message });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Category not found." });

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category deleted successfully." });
        }

    }
}
