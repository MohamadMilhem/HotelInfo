using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles
{
    public class PhotoProfile : Profile
    {
        public PhotoProfile()
        {
            CreateMap<Entites.Photo, PhotoDto>();
            CreateMap<PhotoDto, Entites.Photo>();
            CreateMap<Entites.Photo, PhotoForCreationDto>();
            CreateMap<PhotoForCreationDto, Entites.Photo>();
            CreateMap<Entites.Photo, PhotoForUpdateDto>();
            CreateMap<PhotoForUpdateDto, Entites.Photo>();
        }
    }
}
