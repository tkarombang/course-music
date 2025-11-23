using AutoMapper;
using Backend.Data;
using Backend.DTOs;
using Backend.DTOs.User;
using Backend.Exceptions;
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


    public async Task<List<UserDto>> GetAllUsersAsync()
    {
      try
      {
        var users = await _context.Users.ToListAsync();
        if (users == null || users.Count == 0)
        {
          _logger.LogWarning("[SERVICE] DATA USERS KOSONG");
          return [];
        }

        return _mapper.Map<List<UserDto>>(users);
      }
      catch (Exception err)
      {
        _logger.LogError(err, "ERROR SAAT GET ALL USERR");
        throw new DatabaseException("GAGAL MENGAMBIL SEMUA DATA USER", err);
      }
    }





    public async Task<UserListDto> GetUsersPagedAsync(
      string? searchTerm = null,
      int pageNumber = 1,
      int pageSize = 10)
    {
      try
      {
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        IQueryable<UserModel> query = _context.Users;

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
      catch (Exception err)
      {
        _logger.LogError("ERROR [GET] USER: {errMessage}", err.Message);
        throw new DatabaseException("GAGAL MENAMPILKAN DATA  USER", err);
      }
    }










    public async Task<UserDto>? CreateUserAsync(CreateUserDto dto)
    {

      bool existingEmail = await _context.Users.AnyAsync(u => u.Email == dto.Email);
      if (existingEmail)
      {
        _logger.LogWarning("[SERVICE] ERROR CREATE USER: EMAIL ALREADY EXISTS {email}", dto.Email);
        throw new InvalidOperationException("EMAIL_SUDAH_TERDAFTAR");
      }

      if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
      {
        _logger.LogError("ERROR CREATE USER: MISSING REQUIRED FIELDS");
        throw new BadRequestException("USERNAME-EMAIL-PASSWORD HARUS TERISI ");
      }

      try
      {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        dto.Password = hashedPassword;

        var user = _mapper.Map<UserModel>(dto);
        user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("[SERVICE] USER BERHASIL DIBUAT: {email}", user.Email);

        return _mapper.Map<UserDto>(user);
      }
      catch (Exception err)
      {
        _logger.LogError("[SERVICE] ERROR CREATE USER: {errMessage}", err.Message);
        throw new DatabaseException("GAGAL MEMBUAT USER", err);
      }
    }















    public async Task<UserDto> GetUserByIdAsync(int id)
    {
      var user = await _context.Users.FindAsync(id);

      if (user == null)
      {
        _logger.LogWarning("USER TIDAK DI DAPATKAN DENGAN ID: {UserId}", id);
        throw new NotFoundException($"USER WITH ID: {id} NOT FOUND");
      }

      var userDto = _mapper.Map<UserDto>(user);
      return userDto;
    }










    public async Task<UserModel> UpdateUserAsync(int id, UpdateUserDto dto)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

      if (user == null)
      {
        _logger.LogWarning("UPDATE GAGAL: ID {UserId} NOT FOUND", id);
        throw new NotFoundException($"USER DENGAN ID: {id} TIDAK DITEMUKAN");
      }

      if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Username))
      {
        _logger.LogWarning("USERNAME DAN EMAIL TIDAK BOLEH KOSONG");
        throw new BadRequestException("USERNAME DAN EMAIL HARUS DIISI");
      }

      var emailUsed = await _context.Users.AnyAsync(u => u.Email == dto.Email && u.Id != id);
      if (emailUsed)
      {
        _logger.LogWarning("EMAIL SUDAH DIGUNAKAN {Email}", emailUsed);
        throw new BadRequestException("COBA GUNAKAN EMAIL YANG LAIN");
      }

      _mapper.Map(dto, user);
      user.CreatedAt = DateTime.UtcNow;

      try
      {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        var result = _mapper.Map<UserDto>(user);
        _logger.LogInformation("USER UPDATE SUCCESS: ID {UserId} ", id);
      }
      catch (Exception err)
      {
        _logger.LogError(err, "ERROR UPDATE USER ID: {UserId}", err.Message);
        throw new DatabaseException("GAGAL UPDATE USER.", err);
      }

      return user;
    }


  }
}