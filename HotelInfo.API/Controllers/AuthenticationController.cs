using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IHotelInfoRepository _hotelInfoRepository;  
    public AuthenticationController(IHotelInfoRepository hotelInfoRepository)
    {
        _hotelInfoRepository = hotelInfoRepository ?? throw new ArgumentNullException(nameof(hotelInfoRepository));
    }
    
    
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest("Username and password are required.");
        }
        if (request.Username.ToLower() == "user" || request.Username.ToLower() == "admin")
        {
            string authToken = $"your_generated_token_for_{request.Username}";
            string userType = request.Username.ToLower() == "admin" ? "Admin" : "User";
            //Response.Headers.Add("Authorization", $"Bearer {authToken}");
            var response = new { UserType = userType, Authorization = $"Bearer {authToken}" };
            return Ok(response);
        }
        return Unauthorized("Invalid username or password.");
    }


    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    
}