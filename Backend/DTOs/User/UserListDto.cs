namespace Backend.DTOs.User
{
  public class UserListDto
  {
    public List<UserDto> Users {get;set;} = new List<UserDto>();
    public int TotalCount{get;set;}
    public int PagedNumber{get;set;}
    public int PageSize{get;set;}
  }
}