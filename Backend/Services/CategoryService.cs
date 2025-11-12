using AutoMapper;
using Backend.Data;
using Backend.DTOs.Category;
using Backend.Interface;
using Backend.Models;
using Microsoft.EntityFrameworkCore;


namespace Backend.Services
{
  public class CategoryService : ICategoryInterface
  {
    private readonly AppDbContext _context;
    private readonly IFileUploadInterface _uploadService;
    private readonly IMapper _mapper;

    public CategoryService(AppDbContext context, IFileUploadInterface uploadInterface, IMapper mapper)
    {
      _context = context;
      _uploadService = uploadInterface;
      _mapper = mapper;
    }

    public async Task<CategoryDto> CreateAsync(CategoryCreateDto dto)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var category = _mapper.Map<CategoryModel>(dto);

        if (dto.ImageCategory != null)
        {
          category.ImageCategory = await _uploadService.UploadAsync(dto.ImageCategory, "list_category", "cat");
          if (category.ImageCategory == null)
            throw new InvalidOperationException("FORMAT ATAU UKURAN FILE GAMBAR KATEGORI TIDAK VALID");
        }

        if (dto.ImageBanner != null)
        {
          category.ImageBanner = await _uploadService.UploadAsync(dto.ImageBanner, "list_category", "bann");
          if (category.ImageBanner == null)
            throw new InvalidOperationException("FORMAT/UKURAN FILE GAMBAR BANNER TIDAK VALID");
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var response = _mapper.Map<CategoryDto>(category);
        response.CountCourse = await _context.Courses.CountAsync(c => c.IdCategory == category.IdCategory);
        return response;

      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
      var category = await _context.Categories
        .Include(c => c.Courses)
        .FirstOrDefaultAsync(c => c.IdCategory == id);

      if (category == null) return null!;

      var dto = _mapper.Map<CategoryDto>(category);
      dto.CountCourse = category.Courses?.Count ?? 0;
      return dto;
    }

    public async Task<CategoryDto> GetByNameAsync(string nama)
    {
      var category = await _context.Categories
        .Include(c => c.Courses)
        .FirstOrDefaultAsync(c => c.NameCategory.ToLower() == nama.ToLower());

      if (category == null) return null!;

      var dto = _mapper.Map<CategoryDto>(category);
      dto.CountCourse = category.Courses?.Count ?? 0;
      return dto;
    }

    public async Task<CategoryListDto> GetCategPagedAsync(
      string? searchTerm = null,
      int pageNumber = 1,
      int pageSize = 5)
    {
      try
      {
        IQueryable<CategoryModel> query = _context.Categories;
        // var query = _context.Categories
        //   .Include(c => c.Courses)
        //   .OrderBy(c => c.IdCategory);

        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 5;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
          query = query.Where(c =>
            c.NameCategory.Contains(searchTerm) ||
            c.Courses!.Select(nm => nm.NamaCourse).Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        var categories = await query
          .Skip((pageNumber - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

        var categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

        // categoryDtos.ForEach(d => d.CountCourse = _context.Courses.Count(c => c.IdCategory == d.IdCategory));

        return new CategoryListDto
        {
          Categories = categoryDtos,
          TotalCount = totalCount,
          CurrentPage = pageNumber,
          PageSize = pageSize,
        };
      }
      catch (System.Exception)
      {
        throw;
      }
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, CategoryUpdateDto dto)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null!;

        if (!string.IsNullOrEmpty(dto.NameCategory)) category.NameCategory = dto.NameCategory;

        if (!string.IsNullOrEmpty(dto.Deskripsi)) category.Deskripsi = dto.Deskripsi;

        if (dto.ImageCategory != null)
        {
          if (!string.IsNullOrEmpty(category.ImageCategory))
            _uploadService.DeleteFile(category.ImageCategory);

          var newUrl = await _uploadService.UploadAsync(dto.ImageCategory, "list_category", "cat");
          if (newUrl == null) throw new InvalidOperationException("FORMAT/URUKRAN FILE GAMBAR KATEGORI TIDAK VALID");

          category.ImageCategory = newUrl;
        }

        if (dto.ImageBanner != null)
        {
          if (!string.IsNullOrEmpty(category.ImageCategory))
            _uploadService.DeleteFile(category.ImageBanner);

          var newUrl = await _uploadService.UploadAsync(dto.ImageBanner, "list_category", "bann");
          if (newUrl == null) throw new InvalidOperationException("FORMAT/UKURAN FILE GAMBAR BANNER TIDAK VALID");

          category.ImageBanner = newUrl;
        }

        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var response = _mapper.Map<CategoryDto>(category);
        response.CountCourse = await _context.Courses.CountAsync(c => c.IdCategory == category.IdCategory);

        return response;
      }
      catch (System.Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
      await using var transaction = await _context.Database.BeginTransactionAsync();
      try
      {
        var category = await _context.Categories
          .Include(c => c.Courses)
          .FirstOrDefaultAsync(c => c.IdCategory == id);

        if (category == null) return false;

        if (category.Courses?.Any() == true)
          throw new InvalidOperationException("KATEGORI TIDAK BISA DI HAPUS KARENA MEMILIKI COURSE TERKAIT");

        if (!string.IsNullOrEmpty(category.ImageCategory))
          _uploadService.DeleteFile(category.ImageCategory);

        if (!string.IsNullOrEmpty(category.ImageBanner))
          _uploadService.DeleteFile(category.ImageBanner);

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
      }
      catch
      {
        await transaction.RollbackAsync();
        throw;
      }
    }


  }
}