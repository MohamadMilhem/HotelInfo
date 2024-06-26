﻿using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile() 
        {
            CreateMap<Entites.City, CityDto>();
            CreateMap<CityDto, Entites.City>();
            CreateMap<Entites.City, CityWithoutHotels>();
            CreateMap<CityWithoutHotels, Entites.City>();
            CreateMap<Entites.City, CityForCreationDto>();
            CreateMap<CityForCreationDto, Entites.City>();
            CreateMap<Entites.City, CityForUpdateDto>();
            CreateMap<CityForUpdateDto, Entites.City>();
            
            CreateMap<Entites.City, CitySearchResult>()
                .ForMember(dest => dest.ThumbnailImage,
                    opt => opt.MapFrom(src => src.Photos.SingleOrDefault(photo => photo.Id == src.ThumbnailImageId)));
        }
    }
}
