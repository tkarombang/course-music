using AutoMapper;
using Backend.DTOs.Category;
using Backend.Models;

namespace Backend.Mappings
{
  public class CategoriesProfile : Profile
  {
    public CategoriesProfile()
    {
      CreateMap<CategoryModel, CategoryDto>()
        .ForMember(dest => dest.CountCourse, opt => opt.Ignore());
      
      CreateMap<CategoryCreateDto, CategoryModel>()
        .ForMember(dest => dest.IdCategory, opt => opt.Ignore())
        .ForMember(dest => dest.ImageCategory, opt => opt.Ignore())
        .ForMember(dest => dest.ImageBanner, opt => opt.Ignore())
        .ForMember(dest => dest.Courses, opt => opt.Ignore());

      CreateMap<CategoryUpdateDto, CategoryModel>()
        .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
  }
}