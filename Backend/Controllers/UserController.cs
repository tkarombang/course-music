using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs.User;
using Backend.Data;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Backend.DTOs;
using Backend.Interface;

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
		// public UserController(AppDbContext context, ILogger<UserController> logger, IMapper mapper )
		{
			_context = context;
			_logger = logger;
			_mapper = mapper;
			_userService = usersService;
		}

		// ✅ GET: api/user
		[HttpGet]
		[Authorize(Roles = "Admin, User")]
		public IActionResult GetAllUsers()
		{
			var users = _context.Users.ToList();

			// Gunakan AutoMapper untuk mapping list UserModel ke UserDto
			var userDtos = _mapper.Map<List<UserDto>>(users);

			return Ok(userDtos);
		}

		[HttpGet("paged")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetUserPaged(
				[FromQuery] string? searchTerm,
				[FromQuery] int pageNumber = 1,
				[FromQuery] int pageSize = 10)
		{
			// IQueryable<UserModel> query = _context.Users;

			// if (!string.IsNullOrWhiteSpace(searchTerm))
			// {
			// 	query = query.Where(u =>
			// 	u.Username.Contains(searchTerm) ||
			// 	u.Email.Contains(searchTerm));
			// }

			// var totalCount = query.Count();

			// var users = query
			// 		.Skip((pageNumber - 1) * pageSize)
			// 		.Take(pageSize)
			// 		.ToList();

			// var userDtos = _mapper.Map<List<UserDto>>(users);

			// var result = new PagedResult<UserDto>
			// {
			// 	Items = userDtos,
			// 	TotalCount = totalCount,
			// 	PageNumber = pageNumber,
			// 	PageSize = pageSize
			// };

			// return Ok(result);
			// if (pageNumber <= 0) pageNumber = 1;
			// if (pageSize <= 0) pageSize = 10;
			// if (!string.IsNullOrWhiteSpace(searchTerm))
			// {
			// 	searchTerm = searchTerm.Trim();
			// }


			try
			{
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
			catch (Exception err)
			{
				_logger.LogError("ERROR GET USERS PAGED: {errMessage}", err.Message);
				return StatusCode(500, new
				{
					message = "ERROR GETTING PAGED USERS",
					error = err.Message
				});
			}
		}

		[HttpGet("{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetUserById(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
				return NotFound(new { message = "User not found." });

			return Ok(new
			{
				Id = user.Id,
				Username = user.Username,
				Email = user.Email,
				Role = user.Role
			});
		}


		// ✅ POST: api/user
		// [HttpPost]
		// [Authorize(Roles = "Admin")]
		// public async Task<IActionResult> Create(CreateUserDto dto)
		// {
		// 	try
		// 	{
		// 		var createdUser = await _userService.CreateUserAsync(dto)!;
		// 		var userDto = _mapper.Map<UserDto>(createdUser);
		// 		return Ok(new { message = "USER CREATED SUCCESSFULLY", data = userDto });
		// 	}
		// 	catch (Exception err)
		// 	{
		// 		_logger.LogError("ERROR CREATE USER: {errMessage}", err.Message);
		// 		return StatusCode(500, new
		// 		{
		// 			message = "ERROR CREATING USER",
		// 			error = err.Message
		// 		});
		// 	}
		// }
		// [HttpPost]
		// [Authorize(Roles = "Admin")]
		// public IActionResult Create(CreateUserDto dto)
		// {
		// 	try
		// 	{
		// 		if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
		// 			return BadRequest(new { message = "Username and Password are required" });

		// 		var existingEmail = _context.Users.Any(u => u.Email == dto.Email);
		// 		if (existingEmail)
		// 			return BadRequest(new { message = "Email already registered" });

		// 		using var transaction = _context.Database.BeginTransaction();

		// 		var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

		// 		// Mapping DTO ke Model
		// 		var newUser = _mapper.Map<UserModel>(dto);
		// 		newUser.CreatedAt = DateTime.UtcNow;
		// 		newUser.Password = hashedPassword;

		// 		_context.Users.Add(newUser);
		// 		_context.SaveChanges();

		// 		transaction.Commit();

		// 		var userDto = _mapper.Map<UserDto>(newUser);
		// 		return Ok(new { message = "User created successfully", data = userDto });
		// 	}
		// 	catch (Exception ex)
		// 	{
		// 		_logger.LogError(ex, "Error occurred while creating user");
		// 		return StatusCode(500, new { message = "Error creating user", error = ex.Message });
		// 	}
		// }

		// ✅ PUT: api/user/{id}
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin, User")]
		public IActionResult Update(int id, UpdateUserDto dto)
		{
			try
			{
				var user = _context.Users.FirstOrDefault(u => u.Id == id);
				if (user == null)
					return NotFound(new { message = "User not found" });

				using var transaction = _context.Database.BeginTransaction();

				// Mapping DTO ke entitas yang sudah ada
				_mapper.Map(dto, user);

				_context.Users.Update(user);
				_context.SaveChanges();

				transaction.Commit();

				var updatedUserDto = _mapper.Map<UserDto>(user);
				return Ok(new { message = "User updated successfully", data = updatedUserDto });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating user");
				return StatusCode(500, new { message = "Error updating user", error = ex.Message });
			}
		}

		// ✅ DELETE: api/user/{id}
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
		{
			try
			{
				var user = _context.Users.FirstOrDefault(u => u.Id == id);
				if (user == null)
					return NotFound(new { message = "User not found" });

				_context.Users.Remove(user);
				_context.SaveChanges();

				return Ok(new { message = "User deleted successfully" });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting user");
				return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
			}
		}
	}
}
