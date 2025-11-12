using Admin.DTOs.Users;
using Admin.Models;

namespace Admin.Interfaces
{
  public interface IUsersApiInterface
  {
    Task<PageUserListDto> GetUsersPagedAsync(
      string searchTerm = null!,
      int pagedNumber = 1,
      int pageSize = 10); 
    Task<List<UserManModel>> GetAllUsersAsync();
    Task UpdateUserAsync(UserManModel user);
    Task AddNewUserAsync(UserManModel user);
  }
}