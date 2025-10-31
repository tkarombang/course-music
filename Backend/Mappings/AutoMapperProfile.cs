using AutoMapper;
using Backend.Models;
using Backend.DTOs.Checkout;
using Backend.DTOs.Category;
using Backend.DTOs.Course;
using Backend.DTOs.User;
using Backend.DTOs.Payment;

namespace Backend.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // ================================
            // 🔹 MAPPING UNTUK CHECKOUT
            // ================================
            CreateMap<CheckoutModel, CheckoutDto>()
                .ForMember(dest => dest.NamaCourse, opt => opt.MapFrom(src => src.Course.NamaCourse))
                .ForMember(dest => dest.Harga, opt => opt.MapFrom(src => src.Course.Harga))
                .ForMember(dest => dest.Jadwal, opt => opt.MapFrom(src => src.Jadwal))
                .ForMember(dest => dest.Kategori, opt => opt.MapFrom(src =>
                src.Course.Category != null ? src.Course.Category.NameCategory : "(Tidak ada kategori)"))
                .ForMember(dest => dest.ImageCourse, opt => opt.MapFrom(src => src.Course.ImageCourse));

            CreateMap<CreateCheckoutDto, CheckoutModel>();

            // ================================
            // 🔹 MAPPING UNTUK COURSE
            // ================================
            CreateMap<DataCourseModel, CourseDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Deskripsi, opt => opt.MapFrom(src => src.Deskripsi));
            CreateMap<CourseDto, DataCourseModel>()
                .ForMember(dest => dest.Deskripsi, opt => opt.MapFrom(src => src.Deskripsi));

            // CategoryDto dasar
            CreateMap<CategoryModel, CategoryDto>().ReverseMap();

            // ================================
            // 🔹 MAPPING UNTUK CATEGORY DENGAN COURSES
            // ================================
            // Category → CategoryWithCoursesDto
            CreateMap<CategoryModel, CategoryWithCoursesDto>()
                .ForMember(dest => dest.IdCategory, opt => opt.MapFrom(src => src.IdCategory))
                .ForMember(dest => dest.NameCategory, opt => opt.MapFrom(src => src.NameCategory))
                .ForMember(dest => dest.ImageCategory, opt => opt.MapFrom(src => src.ImageCategory))
                .ForMember(dest => dest.Courses, opt => opt.MapFrom(src => src.Courses));

            // Course → CourseInCategoryDto
            CreateMap<DataCourseModel, CourseInCategoryDto>()
                .ForMember(dest => dest.IdCourse, opt => opt.MapFrom(src => src.IdCourse))
                .ForMember(dest => dest.NamaCourse, opt => opt.MapFrom(src => src.NamaCourse))
                .ForMember(dest => dest.Harga, opt => opt.MapFrom(src => src.Harga))
                .ForMember(dest => dest.ImageCourse, opt => opt.MapFrom(src => src.ImageCourse));

            // ================================
            // 🔹 MAPPING UNTUK USER
            // ================================
            CreateMap<UserModel, UserDto>();
            CreateMap<CreateUserDto, UserModel>();
            CreateMap<UpdateUserDto, UserModel>();

            // ================================
            // 🔹 MAPPING UNTUK PAYMENT
            // ================================
            CreateMap<CreatePaymentDto, PaymentModel>();
            CreateMap<PaymentModel, PaymentDto>()
                .ForMember(dest => dest.Created_At, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.PaymentMethodName, opt => opt.Ignore()); // diisi manual nanti
        }
    }
}
