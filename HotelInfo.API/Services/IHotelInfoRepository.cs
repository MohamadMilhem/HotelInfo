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
        void DeleteHotelAmenity(int hotelId, HotelAmenity hotelAmenity);
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
        Task<IEnumerable<Photo>> GetPhotosCityAysnc(int cityId);
        Task<IEnumerable<Photo>> GetPhotosHotelAysnc(int hotelId);
        Task<IEnumerable<Photo>> GetPhotosRoomAysnc(int roomId);
        Task<City?> GetCityWithPhotosAsync(int cityId);
        Task<Hotel?> GetHotelWithPhotosAsync(int hotelId);
        Task<Room?> GetRoomWithPhotosAsync(int roomId);
        Task<Hotel?> GetHotelWithHotelAmenitiesAsync(int hotelId);
        Task<Photo?> GetPhotoAsync(int photoId);
        Task<HotelAmenity?> GetHotelAmenityAsync(int hotelAmentiyId);
        Task AddPhotoToCity(int cityId, Photo photo);
        Task AddPhotoToHotel(int hotelId, Photo photo);
        Task AddPhotoToRoom(int roomId, Photo photo);
        Task AddHotelAmenity(int hotelId, HotelAmenity hotelAmenity);
        Task<(IEnumerable<HotelAmenity>, PaginationMetaData)> GetHotelAmenitiesAsync(string? name,
                int pageSize, int pageNumber);

        Task<(IEnumerable<RoomAmenity>, PaginationMetaData)> GetRoomAmenitiesAsync(string? name,
            int pageSize, int pageNumber);
        Task<IEnumerable<HotelAmenity>> GetHotelAmenitiesAsync(int hotelId);
        public void DeletePhoto(Photo photo);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<RoomAmenity>> GetRoomAmenitiesAsync(int roomId);
        Task<Room?> GetRoomWithRoomAmenitiesAsync(int roomId);
        Task<RoomAmenity?> GetRoomAmenityAsync(int roomAmenityId);
        Task AddRoomAmenity(int roomId, RoomAmenity roomAmenity);
        Task<bool> PhotoExistsAsync(int photoId);
        void DeleteRoomAmenity(int roomId, RoomAmenity roomAmenity);
    }
}