using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.DTOs.Category;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Backend.Interface;

namespace Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CategoryController : ControllerBase
	{
		private readonly ICategoryInterface _categoryService;
		private readonly ILogger<CategoryController> _logger;
		public CategoryController(ICategoryInterface categoryInterface, ILogger<CategoryController> logger)
		{
			_categoryService = categoryInterface;
			_logger = logger;
		}

		/*
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
   */



		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetByIdCategories(int id)
		{
			var result = await _categoryService.GetByIdAsync(id);
			if (result == null) return NotFound(new { message = "KATEGORI TIDAK DITEMUKAN" });

			return Ok(new { data = result });
		}


		[HttpGet("by-name/{name}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetByNameCategories(string name)
		{
			var result = await _categoryService.GetByNameAsync(name);
			if (result == null) return NotFound(new { message = "NAMA KATEGORI TIDAK DITEMUKAN" });

			return Ok(new { date = result });
		}



		[HttpGet]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetCategoriesPagedAsync(
			[FromQuery] string? searchTerm = null,
			[FromQuery] int pageNumber = 1,
			[FromQuery] int pageSize = 5)
		{
			try
			{
				var result = await _categoryService.GetCategPagedAsync(searchTerm!, pageNumber, pageSize);
				return Ok(new
				{
					message = "BERHASIL MENDAPATKAN DAFTAR KATEGORI",
					data = result.Categories,
					pagination = new
					{
						result.TotalCount,
						result.PageSize,
						result.CurrentPage
					}
				});
			}
			catch (Exception err)
			{
				_logger.LogError("ERROR GET CATEGORY PAGED: {errMessage}", err.Message);
				return StatusCode(500, new
				{
					message = "ERROR GETTING PAGED CATEGORIES",
					error = err.Message
				});
			}
		}




		// ✅ POST: api/Category
		[HttpPost]
		[Authorize(Roles = "Admin, User")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			try
			{
				var result = await _categoryService.CreateAsync(dto);
				return CreatedAtAction(nameof(GetCategoriesPagedAsync), new { id = result.IdCategory }, new
				{
					message = "KATEGORI BERHASIL DI BUAT",
					data = result
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Gagal membuat kategori.", error = ex.Message });
			}
		}

		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateDto dto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			try
			{
				var result = await _categoryService.UpdateCategoryAsync(id, dto);
				if (result == null) return NotFound(new { message = "KATEGORU TIDAK DITEMUKAN" });

				return Ok(new
				{
					message = "KATEGORI BERHASIL DIUBAH",
					data = result
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
			catch (Exception err)
			{
				return StatusCode(500, new { message = "GAGAL MEMPERBAHARUI", error = err.Message });
			}
		}

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteCategory(int id)
		{
			try
			{
				var success = await _categoryService.DeleteCategoryAsync(id);
				if (!success) return NotFound(new { message = "KATEGORI TIDAK DITEMUKAN" });

				return Ok(new { message = "KATEGORI BERHASIL DIHAPUS" });
			}
			catch (InvalidOperationException err)
			{
				return BadRequest(new { message = err.Message });
			}
			catch (Exception err)
			{
				return StatusCode(500, new { message = "GAGAL MENGHAPUS KATEGORI", error = err.Message });
			}
		}



		/*
        // PUT: api/Category/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CreateCourseDto dtoCourse)
        {
          await using var transaction = await _context.Database.BeginTransactionAsync();

          var category = await _context.Categories.FindAsync(id);
          if (category == null)
            return NotFound(new { message = "Category not found." });

          try
          {
            // Ambil data form
            var form = await Request.ReadFormAsync();

            // Update text fields
            if (form.TryGetValue("NameCategory", out var name))
              category.NameCategory = name!;

            if (form.TryGetValue("Deskripsi", out var deskripsi))
              category.Deskripsi = deskripsi;

            // Update ImageCategory jika ada file baru
            if (dtoCourse.ImageCourse != null && dtoCourse.ImageCourse.Length > 0)
            {
              if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);

              if(!string.IsNullOrEmpty(category.ImageCategory))
              {
                var oldFilePath = Path.Combine(_env.WebRootPath, category.ImageBanner.TrimStart('/'));
                if(System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
              }

              var uniqueCateName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dtoCourse.ImageCourse.FileName);

              var filePath = Path.Combine(_uploadsFolder, uniqueCateName);

              using (var fileStream = new FileStream(filePath, FileMode.Create))
              {
                await dtoCourse.ImageCourse.CopyToAsync(fileStream);
              }

              category.ImageCategory = $"/img/list_category/{uniqueCateName}";
            }


            if (dtoCourse.ImageBanner != null && dtoCourse.ImageBanner.Length > 0)
            {
              // Update ImageBanner jika ada file baru
              if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);

              if (!string.IsNullOrEmpty(category.ImageBanner))
              {
                var oldFilePath = Path.Combine(_env.WebRootPath, category.ImageBanner.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
              }

              var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dtoCourse.ImageBanner.FileName);
              var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

              using (var fileStream = new FileStream(filePath, FileMode.Create))
              {
                await dtoCourse.ImageBanner.CopyToAsync(fileStream);
              }

              category.ImageBanner = $"/img/list_category/{uniqueFileName}";
            }


            category.Deskripsi = dtoCourse.Deskripsi;

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

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

    */


	}
}
