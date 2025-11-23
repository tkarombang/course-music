using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs.Course;
using Backend.DTOs.Category;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class CourseController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly IMapper _mapper;

		public CourseController(AppDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		// ✅ GET: api/Course
		[HttpGet]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetCourses()
		{
			try
			{
				var courses = await _context.Courses
				.Include(c => c.Category)
				.ToListAsync();

				var courseDtos = _mapper.Map<List<CourseDto>>(courses);

				return Ok(courseDtos);
			}
			catch (Exception ex)
			{
				// 🧠 Added error handling for GET
				return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data kursus.", error = ex.Message });
			}
		}

		// ✅ GET: api/Course/{id}
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetCourse(int id)
		{
			try
			{
				var course = await _context.Courses
						.Include(c => c.Category)
						.FirstOrDefaultAsync(c => c.IdCourse == id);

				if (course == null)
					return NotFound(new { message = "Kursus tidak ditemukan." });

				var dto = _mapper.Map<CourseDto>(course);
				return Ok(dto);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil detail kursus.", error = ex.Message });
			}
		}

		// ✅ POST: api/Course
		[HttpPost]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> CreateCourse(CourseDto dto)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(); // 🧠 Added transaction start
			try
			{
				var course = _mapper.Map<DataCourseModel>(dto);

				_context.Courses.Add(course);
				await _context.SaveChangesAsync();

				var createdDto = _mapper.Map<CourseDto>(course);

				await transaction.CommitAsync(); // 🧠 Commit after success

				dto.IdCourse = course.IdCourse;

				return CreatedAtAction(nameof(GetCourse), new { id = createdDto.IdCourse }, createdDto);
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync(); // 🧠 Rollback on failure
				return StatusCode(500, new { message = "Gagal membuat kursus baru.", error = ex.Message });
			}
		}

		// ✅ PUT: api/Course/{id}
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> UpdateCourse(int id, CourseDto dto)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(); // 🧠 Added transaction
			try
			{
				if (id != dto.IdCourse)
					return BadRequest(new { message = "Id tidak sesuai dengan data kursus." });

				var course = await _context.Courses.FindAsync(id);
				if (course == null)
					return NotFound(new { message = "Kursus tidak ditemukan." });

				// Mapping update dari DTO → Model
				_mapper.Map(dto, course);

				_context.Courses.Update(course);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync(); // ✅ Commit changes

				return Ok(new { message = "Kursus berhasil diperbarui.", course });
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync(); // ❌ Rollback on error
				return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui kursus.", error = ex.Message });
			}
		}

		// ✅ DELETE: api/Course/{id}
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> DeleteCourse(int id)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync(); // 🧠 Added transaction
			try
			{
				var course = await _context.Courses.FindAsync(id);
				if (course == null)
					return NotFound(new { message = "Kursus tidak ditemukan." });

				_context.Courses.Remove(course);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync(); // ✅ Commit after delete

				return Ok(new { message = "Kursus berhasil dihapus." });
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync(); // ❌ Rollback on failure
				return StatusCode(500, new { message = "Gagal menghapus kursus.", error = ex.Message });
			}
		}

		[HttpPost("upload")]
		[RequestSizeLimit(10_000_000)] // Batas max 10MB
		public async Task<IActionResult> UploadCourse([FromForm] CourseUploadRequest request)
		{
			try
			{
				if (request.ImageFile == null || request.ImageFile.Length == 0)
					return BadRequest(new { message = "File gambar tidak ditemukan." });

				// 🧩 Pastikan folder penyimpanan ada
				var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "list_kelas");
				if (!Directory.Exists(uploadFolder))
					Directory.CreateDirectory(uploadFolder);

				// 🧩 Generate nama unik untuk file
				var uniqueFileName = $"{Guid.NewGuid()}_{request.ImageFile.FileName}";
				var filePath = Path.Combine(uploadFolder, uniqueFileName);

				// 🧩 Simpan file fisik ke wwwroot
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await request.ImageFile.CopyToAsync(stream);
				}

				// 🧩 Simpan data ke database
				var course = new DataCourseModel
				{
					NamaCourse = request.NamaCourse,
					Harga = request.Harga,
					Deskripsi = request.Deskripsi,
					ImageCourse = uniqueFileName,
					IdCategory = request.IdCategory
				};

				_context.Courses.Add(course);
				await _context.SaveChangesAsync();

				return Ok(new
				{
					message = "Course uploaded successfully!",
					data = new
					{
						course.IdCourse,
						course.NamaCourse,
						course.Harga,
						ImageUrl = $"/img/list_kelas/{uniqueFileName}"
					}
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Terjadi kesalahan saat upload course.", error = ex.Message });
			}
		}

		// ✅ PUT: api/Course/{id}/upload
		[HttpPut("{id}/upload")]
		[RequestSizeLimit(10_000_000)] // Maks 10MB
		public async Task<IActionResult> UpdateCourseWithImage(int id, [FromForm] CourseUploadRequest request)
		{
			await using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var course = await _context.Courses.FindAsync(id);
				if (course == null)
					return NotFound(new { message = "Kursus tidak ditemukan." });

				// 🧩 Update field text
				if (!string.IsNullOrWhiteSpace(request.NamaCourse))
					course.NamaCourse = request.NamaCourse;

				if (!string.IsNullOrWhiteSpace(request.Deskripsi))
					course.Deskripsi = request.Deskripsi;

				if (request.Harga > 0)
					course.Harga = request.Harga;

				if (request.IdCategory != 0)
					course.IdCategory = request.IdCategory;

				// 🧩 Jika user upload file baru, proses. Kalau tidak, biarkan gambar lama tetap ada.
				if (request.ImageFile != null && request.ImageFile.Length > 0)
				{
					var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "list_kelas");
					if (!Directory.Exists(uploadFolder))
						Directory.CreateDirectory(uploadFolder);

					// Hapus file lama jika ada
					if (!string.IsNullOrEmpty(course.ImageCourse))
					{
						var oldFilePath = Path.Combine(uploadFolder, course.ImageCourse);
						if (System.IO.File.Exists(oldFilePath))
							System.IO.File.Delete(oldFilePath);
					}

					// Simpan file baru
					var uniqueFileName = $"{Guid.NewGuid()}_{request.ImageFile.FileName}";
					var filePath = Path.Combine(uploadFolder, uniqueFileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await request.ImageFile.CopyToAsync(stream);
					}

					// Update nama file di DB
					course.ImageCourse = uniqueFileName;
				}

				_context.Courses.Update(course);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new
				{
					message = "Course updated successfully!",
					data = new
					{
						course.IdCourse,
						course.NamaCourse,
						course.Harga,
						ImageUrl = $"/img/list_kelas/{course.ImageCourse}"
					}
				});
			}
			catch (Exception ex)
			{
				await transaction.RollbackAsync();
				return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui course.", error = ex.Message });
			}
		}



	}
}
