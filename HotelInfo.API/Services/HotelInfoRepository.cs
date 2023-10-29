using AutoMapper.Configuration.Conventions;
using CityInfo.API.Services;
using HotelInfo.API.DbContexts;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HotelInfo.API.Services
{
    public class HotelInfoRepository : IHotelInfoRepository
    {
        private readonly HotelInfoContext _hotelInfoContext;

        public HotelInfoRepository(HotelInfoContext hotelInfoContext)
        {
            _hotelInfoContext = hotelInfoContext ?? throw new ArgumentNullException(nameof(hotelInfoContext));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            var query = await _hotelInfoContext.Cities.OrderBy(city => city.Name).ToListAsync();
            return query;
        }

        public async Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery,
            int pageSize, int pageNumber)
        {
            var collection = _hotelInfoContext.Cities as IQueryable<City>;
            collection = collection.Include(city => city.Photos);Ho
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                                        || (!string.IsNullOrWhiteSpace(a.Description) &&
                                        a.Description.Contains(searchQuery)));
            }

            var itemsCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(pageNumber, itemsCount, pageSize);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }
        public async Task<(IEnumerable<Hotel>, PaginationMetaData)> GetHotelsAsync(string? name, string? searchQuery,
            int pageSize, int pageNumber)
        {
            var collection = _hotelInfoContext.Hotels as IQueryable<Hotel>;
            collection = collection.Include(hotel => hotel.Photos);
            
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(hotel => hotel.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(a => a.Name.Contains(searchQuery)
                                        || (!string.IsNullOrWhiteSpace(a.Description) &&
                                        a.Description.Contains(searchQuery)));
            }

            var itemsCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(pageNumber, itemsCount, pageSize);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }
        public async Task<(IEnumerable<HotelAmenity>, PaginationMetaData)> GetHotelAmenitiesAsync(string? name,
                int pageSize, int pageNumber)
        {
            var collection = _hotelInfoContext.HotelAmenities as IQueryable<HotelAmenity>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(amenity => amenity.Name.Contains(name));
            }

            var itemsCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(pageNumber, itemsCount, pageSize);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }
        public async Task<(IEnumerable<RoomAmenity>, PaginationMetaData)> GetRoomAmenitiesAsync(string? name,
            int pageSize, int pageNumber)
        {
            var collection = _hotelInfoContext.RoomAmenities as IQueryable<RoomAmenity>;

            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                collection = collection.Where(amenity => amenity.Name.Contains(name));
            }

            var itemsCount = await collection.CountAsync();

            var paginationMetaData = new PaginationMetaData(pageNumber, itemsCount, pageSize);

            var collectionToReturn = await collection
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetaData);
        }
        public async Task<IEnumerable<HotelAmenity>> GetHotelAmenitiesAsync(int hotelId)
        {
            var query = await _hotelInfoContext.Hotels.Include(hotel => hotel.HotelAmenities).SingleOrDefaultAsync(hotel => hotel.Id == hotelId);
            if (query != null)
            {
                return query.HotelAmenities;
            }
            return new List<HotelAmenity>();
        }
        public async Task<IEnumerable<RoomAmenity>> GetRoomAmenitiesAsync(int roomId)
        {
            var query = await _hotelInfoContext.Rooms.Include(room => room.RoomAmenities)
                .SingleOrDefaultAsync(room => room.Id == roomId);
            if (query != null)
            {
                return query.RoomAmenities;
            }
            return new List<RoomAmenity>();
        }
        public async Task<IEnumerable<Hotel>> GetHotelsAysnc(int cityId)
        {
            var query = await _hotelInfoContext.Hotels.Where(hotel => hotel.CityId == cityId).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Room>> GetRoomsAysnc(int hotelId)
        {
            var query = await _hotelInfoContext.Rooms.Where(room => room.HotelId == hotelId).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Photo>> GetPhotosCityAysnc(int cityId)
        {
            var query = await _hotelInfoContext.Photos.Where(photo => photo.CityId == cityId).ToListAsync();
            return query;
        }

        public async Task<IEnumerable<Photo>> GetPhotosHotelAysnc(int hotelId)
        {
            var query = await _hotelInfoContext.Photos.Where(photo => photo.HotelId == hotelId).ToListAsync();
            return query;
        }
        public async Task<IEnumerable<Photo>> GetPhotosRoomAysnc(int roomId)
        {
            var query = await _hotelInfoContext.Photos.Where(photo => photo.RoomId == roomId).ToListAsync();
            return query;
        }
        public async Task<City?> GetCityAsync(int cityId, bool includeHotels)
        {
            if (includeHotels)
            {
                return await _hotelInfoContext.Cities
                    .Include(c => c.Hotels)
                    .Where(c => c.Id == cityId)
                    .SingleOrDefaultAsync();
            }
            return await _hotelInfoContext.Cities
                .Where(c => c.Id == cityId)
                .SingleOrDefaultAsync();
        }

        public async Task<City?> GetCityWithPhotosAsync(int cityId)
        {
            return await _hotelInfoContext.Cities
                .Include(c => c.Photos)
                .Where(c => c.Id == cityId)
                .SingleOrDefaultAsync();
        }
        public async Task<Hotel?> GetHotelAsync(int hotelId, bool includeRooms)
        {
            if (includeRooms)
            {
                return await _hotelInfoContext.Hotels
                    .Include(c => c.Rooms)
                    .SingleOrDefaultAsync(hotel => hotel.Id == hotelId);
            }

            return await _hotelInfoContext.Hotels
                .SingleOrDefaultAsync(hotel => hotel.Id == hotelId);
        }

        public async Task<Hotel?> GetHotelWithHotelAmenitiesAsync(int hotelId)
        {
            var query = await _hotelInfoContext.Hotels.Include(hotel => hotel.HotelAmenities)
                .SingleOrDefaultAsync(hotel => hotel.Id == hotelId);
            return query;
        }
        public async Task<Room?> GetRoomWithRoomAmenitiesAsync(int roomId)
        {
            var query = await _hotelInfoContext.Rooms.Include(room => room.RoomAmenities)
                .SingleOrDefaultAsync(room => room.Id == roomId);
            return query;
        }
        public async Task<HotelAmenity?> GetHotelAmenityAsync(int hotelAmentiyId)
        {
            return await _hotelInfoContext.HotelAmenities
                .SingleOrDefaultAsync(hotelAmenity => hotelAmenity.Id == hotelAmentiyId);
        } 
        public async Task<RoomAmenity?> GetRoomAmenityAsync(int roomAmenityId)
        {
            return await _hotelInfoContext.RoomAmenities
                .SingleOrDefaultAsync(roomAmenity => roomAmenity.Id == roomAmenityId);
        } 
        public async Task<Hotel?> GetHotelWithPhotosAsync(int hotelId)
        {
            return await _hotelInfoContext.Hotels
                .Include(h => h.Photos)
                .Where(h => h.Id == hotelId)
                .SingleOrDefaultAsync();
        }
        public async Task<Room?> GetRoomAsync(int roomId)
        {
            return await _hotelInfoContext.Rooms.SingleOrDefaultAsync(room => room.Id == roomId);
        }
        public async Task<Room?> GetRoomWithPhotosAsync(int roomId)
        {
            return await _hotelInfoContext.Rooms
                .Include(r => r.Photos)
                .Where(r => r.Id == roomId)
                .SingleOrDefaultAsync();
        }
        public async Task<Photo?> GetPhotoAsync(int photoId)
        {
            return await _hotelInfoContext.Photos.SingleOrDefaultAsync(photo => photo.Id == photoId);
        }

        public async Task CreateCityAsync(City city)
        {
            await _hotelInfoContext.Cities.AddAsync(city);
        }

        public async Task CreateHotelAsync(int cityId, Hotel hotel)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.Hotels.Add(hotel);
            }
        }

        public async Task CreateRoom(int hotelId, Room room)
        {
            var hotel = await GetHotelAsync(hotelId, false);
            if (hotel != null)
            {
                hotel.Rooms.Add(room);
            }
        }
        public async Task AddPhotoToCity(int cityId, Photo photo) 
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.Photos.Add(photo);
            }
        }
        public async Task AddPhotoToHotel(int hotelId, Photo photo)
        {
            var hotel = await GetHotelAsync(hotelId, false);
            if (hotel != null)
            {
                hotel.Photos.Add(photo);
            }
        }

        public async Task AddPhotoToRoom(int roomId, Photo photo)
        {
            var room = await GetRoomAsync(roomId);
            if (room != null)
            {
                room.Photos.Add(photo);
            }
        }

        public async Task AddHotelAmenity(int hotelId, HotelAmenity hotelAmenity)
        {
            var hotel = await GetHotelAsync(hotelId, false);
            if (hotel != null)
            {
                hotel.HotelAmenities.Add(hotelAmenity);
            }
        }
        public async Task AddRoomAmenity(int roomId, RoomAmenity roomAmenity)
        {
            var room = await GetRoomAsync(roomId);
            if (room != null)
            {
                room.RoomAmenities.Add(roomAmenity);
            }
        }
        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _hotelInfoContext.Cities.AnyAsync(city => city.Id == cityId);
        }

        public async Task<bool> HotelExistsAsync(int hotelId)
        {
            return await _hotelInfoContext.Hotels.AnyAsync(hotel => hotel.Id == hotelId);
        }

        public async Task<bool> RoomExistsAsync(int roomId)
        {
            return await _hotelInfoContext.Rooms.AnyAsync(room => room.Id == roomId);
        }
        public async Task<bool> PhotoExistsAsync(int photoId)
        {
            return await _hotelInfoContext.Photos.AnyAsync(photo => photo.Id == photoId);
        }

        public void DeleteCity(City city)
        {
            _hotelInfoContext.Cities.Remove(city);
        }

        public void DeleteHotel(Hotel hotel)
        {
            _hotelInfoContext.Hotels.Remove(hotel);
        }

        public void DeleteRoom(Room room)
        {
            _hotelInfoContext.Rooms.Remove(room);
        }
        public void DeletePhoto(Photo photo)
        {
            _hotelInfoContext.Photos.Remove(photo);
        }

        public async void DeleteHotelAmenity(int hotelId, HotelAmenity hotelAmenity)
        {
            var hotel = await _hotelInfoContext.Hotels.SingleOrDefaultAsync(hotel => hotel.Id == hotelId);
            if (hotel != null)
            {
                hotel.HotelAmenities.Remove(hotelAmenity);
            }
        }
        public async void DeleteRoomAmenity(int roomId, RoomAmenity roomAmenity)
        {
            var room = await _hotelInfoContext.Rooms.SingleOrDefaultAsync(room => room.Id == roomId);
            if (room != null)
            {
                room.RoomAmenities.Remove(roomAmenity);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _hotelInfoContext.SaveChangesAsync() > 0);
        }

    }
}
