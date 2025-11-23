using Backend.DTOs;
using Backend.DTOs.User;
using Backend.Models;

namespace Backend.Interface
{
  public interface IUsersInterface
  {
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserListDto> GetUsersPagedAsync(
      string? searchTerm = null,
      int pageNumber = 1, 
      int pageSize = 10
      );

    Task<UserDto>? CreateUserAsync(CreateUserDto dto);

    Task<UserDto> GetUserByIdAsync(int id);

    Task<UserModel> UpdateUserAsync(int id, UpdateUserDto dto);

  }
}