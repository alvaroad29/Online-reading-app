using System;
using AutoMapper;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.Identity.Client;
namespace backend.Mapping;

public class ChapterProfile: Profile
{
    public ChapterProfile()
    {
        CreateMap<Chapter, ChapterReadDto>();
        CreateMap<Chapter, ChapterDto>();
    }
}