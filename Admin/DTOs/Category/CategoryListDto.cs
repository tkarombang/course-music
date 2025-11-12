using System.Text.Json.Serialization;

namespace Admin.DTOs.Category
{
  public class CategoryListDto
  {
    [JsonPropertyName("data")]
    public List<CategoryModel> Categories { get; set; } = [];

    [JsonPropertyName("pagination")]
    public PaginationData Pagination { get; set; } = new PaginationData();

    [JsonPropertyName("message")]
    public string? Message { get; set; }
  }

  public class PaginationData
  {
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }
  }
}