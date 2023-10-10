using CityInfo.API.Services;
using HotelInfo.API.Entites;

namespace HotelInfo.API.Services
{
    public interface IHotelInfoRepository
    {
        Task<bool> CityExistsAsync(int cityId);
        Task CreateCityAsync(City city);
        Task CreateHotelAsync(int cityId, Hotel hotel);
        Task CreateRoom(int hotelId, Room room);
        void DeleteCity(City city);
        void DeleteHotel(Hotel hotel);
        void DeleteRoom(Room room);
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageSize, int pageNumber);
        Task<City?> GetCityAsync(int cityId, bool includeHotels);
        Task<Hotel?> GetHotelAsync(int hotelId, bool includeRooms);
        Task<(IEnumerable<Hotel>, PaginationMetaData)> GetHotelsAsync(string? name, string? searchQuery, int pageSize, int pageNumber);
        Task<IEnumerable<Hotel>> GetHotelsAysnc(int cityId);
        Task<Room?> GetRoomAsync(int roomId);
        Task<IEnumerable<Room>> GetRoomsAysnc(int hotelId);
        Task<bool> HotelExistsAsync(int hotelId);
        Task<bool> RoomExistsAsync(int roomId);
        Task<bool> SaveChangesAsync();
    }
}