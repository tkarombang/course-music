using AutoMapper;
using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.User;
using Backend.Interface;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
  public class UsersService : IUsersInterface
  {
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersService> _logger;

    public UsersService(AppDbContext context, IMapper mapper, ILogger<UsersService> logger)
    {
      _context = context;
      _mapper = mapper;
      _logger = logger;
    }

    public async Task<UserListDto> GetUsersPagedAsync(
      string? searchTerm = null,
      int pageNumber = 1,
      int pageSize = 10)
    {
      try
      {
        IQueryable<UserModel> query = _context.Users;
        
        if(pageNumber <= 0) pageNumber = 1;
        if(pageSize <= 0) pageSize = 10;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
          query = query.Where(u =>
            u.Username.Contains(searchTerm) ||
            u.Email.Contains(searchTerm));
        }
        var totalCount = await query.CountAsync();
        var users = await query
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        var userDtos = _mapper.Map<List<UserDto>>(users);

        var result = new UserListDto
        {
          Users = userDtos,
          TotalCount = totalCount,
          PagedNumber = pageNumber,
          PageSize = pageSize
        };
        return result;
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public async Task<UserModel>? CreateUserAsync(CreateUserDto dto)
    {
      try
      {
        if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
        {
          _logger.LogError("ERROR CREATE USER: MISSING REQUIRED FIELDS");
          return null!;
        }

        var existingEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (existingEmail)
        {
          _logger.LogError("ERROR CREATE USER: EMAIL ALREADY EXISTS");
          return null!;
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        dto.Password = hashedPassword;

        var user = _mapper.Map<UserModel>(dto);
        user.CreatedAt = DateTime.UtcNow;
        user.Password = hashedPassword;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
      }
      catch (Exception err)
      {
        _logger.LogError("ERROR CREATE USER: {errMessage}", err.Message);
        throw;
      }
    }
  }
}