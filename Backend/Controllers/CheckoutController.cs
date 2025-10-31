using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Backend.Models;
using Backend.DTOs.Checkout;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CheckoutController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //  GET: /api/Checkout
        [HttpGet]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var checkouts = await _context.Checkouts
                    .Include(c => c.User)
                    .Include(c => c.Course)
                    .ToListAsync();

                var result = _mapper.Map<List<CheckoutDto>>(checkouts);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal mengambil data checkout.", error = ex.Message });
            }
        }

        [HttpGet("{userId:int}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return NotFound(new { message = $"User dengan Id '{userId}' tidak ditemukan." });

                var checkouts = await _context.Checkouts
                    .Include(c => c.User)
                    .Include(c => c.Course)
                        .ThenInclude(course =>course.Category)
                    .Where(c => c.IdUser == userId)
                    .ToListAsync();

                if (!checkouts.Any())
                    return NotFound(new { message = "Tidak ada data checkout untuk user ini." });

                var result = _mapper.Map<List<CheckoutDto>>(checkouts);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal mengambil data checkout user.", error = ex.Message });
            }
        }

        // POST: /api/Checkout
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Create([FromBody] CreateCheckoutDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🔹 Validasi user & course
                var userExists = await _context.Users.AnyAsync(u => u.Id == dto.IdUser);
                var courseExists = await _context.Courses.AnyAsync(c => c.IdCourse == dto.IdCourse);

                if (!userExists)
                    return BadRequest(new { message = $"User dengan IdUser = {dto.IdUser} tidak ditemukan." });

                if (!courseExists)
                    return BadRequest(new { message = $"Course dengan IdCourse = {dto.IdCourse} tidak ditemukan." });

                // 🔹 Cek apakah user sudah pernah checkout course ini
                var existingCheckout = await _context.Checkouts
                    .FirstOrDefaultAsync(c => c.IdUser == dto.IdUser && c.IdCourse == dto.IdCourse);

                if (existingCheckout != null)
                    return BadRequest(new { message = "User sudah memiliki checkout untuk course ini." });

                // 🔹 Mapping DTO → Model
                var checkout = _mapper.Map<CheckoutModel>(dto);

                await _context.Checkouts.AddAsync(checkout);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 🔹 Mapping kembali ke DTO untuk response
                var result = _mapper.Map<CheckoutDto>(checkout);

                return Ok(new
                {
                    message = "Checkout berhasil dibuat.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Gagal membuat checkout.", error = ex.Message });
            }
        }

        // DELETE: /api/Checkout/{idUser}/{idCourse}
        [HttpDelete("{idUser}/{idCourse}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Delete(int idUser, int idCourse)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkout = await _context.Checkouts
                    .FirstOrDefaultAsync(c => c.IdUser == idUser && c.IdCourse == idCourse);

                if (checkout == null)
                    return NotFound(new { message = $"Checkout dengan IdUser = {idUser} dan IdCourse = {idCourse} tidak ditemukan." });

                _context.Checkouts.Remove(checkout);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var deletedData = _mapper.Map<CheckoutDto>(checkout);

                return Ok(new
                {
                    message = $"Checkout dengan IdCourse = {idCourse} untuk user {idUser} berhasil dihapus.",
                    deleted = deletedData
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Gagal menghapus checkout.", error = ex.Message });
            }
        }
    }
}
