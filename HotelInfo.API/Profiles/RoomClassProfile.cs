using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles;

public class RoomClassProfile : Profile
{
    public RoomClassProfile()
    {
        CreateMap<Entites.RoomClass, RoomClassDto>();
        CreateMap<RoomClassDto, Entites.RoomClass>();
        CreateMap<Entites.RoomClass, RoomClassForCreationDto>();
        CreateMap<RoomClassForCreationDto, Entites.RoomClass>();
        CreateMap<Entites.RoomClass, RoomClassForUpdateDto>();
        CreateMap<RoomClassForUpdateDto, Entites.RoomClass>();
    }
}