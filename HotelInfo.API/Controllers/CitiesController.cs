using AutoMapper;
using HotelInfo.API.Entites;
using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly IHotelInfoRepository _hotelInfoRepository;
        private readonly IMapper _mapper;
        const int maxPageSize = 20;

        public CitiesController(IHotelInfoRepository hotelInfoRepository, IMapper mapper)
        {
            _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Retrieve a list of cities by searching for specific criteria.
        /// </summary>
        /// <param name="name">The name of the city or a part of it to search for.</param>
        /// <param name="searchQuery">A string to search for in the city descriptions.</param>
        /// <param name="pageSize">The number of results to display per page (default is 10).</param>
        /// <param name="pageNumber">The page number for paginated results (default is 1).</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing a collection of cities that match the specified criteria.
        /// </returns>
        /// <response code="200">Returns the matching cities when found.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CityWithoutHotels>>> GetCities(
            string? name, string? searchQuery, int pageSize = 10, int pageNumber = 1)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            IEnumerable<CityWithoutHotels> results = new List<CityWithoutHotels>()
            {
                new CityWithoutHotels()
                {
                    Id = 1,
                    Name = "Ramallah",
                    Description = "Ramallah is very beautiful city.",
                },
                new CityWithoutHotels()
                {
                    Id = 2,
                    Name = "Los Angeles",
                    Description = "Los Angeles, the entertainment capital of the world, sprawls across Southern California with its palm-lined boulevards and famous Hollywood sign. Home to the global film and television industry, LA boasts a diverse cultural scene, beautiful beaches, and iconic landmarks like the Hollywood Walk of Fame. This sprawling city embodies the American dream and remains a magnet for those seeking fame, fortune, and a sun-soaked lifestyle.",
                },
                new CityWithoutHotels()
                {
                    Id = 3,
                    Name = "New York",
                    Description = "New York, the iconic metropolis on the northeastern coast of the United States, is a global hub of finance, culture, and diversity. With its towering skyline, world-renowned landmarks like Times Square and Central Park, and a melting pot of cultures, New York City stands as a symbol of dynamism and opportunity.",
                },
                new CityWithoutHotels()
                {
                    Id = 4,
                    Name = "Denver",
                    Description = "Nestled at the foothills of the Rocky Mountains, Denver, the Mile-High City, is a vibrant metropolis known for its outdoor recreation, craft breweries, and cultural attractions. With a mix of modern skyscrapers and historic architecture, Denver offers a dynamic urban experience against the stunning backdrop of the nearby mountains, making it a gateway to both urban sophistication and natural beauty.",
                },
                new CityWithoutHotels()
                {
                    Id = 5,
                    Name = "San Francisco",
                    Description = "San Francisco, perched on the hilly terrain of Northern California, is renowned for its iconic Golden Gate Bridge, historic cable cars, and a rich tech culture in Silicon Valley's vicinity. This city blends Victorian charm with modern innovation, featuring diverse neighborhoods, the famous Alcatraz Island, and a picturesque waterfront. Known for its progressive values and cultural vibrancy, San Francisco stands as a symbol of creativity and innovation on the west coast.",
                },
                new CityWithoutHotels()
                {
                    Id = 6,
                    Name = "Paris",
                    Description = "Fall in love with the romantic charm of Paris, the 'City of Lights.' Admire the Eiffel Tower, stroll along the Seine River, and savor delicious pastries in charming cafes."
                },
                new CityWithoutHotels()
                {
                    Id = 7,
                    Name = "Tokyo",
                    Description = "Immerse yourself in the futuristic cityscape of Tokyo. Discover a perfect blend of traditional temples and modern technology. Experience vibrant street life and exquisite Japanese cuisine.",
                },
                new CityWithoutHotels()
                {
                    Id = 8,
                    Name = "Cape Town",
                    Description = "Enjoy the breathtaking landscapes of Cape Town. From the iconic Table Mountain to pristine beaches, experience the natural beauty and cultural richness of this South African gem.",
                }
            };
            /*
            var (cityEntities, paginationMetaData) = await _hotelInfoRepository.GetCitiesAsync(name, searchQuery, pageSize, pageNumber);
            var results = _mapper.Map<IEnumerable<CityWithoutHotels>>(cityEntities);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(results);
            */
            if (!string.IsNullOrWhiteSpace(name))
            {
                name = name.Trim();
                results = results.Where(c => c.Name.Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                results = results.Where(a => a.Name.Contains(searchQuery)
                                             || (!string.IsNullOrWhiteSpace(a.Description) &&
                                                 a.Description.Contains(searchQuery)));
            }

            //var itemsCount =  collection.Count();

            //var paginationMetaData = new PaginationMetaData(pageNumber, itemsCount, pageSize);

            var collectionToReturn = results
                .OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToList();

            return Ok(collectionToReturn);
        }
        /// <summary>
        /// Get detailed information about a city by its unique identifier.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city to retrieve.</param>
        /// <param name="includeHotels">Specify whether or not to include information about hotels in the city.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing the requested city's information or a not-found response if the city does not exist.
        /// </returns>
        /// <response code="200">Returns the requested city when found.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpGet("{cityId}", Name = "GetCity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCityAsync(int cityId, bool includeHotels = false)
        {
            var city = await _hotelInfoRepository.GetCityAsync(cityId, includeHotels);

            if (city == null)
            {
                return NotFound();
            }

            if (includeHotels)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutHotels>(city));

        }
        /// <summary>
        /// Create a new city.
        /// </summary>
        /// <param name="city">The data for creating the new city.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing the newly created city or a 201 Created response.
        /// </returns>
        /// <response code="201">Returns the newly created city when the operation is successful.</response>
        [HttpPost]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<CityDto>> CreateCity(CityForCreationDto city)
        {
            var cityToStore = _mapper.Map<Entites.City>(city);

            await _hotelInfoRepository.CreateCityAsync(cityToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var cityToReturn = _mapper.Map<CityDto>(cityToStore);

            return CreatedAtRoute("GetCity",
                new { cityId = cityToReturn.Id },
                cityToReturn);
        }
        /// <summary>
        /// Update an entire city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city to update.</param>
        /// <param name="cityForUpdateDto">The data for updating the city.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the update operation, which may be a 204 No Content response when successful.
        /// </returns>
        /// <response code="204">Indicates a successful update with no content returned.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpPut("{cityId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCity(int cityId, CityForUpdateDto cityForUpdateDto) 
        { 
            var cityEntity = await _hotelInfoRepository.GetCityAsync(cityId, false);

            if (cityEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(cityForUpdateDto, cityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();

        }
        /// <summary>
        /// Partially update a city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city to partially update.</param>
        /// <param name="patchDocument">The JSON patch document containing the changes to apply to the city.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the partial update operation. 
        /// This may include a 204 No Content response if successful, 400 Bad Request if the request is invalid, or 404 Not Found if the city does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful partial update with no content returned.</response>
        /// <response code="400">Indicates a bad request due to an invalid patch document or other errors.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpPatch("{cityId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PartiallyUpdateCity(int cityId,
            JsonPatchDocument<CityForUpdateDto> patchDocument)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var cityEntity = await _hotelInfoRepository.GetCityAsync(cityId, false);


            var cityToUpdate = _mapper.Map<CityForUpdateDto>(cityEntity);

            patchDocument.ApplyTo(cityToUpdate, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(cityToUpdate, cityEntity);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Delete a city and associated data.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation, which may include a 204 No Content response when successful, or a 404 Not Found response if the city does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpDelete("{cityId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCity(int cityId)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var city = await _hotelInfoRepository.GetCityAsync(cityId, false);


            _hotelInfoRepository.DeleteCity(city);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Retrieve a list of hotels in a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city for which you want to retrieve hotels.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of hotels in the specified city.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of hotels in the specified city.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpGet("{cityId}/hotels")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<HotelWithoutRooms>>> GetHotelsAsync(int cityId)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var hotels = await _hotelInfoRepository.GetHotelsAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<HotelWithoutRooms>>(hotels));

        }
        /// <summary>
        /// Create a new hotel in a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city where the hotel will be created.</param>
        /// <param name="hotelForCreationDto">The data for creating the hotel.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{cityId}/hotels")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<HotelDto>> CreateHotel(int cityId, HotelForCreationDto hotelForCreationDto)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var hotelToStore = _mapper.Map<Entites.Hotel>(hotelForCreationDto);

            await _hotelInfoRepository.CreateHotelAsync(cityId, hotelToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var hotelToReturn = _mapper.Map<HotelDto>(hotelToStore);

            return CreatedAtRoute("GetHotel",
                new { hotelId = hotelToReturn.Id },
                hotelToReturn);

        }
        /// <summary>
        /// Delete a hotel within a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city that contains the hotel.</param>
        /// <param name="hotelId">The unique identifier of the hotel to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the city or hotel does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified city or hotel was not found.</response>
        [HttpDelete("{cityId}/hotels/{hotelId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteHotel(int cityId, int hotelId)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var city = await _hotelInfoRepository.GetCityAsync(cityId, true);

            var hotelToDelete = city.Hotels
                .Where(hotel => hotel.Id == hotelId)
                .SingleOrDefault();

            if (hotelToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeleteHotel(hotelToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
        /// <summary>
        /// Retrieve a list of photos of a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city for which you want to retrieve photos.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a collection of photos in the specified city.
        /// </returns>
        /// <response code="200">Indicates a successful retrieval of photos of the specified city.</response>
        /// <response code="404">Indicates that the specified city was not found.</response>
        [HttpGet("{cityId}/photos")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosAsync(int cityId)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var photos = await _hotelInfoRepository.GetPhotosCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PhotoDto>>(photos));

        }
        /// <summary>
        /// Add a new photo to a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city where the photo will be added.</param>
        /// <param name="photoForCreationDto">The data for creating the photo.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.
        /// </returns>
        [HttpPost("{cityId}/photos")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PhotoDto>> AddPhoto(int cityId,  PhotoForCreationDto photoForCreationDto)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var photoToStore = _mapper.Map<Entites.Photo>(photoForCreationDto);

            await _hotelInfoRepository.AddPhotoToCity(cityId, photoToStore);
            await _hotelInfoRepository.SaveChangesAsync();

            var photoToReturn = _mapper.Map<PhotoDto>(photoToStore);

            return CreatedAtRoute("GetPhoto",
                new { photoId = photoToReturn.Id },
                photoToReturn);

        }
        /// <summary>
        /// Delete a photo for a specific city.
        /// </summary>
        /// <param name="cityId">The unique identifier of the city that the photo belongs to.</param>
        /// <param name="photoId">The unique identifier of the photo to be deleted.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> indicating the result of the deletion operation. This may include a 204 No Content response when successful, or a 404 Not Found response if the city or photo does not exist.
        /// </returns>
        /// <response code="204">Indicates a successful deletion with no content returned.</response>
        /// <response code="404">Indicates that the specified city or photo was not found.</response>
        [HttpDelete("{cityId}/photos/{photoId}")]
        [Authorize(policy:"Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeletePhoto(int cityId, int photoId)
        {
            if (!await _hotelInfoRepository.CityExistsAsync(cityId))
            {
                return NotFound();
            }

            var city = await _hotelInfoRepository.GetCityWithPhotosAsync(cityId);

            var photoToDelete = city.Photos
                .Where(photo => photo.Id == photoId)
                .SingleOrDefault();

            if (photoToDelete == null)
            {
                return NotFound();
            }

            _hotelInfoRepository.DeletePhoto(photoToDelete);

            await _hotelInfoRepository.SaveChangesAsync();

            return NoContent();
        }
    }






}
