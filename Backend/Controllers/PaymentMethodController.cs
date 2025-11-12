using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Backend.DTOs.PaymentMethod;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
  public class PaymentMethodController : ControllerBase
  {
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _hostingEnvironment;
    public PaymentMethodController(AppDbContext context, IWebHostEnvironment hostEnvironment)
    {
      _context = context;
      _hostingEnvironment = hostEnvironment;
    }

    // ✅ GET: /api/PaymentMethod
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    public IActionResult GetAllMethods()
    {
      try
      {
        var methods = _context.PaymentMethods
            .Select(m => new
            {
              m.Id,
              m.Name_Methode,
              m.Logo_Methode,
              m.Status
            })
            .ToList();

        return Ok(methods);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error fetching payment methods", error = ex.Message });
      }
    }

    // ✅ GET: /api/PaymentMethod/{id}
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin, User")]
    public IActionResult GetMethodById(int id)
    {
      try
      {
        var method = _context.PaymentMethods
            .Where(m => m.Id == id)
            .Select(m => new
            {
              m.Id,
              m.Name_Methode,
              m.Logo_Methode,
              m.Status
            })
            .FirstOrDefault();

        if (method == null)
          return NotFound(new { message = "Payment method not found." });

        return Ok(method);
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { message = "Error fetching payment method", error = ex.Message });
      }
    }

    // ✅ POST: /api/PaymentMethod
    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Create([FromForm] CreatePaymentMethodDto dto)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var exists = await _context.PaymentMethods.AnyAsync(m => m.Name_Methode == dto.NameMethod);
        if (exists)
          return BadRequest(new { message = "Payment method name already exists." });


        string? imageUrl = null;
        if (dto.LogoMethod != null && dto.LogoMethod.Length > 0)
        {
          var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img", "bank");
          if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

          var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.LogoMethod.FileName);
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          using (var fileStream = new FileStream(filePath, FileMode.Create))
          {
            await dto.LogoMethod.CopyToAsync(fileStream);
          }

          imageUrl = $"/img/bank/{uniqueFileName}";
        }


        var method = new PaymentMethodModel
        {
          Name_Methode = dto.NameMethod,
          Logo_Methode = imageUrl!,
          Status = dto.Status ?? "Active"
        };

        _context.PaymentMethods.Add(method);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();


        return CreatedAtAction(nameof(GetMethodById), new { id = method.Id }, method);
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return StatusCode(500, new { message = "Error creating payment method", error = ex.Message });
      }
    }

    // ✅ PUT: /api/PaymentMethod/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, User")]
    public async Task<IActionResult> Update(int id, [FromForm] CreatePaymentMethodDto dto)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var method = await _context.PaymentMethods.FindAsync(id);
        if (method == null)
          return NotFound(new { message = "Payment method not found." });

        if (method.Name_Methode != dto.NameMethod)
        {
          var exist = await _context.PaymentMethods.AnyAsync(m => m.Name_Methode == dto.NameMethod && m.Id != id);

          if (exist) return BadRequest(new { message = "NAMA BANK SUDAH ADA" });
        }

        if (dto.LogoMethod != null && dto.LogoMethod.Length > 0)
        {
          var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img", "bank");
          if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

          if (!string.IsNullOrEmpty(method.Logo_Methode))
          {
            var oldFilePath = Path.Combine(_hostingEnvironment.WebRootPath, method.Logo_Methode.TrimStart('/'));
            if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
          }

          var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.LogoMethod.FileName);
          var filePath = Path.Combine(uploadsFolder, uniqueFileName);

          using (var fileStream = new FileStream(filePath, FileMode.Create))
          {
            await dto.LogoMethod.CopyToAsync(fileStream);
          }

          method.Logo_Methode = $"/img/bank/{uniqueFileName}";
        }

        method.Name_Methode = dto.NameMethod;
        // method.Logo_Methode = dto.LogoMethod;
        method.Status = dto.Status ?? method.Status;

        _context.PaymentMethods.Update(method);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new { message = "Payment method updated successfully.", method });
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return StatusCode(500, new { message = "Error updating payment method", error = ex.Message });
      }
    }

    // ✅ DELETE: /api/PaymentMethod/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var method = await _context.PaymentMethods.FindAsync(id);
        if (method == null)
          return NotFound(new { message = "Payment method not found." });

        _context.PaymentMethods.Remove(method);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return Ok(new { message = "Payment method deleted successfully." });
      }
      catch (Exception ex)
      {
        await transaction.RollbackAsync();
        return StatusCode(500, new { message = "Error deleting payment method", error = ex.Message });
      }
    }
  }
}
