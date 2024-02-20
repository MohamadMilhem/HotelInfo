using HotelInfo.API.Models;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HotelInfo.API.Controllers;

[ApiController]
[Route("api/search-results")]
public class SearchResultsPageController : ControllerBase
{
    private readonly IHotelInfoRepository _hotelInfoRepository;
    
    public SearchResultsPageController(IHotelInfoRepository hotelInfoRepository)
    {
        _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
    }
    
    [HttpGet("amenities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FilterAmenityDto>>> GetAmenities()
    {
        var result = new List<FilterAmenityDto>()
        {
            new FilterAmenityDto()
            {
                Name = "Free Wi-Fi",
                Description = "Stay connected with complimentary high-speed Wi-Fi available in all rooms."
            },
            new FilterAmenityDto()
            {
                Name = "Air Conditioning",
                Description = "Enjoy a comfortable stay with individually controlled air conditioning in every room."
            },
            new FilterAmenityDto()
            {
                Name = "Mini Bar",
                Description = "Unwind with a selection of beverages and snacks from the in-room mini bar."
            },
            new FilterAmenityDto()
            {
                Name = "Flat-screen TV",
                Description = "Relax and enjoy your favorite shows or movies on a modern flat-screen TV."
            },
            new FilterAmenityDto()
            {
                Name = "Private Balcony",
                Description = "Take in breathtaking views from your private balcony, available in select rooms."
            }
        };

        return Ok(result);
    }
}