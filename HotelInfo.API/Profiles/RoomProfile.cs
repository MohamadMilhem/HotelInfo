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
            CreateMap<Entites.Room, RoomForUpdateDto>();
            CreateMap<RoomForUpdateDto, Entites.Room>();
            CreateMap<Entites.Room, RoomForCreationDto>();
            CreateMap<RoomForCreationDto, Entites.Room>();
        }
    }
}
