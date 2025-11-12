namespace Backend.DTOs.Category
{
  public class CategoryListDto
  {
    public List<CategoryDto> Categories {get;set;} = new List<CategoryDto>();
    public int TotalCount{get;set;}
    public int PageSize{get;set;}
    public int CurrentPage{get;set;}
  }
}