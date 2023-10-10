using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Entites.Room, RoomDto>();
            CreateMap<RoomDto, Entites.Room>();
            CreateMap<Entites.Room, RoomForUpdate>();
            CreateMap<RoomForUpdate, Entites.Room>();
        }
    }
}
