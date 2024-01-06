using AutoMapper;
using HotelInfo.API.Models;

namespace HotelInfo.API.Profiles;

public class RoomAmenityProfile : Profile
{
    public RoomAmenityProfile()
    {
        CreateMap<Entites.RoomAmenity, RoomAmenityDto>();
        CreateMap<RoomAmenityDto, Entites.RoomAmenity>();
        CreateMap<Entites.RoomAmenity, RoomAmenityForCreationDto>();
        CreateMap<RoomAmenityForCreationDto, Entites.RoomAmenity>();
        CreateMap<Entites.RoomAmenity, RoomAmenityForUpdateDto>();
        CreateMap<RoomAmenityForUpdateDto, Entites.RoomAmenity>();
    }
}