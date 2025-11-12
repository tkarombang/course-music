using System.Text.Json.Serialization;
using Admin.Models;

namespace Admin.DTOs.Users
{
  public class PageUserListDto
  {
    [JsonPropertyName("data")]
    public List<UserManModel> Users { get; set; } = [];

    [JsonPropertyName("pagination")]
    public PaginationDto Pagination {get;set;} = new PaginationDto();
  }

  public class PaginationDto
  {
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("pagedNumber")]
    public int PagedNumber { get; set; }
  }
}