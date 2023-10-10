using CityInfo.API.Services;
using HotelInfo.API.DbContexts;
using HotelInfo.API.Entites;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Room?> GetRoomAsync(int roomId)
        {
            return await _hotelInfoContext.Rooms.SingleOrDefaultAsync(room => room.Id == roomId);
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

        public async Task<bool> SaveChangesAsync()
        {
            return (await _hotelInfoContext.SaveChangesAsync() > 0);
        }

    }
}
