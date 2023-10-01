using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelInfo.API.Controllers
{
    [ApiController]
    [Route("api/{version:apiVersion}/cities")]
    [Authorize]
    public class HomeController : ControllerBase
    {
       
    }
}
