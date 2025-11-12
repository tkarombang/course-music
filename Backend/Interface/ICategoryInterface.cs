using Backend.DTOs.Category;

namespace Backend.Interface
{
  public interface ICategoryInterface
  {
    Task<CategoryDto> CreateAsync(CategoryCreateDto dto); 
    Task<CategoryDto> GetByIdAsync(int id);
    Task<CategoryDto> GetByNameAsync(string nama);
    Task<CategoryListDto> GetCategPagedAsync(string searchTerm = null!, int pageNumber = 1, int pageSize = 5);
    Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryUpdateDto dto);
    Task<bool> DeleteCategoryAsync(int id);
  }
}