using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.DTOs.Invoice;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public InvoiceController(AppDbContext context) => _context = context;

        [HttpGet("dashboard")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetDashboardInvoice(
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                var payments = await _context.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                // Ambil semua payment (latest first)
                var allPayments = await _context.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                if (!allPayments.Any())
                    return Ok(new List<InvoiceDto>());

                // Ambil semua userId unik dari payment
                var userIds = allPayments.Select(p => p.IdUser).Distinct().ToList();

                // Ambil email tiap user (dictionary userId -> email)
                var userEmails = await _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.Email })
                    .ToDictionaryAsync(u => u.Id, u => u.Email);

                // Ambil semua id payment yang ada
                var paymentIds = allPayments.Select(p => p.Id).ToList();

                // Hitung jumlah checkout per IdPayment
                var checkoutCounts = await _context.Checkouts
                    .Where(c => c.IdPayment.HasValue && paymentIds.Contains(c.IdPayment.Value))
                    .GroupBy(c => c.IdPayment.Value)
                    .Select(g => new
                    {
                        IdPayment = g.Key,
                        TotalCourses = g.Count()
                    })
                    .ToDictionaryAsync(x => x.IdPayment, x => x.TotalCourses);

                int totalInvoices = allPayments.Count; // total invoice untuk pembalik index

                var result = allPayments.Select((p, index) =>
                {
                    userEmails.TryGetValue(p.IdUser, out string? email);
                    checkoutCounts.TryGetValue(p.Id, out int totalCourses);

                    // index 0 (terbaru) mendapat nomor terbesar
                    int reversedNumber = totalInvoices - index;

                    return new InvoiceDto
                    {
                        Email = email ?? $"user_{p.IdUser}@timtiga.com",
                        NoInvoice = $"INV-{reversedNumber.ToString("D5")}",
                        TanggalBeli = p.CreatedAt,
                        JumlahKursus = totalCourses,
                        TotalHarga = p.Amount,
                    };
                }).ToList();

                return Ok(result);
            }
            catch (Exception err)
            {
                return StatusCode(500, new
                {
                    message = "TERJADI KESALAHAN SAAT MENGAMBIL DATA INVOICE DASHBOARD.!",
                    error = err.Message
                });
            }
        }



        // ✅ GET: /api/Invoice/{idUser}
        [HttpGet("{idUser}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetInvoiceByUser(int idUser)
        {
            // 🧩 Start a read-only transaction for data consistency
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 🧾 Ambil semua pembayaran user
                var payments = await _context.Payments
                    .Where(p => p.IdUser == idUser)
                    .OrderBy(p => p.CreatedAt)
                    .ToListAsync();

                if (!payments.Any())
                    return NotFound(new { message = $"User dengan IdUser = {idUser} belum memiliki pembayaran." });

                var invoiceList = new List<InvoiceDto>();

                // 🔹 Loop setiap payment dan ambil kursus yang dibayar
                foreach (var p in payments)
                {
                    var courses = await _context.Checkouts
                        .Include(c => c.Course)
                        .Where(c => c.IdUser == idUser && c.IsPaid && c.IdPayment == p.Id)
                        .Select(c => new CourseInvoiceDto
                        {
                            IdCourse = c.Course.IdCourse,
                            NamaCourse = c.Course.NamaCourse,
                            Harga = c.Course.Harga
                        })
                        .ToListAsync();

                    if (!courses.Any())
                        continue; // Lewati payment tanpa course

                    var jumlahKursus = courses.Count;

                    invoiceList.Add(new InvoiceDto
                    {
                        NoInvoice = $"APM{(invoiceList.Count + 1).ToString("D5")}",
                        TanggalBeli = p.CreatedAt,
                        JumlahKursus = jumlahKursus,
                        TotalHarga = p.Amount,
                        Courses = courses
                    });
                }

                if (!invoiceList.Any())
                    return NotFound(new { message = "Tidak ditemukan invoice yang valid untuk user ini." });

                // ✅ Commit transaction (optional untuk read-only)
                await transaction.CommitAsync();

                return Ok(invoiceList);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new
                {
                    message = "Terjadi kesalahan saat mengambil data invoice.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("detail/{userId:int}/{noInvoice}")]
        [Authorize(Roles = "Admin, User")]
        public async Task<IActionResult> GetInvoiceDetail(int userId, string noInvoice)
        {
            // Pastikan format "APM00001" -> ambil angka invoice-nya
            if (!noInvoice.StartsWith("APM") || !int.TryParse(noInvoice.Substring(3), out int invoiceNumber))
                return BadRequest(new { message = "Format nomor invoice tidak valid." });

            var payments = await _context.Payments
                .Where(p => p.IdUser == userId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            if (invoiceNumber < 1 || invoiceNumber > payments.Count)
                return NotFound(new { message = "Nomor invoice tidak ditemukan." });

            // Ambil payment berdasarkan urutan invoice
            var payment = payments[invoiceNumber - 1];

            // Ambil data checkouts yang terkait dengan payment ini
            var checkouts = await _context.Checkouts
            .Include(c => c.Course)
                .ThenInclude(c => c.Category)
            .Where(c => c.IdUser == userId && c.IsPaid && c.IdPayment == payment.Id)
            .ToListAsync();


            if (!checkouts.Any())
                return NotFound(new { message = "Tidak ditemukan data kursus untuk invoice ini." });

            var result = new
            {
                NoInvoice = noInvoice,
                TanggalBeli = payment.CreatedAt,
                TotalHarga = payment.Amount,
                DetailCourses = checkouts.Select((c, i) => new
                {
                    Index = i + 1,
                    NamaCourse = c.Course.NamaCourse,
                    Kategori = c.Course.Category != null ? c.Course.Category.NameCategory : "Tidak diketahui",
                    Jadwal = c.Jadwal, // ambil dari CheckoutModel, bukan Course
                    Harga = c.Course.Harga
                })
            };

            return Ok(result);
        }


    }
}
