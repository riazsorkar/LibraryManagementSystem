using AutoMapper;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using static LibraryManagementSystem.DTOs.AuthorDTOs;
using static LibraryManagementSystem.DTOs.BookDTOs;
using static LibraryManagementSystem.DTOs.CategoryDTOs;

namespace LibraryManagementSystem.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book Mappings
            CreateMap<Book, BookResponseDTO>()
                .ForMember(dest => dest.AuthorName,
                    opt => opt.MapFrom(src => src.Author.Name))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<BookCreateDTO, Book>();

            // Author Mappings
            CreateMap<Author, AuthorResponseDTO>();
            CreateMap<AuthorCreateDTO, Author>();

            // Category Mappings
            CreateMap<Category, CategoryResponseDTO>();
            CreateMap<CategoryCreateDTO, Category>();
        }
    }
}