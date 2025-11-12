using Backend.DTOs;
using Backend.DTOs.User;
using Backend.Models;

namespace Backend.Interface
{
  public interface IUsersInterface
  {
    Task<UserListDto> GetUsersPagedAsync(
      string? searchTerm = null,
      int pageNumber = 1, 
      int pageSize = 10
      );

    Task<UserModel>? CreateUserAsync(CreateUserDto dto);
  }
}