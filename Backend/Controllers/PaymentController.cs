using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs.Payment;
using Backend.Data;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PaymentController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ✅ GET: /api/Payment/{idUser}
        [HttpGet("{idUser}")]
        [Authorize(Roles = "Admin, User")]
        public IActionResult GetByUserId(int idUser)
        {
            try
            {
                var query = from checkout in _context.Checkouts
                            join course in _context.Courses on checkout.IdCourse equals course.IdCourse
                            join category in _context.Categories on course.IdCategory equals category.IdCategory
                            join payment in _context.Payments on checkout.IdPayment equals payment.Id
                            join method in _context.PaymentMethods on payment.IdPaymentMethod equals method.Id
                            where checkout.IdUser == idUser
                            select new
                            {
                                CategoryName = category.NameCategory,
                                CourseName = course.NamaCourse,
                                Jadwal = checkout.Jadwal,
                                Harga = course.Harga,
                                Amount = payment.Amount,
                                MethodName = method.Name_Methode
                            };

                var data = query.ToList();

                if (!data.Any())
                    return NotFound(new { message = $"Tidak ada data pembayaran untuk user dengan IdUser = {idUser}" });

                var totalAmount = data.First().Amount;
                var methodName = data.First().MethodName;

                var response = new
                {
                    paymentMethod = methodName,
                    courses = data.Select(d => new
                    {
                        d.CategoryName,
                        d.CourseName,
                        d.Jadwal,
                        d.Harga
                    }),
                    totalAmount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data pembayaran.", error = ex.Message });
            }
        }

        // ✅ POST: /api/Payment
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> Create(CreatePaymentDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🔹 Validasi metode pembayaran
                var paymentMethod = await _context.PaymentMethods
                    .FirstOrDefaultAsync(pm => pm.Id == dto.IdPaymentMethod);

                if (paymentMethod == null)
                    return BadRequest(new { message = "Metode pembayaran tidak ditemukan." });

                if (paymentMethod.Status.ToLower() == "inactive")
                    return BadRequest(new { message = $"Metode pembayaran '{paymentMethod.Name_Methode}' sedang tidak aktif." });

                // 🔹 Ambil semua checkout milik user yang belum dibayar
                var checkouts = await _context.Checkouts
                    .Include(c => c.Course)
                    .Where(c => c.IdUser == dto.IdUser && !c.IsPaid)
                    .ToListAsync();

                if (!checkouts.Any())
                    return BadRequest(new { message = "User belum memiliki checkout baru untuk dihitung total pembayaran." });

                // 🔹 Hitung total harga
                var totalAmount = checkouts.Sum(c => c.Course.Harga);

                // 🔹 Buat payment baru
                var payment = _mapper.Map<PaymentModel>(dto);
                payment.Amount = totalAmount;
                payment.CreatedAt = DateTime.UtcNow;

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(); // simpan agar payment.Id tersedia

                // 🔹 Tandai checkout sudah dibayar dan link ke payment
                checkouts.ForEach(c =>
                {
                    c.IsPaid = true;
                    c.IdPayment = payment.Id;
                });

                await _context.SaveChangesAsync();

                // 🔹 Mapping hasil ke DTO
                var paymentDto = _mapper.Map<PaymentDto>(payment);
                paymentDto.PaymentMethodName = paymentMethod.Name_Methode;

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Payment berhasil dibuat.",
                    payment = paymentDto
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = "Gagal membuat pembayaran.", error = ex.Message });
            }
        }
    }
}
