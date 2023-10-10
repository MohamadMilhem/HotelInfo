using AutoMapper;
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
        }
    }
}
