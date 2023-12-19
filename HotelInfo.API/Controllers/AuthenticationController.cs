using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HotelInfo.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public class AuthenticationRequestBody
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }

    public class HotelInfoUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserType { get; set; }

        public HotelInfoUser(int userId,
            string userName,
            string firstName,
            string lastName,
            string userType)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
        }
    }
    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [HttpPost("authenticate")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
    {
        var user = ValidateCredentials(authenticationRequestBody.UserName, authenticationRequestBody.Password);

        if (user == null)
        {
            return Unauthorized();
        }
        var securityKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes("thisisthesecretforgeneratingakey(mustbeatleast32bitlong)"));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenClaims = new List<Claim>();
        tokenClaims.Add(new Claim("user_id", user.UserId.ToString()));
        tokenClaims.Add(new Claim("given_name", user.FirstName));
        tokenClaims.Add(new Claim("family_name", user.LastName));
        tokenClaims.Add(new Claim("userType", user.UserType));

        var jwtSecurityToken = new JwtSecurityToken("https://app-hotel-reservation-webapi-uae-dev-001.azurewebsites.net",
            null, tokenClaims, DateTime.UtcNow, DateTime.UtcNow.AddHours(1),
            signingCredentials);

        var tokenToReturn = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        var result = new
        {
            UserType = user.UserType,
            Authentication = tokenToReturn
        };

        return Ok(result);
    }

    private HotelInfoUser ValidateCredentials(string? userName, string? password)
    {
        if (userName == "admin" && password == "admin")
            return new HotelInfoUser(1, userName ?? "", "Mohamad", "Milhem", "Admin");
        if (userName == "user" && password == "user")
            return new HotelInfoUser(2, userName ?? "", "Mazen", "Sami", "User");
        return null;
    }
    
    
}