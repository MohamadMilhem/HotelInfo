using HotelInfo.API.Entites;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles;

public class HotelAmenityProfile : Profile
{
    public HotelAmenityProfile()
    {
        CreateMap<Entites.HotelAmenity, HotelAmenityDto>();
        CreateMap<HotelAmenityDto, Entites.HotelAmenity>();
        CreateMap<Entites.HotelAmenity, HotelAmenityForCreationDto>();
        CreateMap<HotelAmenityForCreationDto, Entites.HotelAmenity>();
        CreateMap<Entites.HotelAmenity, HotelAmenityForUpdateDto>();
        CreateMap<HotelAmenityForUpdateDto, Entites.HotelAmenity>();
    }
}