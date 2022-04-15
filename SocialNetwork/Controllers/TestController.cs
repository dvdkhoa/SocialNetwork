using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api.Helpers;

namespace SocialNetwork.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult getTest()
        {
            return Ok("Hello world");
        }
        [HttpPost]
        public IActionResult CreateTest(string naem)
        {
            return Ok(naem);
        }
    }
}
