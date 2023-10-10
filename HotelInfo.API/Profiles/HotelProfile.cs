using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles
{
    public class HotelProfile : Profile
    {
        public HotelProfile() 
        {
            CreateMap<Entites.Hotel, HotelDto>();
            CreateMap<HotelDto, Entites.Hotel>();
            CreateMap<Entites.Hotel, HotelWithoutRooms>();
            CreateMap<HotelWithoutRooms, Entites.Hotel>();
            CreateMap<Entites.Hotel, HotelForCreationDto>();
            CreateMap<HotelForCreationDto, Entites.Hotel>();
            CreateMap<Entites.Hotel, HotelForUpdateDto>();
            CreateMap<HotelForUpdateDto, Entites.Hotel>();
        }
    }
}
