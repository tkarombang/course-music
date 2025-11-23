using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs.User;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Backend.DTOs;
using Backend.Interface;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly ILogger<UserController> _logger;
		private readonly IMapper _mapper;
		private readonly IUsersInterface _userService;

		public UserController(
			AppDbContext context,
			ILogger<UserController> logger,
			IMapper mapper,
			IUsersInterface usersService
			)
		{
			_context = context;
			_logger = logger;
			_mapper = mapper;
			_userService = usersService;
		}

		// ✅ GET: api/user
		[HttpGet]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetAllUsers()
		{
			_logger.LogInformation("[GET ALL] api/user DIPANGGIL");
			var users = _userService.GetAllUsersAsync();
			return Ok(new
			{
				message = "BERHASIL MENDAPATKAN SEMUA USERS",
				data = users
			});
		}





		[HttpGet("paged")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetUserPaged(
				[FromQuery] string? searchTerm,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10)
		{
			_logger.LogInformation("[GET/PAGED] /api/users/paged DIPANGGIL page {pageNumber}", pageNumber);

			var result = await _userService.GetUsersPagedAsync(searchTerm, pageNumber, pageSize);

			return Ok(new
			{
				message = "BERHASIL MENDAPATKAN DAFTAR USER",
				data = result.Users,
				pagination = new
				{
					result.TotalCount,
					result.PageSize,
					result.PagedNumber
				}
			});
		}





		// ✅ POST: api/user
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
		{
			_logger.LogInformation("[POST] /api/users DIPANGGIL DENGAN EMAIL {email}", dto.Email);

			if (!ModelState.IsValid)
			{
				return BadRequest(new { message = "INPUT TIDAK VALID", error = ModelState });
			}

			var createUser = await _userService.CreateUserAsync(dto)!;

			var response = new
			{
				message = "BERHASIL MEMBUAT USER",
				data = new
				{
					createUser.Id,
					createUser.Email,
					createUser.Username,
					createUser.CreatedAt
				}
			};

			return Ok(response);
		}





		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetUserById(int id)
		{
			_logger.LogInformation("[GET] api/get/id DIPANGGIL dengan ID: {id}", id);
			var user = await _userService.GetUserByIdAsync(id);

			return Ok(new
			{
				message = "SUCCESS MENDAPATKAN USER",
				data = user
			});
		}










		// ✅ PUT: api/user/{id}
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, User")]
		public IActionResult Update(int id, UpdateUserDto dto)
		{
			_logger.LogInformation("[PUT] api/user/{id}", id);
			if (!ModelState.IsValid)
			{
				_logger.LogWarning("[PUT] INPUTAN INVALID {dto}", dto);
				return BadRequest(new { message = "INPUT TIDAK VALID", errors = ModelState });
			}

			var updateUser = _userService.UpdateUserAsync(id, dto);

			return Ok(new
			{
				message = "USER UPDATE SUCCESSFULLY",
				data = updateUser
			});
		}





		// // ✅ DELETE: api/user/{id}
		// [HttpDelete("{id}")]
		// [Authorize(Roles = "Admin")]
		// public IActionResult Delete(int id)
		// {
		// 	try
		// 	{
		// 		var user = _context.Users.FirstOrDefault(u => u.Id == id);
		// 		if (user == null)
		// 			return NotFound(new { message = "User not found" });

		// 		_context.Users.Remove(user);
		// 		_context.SaveChanges();

		// 		return Ok(new { message = "User deleted successfully" });
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		_logger.LogError(ex, "Error occurred while deleting user");
		// 		return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
		// 	}
		// }
	}
}
