using System;
using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.Identity.Client;
namespace backend.Mapping;

public class BookProfile: Profile
{
    public BookProfile()
    {
        CreateMap<Book, BookDto>()
        .ForMember(dest => dest.Genres, opt => opt.MapFrom(src => src.Genres))
        .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()))
        .ReverseMap();


        CreateMap<Book, CreateBookDto>().ReverseMap();
        // CreateMap<Book, UpdateBookDto>().ReverseMap();

        CreateMap<Book, BookDetailsDto>()
        .ForMember(dest => dest.Volumes, opt => opt.MapFrom(src =>
            src.Volumes
            .Where(v => v.IsActive)
            .OrderBy(v => v.Order)));

        CreateMap<BookRating, BookRatingResponseDto>()
           .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName));
            
        CreateMap<BookRating, BookRatingDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName));

        // Mapeo para UserBookRatingDto (calificaciones de un usuario)
        CreateMap<BookRating, UserBookRatingDto>()
            .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
            .ForMember(dest => dest.BookImageUrl, opt => opt.MapFrom(src => src.Book.ImageUrl));
    }
}