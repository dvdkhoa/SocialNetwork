using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialNetwork.BLL.Services;
using SocialNetwork.DAL.Identity.Models;
using SocialNetwork.DTO.Account;
using SocialNetwork.DTO.Extensions;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly AppSettings _appSettings;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterModel registerModel)
        {
            var result = await _accountService.CreateUserAsync(registerModel);

            if (result.Succeeded)
            {
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("registerFake")]
        public async Task<IActionResult> RegisterWithFakeData()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://randomuser.me/api/?results=100&nat=gb,us&inc=gender,name,email,picture");

                var results = JsonConvert.DeserializeObject<JObject>(response).Value<JArray>("results");

                string UpFirst(string input)
                {
                    return char.ToUpper(input[0]) + input.Substring(1);
                }

                foreach (var randUser in results)
                {
                    var gender = UpFirst(randUser.Value<string>("gender"));
                    var first = UpFirst(randUser.SelectToken("name.first").Value<string>());
                    var last = UpFirst(randUser.SelectToken("name.last").Value<string>());
                    var email = randUser.Value<string>("email");
                    var picture = randUser.SelectToken("picture.large").Value<string>();

                    var model = new RegisterModel()
                    {
                        Email = email,
                        Name = first + last,
                        Gender = gender,
                        Image = picture,
                        Password = "123123"
                    };

                    await Register(model);
                }
            }

            return this.Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            var result = await _accountService.LoginAsync(loginModel);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }


        [HttpPost("Follow")]
        public async Task<IActionResult> Follow(string userId, string destId)
        {
            var result = await _accountService.FollowAsync(userId, destId);

            if(result)
                return Ok(result);
            return BadRequest(result);
        }
        
    }
}
